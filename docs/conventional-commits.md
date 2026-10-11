# Pull Requests and Conventional Commits

Every commit on `main` is a [Conventional Commits](https://www.conventionalcommits.org/)
message. The repo gets that history from **PR titles**, not from the commits on
your branch.

This is the only copy of these rules. Developers and AI coding agents both
follow it.

## How `main` is protected

- `main` accepts no direct pushes. The only way in is a pull request.
- Squash merge is the only merge method allowed.
- The squash commit **title** is the PR title, and the squash commit **body**
  is the PR body.
- The branch ruleset `conventional-commits-main` requires the
  **Validate conventional PR title** status check, which runs in
  [`.github/workflows/pr-title.yml`](../.github/workflows/pr-title.yml).

So the PR title *is* the commit message that lands on `main`. Commit
messages on your branch can be anything, because the squash discards them.

## PR title format

```text
type(scope)!: description
```

- `type` is required. It must be one of the types listed in the `types:`
  block of [`pr-title.yml`](../.github/workflows/pr-title.yml). That list
  is the only list of accepted types.
- `(scope)` is optional, for example `docs(readme): ...`.
- `!` marks a breaking change (see below).
- `description` is a short summary written in the imperative mood.

Examples:

```text
feat: add recent benches page
fix(bench): keep working directory when saving a copy
docs: document conventional PR titles
ci: add conventional PR title check
```

A squash merge produces **one** commit, so a PR gets **one** type. If a
branch mixes unrelated changes, split it into separate PRs rather than
combining types.

Dependabot PRs use the `ci` and `build` types. See
[dependabot.md](dependabot.md).

## Breaking changes

Mark a breaking change in two places:

1. Put `!` before the colon in the title: `feat(bench)!: rename Bench root property`.
2. End the PR body with a `BREAKING CHANGE:` footer that describes what
   consumers must change. Make it the last paragraph of the body.

The PR body becomes the squash commit body, so the footer reaches `main`.

## Fixing a failing PR Title check

Edit the PR title on GitHub (or run `gh pr edit <number> --title "..."`).
The check runs again when the title changes, so you don't need to push a
new commit.

## Merging: do not edit the squash commit message

When you click **Squash and merge**, GitHub fills in the commit message
from the PR title and body. **Leave it as it is.**

The PR Title check validates the PR title, not the text in the merge box.
A hand edit there is not validated and can put a non-conventional commit
on `main`. To change the message, edit the PR title or body *before*
merging, which also lets the check validate the new title.

From the command line, run `gh pr merge <number> --squash` without
`--subject` or `--body`, so the repo defaults are used.

## Tooling

- **`write-pr-title` skill** (`/write-pr-title`): writes a conventional
  title, and a body with a breaking-change footer when one is needed, from
  the branch's actual diff. It then checks the result against the PR Title
  check. Use it when you open or retitle a PR with an AI coding agent.
  [`references/type-guide.md`](../.claude/skills/write-pr-title/references/type-guide.md)
  helps when a change could fit more than one type.
- **`enforce-conventional-commits` skill**: the repo-admin procedure that
  set up the merge settings, workflow, and ruleset above. Ordinary
  contributors don't need it.

Both skills are vendored from an upstream repository. See
[vendored-skills.md](vendored-skills.md).
