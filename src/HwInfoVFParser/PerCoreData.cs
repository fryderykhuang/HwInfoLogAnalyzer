using System.Collections.ObjectModel;

namespace HwInfoVFParser;

record PerCoreData
{
    public HashSet<VfEntry> VfEntries { get; } = new();
    public ObservableCollection<VfEntry> Observable { get; } = new();
}