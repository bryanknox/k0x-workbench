# Copilot instructions

Project orientation for this repository is canonical in `AGENTS.md` at
the repository root. Read it before answering questions about this
project or changing code in it.

Path-scoped rules in `.github/instructions/` load automatically when
your work matches their `applyTo` glob.

Task procedures live in `.claude/skills/`. Despite the directory name
these are **not** Claude-only: `.claude/skills/` is one of the project
skill roots Copilot reads, so those skills are available here, including
as slash commands — for example `/normalize-bench-file`. That path is
used because it is the one root every harness this project targets can
read; see `AGENTS.md` for the table.

Do not restate project facts in this file; put them in `AGENTS.md` or in
the canonical rule document they belong to. See
`docs/agent-primitives-design.md` for the layout and why.
