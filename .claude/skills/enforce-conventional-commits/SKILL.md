---
name: enforce-conventional-commits
description: >-
  Use this skill when the user wants to set up, scaffold, or
  configure a GitHub repository to use Conventional Commits enforced
  at squash-merge time: squash-only merge settings, a main-branch
  ruleset requiring PRs and a PR-title check, and a PR-title lint
  workflow. Also use when the user asks to "protect main", "prevent
  direct pushes", "enforce commit message format on main", or is
  setting up a new .NET (or any) app repo that should keep a clean
  conventional history. Configures the repo via gh CLI. Requires
  ADMIN access to the target repo. Does NOT create the repo or app
  code, and does NOT write PR titles (see write-pr-title).
---

# Enforce Conventional Commits (squash-merge enforcement)

Configure an EXISTING GitHub repository so that the only path to
the default branch is a squash-merged PR whose title is a valid
Conventional Commits message. The whole footprint is three levels,
applied in this order:

- Level 1 - repo merge settings: squash-only; default squash
  message = PR title.
- Level 2 - a PR-title lint workflow
  (`assets/pr-title-workflow.yml`, using
  amannn/action-semantic-pull-request), pinning the allowed commit
  types to the Conventional Commits de-facto set.
- Level 3 - a default-branch ruleset: require a PR, require the
  PR-title status check, block force pushes and deletions, empty
  bypass list.

There is no Level 4: no local git hooks, no commit tooling.
Branch commits stay unconstrained by design; the PR title at merge
time is the only gate.

Boundaries: this skill does NOT create the repo, does NOT scaffold
application code, and does NOT compose PR titles (companion skill:
`write-pr-title`).

## Who can run this

| | |
|---|---|
| Intended user | Repo owner / admin (personal account: the owner) |
| GitHub permission | `admin` on the target repo |
| gh token scopes | `repo` AND `workflow` |
| Runs how often | Once per repo; re-runs are idempotent no-ops |
| On missing permission | Fail fast BEFORE any mutation |

## Process

Follow the steps in order. Every fact about the repo comes from a
`gh` read, never from memory or conversation history. Every
mutation is followed by a read-back verification. Do not start
mutating before the checkpoint in step 4 is approved.

### Step 1 - Preflight (read-only)

Identify the target repo (default: the `origin` remote of the
current directory; otherwise ask). Then gather facts:

```
gh auth status                          # authenticated? scopes include repo, workflow?
gh api repos/{owner}/{repo}             # permissions.admin, visibility, default_branch,
                                        #   allow_squash_merge, allow_merge_commit,
                                        #   allow_rebase_merge, squash_merge_commit_title,
                                        #   delete_branch_on_merge
gh api user --jq .plan.name             # owner's plan (personal accounts); EMPTY unless
                                        #   the token has the user scope (gh default
                                        #   auth does not) -> treat empty as unknown
gh api repos/{owner}/{repo}/rulesets    # existing rulesets; 403 "Upgrade to
                                        #   GitHub Pro..." = free-plan private
                                        #   repo (plan gate CONFIRMED)
gh api repos/{owner}/{repo}/contents/.github/workflows/pr-title.yml
                                        # existing workflow? (404 = absent)
```

Use `default_branch` from the repo read everywhere this skill says
"main". Run the preflight reads as independent commands, not
`&&`-chained or under `set -e`: `gh api` exits nonzero on the
EXPECTED outcomes here (404 when the workflow file is absent;
empty `--jq` result when the plan is unknown; 403 on the rulesets
read when a private repo sits on a free plan).

Fail fast (before ANY mutation) when:

- `permissions.admin` is false: report that only a repo admin (in
  personal accounts, the account owner) can run this skill, and
  name them. For a personal-account repo the admin IS the owner --
  take the name from the repo slug / `owner.login`; for an
  org-owned repo say "an admin of {org}/{repo}" (a non-admin token
  cannot list who that is).
