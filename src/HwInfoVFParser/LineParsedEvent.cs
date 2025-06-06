namespace HwInfoVFParser;

public class LineParsedEventArgs: EventArgs
{
    public LineParsedEventArgs(int processedLines, int successCount, int errorCount,
        IReadOnlyDictionary<int, PerCoreStats> perCoreData)
    {
        ProcessedLines = processedLines;
        SuccessCount = successCount;
        ErrorCount = errorCount;
        PerCoreDataPoints = perCoreData;
    }

    public int ProcessedLines { get; }
    public int SuccessCount { get; }
    public int ErrorCount { get; }
    public IReadOnlyDictionary<int, PerCoreStats> PerCoreDataPoints { get; }
}