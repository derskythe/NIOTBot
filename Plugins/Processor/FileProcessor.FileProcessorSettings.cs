namespace Plugins.Processor;

// ReSharper disable once ClassNeverInstantiated.Global
/// <inheritdoc />
public sealed partial class FileProcessor
{
    private class FileProcessorSettings
    {
        public string InputDir { get; }
        public string OutputDir { get; }
        public string[] Extensions { get; }
        public MessageType Type { get; }

        // ReSharper disable once UnusedMember.Local
        public FileProcessorSettings()
        {
            InputDir = string.Empty;
            OutputDir = string.Empty;
            Extensions = Array.Empty<string>();
            Type = MessageType.Unknown;
        }

        public FileProcessorSettings(MessageType type, string inputDir, string outputDir, string[] extensions)
        {
            InputDir = inputDir.GetAbsolutePath();
            OutputDir = outputDir.GetAbsolutePath();
            Extensions = extensions;
            Type = type;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"Type: {Type.AsString()}, InputDir: {InputDir}, OutputDir: {OutputDir}, Extensions: {Extensions.GetStringFromArraySingleLine()}";
        }
    }
}