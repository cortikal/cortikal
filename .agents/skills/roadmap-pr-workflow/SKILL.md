---
name: roadmap-pr-workflow
description: Use when starting work on a roadmap item from ROADMAP.md, or whenever the user asks to "work on the next thing"/"pick a task"/build a feature for this project. Turns a roadmap item into a feature branch, a validated commit, and a pull request, and keeps ROADMAP.md and CHANGELOG.md in sync as work completes.
---

# Roadmap-driven branch & PR workflow

`ROADMAP.md` at the repo root is the source of truth for what to build next, organized by the 4 phases from `README.md`. This skill defines how a roadmap item becomes a branch, a commit, and a pull request — it builds on top of the `git-commit-push` skill (commit message conventions, validation, worktree/detached-HEAD handling) rather than replacing it.

## Picking work

1. Read `ROADMAP.md`. If the user hasn't specified which item to work on, ask which unchecked item to prioritize rather than guessing — items vary hugely in size (e.g. "persist created projects" vs. "wire real agent invocation into the state machine").
2. If the chosen item is large, scope the current PR to a meaningfully complete slice of it and say so explicitly, rather than silently doing a partial implementation and checking the box anyway.

## Creating the branch

1. Check current git state first (`git status`, `git branch --show-current`) — this repo's worktrees can be in detached HEAD (see `git-commit-push` skill for why). Branch off current `HEAD` regardless of whether it's detached:
   ```
   git checkout -b <type>/<short-slug>
   ```
2. Name the branch `<type>/<short-slug>` where `<type>` matches the Conventional Commit type the work will use (`feat`, `fix`, `refactor`, `test`, `docs`, `chore`) and `<short-slug>` is a few kebab-case words describing the roadmap item (e.g. `feat/qa-devops-agents`, `fix/project-persistence`).
3. One branch per roadmap item/PR — don't bundle multiple unrelated roadmap items into one branch.

## Doing the work

Follow whichever other skills apply to the files being touched (`agent-role-design`, `arch-md-authoring`, `arch-parser-parity`, `security-scanner-conventions`, `dotnet-test-conventions`, `api-contract-sync`, `distinctive-ui`, etc.). This skill only governs the branch/commit/PR mechanics, not the implementation content.

## Committing

Follow the `git-commit-push` skill's validation and message conventions exactly (Conventional Commits type prefix, imperative capitalized subject, build/test validation before committing, reviewing `git status`/`diff` before staging). Do not skip validation just because the work will go through a PR — a red PR is still a red PR.

## Pushing and opening the PR

1. Push the branch with upstream tracking:
   ```
   git push -u origin <type>/<short-slug>
   ```
2. Check whether the GitHub CLI is available and authenticated:
   ```
   gh auth status
   ```
   - **If `gh` is available and authenticated**, open the PR directly:
     ```
     gh pr create --base main --head <type>/<short-slug> --title "<same style as commit subject>" --body "<summary, plus a line linking to the ROADMAP.md item this addresses>"
     ```
   - **If `gh` is not installed or not authenticated**, do not attempt to call the GitHub REST API directly with credentials you don't have. Instead, report the branch name and construct the compare URL for the user to open manually:
     ```
     https://github.com/<owner>/<repo>/compare/main...<type>/<short-slug>?expand=1
     ```
     (get `<owner>/<repo>` from `git remote get-url origin`). Tell the user the branch is pushed and ready, and give them this link.
3. **Never merge a PR automatically.** Opening the PR is the end of this skill's automated scope — merging is a human decision, even if CI is green, unless the user explicitly asks you to merge it.

## After the PR merges

Only do this once the user confirms the PR is merged (or you've checked with `gh pr view <number> --json state` and it shows `MERGED`):

1. Switch back to `main` conceptually for the roadmap/changelog update — if `main` is checked out in another worktree (`git worktree list`), don't try to `git checkout main` here; instead make this small update on a fresh short-lived branch (`docs/update-roadmap`) and open a tiny follow-up PR for it, or apply it directly to `main` via the `HEAD:main` push technique from the `git-commit-push` skill if you're already up to date with the merge commit.
2. Check off the completed item in `ROADMAP.md` (or split it into remaining sub-items if only partially done).
3. Add the corresponding entry to `CHANGELOG.md` per the `changelog-discipline` skill, under the right phase heading.
