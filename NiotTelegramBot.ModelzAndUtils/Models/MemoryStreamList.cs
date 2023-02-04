using System.IO;
using File = System.IO.File;

namespace NiotTelegramBot.ModelzAndUtils.Models;

public class MemoryStreamList : IDisposable
{
    public IList<TelegramFile> List { get; } = new List<TelegramFile>();
    private bool _IsDisposed;

    public void AddTelegramFile(IReadOnlyList<TelegramFile> files)
    {
        foreach (var telegramFile in files)
        {
            var stream = new FileStream(telegramFile.TmpPath,
                                        FileMode.Open);
            var fileStream = new TelegramFile(
                                              telegramFile.Type,
                                              telegramFile.TmpPath,
                                              telegramFile.OriginalName,
                                              telegramFile.Info,
                                              stream);
            List.Add(fileStream);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_IsDisposed)
        {
            return;
        }

        _IsDisposed = true;
        foreach (var stream in List)
        {
            stream.Media.Content?.Dispose();
            File.Delete(stream.TmpPath);
        }
    }
}