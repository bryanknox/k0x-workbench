# Copilot instructions

Project orientation for this repository is canonical in `AGENTS.md` at
the repository root. Read it before answering questions about this
project or changing code in it.

Shared agent guidance — bench file rules, PowerShell guidelines, expert
lens bodies — is canonical in the `ai/` directory, in harness-neutral
markdown. Everything under `.github/` is a thin entrypoint that links
there; read the linked file rather than working from the entrypoint
alone.

Path-scoped rules in `.github/instructions/` load automatically when
your work matches their `applyTo` glob.

Task procedures live in `.claude/skills/`. Despite the directory name
these are **not** Claude-only: `.claude/skills/` is one of the project
skill roots Copilot reads, so those skills are available here, including
as slash commands — for example `/normalize-bench-file`. That path is
used because it is the one root every harness this project targets can
read; `ai/README.md` has the table.

Do not restate project facts in this file; put them in `AGENTS.md` or in
the canonical `ai/` document they belong to. `ai/README.md` maps the
layout and says which file to read for what; read it before adding,
moving, or editing any agent primitive. Ordinary work does not need it.
