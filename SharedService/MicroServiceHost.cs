using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace SharedService;

/// <summary>
/// The micro service host class
/// </summary>
public static class MicroServiceHost
{
    private static string _MicroServiceName = string.Empty;
    private static string _PathToContentRoot = string.Empty;
    private static string _BuildNumber = string.Empty;
    private static string _GitInfo = string.Empty;

    public static string BuildNumber
    {
        get
        {
            if (!string.IsNullOrEmpty(_BuildNumber))
            {
                return _BuildNumber;
            }
            _BuildNumber = Environment.GetEnvironmentVariable("ASPNETCORE_BUILD_NUMBER") ?? "unknown";
            if (_BuildNumber.StartsWith(':'))
            {
                _BuildNumber = _BuildNumber[1..];
            }

            return _BuildNumber;
        }
    }

    public static string GitInfo
    {
        get
        {
            if (string.IsNullOrEmpty(_GitInfo))
            {
                _GitInfo = Environment.GetEnvironmentVariable("ASPNETCORE_GIT_INFO") ?? "unknown";
            }

            return _GitInfo;
        }
    }

    public static string MicroServiceName
    {
        get
        {
            if (!string.IsNullOrEmpty(_MicroServiceName))
            {
                return _MicroServiceName;
            }
            var name = Assembly.GetCallingAssembly().GetName();
            _MicroServiceName = name.Name ?? string.Empty;

            return _MicroServiceName;
        }
    }
    
    /// <summary>
    /// Gets the current root path using the specified is service
    /// </summary>
    /// <param name="isService">The is service</param>
    /// <returns>The path to content root</returns>
    public static string GetCurrentRootPath(bool isService = false)
    {
        if (!string.IsNullOrEmpty(_PathToContentRoot))
        {
            return _PathToContentRoot;
        }

        if (isService)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var pathToExe = processModule.FileName;
                _PathToContentRoot = Path.GetDirectoryName(pathToExe) ??
                                     throw new InvalidOperationException("Can't obtain current directory name!");
                return _PathToContentRoot;
            }
        }

        _PathToContentRoot = Directory.GetCurrentDirectory();

        return _PathToContentRoot;
    }
    
    /// <summary>
    /// Create the console host using the specified args
    /// CreateDefaultBuilder with provided arguments
    /// Make application configuration with switch of DEBUG/RELEASE to files appsettings.json and appsettings.Development.json
    /// Set Base Path for Docker container and set env variables
    /// Configuring logging with SimpleConsole and setup NLog for Dependency injection
    /// Call method ConfigureServices with delegate
    /// </summary>
    /// <param name="configureDelegate">The configure delegate</param>
    /// <param name="args">The args. Can be empty or null</param>
    public static IHostBuilder CreateConsoleHost(
        Action<HostBuilderContext, IServiceCollection> configureDelegate,
        string[]? args = null)
    {
        args ??= Array.Empty<string>();
        return Host.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((_, config) =>
                                              {
                                                  // Config file doesn't working in Docker without this settings
                                                  config.SetBasePath(GetCurrentRootPath());
                                                  config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                                                  config.AddEnvironmentVariables();
                                              }
                                             )
                   .ConfigureLogging(
                                     logging =>
                                     {
                                         logging.ClearProviders();
                                         logging.AddSimpleConsole();
                                         logging.SetMinimumLevel(LogLevel.Trace);
                                         logging.AddNLog();
                                     }
                                    )
                   .ConfigureServices(configureDelegate);
    }
}