---
name: security-scanner-conventions
description: Use whenever adding or modifying a scanner under server/src/Cortikal.Security/Scanners (secret detection, vulnerability patterns, or new scan categories) or its tests. Keeps scanners consistent with the ISecurityScanner interface and existing patterns.
---

# Writing security scanners

`server/src/Cortikal.Security/Scanners/` currently has two scanners, both intentionally simple placeholders to be hardened later:

- `SecretScanner.cs` — regex-based detection of hardcoded API keys/secrets/tokens/passwords.
- `VulnerabilityScanner.cs` — naive substring matching for dangerous patterns (`eval(`, `exec(`, a crude SQL-injection shape).

Both implement `ISecurityScanner` from `Cortikal.Core.Interfaces`:

```csharp
public interface ISecurityScanner
{
    string ScannerName { get; }
    Task<IEnumerable<string>> ScanCodeAsync(string filePath, string codeContent);
}
```

## Conventions to follow

1. **Implement `ISecurityScanner` directly** — don't introduce a parallel base class or a different method shape. `ScanCodeAsync(filePath, codeContent)` takes the file path (for reporting) and raw content, and returns a flat list of human-readable issue strings.
2. **Issue message format**: existing scanners produce messages like `"Potential hardcoded secret found in {filePath}"` and `"Potential vulnerability (dangerous pattern '{pattern}') found in {filePath}"`. Keep new scanners' messages in this "Potential <issue> found in {filePath}" style (optionally with the specific pattern/rule quoted) so consumers (e.g. a future report UI) can parse/display them consistently.
3. **`ScannerName`** should be a short, human-readable label (`"Secret Scanner"`, `"Vulnerability Scanner"`) used for grouping/reporting — follow that naming style for new scanners (e.g. `"Dependency Scanner"`, `"License Scanner"`).
4. Return `Task.FromResult<IEnumerable<string>>(issues)` for synchronous regex/string checks (matches existing style); use real `async`/`await` only if the scanner does actual I/O (e.g. calling an external vulnerability DB).
5. Comments in the existing scanners explicitly flag them as **mocks for Phase 4** (`// Simple regex for mocking Phase 4`, `// Mock vulnerability detection`). If you're hardening a scanner into something production-ready, remove that comment and make sure the improved logic is actually more robust (e.g. replace the naive SQL-injection substring match with something that won't produce excessive false positives/negatives) rather than just relabeling the mock.

## Registration and consumption

- New scanners must be registered for DI wherever `ISecurityScanner` implementations are collected (check `Cortikal.Api/Program.cs` for the existing registration pattern before assuming one) so they're picked up alongside `SecretScanner`/`VulnerabilityScanner` without callers needing to know the concrete list.
- If a scanner needs configuration (e.g. custom regex patterns, an allowlist), follow the existing `Cortikal:*` configuration-section convention used elsewhere (e.g. `Cortikal:OpenAi:ApiKey` in `appsettings.json`) rather than hardcoding values that should be configurable.

## Tests

Add coverage under `server/tests/Cortikal.Security.Tests/`. That project currently only has the default `UnitTest1.cs` scaffold — there's no established pattern yet, so when you add real scanner tests, establish one deliberately: test class named after the scanner under test (e.g. `SecretScannerTests`), one `[Fact]` per meaningfully distinct input (a clean file with no issues, a file with an obvious secret, edge cases like a key-shaped string that's actually a false positive you want to avoid flagging). See the `dotnet-test-conventions` skill for naming/structure conventions to reuse from `Cortikal.ArchParser.Tests`.
