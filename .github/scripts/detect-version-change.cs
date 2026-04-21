#:property PublishAot=false

using System.Diagnostics;
using System.Xml.Linq;

if (args.Length != 1)
{
    throw new InvalidOperationException("Expected exactly one argument: the project file path.");
}

var projectPath = args[0];
var gitProjectPath = projectPath.Replace('\\', '/');

var currentVersion = ReadVersion(File.ReadAllText(projectPath));
if (string.IsNullOrWhiteSpace(currentVersion))
{
    throw new InvalidOperationException("Package version is missing in the project file.");
}

WriteOutput("current_version", currentVersion);

var previousCommit = RunGit("rev-parse", "HEAD^1");
if (string.IsNullOrWhiteSpace(previousCommit))
{
    WriteOutput("previous_version", string.Empty);
    WriteOutput("version_changed", "true");
    return;
}

var previousProjectContent = RunGit("show", $"{previousCommit}:{gitProjectPath}");
if (string.IsNullOrWhiteSpace(previousProjectContent))
{
    WriteOutput("previous_version", string.Empty);
    WriteOutput("version_changed", "true");
    return;
}

var previousVersion = ReadVersion(previousProjectContent);
WriteOutput("previous_version", previousVersion ?? string.Empty);
WriteOutput("version_changed", (!string.Equals(previousVersion, currentVersion, StringComparison.Ordinal)).ToString().ToLowerInvariant());

static string? ReadVersion(string xmlContent)
{
    var document = XDocument.Parse(xmlContent.TrimStart('\uFEFF').Trim());
    return document.Root?
        .Elements("PropertyGroup")
        .Elements("Version")
        .Select(element => element.Value?.Trim())
        .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
}

static string? RunGit(params string[] arguments)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        }
    };

    foreach (var argument in arguments)
    {
        process.StartInfo.ArgumentList.Add(argument);
    }

    process.Start();
    var standardOutput = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    return process.ExitCode == 0 ? standardOutput.Trim() : null;
}

static void WriteOutput(string name, string value)
{
    var outputPath = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");
    if (!string.IsNullOrWhiteSpace(outputPath))
    {
        File.AppendAllLines(outputPath, [$"{name}={value}"]);
        return;
    }

    Console.WriteLine($"{name}={value}");
}
