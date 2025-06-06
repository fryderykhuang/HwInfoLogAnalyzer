using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using PropertyChanged.SourceGenerator;

namespace HwInfoVFParser;

public partial class HwInfoVfLogParser : IDisposable, INotifyPropertyChanged
{
    private readonly Dictionary<int, int> _clockIndices = new();
    private readonly Regex _clockRegex;
    private readonly Dictionary<int, PerCoreData> _perCoreData = new();

    private readonly Dictionary<int, PerCoreStats> _perCoreStats = new();
    private readonly StreamReader _sr;

    private readonly Dictionary<int, int> _voltageIndices = new();
    private readonly Regex _voltageRegex;

    [Notify(Setter.Private)] private int _errorRecords;
    [Notify(Setter.Private)] private int _successRecords;
    [Notify(Setter.Private)] private bool _isHeaderParsed;
    [Notify(Setter.Private)] private int _processedLines;
    [Notify(Setter.Private)] private ParserState _state;
    private readonly double _maxVoltage;
    private readonly double _maxClock;
    private readonly double _minVoltage;
    private readonly double _minClock;


    public HwInfoVfLogParser(string logFilePath, Config? config = null)
    {
        config ??= new Config();
        _maxVoltage = config.MaxVoltage;
        _maxClock = config.MaxClock;
        _minVoltage = config.MinVoltage;
        _minClock = config.MinClock;
        _voltageRegex = new Regex(config.CoreVidHeaderRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        _clockRegex = new Regex(config.CoreClockHeaderRegex, RegexOptions.Compiled | RegexOptions.CultureInvariant);
        _sr = new StreamReader(File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    }

    public void Dispose()
    {
        _sr.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task RunAsync(CancellationToken ct)
    {
        State = ParserState.Running;
        try
        {
            while (true)
            {
                var ln = await _sr.ReadLineAsync(ct);
                if (ln == null)
                {
                    await Task.Delay(500, ct);
                    continue;
                }

                ParseLine(ln);
            }
        }
        finally
        {
            State = ParserState.Stopped;
        }
    }

    private void ParseLine(string ln)
    {
        ProcessedLines++;
        if (!IsHeaderParsed) // header
            try
            {
                if (!ParseHeader(ln)) return;
                HeaderParsed?.Invoke(this, new HeaderParsedEventArgs(this.GetPerCoreDataSource()));
                IsHeaderParsed = true;
            }
            catch
            {
                // ignored
            }
        else
            try
            {
                ParseDataLine(ln);
                SuccessRecords++;
                LineParsed?.Invoke(this,
                    new LineParsedEventArgs(ProcessedLines, SuccessRecords, ErrorRecords, _perCoreStats));
            }
            catch
            {
                ErrorRecords++;
            }
    }

    public event EventHandler<HeaderParsedEventArgs>? HeaderParsed;
    public event EventHandler<LineParsedEventArgs>? LineParsed;

    public IReadOnlyDictionary<int, ReadOnlyObservableCollection<VfEntry>> GetPerCoreDataSource()
    {
        var ret = new Dictionary<int, ReadOnlyObservableCollection<VfEntry>>();
        lock (_perCoreData)
        {
            foreach (var (coreIdx, data) in _perCoreData)
                ret[coreIdx] = new ReadOnlyObservableCollection<VfEntry>(data.Observable);
        }

        return ret;
    }

    private static readonly Regex DataRegex = new(@"\s*\d+(\.\d+)?\s*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private void ParseDataLine(string ln)
    {
        var fields = ln.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var (coreIdx, voltageFieldIdx) in _voltageIndices)
        {
            var clockFieldIdx = _clockIndices[coreIdx];
            var voltageField = fields[voltageFieldIdx];
            var clockField = fields[clockFieldIdx];

            if (!DataRegex.IsMatch(voltageField) || !DataRegex.IsMatch(clockField)) throw new FormatException();

            var voltage = double.Parse(fields[voltageFieldIdx]);
            if (voltage > _maxVoltage || voltage < _minVoltage) throw new ArgumentOutOfRangeException(nameof(voltage));
            var clock = double.Parse(fields[clockFieldIdx]);
            if (clock > _maxClock || clock < _minClock) throw new ArgumentOutOfRangeException(nameof(clock));

            lock (_perCoreData)
            {
                var data = _perCoreData[coreIdx];
                var stats = _perCoreStats[coreIdx];
                var entry = new VfEntry(voltage, clock);
                var added = data.VfEntries.Add(entry);
                if (!added)
                {
                    stats.DuplicateCount++;
                }
                else
                {
                    stats.RecordCount = data.VfEntries.Count;
                    data.Observable.Add(entry);
                }
            }
        }
    }

    private bool ParseHeaderField(int fieldIndex, string field, Regex regex, Dictionary<int, int> map)
    {
        var match = regex.Match(field);
        if (!match.Success) return false;
        var coreIdx = int.Parse(match.Groups["index"].Value);
        map.Add(coreIdx, fieldIndex);
        lock (_perCoreData)
        {
            if (!_perCoreData.ContainsKey(coreIdx)) _perCoreData.Add(coreIdx, new PerCoreData());
            if (!_perCoreStats.ContainsKey(coreIdx)) _perCoreStats.Add(coreIdx, new PerCoreStats());
        }

        return true;
    }

    private bool ParseHeader(string ln)
    {
        var voltParsed = false;
        var clockParsed = false;
        var fields = ln.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (var index = 0; index < fields.Length; index++)
        {
            var field = fields[index];
            if (ParseHeaderField(index, field, _voltageRegex, _voltageIndices))
            {
                voltParsed = true;
                continue;
            }

            if (ParseHeaderField(index, field, _clockRegex, _clockIndices))
            {
                clockParsed = true;
                continue;
            }
        }

        var ok = voltParsed && clockParsed;
        if (!ok)
        {
            _perCoreData.Clear();
            _perCoreStats.Clear();
        }

        return ok;
    }
}