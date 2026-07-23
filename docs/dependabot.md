# Dependabot

Dependabot keeps the repo's dependencies up to date by opening pull requests when
newer versions are available.

## Configuration

Dependabot is configured in [`.github/dependabot.yml`](../.github/dependabot.yml).

Two package ecosystems are watched, both rooted at the repo root (`/`):

| Ecosystem        | Covers                                      | PR title prefix |
| ---------------- | ------------------------------------------- | --------------- |
| `github-actions` | Action versions in `.github/workflows/`     | `ci(deps)`      |
| `nuget`          | NuGet packages (`Directory.Packages.props`) | `build(deps)`   |

Prefixes follow [Conventional Commits](https://www.conventionalcommits.org/): `ci`
for CI configuration (the Actions the workflows run) and `build` for the build
system's external dependencies (NuGet packages). The `(deps)` scope comes from
`include: "scope"`, which makes Dependabot append the dependency scope it resolved.
So a grouped Actions PR is titled, for example:

```text
ci(deps): bump the actions group with 7 updates
```

Both run on a **weekly** schedule (Saturday 03:00 America/Los_Angeles) and **group**
all updates in the ecosystem into a single PR (one PR for Actions, one for NuGet)
to keep review overhead low.

Because the repo uses Central Package Management with transitive pinning and
committed NuGet lock files, a merged NuGet update PR may require regenerating the
lock files (see the restore commands in [AGENTS.md](../AGENTS.md)) if Dependabot's
own update does not.

## Check that Dependabot is enabled

Dependabot Version Updates run automatically once `.github/dependabot.yml` is
present on the default branch — no extra toggle is required for the update PRs.
To confirm it is active:

- **GitHub UI:** Repo → **Insights** → **Dependency graph** → **Dependabot** tab.
  This lists each configured ecosystem and the last time it checked.
- **Repo settings:** Repo → **Settings** → **Code security**. Confirm
  *Dependency graph* is on (required for Dependabot to work); *Dependabot security
  updates* is optional and separate from the version updates configured here.
- **GitHub CLI:**

  ```powershell
  gh api repos/bryanknox/k0x-workbench/automated-security-fixes
  ```

## Manually invoke Dependabot

Dependabot does not need to wait for its weekly schedule — you can trigger a check
on demand:

- **GitHub UI:** Repo → **Insights** → **Dependency graph** → **Dependabot** tab →
  click the ecosystem → **Check for updates**.
- Re-running a check will open new PRs only when updates are actually available.

To act on an existing Dependabot PR, comment on the PR with one of Dependabot's
commands, for example:

- `@dependabot rebase` — rebase the PR on the latest default branch.
- `@dependabot recreate` — rebuild the PR from scratch.
- `@dependabot merge` — merge once checks pass.
