---
name: write-pr-title
description: >-
  Use this skill when creating or updating a GitHub pull request
  whose title must be a Conventional Commits message -- squash-merge
  repos where the PR title becomes the commit on the default
  branch. Also use when a "PR Title" / semantic-pull-request check
  is failing, when the user asks which commit type (feat, fix,
  chore, ...) a change is, or when opening any PR in a repo set up
  by enforce-conventional-commits. Composes type(scope)!: description
  titles and BREAKING CHANGE footers from the actual branch diff.
  Works for any contributor who can open a PR. Does NOT configure
  repo settings, rulesets, or workflows (see
  enforce-conventional-commits).
---

# Write PR Title (conventional title authoring)

Create or update a GitHub pull request so its TITLE is a valid
Conventional Commits message. Built for squash-merge repos where
the PR title becomes the commit message on the default branch;
still useful anywhere a conventional PR title is wanted.

Branch commits stay unconstrained by design: compose and check the
PR title only; never rewrite, squash, or lint the branch's own
commits.

Boundaries: this skill does NOT configure repo settings, rulesets,
or workflows (companion skill: `enforce-conventional-commits`), and
it never attempts that scaffold itself, even when it detects the
scaffold is missing.

## Who can run this

| | |
|---|---|
| Intended user | Any contributor who can open a PR |
| GitHub permission | Push to a branch or fork; `triage`+ only to retitle someone else's PR |
| gh token scopes | `repo` (default gh auth) |
| Runs how often | Every PR |
| On missing permission | Degrade: compose the title anyway; report what could not be applied |

## Title rules

```
type(scope)!: description
```

- `type`: lowercase, from the de-facto ten: feat, fix, docs,
  style, refactor, perf, test, build, ci, chore.
- `(scope)`: optional; a short noun for the area touched (e.g.
  `api`, `parser`). Omit rather than invent one.
- `!`: only when the change is breaking.
- `description`: imperative mood ("add", not "added" or "adds"),
  lowercase start, no trailing period.
- Whole title <= 72 characters.
- Breaking changes ALSO get a `BREAKING CHANGE: <explanation>`
  footer as the last paragraph of the PR body.

For spec-edge cases (reverts, multiple footers, unusual formats),
verify against the live spec at https://www.conventionalcommits.org/
rather than extrapolating.

## Process

Follow the steps in order. Every fact about the branch, repo, and
PR comes from a `git`/`gh` read, never from memory or conversation
history. Every mutation is followed by a read-back verification.

### Step 1 - Gather facts (read-only)

```
gh repo view --json nameWithOwner,defaultBranchRef   # repo + default branch
git branch --show-current                            # head branch
git log {default_branch}..HEAD --oneline             # commits on the branch
git diff {default_branch}...HEAD --stat              # shape; then read the diff itself
gh pr list --head {branch} --json number,title,author,body   # existing PR?
```

If the user named an existing PR number, read it directly with
`gh pr view {number} --json number,title,author,body`. If the
branch has no commits ahead of the default branch, report that
there is nothing to open a PR for and stop.

### Step 2 - Detect the scaffold

```
gh api -H "Accept: application/vnd.github.raw" repos/{owner}/{repo}/contents/.github/workflows/pr-title.yml   # 404 = absent; raw = the file itself
gh api repos/{owner}/{repo} --jq '{allow_squash_merge: .allow_squash_merge, squash_merge_commit_title: .squash_merge_commit_title, squash_merge_commit_message: .squash_merge_commit_message}'
```

SCAFFOLDED = the PR-title workflow exists. If that exact filename
is 404, list `.github/workflows` and treat any workflow that lints
the PR title (e.g. uses amannn/action-semantic-pull-request) as
the scaffold; the listing may include subdirectories -- inspect
only `.yml`/`.yaml` entries, and read the match the same raw way.

Read the check's title rules from that file, not only its
existence: the `with:` settings of the PR-title step. The ones that
constrain a title are `types` (the allowed types), `requireScope`
and `scopes` (whether a scope is mandatory, and which are allowed),
and `subjectPattern` (a regex the description must match). Absent
means the action's default. A value taken from a variable
(`${{ vars.X }}`) cannot be read here; treat it as unknown. These
rules are what step 7's check will hold the title to, so compose
for them in step 4.

