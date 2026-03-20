---
name: normalize-bench-file
description: Normalize the content of K0xBench.json files to use preferred patterns.
---

## Goal

Normalize `*K0xBench.json` files by applying all **Common Cleanup Tasks** and
**Preferred patterns for common tools** from
`.github/instructions/K0xWorkbench.instructions.md`.

Read that instructions file first — it defines the bench file structure,
preferred tool patterns, and cleanup rules.

## Which files to normalize

1. Act on any `*K0xBench.json` files in editor context or referenced by the user.
2. If none are apparent, list available `*K0xBench.json` files and ask.
3. **Skip** files that don't match the naming pattern or lack the expected structure (`Bench` → `Label` + `Kits`). Inform the user why.

## Additional normalization guidance

- Only rewrite a tool's pattern when the rewrite preserves the same effective behavior.
- For nested Kits, respect `DefaultWorkingDirectory` inheritance — don't duplicate a value already inherited from an ancestor.
- Ensure JSON parses correctly.

## Output

Briefly summarize what changed in each file. If already normalized, say so.
