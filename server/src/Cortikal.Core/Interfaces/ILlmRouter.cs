using Microsoft.SemanticKernel;

namespace Cortikal.Core.Interfaces;

public interface ILlmRouter
{
    /// <summary>
    /// Gets an instance of Semantic Kernel configured for the optimal provider
    /// based on the complexity and requirements of the task.
    /// </summary>
    Kernel GetKernelForTask(string taskType);
}
