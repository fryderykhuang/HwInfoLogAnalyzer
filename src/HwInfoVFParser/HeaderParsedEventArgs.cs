using System.Collections.ObjectModel;

namespace HwInfoVFParser;

public class HeaderParsedEventArgs : EventArgs
{
    public IReadOnlyDictionary<int, ReadOnlyObservableCollection<VfEntry>> PerCoreDataSources { get; }

    public HeaderParsedEventArgs(IReadOnlyDictionary<int, ReadOnlyObservableCollection<VfEntry>> perCoreDataSources)
    {
        PerCoreDataSources = perCoreDataSources;
    }
    
}