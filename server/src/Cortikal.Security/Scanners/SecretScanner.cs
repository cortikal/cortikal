using System.Text.RegularExpressions;
using Cortikal.Core.Interfaces;

namespace Cortikal.Security.Scanners;

public class SecretScanner : ISecurityScanner
{
    public string ScannerName => "Secret Scanner";

    // Simple regex for mocking Phase 4
    private static readonly Regex ApiKeyRegex = new Regex(@"(?i)(api[_-]?key|secret|token|password)[\s:=]+['""]?[a-zA-Z0-9_\-]{16,}['""]?");

    public Task<IEnumerable<string>> ScanCodeAsync(string filePath, string codeContent)
    {
        var issues = new List<string>();

        var matches = ApiKeyRegex.Matches(codeContent);
        if (matches.Count > 0)
        {
            issues.Add($"Potential hardcoded secret found in {filePath}");
        }

        return Task.FromResult<IEnumerable<string>>(issues);
    }
}
