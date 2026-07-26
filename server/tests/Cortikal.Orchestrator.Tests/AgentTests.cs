using Cortikal.Core.Enums;
using Cortikal.Core.Interfaces;
using Cortikal.Orchestrator.Agents;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cortikal.Orchestrator.Tests;

public class AgentTests
{
    private readonly Mock<ILlmRouter> _mockRouter;
    private readonly Mock<ILogger<ArchitectAgent>> _mockArchitectLogger;
    private readonly Mock<ILogger<FrontendDevAgent>> _mockFrontendLogger;
    private readonly Mock<ILogger<BackendDevAgent>> _mockBackendLogger;
    private readonly Mock<ILogger<QAEngineerAgent>> _mockQaLogger;
    private readonly Mock<ILogger<DevOpsEngineerAgent>> _mockDevOpsLogger;

    public AgentTests()
    {
        _mockRouter = new Mock<ILlmRouter>();
        _mockArchitectLogger = new Mock<ILogger<ArchitectAgent>>();
        _mockFrontendLogger = new Mock<ILogger<FrontendDevAgent>>();
        _mockBackendLogger = new Mock<ILogger<BackendDevAgent>>();
        _mockQaLogger = new Mock<ILogger<QAEngineerAgent>>();
        _mockDevOpsLogger = new Mock<ILogger<DevOpsEngineerAgent>>();
    }

    [Fact]
    public void QAEngineerAgent_Has_Correct_Role()
    {
        var agent = new QAEngineerAgent(_mockRouter.Object, _mockQaLogger.Object);
        Assert.Equal(AgentRole.QualityAssurance, agent.Role);
    }

    [Fact]
    public void DevOpsEngineerAgent_Has_Correct_Role()
    {
        var agent = new DevOpsEngineerAgent(_mockRouter.Object, _mockDevOpsLogger.Object);
        Assert.Equal(AgentRole.DevOps, agent.Role);
    }
}
