# Conventional Commit Types - decision guide

Loaded when a change fits more than one type or none cleanly.
The de-facto ten types (feat and fix from the spec; the other
eight from the Angular convention):
build, chore, ci, docs, feat, fix, perf, refactor, style, test.

## Per-type one-liners

- `feat` - a new user-facing capability users can interact with
  or rely on.
- `fix` - corrects incorrect behavior (a bug).
- `perf` - makes code faster or more efficient without changing
  behavior.
- `refactor` - restructures code without altering behavior
  (renames, reorganizing modules, simplifying logic).
- `style` - formatting only: whitespace, semicolons, lint fixes;
  no logic change.
- `test` - adds or updates tests, fixtures, mocks; no production
  code change.
- `docs` - documentation only: README, API docs, comments that
  explain behavior.
- `build` - build system or external dependencies: project files,
  package manifests, build scripts.
- `ci` - continuous integration: pipelines, workflows, actions.
- `chore` - maintenance fitting none of the above: cleanup, file
  renames, config not related to build or CI.

## Decision rules

Ask in order; first match wins.

1. Does the change affect users (observable behavior)?
   Yes -> `feat` (new), `fix` (broken to correct), or `perf`
   (same behavior, faster). No -> continue.
2. Does it change production code at all?
   Yes, without behavior change -> `refactor` (structure) or
   `style` (formatting). No -> continue.
3. Tests only -> `test`. Docs only -> `docs`.
4. Tooling or infrastructure: build system -> `build`;
   CI/CD -> `ci`; anything else -> `chore`.

## Common tie-breakers

- Dependency bumps: `build` when the dependency ships in the
  product; `chore` when it is dev-only tooling.
- A refactor that corrects behavior along the way is a `fix`.
- Tests added alongside a feature or fix take the feature/fix
  type; `test` is for test-only changes.
- Comments that explain behavior are `docs`; deleting
  commented-out code is `refactor` or `chore`.

## Breaking changes

Any type can be breaking. Mark BOTH ways:

- `!` after the type/scope in the title:
  `feat(api)!: remove deprecated v1 endpoints`
- A `BREAKING CHANGE: <explanation>` footer as the last paragraph
  of the PR body.

For anything these rules do not settle (reverts, multiple
footers, exotic formats), consult the live spec at
https://www.conventionalcommits.org/ -- do not extrapolate from
recall.
