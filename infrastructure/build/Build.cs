using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Pulumi;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions(
    "continuous",
    GitHubActionsImage.UbuntuLatest,
    //On = new[] { GitHubActionsTrigger.Push },
    OnPushBranches = new[] { "main" },
    OnPullRequestBranches = new[] { "main" },
    OnWorkflowDispatchOptionalInputs = new string[] {},
    InvokedTargets = new[] { nameof(Compile) },
    AutoGenerate = false,
    ImportSecrets = new[] { nameof(PulumiAccessToken) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    AbsolutePath InfrastructureDirectory => RootDirectory / "infrastructure" / "base" / "pulumi";

    [Parameter][Secret] readonly string PulumiAccessToken;

    public static int Main () => Execute<Build>(x => x.ProvisionInfra);

    GitHubActions GitHubActions => GitHubActions.Instance;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });
    
    Target Print => _ => _
    .Executes(() =>
    {
        Log.Information("Branch = {Branch}", GitHubActions?.Ref ?? "Null");
        Log.Information("Commit = {Commit}", GitHubActions?.Sha ?? "Null");
    });

    Target ProvisionInfra => _ => _
    .Description("Provision the infrastructure on Azure")
    .Executes(() =>
    {
        PulumiTasks.PulumiPreview(_ => _
            .SetCwd(InfrastructureDirectory)
            .SetStack("base-dev"));
            //.EnableSkipPreview()
            //.EnableDebug());
    });

}