- Token lacks `repo` or `workflow` scope: report the missing scope
  and the remedy for the credential `gh auth status` says is in use.
  For a keyring login: `gh auth refresh -h github.com -s
  repo,workflow`. For a token from the `GH_TOKEN` or `GITHUB_TOKEN`
  environment variable, `gh auth refresh` cannot change it: the
  remedy is a token with both scopes, or unsetting the variable so
  a keyring login with both scopes is used.

Evaluate these as soon as each becomes knowable (scopes from the
`gh auth status` read; admin from the repo read) and stop at the
first hit -- the remaining preflight reads are unnecessary. The
fail-fast report replaces the Step 8 report: state that setup was
aborted at preflight with NOTHING changed, name the failed
condition and who can run the skill, and give the remedy. State
only what was read: a fact the stopped preflight never reached --
visibility, plan, rulesets, the workflow -- is unknown, not
inferred.

Detect the private-plan gap: rulesets on private personal repos
require GitHub Pro. On a free-plan private repo ALL ruleset
endpoints are plan-gated: the preflight rulesets read fails with
HTTP 403 "Upgrade to GitHub Pro or make this repository public to
enable this feature" (verified live 2026-07-28), and the create
POST would be refused with the same 403 -- the ruleset cannot be
created at all. That preflight 403 is the DEFINITIVE detector; the
plan read is only best-effort corroboration (`GET /user` includes
`plan` only when the token has the `user` scope, which gh's
default auth does not -- an empty result means UNKNOWN, not
`free`). Do not abort: Levels 1 and 2 work on any plan. Carry the
confirmed gate (403) -- Level 3 BLOCKED -- or, on a private repo
whose rulesets read succeeded, the remaining plan uncertainty,
into the checkpoint. When the gate is confirmed, plan to skip
Step 7's POST and report Level 3 as blocked with the remedy:
upgrade to Pro or make the repo public, then re-run this skill
(the re-run will be a Levels 1+2 no-op and add only the ruleset).

### Step 2 - Compute the diff

