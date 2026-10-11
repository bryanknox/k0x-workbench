# Vendored Agent Skills

Some skill folders in `.claude/skills/` are copied verbatim from other
repositories, so every contributor gets them without installing anything.
Despite the folder name these skills are not Claude-only; see
[ai/README.md](../ai/README.md#why-skills-live-under-claude).

This is the only copy of these rules. Developers and AI coding agents both
follow it.

## `scripts/vendored-skills.json`

[`scripts/vendored-skills.json`](../scripts/vendored-skills.json) is the
only list of vendored skills. Check that file to see what is vendored
today.

Each entry in `sources` has these fields:

| Field | Meaning |
|---|---|
| `name` | Short name for the source, used with `-Source`. |
| `repository` | The upstream GitHub repository URL. |
| `tagPrefix` | Prefix of the upstream release tags. Default `v`. |
| `skillsPath` | Folder in the upstream repo that holds the skills. Default `skills`. |
| `skills` | The skill folder names to copy into `.claude/skills/`. |
| `vendored` | The tag (`ref`) and `commit` the skills were last copied from. **The script writes this field. Never edit it by hand.** |

## Rules

- **Do not edit a vendored skill in place.** Fix it upstream, release it,
  then refresh. The next refresh overwrites any in-place edit.
- Skill names must be unique across sources and must not collide with a
  skill this repo owns. The script checks both before it copies anything.

## `scripts/UpdateVendoredSkills.ps1`

```powershell
.\scripts\UpdateVendoredSkills.ps1                                   # refresh every source to its latest release tag
.\scripts\UpdateVendoredSkills.ps1 -Source <name>                    # refresh one source
.\scripts\UpdateVendoredSkills.ps1 -Source <name> -Ref <tag|branch|sha>  # pin something other than the latest
.\scripts\UpdateVendoredSkills.ps1 -Check                            # verify only; changes nothing
```

- **Refresh** copies each source's skills from the chosen ref and updates
  `vendored` in the JSON file. Running it again at the same ref copies
  again, which also undoes accidental local edits. Review the diff and
  commit it yourself. The script never commits.
- **Check** reports newer upstream releases. It exits 1 if any vendored
  folder differs from its pinned commit.

## Common tasks

- **Add a source or skill:** add or extend the entry (`name`,
  `repository`, `skills`, plus `tagPrefix` / `skillsPath` if they differ
  from the defaults), then run the script.
- **Remove a skill:** delete its name from `skills` and delete its
  `.claude/skills/<name>/` folder in the same commit. The script never
  deletes a folder it is not told about.

## Personal copies

In Claude Code, a personal copy of a skill with the same name
(`~/.claude/skills/<name>/`) takes precedence over the project copy.
