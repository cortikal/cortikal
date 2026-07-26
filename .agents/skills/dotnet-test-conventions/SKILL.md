---
name: dotnet-test-conventions
description: Use whenever adding or editing tests in server/tests (Cortikal.ArchParser.Tests, Cortikal.Orchestrator.Tests, Cortikal.Security.Tests) or scaffolding a new test project for a Cortikal.* backend project. Keeps .NET test structure and naming consistent across the solution.
---

# .NET test conventions for the Cortikal backend

All backend tests use **xUnit** (`xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet.collector` for coverage), targeting `net10.0` with `ImplicitUsings`/`Nullable` enabled — see any `*.Tests.csproj` under `server/tests/` for the reference package set. `<Using Include="Xunit" />` is set globally in each test project, so you don't need `using Xunit;` in every file.

## Reference implementation: `Cortikal.ArchParser.Tests`

This is the one test project in the repo with real, well-structured coverage — treat it as the template for any other test project:

- **One test class per class-under-test**, named `<ClassUnderTest>Tests` (`ParserTests`, `SerializerTests`, `ValidationTests` for `ArchMarkdownParser`, `ArchSerializer`, and the validation logic respectively).
- **Test method naming**: `<MethodOrScenario>_<Condition>_<ExpectedResult>`, e.g. `Parse_SimpleWebApp_ReturnsSuccess`, `Parse_NoFrontmatter_ReturnsFail`, `Parse_Microservices_ExtractsConfig`. Keep this three-part shape (unit under test, scenario, expected outcome) for new tests.
- **Group related tests with a comment banner**, e.g.:
  ```csharp
  // ============================================================
  // Parsing — Valid Documents
  // ============================================================
  ```
  and a second group for invalid/error-path cases. Mirror this grouping (valid-input cases, then invalid/error-path cases) in new test classes.
- **Fixtures over inline strings** for anything beyond a couple of lines: real `.arch.md` documents live in `Fixtures/` (e.g. `simple-web-app.arch.md`, `microservices.arch.md`, `invalid-no-frontmatter.arch.md`) and are loaded via a small `LoadFixture(name)` helper using `Path.Combine(AppContext.BaseDirectory, "Fixtures", name)`. Only use short inline string literals for trivial/one-off invalid-input cases (see `Parse_FrontmatterButNoArchBlock_ReturnsFail` for an example of when an inline string is acceptable).
- **Assert on recognizable substrings for error messages**, not exact strings, so error-message wording can evolve without breaking tests: `Assert.Contains(result.Errors, e => e.Contains("frontmatter"))`.
- Prefer `Assert.NotNull(...)` before dereferencing a nullable result (`result.Document!.Metadata`) — the codebase uses the null-forgiving operator `!` after an explicit `Assert.NotNull` check, not before.

## Where this needs to be established, not just followed

`Cortikal.Orchestrator.Tests` and `Cortikal.Security.Tests` currently only contain the default `UnitTest1.cs` scaffold from `dotnet new xunit` — there is **no real coverage yet** for the agent framework, state machine, or security scanners. When you add tests to these projects:

1. Delete the placeholder `UnitTest1.cs` once you've replaced it with real, appropriately-named test classes — don't leave dead scaffold files alongside real tests.
2. Apply the same naming/structure conventions from `Cortikal.ArchParser.Tests` described above (per-class test files, `Method_Scenario_Expected` naming, valid/invalid grouping).
3. For `Cortikal.Orchestrator.Tests`: test `BaseAgent`/concrete agents by mocking `Kernel` behavior where feasible, and test `OrchestratorStateMachine` transitions by asserting `StateChanged` events fire with the expected `OldState`/`NewState`/`Message` in the right order — don't just test that `CurrentState` ends up correct, since the event stream is what the frontend actually consumes via SignalR.
4. For `Cortikal.Security.Tests`: see the `security-scanner-conventions` skill for what scanner test cases should cover.

## Running tests

```bash
dotnet test server/Cortikal.sln
```
runs the whole solution (per `docs/guides/contributing.md`). Prefer running the specific test project you changed during iteration (`dotnet test server/tests/Cortikal.ArchParser.Tests`) and the full solution before considering a change complete.
