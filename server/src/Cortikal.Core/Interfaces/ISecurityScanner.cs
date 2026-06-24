namespace Cortikal.Core.Interfaces;

public interface ISecurityScanner
{
    string ScannerName { get; }
    
    /// <summary>
    /// Scans the given code for vulnerabilities or secrets.
    /// Returns a list of identified issues, or empty if clean.
    /// </summary>
    Task<IEnumerable<string>> ScanCodeAsync(string filePath, string codeContent);
}
