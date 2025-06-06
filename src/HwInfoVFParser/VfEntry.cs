namespace HwInfoVFParser;

public record VfEntry : IEquatable<VfEntry>, IComparable<VfEntry>, IComparable
{
    public int CompareTo(VfEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var voltageComparison = Voltage.CompareTo(other.Voltage);
        if (voltageComparison != 0) return voltageComparison;
        return Clock.CompareTo(other.Clock);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is VfEntry other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(VfEntry)}");
    }

    public static bool operator <(VfEntry? left, VfEntry? right)
    {
        return Comparer<VfEntry>.Default.Compare(left, right) < 0;
    }

    public static bool operator >(VfEntry? left, VfEntry? right)
    {
        return Comparer<VfEntry>.Default.Compare(left, right) > 0;
    }

    public static bool operator <=(VfEntry? left, VfEntry? right)
    {
        return Comparer<VfEntry>.Default.Compare(left, right) <= 0;
    }

    public static bool operator >=(VfEntry? left, VfEntry? right)
    {
        return Comparer<VfEntry>.Default.Compare(left, right) >= 0;
    }

    public virtual bool Equals(VfEntry? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Voltage.Equals(other.Voltage) && Clock.Equals(other.Clock);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Voltage, Clock);
    }

    public VfEntry(double voltage, double clock)
    {
        Voltage = voltage;
        Clock = clock;
    }

    public double Voltage { get; set; }
    public double Clock { get; set; }
}