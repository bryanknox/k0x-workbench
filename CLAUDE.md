# CLAUDE.md

Claude Code entrypoint for this repository. Project orientation is
canonical in `AGENTS.md` and imported below — keep this file thin and do
not restate project facts here.

@AGENTS.md

## Claude Code specifics

- Path-scoped rules live in `.claude/rules/`. They load automatically
  when you read a matching file and point at the canonical rule
  document; read that document before editing.
- Repo skills live in `.claude/skills/`. `/normalize-bench-file` cleans
  up `*K0xBench.json` files.
- `.claude/agents/workflow-pwsh-dev.md` is the lens for GitHub Actions
  PowerShell work. Load it via the `Task` tool when working under
  `.github/workflows/pwsh` or `.github/workflows/pwsh-unit-tests`.
- Prefer PowerShell for shell commands; this is a Windows-only project.
