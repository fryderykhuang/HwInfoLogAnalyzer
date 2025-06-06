namespace HwInfoVFParser;

public record PerCoreStats
{
    public int RecordCount { get; internal set; }
    public int DuplicateCount { get; internal set; }
}