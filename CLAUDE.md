# CLAUDE.md

Claude Code entrypoint for this repository. Project orientation is
canonical in `AGENTS.md` and imported below — keep this file thin and do
not restate project facts here.

@AGENTS.md

## Claude Code specifics

- Everything under `.claude/` is a thin entrypoint. The content it
  points at lives in `ai/`; read the linked file rather than working
  from the entrypoint alone.
- Path-scoped rules live in `.claude/rules/`. They load automatically
  when you read a matching file.
- Repo skills live in `.claude/skills/`. `/normalize-bench-file` cleans
  up `*K0xBench.json` files.
- `.claude/agents/gh-workflow-pwsh-dev.md` is the lens for GitHub Actions
  PowerShell work. Load it via the `Task` tool when working under
  `.github/workflows/pwsh` or `.github/workflows/pwsh-unit-tests`.
- `@AGENTS.md` above is the only import in this repo's agent guidance.
  Everything below the always-on layer is linked, not imported, so it
  loads on demand — see `ai/README.md`.
- Prefer PowerShell for shell commands; this is a Windows-only project.
