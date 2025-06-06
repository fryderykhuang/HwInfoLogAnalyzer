using System.ComponentModel;
using HwInfoVFParser;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SelectionMode = OxyPlot.SelectionMode;

namespace HwInfoLogAnalyzerWinForms;

public partial class MainForm : Form
{
    private HwInfoVfLogParser _vfParser;
    private CancellationTokenSource _cts;
    private PlotModel _plotModel;
    private PlotController _plotController;
    private string _filePath;

    public MainForm()
    {
        InitializeComponent();
        InitializePlotModel();
    }

    private void InitializePlotModel()
    {
        _plotModel = new PlotModel()
        {
            Title = "VF Chart",
            Axes =
            {
                new LinearAxis() {Position = AxisPosition.Bottom, Title = "Voltage(V)",},
                new LinearAxis() {Position = AxisPosition.Left, Title = "Clock(Mhz)"}
            },
            PlotType = PlotType.XY,
        };
        this.plotView.Model = _plotModel;
        // this.plotView.ActualController.AddMouseManipulator(this.plotView, new MouseManipulator(this.plotView), null);
        // this.plotView.ActualController.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.None, PlotCommands.HoverTrack);
        this.plotView.ActualController.BindMouseEnter(PlotCommands.HoverSnapTrack);
        this.plotView.ActualController.BindMouseDown(OxyMouseButton.Left, OxyModifierKeys.None,
            new DelegatePlotCommand<OxyMouseDownEventArgs>((
                view, controller, args) =>
            {
                var plotModel = view.ActualModel;
                if (plotModel is not null && args.HitTestResult?.Element is LegendBase &&
                    args.HitTestResult.Item is Series thisSeries)
                {
                    thisSeries.IsVisible = !thisSeries.IsVisible;
                    plotModel.InvalidatePlot(false);
                }
            }));
        this.plotView.ActualController.BindMouseDown(OxyMouseButton.Left,
            OxyModifierKeys.Control,
            new DelegatePlotCommand<OxyMouseDownEventArgs>((
                view, controller, args) =>
            {
                var plotModel = view.ActualModel;
                if (plotModel is not null && args.HitTestResult?.Element is LegendBase &&
                    args.HitTestResult.Item is Series thisSeries)
                {
                    foreach (var series in plotModel.Series)
                    {
                        series.IsVisible = thisSeries == series;
                    }

                    plotModel.InvalidatePlot(false);
                }
            }));
        this.plotView.ActualController.BindMouseDown(OxyMouseButton.Left,
            OxyModifierKeys.Control | OxyModifierKeys.Shift,
            new DelegatePlotCommand<OxyMouseDownEventArgs>((
                view, controller, args) =>
            {
                var plotModel = view.ActualModel;
                if (plotModel is not null && args.HitTestResult?.Element is LegendBase &&
                    args.HitTestResult.Item is Series thisSeries)
                {
                    foreach (var series in plotModel.Series)
                    {
                        series.IsVisible = thisSeries != series;
                    }

                    plotModel.InvalidatePlot(false);
                }
            }));

        // _plotController = new PlotController();
        //
        // // // Bind custom mouse down command for legend clicks
        // _plotController.AddMouseManipulator(this.plotView, new MouseManipulator(this.plotView), null);
        //
        // this.plotView.Controller = _plotController;
    }

    private void openLogFileButton_Click(object sender, EventArgs e)
    {
        using OpenFileDialog openFileDialog = new();
        openFileDialog.Filter = "Csv files (*.csv)|*.csv|All files (*.*)|*.*";
        openFileDialog.Title = "Open Log File";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            _filePath = openFileDialog.FileName;
            if (_filePath is not null) this.reloadFileButton.Enabled = true;
            this.RebuildParser();
        }
    }

    private void RebuildParser()
    {
        this.runButton.Enabled = false;
        this.logFilePathLabel.Text = _filePath;
        if (_vfParser != null)
        {
            Stop();
            try
            {
                _vfParser.PropertyChanged -= OnParserPropertyChanged;
                _vfParser.LineParsed -= vfParserOnLineParsed;
                _vfParser.HeaderParsed -= VfParserOnHeaderParsed;
                _vfParser.Dispose();
            }
            catch
            {
            }
        }

        if (_filePath is null) return;

        try
        {
            _vfParser = new HwInfoVfLogParser(_filePath);
            _vfParser.PropertyChanged += OnParserPropertyChanged;
            _vfParser.LineParsed += vfParserOnLineParsed;
            _vfParser.HeaderParsed += VfParserOnHeaderParsed;
            this.runButton.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing parser: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void VfParserOnHeaderParsed(object? sender, HeaderParsedEventArgs e)
    {
        _plotModel.Series.Clear();
        _plotModel.Legends.Clear();
        foreach (var (coreIndex, dataSource) in e.PerCoreDataSources)
        {
            _plotModel.Legends.Add(new CustomLegend()
            {
                LegendTitle = $"Core {coreIndex}",
                Selectable = true,
                SelectionMode = SelectionMode.Multiple,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightBottom
            });
            _plotModel.Series.Add(new ScatterSeries()
            {
                Title = $"Core {coreIndex}",
                DataFieldX = "Voltage",
                DataFieldY = "Clock",
                ItemsSource = dataSource,
                MarkerType = MarkerType.Diamond,
                MarkerSize = 4,
            });
        }
    }

    private void vfParserOnLineParsed(object? sender, LineParsedEventArgs lineParsedEventArgs)
    {
        this._plotModel.InvalidatePlot(true);
    }

    private void OnParserPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.Invoke(() =>
        {
            if (e.PropertyName is nameof(HwInfoVfLogParser.SuccessRecords)
                or nameof(HwInfoVfLogParser.ErrorRecords) or nameof(HwInfoVfLogParser.ProcessedLines)
                or nameof(HwInfoVfLogParser.State))
            {
                this.parserInfoLabel.Text =
                    $"[{_vfParser.State}] Processed: {_vfParser.ProcessedLines} Success: {_vfParser.SuccessRecords} Error: {_vfParser.ErrorRecords}";
            }
        });
    }

    public bool IsRunning => _cts is not null;

    private void Stop()
    {
        if (!IsRunning) return;
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
        this.runButton.Text = "Start";
    }

    private void Start()
    {
        if (IsRunning) return;
        _cts = new CancellationTokenSource();
        Task.Run(() => _vfParser.RunAsync(_cts.Token));
        this.runButton.Text = "Stop";
    }

    private void runButton_Click(object sender, EventArgs e)
    {
        if (IsRunning)
            Stop();
        else
            Start();
    }

    private void reloadFileButton_Click(object sender, EventArgs e)
    {
        RebuildParser();
    }
}