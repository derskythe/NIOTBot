using System.IO;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace NiotTelegramBot.ModelzAndUtils.Models;

public class TelegramFile : InputMediaBase, IAlbumInputMedia
{
    public string TmpPath { get; }
    public FileInfo Info { get; }
    public string OriginalName { get; }
    /// <inheritdoc />
    public override InputMediaType Type { get; }

    public TelegramFile(string originalLocation, FileInfo fileInfo)
        : base(originalLocation)
    {
        Type = InputMediaType.Photo;
        TmpPath = Path.GetTempFileName();
        Info = fileInfo;
        OriginalName = Info.Name;
        Type = GetMediaType(Info.Extension);

        File.Copy(originalLocation, TmpPath);
    }

    public TelegramFile(InputMediaType type, string tmpPath, string originalName, FileInfo info, Stream stream)
        : base(new InputMedia(stream, originalName))
    {
        Type = type;
        TmpPath = tmpPath;
        Info = info;
        OriginalName = originalName;
    }

    private static InputMediaType GetMediaType(string extension)
    {
        if (extension.IsEqual(".txt") && extension.IsEqual(".log"))
        {
            return InputMediaType.Document;
        }
        else
        {
            return InputMediaType.Photo;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"TmpPath: {TmpPath}, OriginalName: {OriginalName}, MediaType: {Type.AsString()}";
    }
}