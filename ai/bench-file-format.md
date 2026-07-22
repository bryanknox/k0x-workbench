# K0xBench.json Guidelines

Structure, inheritance, preferred tool patterns, and cleanup rules for
K0x Workbench bench files (`*K0xBench.json`). Also called Bench files
or Workbench files.

This file is the **canonical** definition of these rules, in a
harness-neutral location. Every primitive that needs them points here —
the Copilot instructions file, the Claude Code rule file, the
`normalize-bench-file` skill, and `AGENTS.md`. Do not restate these
rules in any of them.

## Structure

Root object with a `Bench` property containing a `Kit` object (the root Kit). The property is named `Bench` for backward file compatibility. The root Kit's `Label` is the workbench label, and like any Kit it can have a `DefaultWorkingDirectory`, `Tools`, and nested `Kits`.

**Kit** object:
- `Label` (string)
- `DefaultWorkingDirectory` (string) — inherited from nearest ancestor Kit if unset (null/empty/whitespace).
- `Tools` (array of `Tool` objects)
- `Kits` (array of child `Kit` objects)

**Tool** object:
- `Label` (string)
- `Command` (string)
- `Arguments` (string)
- `WorkingDirectory` (string) — inherited from containing Kit's `DefaultWorkingDirectory` if unset.

## Preferred Tool Patterns

Use `WorkingDirectory` (or inherited `DefaultWorkingDirectory`) to set the cwd; specify `Arguments` paths relative to it.

```json
{
    "Label": "VS Code",
    "Command": "code",
    "Arguments": ".",
    "WorkingDirectory": "C:/root-dir/some-folder"
},
{
    "Label": "File Explorer",
    "Command": "explorer.exe",
    "Arguments": ".",
    "WorkingDirectory": "C:/root-dir/some-folder"
},
{
    "Label": "Windows Terminal",
    "Command": "wt.exe",
    "Arguments": "-d .",
    "WorkingDirectory": "C:/root-dir/some-folder"
}
```

## Common Cleanup Tasks

1. **Remove unset Tool properties** — Delete `Arguments` and `WorkingDirectory` from any Tool where they are null, empty, or whitespace.
2. **Use `/` path separators** — Replace Windows backslashes (`\\`) with forward slashes (`/`) in all `DefaultWorkingDirectory` and `WorkingDirectory` values.
3. **Refactor to use inherited `DefaultWorkingDirectory`** — Move shared `WorkingDirectory` values up to the Kit's `DefaultWorkingDirectory` so Tools inherit it. Account for nested Kits inheriting from their nearest ancestor.