Compare current state against the target state (see "Target state
per level" below) and classify each level: `apply` (change
needed), `no-op` (already configured), or `conflict` (something
different exists, e.g. a stray ruleset targeting the default
branch, or a pr-title.yml with different content). A level can be
`conflict` AND `apply` at once (a stray ruleset exists and this
skill's ruleset is absent): surface both together. Compare only
the fields this skill specifies -- ruleset reads include
server-populated defaults (e.g. `pull_request.allowed_merge_methods`,
`required_status_checks.do_not_enforce_on_create`) that are NOT
conflicts. An existing pr-title.yml whose ONLY difference from the
template is the `with: types:` list is a customization, not a
conflict -- the list is the user's to set. Report it as a no-op
naming the types in force; propose reverting it only if the user
asks.

If all three levels are `no-op`, say so and stop after presenting
the (empty) diff; re-running on a configured repo must not mutate
anything. The no-op diff doubles as the final report (Steps 3-7
are skipped); optionally still run Step 6's read-only
action-version check and note any lag.

### Step 3 - Write the checklist

Before mutating anything, write this checklist to a working file
(e.g. `cc-setup-checklist-{repo}.md` -- repo-scoped name, so runs
against other repos sharing the scratch area never collide -- in
the session's scratch area, OUTSIDE the repo working tree so it
cannot end up committed).
Re-read it at the start of each level and update the Status column
after each verification; its header restates the goal and
constraints so they survive a long session.

```markdown
# Conventional commits setup: {owner}/{repo}
Goal: only path to {default_branch} is a squash-merged PR whose
title is a valid Conventional Commits message.
Constraints: empty bypass list; no branch-commit enforcement;
apply order Level 1 -> Level 2 -> Level 3.

| Step | Target | Current | Action | Status |
|---|---|---|---|---|
| L1 merge settings | squash-only, title=PR_TITLE | {found} | {apply/no-op} | pending |
| L2 pr-title workflow | present, action @v6, types pinned | {found} | {apply/no-op} | pending |
| L3 ruleset on {default_branch} | active; PR + check required | {found} | {apply/no-op} | pending |
| Read-back verification | all three levels green | - | - | pending |

Gaps: {none | private free-plan repo: ruleset will be created but
not enforced until upgrade or repo goes public}
```

### Step 4 - Checkpoint (human approval)

Present the proposed changes as one reviewable diff: per level,
current -> target, plus any detected gap or conflict. Wait for
approval before mutating. Two things ALWAYS require explicit
acknowledgment, never silent action:

- Overwriting an existing ruleset or an existing pr-title.yml that
  differs from the template.
- Proceeding on a private repo with the plan gate confirmed
  (Level 3 blocked: the ruleset cannot be created until the plan
  is upgraded or the repo goes public) or with the plan unknown.

State the allowed commit types the workflow will pin (feat, fix,
docs, style, refactor, perf, test, build, ci, chore) as part of
the Level 2 diff, and offer the list as customizable -- one line,
not a prompt to answer. The de-facto ten is the default; take a
different list only if the user gives one.

For a pre-existing ruleset this skill did not create, the default
is COEXIST: leave it untouched and create this skill's ruleset
alongside it (effective rules are the union of active rulesets;
the old ruleset's bypass list applies only to its own rules, never
to this skill's). Deleting or modifying someone's existing ruleset
requires explicit approval, never silent action.

### Step 5 - Level 1: merge settings

```
gh api -X PATCH repos/{owner}/{repo} \
  -F allow_merge_commit=false \
  -F allow_rebase_merge=false \
  -F allow_squash_merge=true \
  -f squash_merge_commit_title=PR_TITLE \
  -f squash_merge_commit_message=BLANK
```

Options the user may have chosen at the checkpoint:
`-f squash_merge_commit_message=PR_BODY` (PR body becomes the
commit body; lets `BREAKING CHANGE:` footers ride along), and
`-F delete_branch_on_merge=true` (tidiness).

Verify: re-read `gh api repos/{owner}/{repo}` and confirm each
field patched above. Update the checklist.

### Step 6 - Level 2: PR-title lint workflow

The workflow lands BEFORE the ruleset (Level 3) so the file can be
committed directly to the default branch without being blocked by
the ruleset this skill is about to create. Never swap Steps 6 and
7.

First check the action's current major version against the pin in
the template:

```
gh api repos/amannn/action-semantic-pull-request/releases/latest --jq .tag_name
```

If the latest major is newer than `v6`, keep the pinned `v6` (the
pin is deliberate; silent drift would change lint behavior) but
note the lag in the final report.

The template pins the allowed types in a `with: types:` block --
feat, fix, docs, style, refactor, perf, test, build, ci, chore --
rather than inheriting the action's defaults, so the repo states
which types it accepts. Commit it AS IS unless the user gave a
different list at the checkpoint; in that case edit only the
`types:` entries, leave the rest of the template alone, and note
the customized list in the final report.

Commit `assets/pr-title-workflow.yml` from this skill's directory
to the repo as `.github/workflows/pr-title.yml`, base64-encoded
per platform:

```
gh api -X PUT repos/{owner}/{repo}/contents/.github/workflows/pr-title.yml \
  -f message="ci: add conventional PR title check" \
  -f content="{base64 of the workflow file}"
```

When overwriting an existing file (approved at the checkpoint),
include `-f sha={existing file sha}` from the preflight read.

Verify: GET the same contents path, decode, and confirm it matches
the template (plus any type-list edit the user asked for). Update
the checklist.

### Step 7 - Level 3: ruleset on the default branch

If preflight confirmed the plan gate (403 on the rulesets read),
SKIP the create -- it would be refused with the same 403. Record
Level 3 as `blocked by plan` in the checklist and go to Step 8.

Otherwise create the ruleset. The required status check is
referenced by the workflow's job name string; no prior workflow
run is needed when creating via the API.

```
gh api -X POST repos/{owner}/{repo}/rulesets --input - <<'JSON'
{
  "name": "conventional-commits-main",
  "target": "branch",
  "enforcement": "active",
  "conditions": { "ref_name": { "include": ["~DEFAULT_BRANCH"], "exclude": [] } },
  "bypass_actors": [],
  "rules": [
    { "type": "deletion" },
    { "type": "non_fast_forward" },
    { "type": "pull_request",
      "parameters": {
        "required_approving_review_count": 0,
        "dismiss_stale_reviews_on_push": false,
        "require_code_owner_review": false,
        "require_last_push_approval": false,
        "required_review_thread_resolution": false
      } },
    { "type": "required_status_checks",
      "parameters": {
        "strict_required_status_checks_policy": false,
        "required_status_checks": [
          { "context": "Validate conventional PR title" }
        ]
      } }
  ]
}
JSON
```

Notes: the heredoc form needs a POSIX shell; from other shells,
write the JSON to a temp file (outside the repo working tree) and
pass `--input that-file`. Required approvals stay 0 (personal
repos; the PR just has to exist). The bypass list stays EMPTY
unless the user explicitly asks otherwise. No merge-method rule is
needed; Level 1 already
makes squash the only option. If the API returns 422, re-verify
the request shape against the live GitHub REST docs for repository
rulesets; do not adjust field names from recall. If it returns 403
"Upgrade to GitHub Pro...", that is the plan gate surfacing here
first: record Level 3 as blocked, do not retry, do not abort the
run.

Verify with the effective-rules endpoint, which returns only
ACTIVELY ENFORCED rules:

```
gh api repos/{owner}/{repo}/rules/branches/{default_branch}
```

Expect entries for `deletion`, `non_fast_forward`, `pull_request`,
and `required_status_checks`. On a free-plan private repo this
endpoint is plan-gated too (same 403), consistent with the blocked
state. An EMPTY result after a successful create means the ruleset
exists but is not enforced (possible for org-owned repos or
non-`active` enforcement): record it as inactive in the checklist;
do not delete it (it activates on upgrade or when the repo goes
public).

### Step 8 - Final report

Re-read the checklist and confirm every row is resolved, then
report:

```markdown
## Conventional commits setup: {owner}/{repo}
| Level | Before | After | Verified |
|---|---|---|---|
| 1 merge settings | {before} | squash-only, title=PR_TITLE | yes/no |
| 2 pr-title workflow | {before} | .github/workflows/pr-title.yml @v6, types {de-facto ten | customized: ...} | yes/no |
| 3 ruleset | {before} | {active | blocked by plan (403, not created) | created but NOT enforced} | yes/no |

Gaps: {none | plan gap details | action version lag}
Residual soft spot: the PR merge box lets the merger hand-edit the
final squash message, and nothing validates that edit. Don't touch
the prefilled message.
Next: open PRs with the companion skill write-pr-title.
```

## Target state per level

- Level 1: `allow_squash_merge=true`, `allow_merge_commit=false`,
  `allow_rebase_merge=false`, `squash_merge_commit_title=PR_TITLE`,
  `squash_merge_commit_message=BLANK`. Optional per user: squash
  message = PR_BODY instead of BLANK; auto-delete head branches.
- Level 2: `.github/workflows/pr-title.yml` matching
  `assets/pr-title-workflow.yml`. The job name is the status-check
  context the ruleset requires; renaming one without the other
  breaks enforcement. The template pins the allowed types to the
  de-facto set -- feat, fix, docs, style, refactor, perf, test,
  build, ci, chore -- in a `with: types:` block, so the accepted
  types are stated in the consumer's repo rather than inherited
  from the action's defaults. The user may edit that list; if they
  ask, apply their list at Step 6 and record the deviation in the
  final report.
- Level 3: one ruleset targeting the default branch, enforcement
  `active`, empty bypass list, rules: deletion, non_fast_forward,
  pull_request (0 required approvals), required_status_checks with
  context `Validate conventional PR title` (the Level 2 job name).
