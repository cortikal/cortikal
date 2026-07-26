---
name: git-commit-push
description: Use after completing and validating a coherent change in this repo (a finished feature, fix, or task — not mid-edit). Commits the change with a proper message and pushes it, following this project's Conventional Commits convention and handling this repo's git-worktree setup safely.
---

# Commit and push after a completed change

For roadmap/feature work, prefer the `roadmap-pr-workflow` skill (branch + PR) instead of pushing straight to `main`. Use the direct-push path in this skill only for small, standalone fixes that don't correspond to a roadmap item, or when the user explicitly asks for a direct commit instead of a PR.

This repo's `docs/guides/contributing.md` mandates [Conventional Commits](https://www.conventionalcommits.org/): `feat:`, `fix:`, `docs:`, `style:`, `refactor:`, `test:`, `chore:`. Combine that prefix convention with good general git message style: capitalize the subject after the prefix, use the imperative mood, keep the subject line short (~50 chars, excluding the prefix), no trailing period, and only add a body when it conveys something the subject can't — wrapped at ~72 chars, no repetition of the subject, no raw diff dumps.

## Before committing

1. **Only commit a complete, working change.** Don't commit mid-task or after a single small edit if the broader task described by the user isn't done yet — commit once the change is coherent and self-contained.
2. **Validate first.** Run whatever is feasible for what changed: `dotnet build server/Cortikal.sln` / relevant `dotnet test` project for backend changes, `npm run lint`/`npm run build`/`npm test` (via `turbo`) for frontend changes, or at minimum check `diagnostics` for the files touched. Don't commit code known to fail to build.
3. **Review what's actually staged.** Run `git status` and `git --no-pager diff` (or `git --no-pager diff --staged` after adding) before committing. Stage only files that belong to the change you just made — never blanket `git add -A`/`git add .` without checking status first, since that can sweep in unrelated pre-existing edits or accidental files (build output, `.env`, secrets).
4. **Never commit secrets** (API keys, `.env` contents, tokens). If a scanner or manual check under `Cortikal.Security/Scanners` would flag something, don't commit it.

## Handling this repo's worktree setup

This project directory can be a **linked git worktree checked out in detached HEAD** (verify with `git status` — it will say `Not currently on any branch` if so), separate from the primary checkout where `main` is actually checked out. Detached HEAD needs different handling than a normal branch checkout:

1. Check state first:
   ```
   git status
   git branch --show-current
   ```
2. **If on a normal (non-detached) branch**: commit normally, then push:
   ```
   git push origin <branch-name>
   ```
   or `git push -u origin <branch-name>` if it has no upstream yet.
3. **If in detached HEAD**: do not leave the commit dangling on a detached HEAD (it can become unreachable and get garbage-collected). Prefer creating a new branch from the current HEAD before committing (`git checkout -b <descriptive-branch-name>`, following `roadmap-pr-workflow`'s naming convention if this is roadmap work) and pushing that branch with `git push -u origin <descriptive-branch-name>`. Only push directly with the refspec form `git push origin HEAD:main` for small standalone fixes where a PR is overkill, and only when HEAD is a clean fast-forward of the remote branch (check `git --no-pager log --oneline -n 1 <branch>` matches your current HEAD's parent first).
   - Note `main` may already be checked out in another worktree (run `git worktree list` to check) — you cannot `git checkout main` in that case; use the `HEAD:main` refspec push form instead, or push a differently-named branch.
4. **Never force-push** (`--force`/`-f`) unless the user explicitly asks for it. If a push is rejected as non-fast-forward, stop and tell the user rather than force-pushing or rebasing unprompted.

## Writing the commit message

Follow the format below (no meta-commentary, no raw diff output in the message body):

```
<type>: <Imperative, capitalized summary, no trailing period>

<Optional body, wrapped at ~72 chars, only if it adds real
information not obvious from the subject line or the diff.>
```

Where `<type>` is one of `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, matching this repo's existing history style (see `git --no-pager log --oneline` for examples like `fix: disable HTTPS redirection to prevent CORS fetch failures on local self-signed certificates`).

## Process

1. Confirm the task is actually complete and validated (see "Before committing" above).
2. `git status` / `git --no-pager diff` to review the exact change set.
3. Stage only the relevant files.
4. Write the commit message per the format above.
5. Commit.
6. Determine branch/detached-HEAD state and push using the correct method from "Handling this repo's worktree setup" above.
7. Report back the commit hash/branch and whether the push succeeded — if it failed, show the actual git error rather than assuming success.
