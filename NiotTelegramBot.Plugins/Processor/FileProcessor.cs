using System.Text;
using NiotTelegramBot.ModelzAndUtils;
using NiotTelegramBot.ModelzAndUtils.Enums;
using NiotTelegramBot.ModelzAndUtils.Interfaces;
using NiotTelegramBot.ModelzAndUtils.Models;
using NiotTelegramBot.ModelzAndUtils.Settings;
using File = System.IO.File;

namespace NiotTelegramBot.Plugins.Processor;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class FileProcessor : IPluginProcessor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The log
    /// </summary>
    private readonly ILogger<FileProcessor> Log;

    /// <inheritdoc />
    public string Name => nameof(FileProcessor);

    /// <inheritdoc />
    public Emoji Icon { get; set; } = Emoji.Robot;

    /// <inheritdoc />
    public string NameForUser { get; set; } = i18n.FileProcessor;

    /// <inheritdoc />
    public TelegramMenu[] Menu { get; set; } = Array.Empty<TelegramMenu>();

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    public SourceProcessors SourceSourceProcessor { get; }

    /// <inheritdoc />
    public int Order { get; set; }

    private bool _InvalidConfig;
    // ReSharper disable NotAccessedField.Local
    private readonly IChatUsers _ChatUsers;
    private readonly IReadOnlyDictionary<string, IPluginDataSource> _DataSources;
    private readonly IReadOnlyDictionary<MessageType, FileProcessorSettings> _Filetypes;
    private readonly CancellationToken _CancellationToken;
    private readonly ICacheService _Cache;
    // ReSharper restore NotAccessedField.Local

    /// <inheritdoc />
    public bool Healthcheck()
    {
        // if (!_NoUsers && (_ChatUsers.Users == null || _ChatUsers.Users.Count == 0))
        // {
        //     _NoUsers = true;
        //     Log.LogWarning("Chat users list is empty!");
        // }

        if (!_InvalidConfig && _DataSources.Count == 0)
        {
            _InvalidConfig = true;
            Log.LogError("DataSources is empty!");
        }

        return true;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Tick()
    {
        // TODO: change this thing from Tick to FileWatcher
        if (!Enabled)
        {
            return Task.FromResult(new ProcessorResponseValue());
        }

        if (_Filetypes.Count == 0)
        {
            Enabled = true;
            return Task.FromResult(new ProcessorResponseValue());
        }

        var errorsText = new StringBuilder();
        var response = new List<OutgoingMessage>();
        var filesToMove = new Dictionary<string, string>();
        foreach (var (_, value) in _Filetypes)
        {
            try
            {
                var foundFiles = new List<string>();
                foreach (var extension in value.Extensions)
                {
                    foundFiles.AddRange(
                                        Directory.EnumerateFiles(
                                                                 value.InputDir,
                                                                 extension,
                                                                 SearchOption.TopDirectoryOnly));
                }

                if (foundFiles.Count == 0)
                {
                    continue;
                }

                var attached = new List<TelegramFile>(foundFiles.Count);
                foreach (var filePath in foundFiles)
                {
                    // var fileBody = Array.Empty<byte>();
                    // {
                    //     await File.ReadAllBytesAsync(filePath, _CancellationToken);
                    // }
                    //
                    // if (fileBody.Length == 0)
                    // {
                    //     Log.LogWarning("{FilePath} Length is 0 , ignoring", filePath);
                    //     continue;
                    // }
                    var fileInfo = new FileInfo(filePath);
                    if (IsFileLocked(fileInfo))
                    {
                        Log.LogWarning("R/W problem with file: {File}", filePath);
                        continue;
                    }
                    if (fileInfo.Length == 0)
                    {
                        Log.LogWarning("File has zero Length: {File}! Just move to output", filePath);
                        filesToMove.Add(filePath, value.OutputDir);
                        continue;
                    }
                    // File more than 45 Mb
                    if (fileInfo.Length > 47_185_920)
                    {
                        Log.LogWarning("File more than 45 MB! Just move to output. Size: {Size} {File}", fileInfo.Length / 1024 / 1024, filePath);
                        filesToMove.Add(filePath, value.OutputDir);
                        continue;
                    }

                    var telegramFile = new TelegramFile(filePath, fileInfo);
                    attached.Add(telegramFile);
                    filesToMove.Add(filePath, value.OutputDir);
                }

                var sampleFile = attached.FirstOrDefault();
                if (sampleFile == null)
                {
                    Log.LogWarning("No files was attached during media scan for {Extension}",
                                   value.Extensions.GetStringFromArraySingleLine());
                    continue;
                }

                var message = new OutgoingMessage(UsersPermissions.Read,
                                                  attached,
                                                  sampleFile.Type,
                                                  SourceSourceProcessor);
                response.Add(message);
            }
            catch (Exception exp)
            {
                var expMessage = $"Error processing filetype: {value}, Message: {exp.Message}";
                Log.LogError(exp, "{Message}", expMessage);
                errorsText.AppendLine(expMessage);
            }
        }

        foreach (var path in filesToMove)
        {
            var fileName = Path.GetFileName(path.Key);
            try
            {
                File.Move(path.Key, Path.Combine(path.Value, fileName));
            }
            catch (Exception exp)
            {
                var expMessage = $"Error moving file: {fileName}, Message: {exp.Message}, trying to delete file!";
                Log.LogError(exp, "{Message}", expMessage);
                errorsText.AppendLine(expMessage);
                try
                {
                    File.Delete(path.Key);
                }
                catch (Exception innerExp)
                {
                    expMessage = $"Error delete file: {fileName}, Message: {innerExp.Message}. Please check logs!";
                    Log.LogError(innerExp, "{Message}", expMessage);
                    errorsText.AppendLine(expMessage);
                }
            }
        }

        return Task.FromResult(errorsText.Length > 0 ?
                                   new ProcessorResponseValue(
                                                              errorsText.ToString(),
                                                              response) :
                                   new ProcessorResponseValue(response));
    }

    private static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using var stream = file.Open(FileMode.Open,
                                         FileAccess.ReadWrite,
                                         FileShare.None);
            stream.Close();
        }
        catch (UnauthorizedAccessException)
        {
            // But we can't do anything with file
            return true;
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }

    /// <inheritdoc />
    public Task<ProcessorResponseValue> Process(MessageProcess message)
    {
        return Task.FromResult(new ProcessorResponseValue());
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public FileProcessor(
        ILoggerFactory loggerFactory,
        ProcessorSettings settings,
        IReadOnlyDictionary<string, IPluginDataSource> dataSources,
        IChatUsers chatUsers,
        ICacheService cache,
        IReadOnlyDictionary<MessageType, OutgoingInputSettings> inputSettings,
        CancellationToken cancellationToken)
    {
        Log = loggerFactory.CreateLogger<FileProcessor>();
        _CancellationToken = cancellationToken;
        _DataSources = dataSources;
        _ChatUsers = chatUsers;
        _Cache = cache;
        SourceSourceProcessor = Enums.Parse<SourceProcessors>(GetType().Name);
        Order = settings.Order;

        var fileTypes = new Dictionary<MessageType, FileProcessorSettings>();
        foreach (var (key, inputValue) in inputSettings)
        {
            if (!inputValue.Enabled)
            {
                continue;
            }

            var filetypes = inputValue.Options.SplitFileTypes();
            if (filetypes.Length == 0)
            {
                continue;
            }

            if (string.IsNullOrEmpty(inputValue.InputDir))
            {
                Log.LogWarning("Input directory for {Key} value is empty. Skip", key);
                continue;
            }

            if (string.IsNullOrEmpty(inputValue.OutputDir))
            {
                Log.LogWarning("Output directory for {Key} value is empty. Skip", key);
                continue;
            }

            var inputDir = inputValue.InputDir.GetAbsolutePath();
            var outputDir = inputValue.OutputDir.GetAbsolutePath();

            if (!Directory.Exists(inputDir))
            {
                Log.LogWarning("Input directory {Directory} for {Key} doesn't exists. Skip", inputDir, key);
                continue;
            }

            if (!Directory.Exists(outputDir))
            {
                Log.LogWarning("Input directory {Directory} for {Key} doesn't exists. Skip", outputDir, key);
                continue;
            }

            fileTypes.Add(key,
                          new FileProcessorSettings(key,
                                                    inputDir,
                                                    outputDir,
                                                    filetypes));
        }

        _Filetypes = fileTypes;
        if (_Filetypes.Count == 0)
        {
            // We don't need to run if we can't find files
            Enabled = false;
        }

        Enabled = settings.Enabled;

        Log.LogInformation("Status: {Status}",
                           Enabled ?
                               Constants.STARTED :
                               Constants.STAY_SLEPPING);
    }
}