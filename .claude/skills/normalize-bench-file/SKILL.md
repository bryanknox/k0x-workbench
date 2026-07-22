---
name: normalize-bench-file
description: >-
  Use this skill when the user wants a K0x Workbench bench file (any
  `*K0xBench.json`) cleaned up, normalized, tidied, or made consistent:
  converting Windows backslash paths to forward slashes, dropping empty
  Tool properties, and hoisting repeated WorkingDirectory values up into
  a Kit's DefaultWorkingDirectory. Trigger it even when the user never
  says "normalize" -- for example "clean up my bench file", "fix the
  paths in this workbench json", "my kits all repeat the same working
  directory", "tidy this K0xBench.json", or when the user shares a bench
  file and asks to make it neat. Do NOT use it for other JSON files, for
  formatting C# or Razor code, or for changing what a tool actually
  launches.
argument-hint: "[path to a *K0xBench.json file]"
---

# Normalize a bench file

Apply the K0x Workbench bench file conventions to one or more
`*K0xBench.json` files without changing what any tool does.

## Step 1 — load the canonical rules

Read `.github/instructions/K0xWorkbench.instructions.md`. It defines the
bench file structure, the `DefaultWorkingDirectory` inheritance model,
the **Preferred Tool Patterns**, and the **Common Cleanup Tasks** you
will apply. It is the only copy of those rules; do not work from memory
of this skill's summary.

If that file is not present (for example the bench file lives outside
this repository), say so and ask the user for the rules before editing
anything.

## Step 2 — choose the files

In priority order:

1. Any `*K0xBench.json` file the user named, pasted, or has open.
2. Otherwise, find candidates with a tool call — do not guess paths:

   ```powershell
   Get-ChildItem -Recurse -Filter "*K0xBench.json" | Select-Object -ExpandProperty FullName
   ```

   List what you found and ask which to normalize.

**Skip** a file, and tell the user why, when either is true:

- the filename does not end in `K0xBench.json`;
- the parsed JSON has no root `Bench` property containing a `Kit`
  object.

Never edit a file you skipped.

## Step 3 — normalize

Apply every item under **Common Cleanup Tasks** in the instructions
file, and move tools toward the shapes under **Preferred Tool Patterns**.

Judgement rules that govern those edits:

- **Behavior is invariant.** Only rewrite a tool's pattern when the
  rewrite launches the same thing with the same effective working
  directory. If you are not sure, leave it and mention it in the report.
- **Respect inheritance.** A Kit inherits `DefaultWorkingDirectory` from
  its nearest ancestor Kit. Do not restate a value a Kit already
  inherits, and do not hoist a value up past a Kit whose other children
  would then inherit something wrong.
- **Hoist only what is shared.** Promote a `WorkingDirectory` to the
  Kit's `DefaultWorkingDirectory` when the Kit's tools agree on it;
  leave the odd one out set explicitly.
- **Check who starts inheriting.** Setting a Kit's
  `DefaultWorkingDirectory` changes the effective directory of every
  tool in that Kit that has no `WorkingDirectory` of its own, and of
  descendant Kits that do not set their own. Confirm that is harmless
  for each of them (a URL tool usually does not care; a script tool
  does) before hoisting, and say so in the report.
- **Order the edits.** Convert path separators first, then hoist, then
  remove now-redundant values. A nested Kit's `DefaultWorkingDirectory`
  often becomes redundant only after its parent's value is hoisted.
- Preserve `Label` text, tool order, and kit order.

## Step 4 — verify before reporting

The file must still parse the way the app parses it. Confirm it with a
tool call — do not assert it:

```powershell
[System.Text.Json.JsonDocument]::Parse((Get-Content -Raw "<path>")) | Out-Null
```

Use this and not `ConvertFrom-Json`: the app loads bench files with
`System.Text.Json` (`K0x.DataStorage.JsonFiles`), which rejects trailing
commas and comments that PowerShell's `ConvertFrom-Json` happily
accepts. A file that only passes the looser check can still break the
app.

If the command fails, revert that file to its previous content, report
the failure, and stop. Do not report a file as normalized until this
command has succeeded for it.

## Step 5 — report

One short block per file:

```
<path>
  changed: <n> edits
    - <what changed, e.g. "3 backslash paths -> forward slashes">
    - <e.g. "hoisted C:/src/foo to Kit 'Work' DefaultWorkingDirectory (4 tools)">
    - <e.g. "removed 2 empty Arguments properties">
  left alone: <anything deliberately not changed, and why>
  verified: JSON parses
```

For a file that needed nothing, say it is already normalized. For a
skipped file, give the path and the reason.
