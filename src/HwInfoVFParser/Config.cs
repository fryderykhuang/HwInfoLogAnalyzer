namespace HwInfoVFParser;

public record Config
{
    public string CoreVidHeaderRegex { get; set; } = @"Core\s+(?<index>\d+)\s+VID";
    public string CoreClockHeaderRegex { get; set; } = @"Core\s+(?<index>\d+)\s+Clock";
    public double MaxVoltage { get; set; } = 2;
    public double MinVoltage { get; set; } = 0.2;
    public double MaxClock { get; set; } = 10000;
    public double MinClock { get; set; } = 50;
}