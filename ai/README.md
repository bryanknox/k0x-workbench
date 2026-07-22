# `ai/` — harness-neutral agent guidance

Every rule, guideline, and expert-lens body that more than one AI coding
agent needs lives in this directory, in plain markdown with no
harness-specific syntax. Files under `.claude/`, `.github/`, and the
root entrypoints are **thin** — frontmatter the harness requires, plus a
link to the file here that holds the actual content.

The rule, in one line: **content lives in `ai/`; harness directories
hold only the entrypoints that point at it.**

## What is here

| File | Holds | Read it when |
|---|---|---|
| [bench-file-format.md](bench-file-format.md) | structure, `DefaultWorkingDirectory` inheritance, preferred tool patterns, and cleanup rules for `*K0xBench.json` | creating or editing any bench file |
| [pwsh-workflow-steps-guidelines.md](pwsh-workflow-steps-guidelines.md) | rules for the PowerShell that GitHub Actions steps run, and its Pester tests | writing or reviewing anything under `.github/workflows/pwsh*` |
| [pwsh-orexit-pattern-guidelines.md](pwsh-orexit-pattern-guidelines.md) | the `OrExit` failure pattern | a workflow script or function must fail the step |
| [workflow-pwsh-dev-lens.md](workflow-pwsh-dev-lens.md) | the shared body of the `workflow-pwsh-dev` expert lens | you were dispatched as that lens |
| [agent-primitives-design.md](agent-primitives-design.md) | the design packet behind this layout, and the portability facts it rests on | changing the layout, or adding/dropping a harness |

Ordinary work needs at most one of these. Do not read them all.

## Where the entrypoints live

Each primitive type has to sit where its harness looks for it; that is
the only reason anything agent-related lives outside `ai/`.

| Primitive | Claude Code | GitHub Copilot | Shared body |
|---|---|---|---|
| Project orientation | `CLAUDE.md` (imports `AGENTS.md`) | `.github/copilot-instructions.md` | `AGENTS.md` |
| Path-scoped rule | `.claude/rules/*.md` (`paths:`) | `.github/instructions/*.instructions.md` (`applyTo:`) | `ai/` |
| Expert lens | `.claude/agents/*.md` | `.github/agents/*.agent.md` | `ai/` |
| Task procedure (skill) | `.claude/skills/*/SKILL.md` | same file — see below | n/a |

`AGENTS.md` stays at the repository root rather than in `ai/`: root is
where every harness that supports it looks, and `CLAUDE.md` imports it
from there with `@AGENTS.md`.

### Why skills live under `.claude/`

`.claude/skills/` is **not** a Claude Code-only directory. GitHub Copilot
and Cursor both read it as a project skill root, so a skill placed there
is available in all three — including as a `/slash-command`. It is used
here because Claude Code reads *only* that path, while every other
harness reads several. It is the one location all our targets share, not
a statement about which tool this project prefers.

If your harness is not listed below, check its own docs for which skill
roots it reads before assuming these skills are unavailable to you.

| Harness | Reads `.claude/skills/`? |
|---|---|
| Claude Code | yes (only root it reads) |
| GitHub Copilot | yes |
| Cursor | yes |
| Codex | **no** — reads `.agents/skills/` |

A skill body cannot be moved to `ai/` the way a rule or a lens body can:
its `description` frontmatter *is* the dispatch signature, so a stub in
each skill root would register the skill more than once in the harnesses
that read several roots. The skill bodies are written as harness-neutral
prose instead, so relocating one is a file move, not a rewrite.

## Adding or changing guidance

1. Put the fact in the `ai/` file it belongs to — or add a new one and
   list it in the table above.
2. From each harness entrypoint that needs it, **link** to that file.
   Never paste the specifics into the entrypoint; a summary of a
   canonical file is a second copy of it, and it will drift.
3. Only `CLAUDE.md` uses Claude Code's `@path` import. Imports are
   expanded into the session prefix at launch, so importing a file that
   should load on demand — a path-scoped rule's content, a lens body, a
   skill's reference material — would defeat the point of scoping it.
   Below the always-on layer, link; do not import.