Also note the `squash_merge_commit_title` value: `PR_TITLE` means
the title literally becomes the commit on the default branch;
`COMMIT_OR_PR_TITLE` (GitHub's default) means the title is used
only for MULTI-commit PRs -- a single-commit PR squashes with the
commit's own message instead. State the setting's effect in the
report; never change the setting. If the squash fields come back
`null`, the account cannot see them (it has no push access): report
the effect as unknown, never guess it.

Not scaffolded? Continue anyway -- a conventional title is
valuable on its own -- but skip step 7 (check polling) and carry
into the final report the gap plus a recommendation that a repo
admin run `enforce-conventional-commits`. Never scaffold from here:
that is repo administration, not title authoring.

### Step 3 - Classify the change

Classify from the DIFF; the branch's commit messages are hints,
not truth. Decision rules:

- Behavior users can observe changed? Broken behavior corrected ->
  `fix`; new capability -> `feat`; same behavior but faster ->
  `perf`.
- Production code changed without behavior change? Restructuring
  -> `refactor`; formatting only -> `style`.
- Tests only -> `test`. Docs only -> `docs`.
- Tooling: build system or shipped dependencies -> `build`;
  CI/workflows -> `ci`; other maintenance -> `chore`.

Read `references/type-guide.md` when the change fits more than one
type or none cleanly.

A squash-merge PR becomes ONE commit, so it gets ONE type. Mixed
changes: pick by dominant user impact (feat > fix > perf > the
rest); if two unrelated changes genuinely share the branch,
recommend splitting the PR instead of blending types.

Breaking = consumers must change their code or configuration
(removed or renamed public API, changed defaults, schema or config
format changes). Mark with `!` in the title AND the
`BREAKING CHANGE:` footer in the body.

### Step 4 - Compose title and body

Title per the rules above. Body: a short summary of what changed
and why; if breaking, end the body with the `BREAKING CHANGE:`
footer. The footer must be the LAST paragraph -- attribution or
tooling trailer lines go above it. When updating an existing PR,
refresh the body too: replace placeholder bodies (e.g. "wip"),
keep meaningful existing content, and repair footer placement.
If step 2 found the squash message setting is not
`PR_BODY`, the footer stays PR-only (it will not ride into the
squash commit); note that in the report rather than trying to
change the setting.

Compose within the check's rules from step 2: a type from its
allowed list, a scope from its allowed list when one is required.
A required scope is not an invented one when an allowed scope
genuinely names the area the diff touches.

Then test the title against those rules. A CONFLICT is when every
title the check would accept breaks the title rules above -- a
required scope with none in the list that describes the change, the
change's real type missing from the allowed list, or a
`subjectPattern` no honest description can match.

### Step 5 - Checkpoint (conditional)

Required in two cases, and only these:

- Editing a PR whose title this session did not compose, or whose
  author (from step 1) is not the current `gh` user: present
  `old title -> new title` plus the one-sentence type rationale,
  and wait for approval before editing. A request that names the PR
  and asks for the retitle ("fix the title on PR 5") is itself the
  approval -- apply directly and show old -> new in the report.
- A CONFLICT from step 4, BEFORE creating or retitling -- even a PR
  this session creates. Show the rule-following title, the rule in
  the check that rejects it, and the choice: apply the
  rule-following title (the check stays red until an admin changes
  the rule), or the title the check accepts (and name the title
  rule it breaks). Never offer to change the workflow or its allowed
  values yourself; that is repo administration. Say what an admin
  could change instead.

Otherwise no checkpoint: a PR created fresh in this session, with a
title that fits the check's rules, needs no approval.

### Step 6 - Apply and verify

```
gh pr create --base {default_branch} --title "{title}" --body "{body}"   # no existing PR
gh pr edit {number} --title "{title}"             # existing PR
```

Read back with `gh pr view {number} --json title,body` and confirm
the title matches exactly. A refused edit on someone else's PR --
an HTTP 403, or gh's GraphQL "does not have the correct permissions
to execute `UpdatePullRequest`" -- means the invoker lacks `triage`
on the repo: report the composed title and who can apply it (the
PR's author, or anyone with triage); do not retry, and do not reach
for another account's credentials.

### Step 7 - Verify the check (scaffolded repos only)

Poll `gh pr checks {number}` until the PR-title check completes
(job name `Validate conventional PR title` in repos scaffolded by
the companion skill; otherwise any check from a
semantic-pull-request / PR-title workflow). Only the PR-title
check gates this skill: ignore other checks (build, tests) and do
not wait for them. Avoid `--watch` while unrelated checks run --
it waits for ALL checks; re-run the plain command at short
intervals instead.

On failure: read the exact complaint
(`gh run view {run_id} --log-failed`), fix the title accordingly,
`gh pr edit`, and re-poll. Maximum 2 retries; after that, report
the check's complaint verbatim and stop. If the only fix the
complaint allows would break the title rules, that is a CONFLICT:
stop and use the step 5 checkpoint rather than retitling. Either
way, never change the workflow; report what an admin could change.

### Step 8 - Report

```markdown
## PR title: {owner}/{repo}#{number}
Title: {title}
PR: {url}
Type rationale: {one sentence: why this type / scope / breaking marker}
Check: {pass | fail: reason | skipped: repo not scaffolded}
{If not scaffolded: this repo does not enforce PR titles; a repo
admin can run the companion skill enforce-conventional-commits to
add the check.}
```
