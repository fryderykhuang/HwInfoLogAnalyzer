﻿using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using HorizontalAlignment = OxyPlot.HorizontalAlignment;

namespace HwInfoLogAnalyzerWinForms;

internal class CustomLegend : LegendBase
{
    private OxyRect legendBox;

    /// <summary>
    /// Initializes a new insance of the Legend class.
    /// </summary>
    public CustomLegend()
    {
        this.IsLegendVisible = true;
        this.legendBox = new OxyRect();
        this.Key = null;
        this.GroupNameFont = null;
        this.GroupNameFontWeight = FontWeights.Normal;
        this.GroupNameFontSize = double.NaN;

        this.LegendTitleFont = null;
        this.LegendTitleFontSize = double.NaN;
        this.LegendTitleFontWeight = FontWeights.Bold;
        this.LegendFont = null;
        this.LegendFontSize = double.NaN;
        this.LegendFontWeight = FontWeights.Normal;
        this.LegendSymbolLength = 16;
        this.LegendSymbolMargin = 4;
        this.LegendPadding = 8;
        this.LegendColumnSpacing = 8;
        this.LegendItemSpacing = 24;
        this.LegendLineSpacing = 0;
        this.LegendMargin = 8;

        this.LegendBackground = OxyColors.Undefined;
        this.LegendBorder = OxyColors.Undefined;
        this.LegendBorderThickness = 1;

        this.LegendTextColor = OxyColors.Automatic;
        this.LegendTitleColor = OxyColors.Automatic;

        this.LegendMaxWidth = double.NaN;
        this.LegendMaxHeight = double.NaN;
        this.LegendPlacement = LegendPlacement.Inside;
        this.LegendPosition = LegendPosition.RightTop;
        this.LegendOrientation = LegendOrientation.Vertical;
        this.LegendItemOrder = LegendItemOrder.Normal;
        this.LegendItemAlignment = HorizontalAlignment.Left;
        this.LegendSymbolPlacement = LegendSymbolPlacement.Left;

        this.ShowInvisibleSeries = true;

        this.SeriesInvisibleTextColor = OxyColor.FromAColor(64, this.LegendTextColor);

        this.SeriesPosMap = new Dictionary<Series, OxyRect>();

        this.Selectable = true;
        this.SelectionMode = OxyPlot.SelectionMode.Single;
    }

    /// <summary>
    /// Override for legend hit test.
    /// </summary>
    /// <param name="args">Arguments passed to the hit test</param>
    /// <returns>The hit test results.</returns>
    protected override HitTestResult LegendHitTest(HitTestArguments args)
    {
        if (!this.IsLegendVisible || !this.PlotModel.IsLegendVisible)
        {
            return null;
        }

        ScreenPoint point = args.Point;
        if (this.IsPointInLegend(point))
        {
            if (this.SeriesPosMap != null && this.SeriesPosMap.Count > 0)
            {
                foreach (KeyValuePair<Series, OxyRect> kvp in this.SeriesPosMap)
                {
                    if (kvp.Value.Contains(point))
                    {
                        if (this.ShowInvisibleSeries)
                        {
                            kvp.Key.IsVisible = !kvp.Key.IsVisible;
                            this.PlotModel.InvalidatePlot(false);
                        }

                        return new HitTestResult(this, point, kvp.Key);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets or sets the group name font.
    /// </summary>
    public string GroupNameFont { get; set; }

    /// <summary>
    /// Gets or sets the group name font size.
    /// </summary>
    public double GroupNameFontSize { get; set; }

    /// <summary>
    /// Gets or sets the group name font weight.
    /// </summary>
    public double GroupNameFontWeight { get; set; }

    private Dictionary<Series, OxyRect> SeriesPosMap { get; set; }

    /// <summary>
    /// Gets or sets the textcolor of invisible series.
    /// </summary>
    public OxyColor SeriesInvisibleTextColor { get; set; }

    /// <summary>
    /// Checks if a screen point is within the legend boundaries.
    /// </summary>
    /// <param name="point">A screen point.</param>
    /// <returns>A value indicating whether the point is inside legend boundaries or not.</returns>
    public bool IsPointInLegend(ScreenPoint point)
    {
        return this.legendBox.Contains(point);
    }

    /////////////////////////// Rendering
    /// <summary>
    /// Makes the LegendOrientation property safe.
    /// </summary>
    /// <remarks>If Legend is positioned left or right, force it to vertical orientation</remarks>
    public override void EnsureLegendProperties()
    {
        switch (this.LegendPosition)
        {
            case LegendPosition.LeftTop:
            case LegendPosition.LeftMiddle:
            case LegendPosition.LeftBottom:
            case LegendPosition.RightTop:
            case LegendPosition.RightMiddle:
            case LegendPosition.RightBottom:
                if (this.LegendOrientation == LegendOrientation.Horizontal)
                {
                    this.LegendOrientation = LegendOrientation.Vertical;
                }

                break;
        }
    }

    /// <summary>
    /// Renders or measures the legends.
    /// </summary>
    /// <param name="rc">The render context.</param>
    public override void RenderLegends(IRenderContext rc)
    {
        this.RenderOrMeasureLegends(rc, this.LegendArea);
    }

    /// <summary>
    /// Measures the legend area and gets the legend size.
    /// </summary>
    /// <param name="rc">The rendering context.</param>
    /// <param name="availableLegendArea">The area available to legend.</param>
    public override OxySize GetLegendSize(IRenderContext rc, OxySize availableLegendArea)
    {
        var availableLegendWidth = availableLegendArea.Width;
        var availableLegendHeight = availableLegendArea.Height;

        // Calculate the size of the legend box
        var legendSize = this.MeasureLegends(rc,
            new OxySize(Math.Max(0, availableLegendWidth), Math.Max(0, availableLegendHeight)));

        // Ensure legend size is valid
        legendSize = new OxySize(Math.Max(0, legendSize.Width), Math.Max(0, legendSize.Height));

        return legendSize;
    }

    /// <summary>
    /// Gets the rectangle of the legend box.
    /// </summary>
    /// <param name="legendSize">Size of the legend box.</param>
    /// <returns>A rectangle.</returns>
    public override OxyRect GetLegendRectangle(OxySize legendSize)
    {
        double top = 0;
        double left = 0;
        if (this.LegendPlacement == LegendPlacement.Outside)
        {
            switch (this.LegendPosition)
            {
                case LegendPosition.LeftTop:
                case LegendPosition.LeftMiddle:
                case LegendPosition.LeftBottom:
                    left = this.PlotModel.PlotAndAxisArea.Left - legendSize.Width - this.LegendMargin;
                    break;
                case LegendPosition.RightTop:
                case LegendPosition.RightMiddle:
                case LegendPosition.RightBottom:
                    left = this.PlotModel.PlotAndAxisArea.Right + this.LegendMargin;
                    break;
                case LegendPosition.TopLeft:
                case LegendPosition.TopCenter:
                case LegendPosition.TopRight:
                    top = this.PlotModel.PlotAndAxisArea.Top - legendSize.Height - this.LegendMargin;
                    break;
                case LegendPosition.BottomLeft:
                case LegendPosition.BottomCenter:
                case LegendPosition.BottomRight:
                    top = this.PlotModel.PlotAndAxisArea.Bottom + this.LegendMargin;
                    break;
            }

            var bounds = this.AllowUseFullExtent
                ? this.PlotModel.PlotAndAxisArea
                : this.PlotModel.PlotArea;

            switch (this.LegendPosition)
            {
                case LegendPosition.TopLeft:
                case LegendPosition.BottomLeft:
                    left = bounds.Left;
                    break;
                case LegendPosition.TopRight:
                case LegendPosition.BottomRight:
                    left = bounds.Right - legendSize.Width;
                    break;
                case LegendPosition.LeftTop:
                case LegendPosition.RightTop:
                    top = bounds.Top;
                    break;
                case LegendPosition.LeftBottom:
                case LegendPosition.RightBottom:
                    top = bounds.Bottom - legendSize.Height;
                    break;
                case LegendPosition.LeftMiddle:
                case LegendPosition.RightMiddle:
                    top = (bounds.Top + bounds.Bottom - legendSize.Height) * 0.5;
                    break;
                case LegendPosition.TopCenter:
                case LegendPosition.BottomCenter:
                    left = (bounds.Left + bounds.Right - legendSize.Width) * 0.5;
                    break;
            }
        }
        else
        {
            switch (this.LegendPosition)
            {
                case LegendPosition.LeftTop:
                case LegendPosition.LeftMiddle:
                case LegendPosition.LeftBottom:
                    left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                    break;
                case LegendPosition.RightTop:
                case LegendPosition.RightMiddle:
                case LegendPosition.RightBottom:
                    left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                    break;
                case LegendPosition.TopLeft:
                case LegendPosition.TopCenter:
                case LegendPosition.TopRight:
                    top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                    break;
                case LegendPosition.BottomLeft:
                case LegendPosition.BottomCenter:
                case LegendPosition.BottomRight:
                    top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                    break;
            }

            switch (this.LegendPosition)
            {
                case LegendPosition.TopLeft:
                case LegendPosition.BottomLeft:
                    left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                    break;
                case LegendPosition.TopRight:
                case LegendPosition.BottomRight:
                    left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                    break;
                case LegendPosition.LeftTop:
                case LegendPosition.RightTop:
                    top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                    break;
                case LegendPosition.LeftBottom:
                case LegendPosition.RightBottom:
                    top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                    break;

                case LegendPosition.LeftMiddle:
                case LegendPosition.RightMiddle:
                    top = (this.PlotModel.PlotArea.Top + this.PlotModel.PlotArea.Bottom - legendSize.Height) * 0.5;
                    break;
                case LegendPosition.TopCenter:
                case LegendPosition.BottomCenter:
                    left = (this.PlotModel.PlotArea.Left + this.PlotModel.PlotArea.Right - legendSize.Width) * 0.5;
                    break;
            }
        }

        return new OxyRect(left, top, legendSize.Width, legendSize.Height);
    }

    /// <summary>
    /// Renders the legend for the specified series.
    /// </summary>
    /// <param name="rc">The render context.</param>
    /// <param name="s">The series.</param>
    /// <param name="rect">The position and size of the legend.</param>
    private void RenderLegend(IRenderContext rc, Series s, OxyRect rect)
    {
        var actualItemAlignment = this.LegendItemAlignment;
        if (this.LegendOrientation == LegendOrientation.Horizontal)
        {
            // center/right alignment is not supported for horizontal orientation
            actualItemAlignment = HorizontalAlignment.Left;
        }

        double x = rect.Left;
        switch (actualItemAlignment)
        {
            case HorizontalAlignment.Center:
                x = (rect.Left + rect.Right) / 2;
                if (this.LegendSymbolPlacement == LegendSymbolPlacement.Left)
                {
                    x -= (this.LegendSymbolLength + this.LegendSymbolMargin) / 2;
                }
                else
                {
                    x -= (this.LegendSymbolLength + this.LegendSymbolMargin) / 2;
                }

                break;
            case HorizontalAlignment.Right:
                x = rect.Right;

                // if (LegendSymbolPlacement == LegendSymbolPlacement.Right)
                x -= this.LegendSymbolLength + this.LegendSymbolMargin;
                break;
        }

        if (this.LegendSymbolPlacement == LegendSymbolPlacement.Left)
        {
            x += this.LegendSymbolLength + this.LegendSymbolMargin;
        }

        double y = rect.Top;
        var maxsize = new OxySize(Math.Max(rect.Width - this.LegendSymbolLength - this.LegendSymbolMargin, 0),
            rect.Height);
        var actualLegendFontSize =
            double.IsNaN(this.LegendFontSize) ? this.PlotModel.DefaultFontSize : this.LegendFontSize;
        var legendTextColor = s.IsVisible ? this.LegendTextColor : this.SeriesInvisibleTextColor;

        rc.SetToolTip(s.ToolTip);
        var textSize = rc.DrawMathText(
            new ScreenPoint(x, y),
            s.Title,
            legendTextColor.GetActualColor(this.PlotModel.TextColor),
            this.LegendFont ?? this.PlotModel.DefaultFont,
            actualLegendFontSize,
            this.LegendFontWeight,
            0,
            actualItemAlignment,
            VerticalAlignment.Top,
            maxsize,
            true);

        this.SeriesPosMap.Add(s, new OxyRect(new ScreenPoint(x, y), textSize));
        double x0 = x;
        switch (actualItemAlignment)
        {
            case HorizontalAlignment.Center:
                x0 = x - (textSize.Width * 0.5);
                break;
            case HorizontalAlignment.Right:
                x0 = x - textSize.Width;
                break;
        }

        if (s.IsVisible)
        {
            var symbolRect =
                new OxyRect(
                    this.LegendSymbolPlacement == LegendSymbolPlacement.Right
                        ? x0 + textSize.Width + this.LegendSymbolMargin
                        : x0 - this.LegendSymbolMargin - this.LegendSymbolLength,
                    rect.Top,
                    this.LegendSymbolLength,
                    textSize.Height);

            s.RenderLegend(rc, symbolRect);
        }

        rc.SetToolTip(null);
    }

    /// <summary>
    /// Measures the legends.
    /// </summary>
    /// <param name="rc">The render context.</param>
    /// <param name="availableSize">The available size for the legend box.</param>
    /// <returns>The size of the legend box.</returns>
    private OxySize MeasureLegends(IRenderContext rc, OxySize availableSize)
    {
        return this.RenderOrMeasureLegends(rc, new OxyRect(0, 0, availableSize.Width, availableSize.Height), true);
    }

    /// <summary>
    /// Renders or measures the legends.
    /// </summary>
    /// <param name="rc">The render context.</param>
    /// <param name="rect">Provides the available size if measuring, otherwise it provides the position and size of the legend.</param>
    /// <param name="measureOnly">Specify if the size of the legend box should be measured only (not rendered).</param>
    /// <returns>The size of the legend box.</returns>
    private OxySize RenderOrMeasureLegends(IRenderContext rc, OxyRect rect, bool measureOnly = false)
    {
        // Render background and border around legend
        if (!measureOnly && rect.Width > 0 && rect.Height > 0)
        {
            this.legendBox = rect;
            rc.DrawRectangle(
                rect,
                this.LegendBackground,
                this.LegendBorder,
                this.LegendBorderThickness,
                this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
        }

        double availableWidth = rect.Width;
        double availableHeight = rect.Height;

        double x = this.LegendPadding;
        double top = this.LegendPadding;

        var size = new OxySize();

        var actualLegendFontSize =
            double.IsNaN(this.LegendFontSize) ? this.PlotModel.DefaultFontSize : this.LegendFontSize;
        var actualLegendTitleFontSize =
            double.IsNaN(this.LegendTitleFontSize) ? actualLegendFontSize : this.LegendTitleFontSize;
        var actualGroupNameFontSize =
            double.IsNaN(this.GroupNameFontSize) ? actualLegendFontSize : this.GroupNameFontSize;

        // Render/measure the legend title
        if (!string.IsNullOrEmpty(this.LegendTitle))
        {
            OxySize titleSize;
            if (measureOnly)
            {
                titleSize = rc.MeasureMathText(
                    this.LegendTitle,
                    this.LegendTitleFont ?? this.PlotModel.DefaultFont,
                    actualLegendTitleFontSize,
                    this.LegendTitleFontWeight);
            }
            else
            {
                titleSize = rc.DrawMathText(
                    new ScreenPoint(rect.Left + x, rect.Top + top),
                    this.LegendTitle,
                    this.LegendTitleColor.GetActualColor(this.PlotModel.TextColor),
                    this.LegendTitleFont ?? this.PlotModel.DefaultFont,
                    actualLegendTitleFontSize,
                    this.LegendTitleFontWeight,
                    0,
                    HorizontalAlignment.Left,
                    VerticalAlignment.Top,
                    null,
                    true);
            }

            top += titleSize.Height;
            size = new OxySize(x + titleSize.Width + this.LegendPadding, top + titleSize.Height);
        }

        double y = top;

        double lineHeight = 0;

        // tolerance for floating-point number comparisons
        const double Epsilon = 1e-3;

        // the maximum item with in the column being rendered (only used for vertical orientation)
        double maxItemWidth = 0;

        var items = this.LegendItemOrder == LegendItemOrder.Reverse
            ? this.PlotModel.Series.Reverse().Where(s => s.RenderInLegend && s.LegendKey == this.Key)
            : this.PlotModel.Series.Where(s => s.RenderInLegend && s.LegendKey == this.Key);

        List<string> itemGroupNames = new List<string>();
        foreach (Series s in items)
        {
            if (!itemGroupNames.Contains(s.SeriesGroupName))
            {
                itemGroupNames.Add(s.SeriesGroupName);
            }
        }

        // Clear the series position map.
        this.SeriesPosMap.Clear();

        // When orientation is vertical and alignment is center or right, the items cannot be rendered before
        // the max item width has been calculated. Render the items for each column, and at the end.
        var seriesToRender = new Dictionary<Series, OxyRect>();
        Action renderItems = () =>
        {
            List<string> usedGroupNames = new List<string>();
            foreach (var sr in seriesToRender)
            {
                var itemRect = sr.Value;
                var itemSeries = sr.Key;

                if (!string.IsNullOrEmpty(itemSeries.SeriesGroupName) &&
                    !usedGroupNames.Contains(itemSeries.SeriesGroupName))
                {
                    usedGroupNames.Add(itemSeries.SeriesGroupName);
                    var groupNameTextSize = rc.MeasureMathText(itemSeries.SeriesGroupName,
                        this.GroupNameFont ?? this.PlotModel.DefaultFont, actualGroupNameFontSize,
                        this.GroupNameFontWeight);
                    double ypos = itemRect.Top;
                    double xpos = itemRect.Left;
                    if (this.LegendOrientation == LegendOrientation.Vertical)
                        ypos -= (groupNameTextSize.Height + this.LegendLineSpacing / 2);
                    else
                        xpos -= (groupNameTextSize.Width + this.LegendItemSpacing / 2);
                    rc.DrawMathText(
                        new ScreenPoint(xpos, ypos),
                        itemSeries.SeriesGroupName,
                        this.LegendTitleColor.GetActualColor(this.PlotModel.TextColor),
                        this.GroupNameFont ?? this.PlotModel.DefaultFont,
                        actualGroupNameFontSize,
                        this.GroupNameFontWeight,
                        0,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Top,
                        null,
                        true);
                }

                double rwidth = availableWidth;
                if (itemRect.Left + rwidth + this.LegendPadding > rect.Left + availableWidth)
                {
                    rwidth = rect.Left + availableWidth - itemRect.Left - this.LegendPadding;
                }

                double rheight = itemRect.Height;
                if (rect.Top + rheight + this.LegendPadding > rect.Top + availableHeight)
                {
                    rheight = rect.Top + availableHeight - rect.Top - this.LegendPadding;
                }

                var r = new OxyRect(itemRect.Left, itemRect.Top, Math.Max(rwidth, 0), Math.Max(rheight, 0));

                this.RenderLegend(rc, itemSeries, r);
            }

            usedGroupNames.Clear();
            seriesToRender.Clear();
        };

        if (!measureOnly)
        {
            rc.PushClip(rect);
        }

        foreach (var g in itemGroupNames)
        {
            var itemGroup = items.Where(i => i.SeriesGroupName == g);
            OxySize groupNameTextSize = new OxySize(0, 0);
            if (itemGroup.Count() > 0 && !string.IsNullOrEmpty(g))
            {
                groupNameTextSize = rc.MeasureMathText(g, this.GroupNameFont ?? this.PlotModel.DefaultFont,
                    actualGroupNameFontSize, this.GroupNameFontWeight);
                if (this.LegendOrientation == LegendOrientation.Vertical)
                    y += groupNameTextSize.Height;
                else
                    x += groupNameTextSize.Width;
            }

            int count = 0;
            foreach (var s in itemGroup)
            {
                // Skip series with empty title
                if (string.IsNullOrEmpty(s.Title) || !s.RenderInLegend)
                {
                    continue;
                }

                // Skip invisible series if we are not configured to show them
                if (!s.IsVisible && !this.ShowInvisibleSeries)
                {
                    continue;
                }

                var textSize = rc.MeasureMathText(s.Title, this.LegendFont ?? this.PlotModel.DefaultFont,
                    actualLegendFontSize, this.LegendFontWeight);
                double itemWidth = this.LegendSymbolLength + this.LegendSymbolMargin + textSize.Width;
                double itemHeight = textSize.Height;

                if (this.LegendOrientation == LegendOrientation.Horizontal)
                {
                    // Add spacing between items
                    if (x > this.LegendPadding)
                    {
                        x += this.LegendItemSpacing;
                    }

                    // Check if the item is too large to fit within the available width
                    if (x + itemWidth > availableWidth - this.LegendPadding + Epsilon)
                    {
                        // new line
                        x = this.LegendPadding;
                        if (count == 0 && groupNameTextSize.Width > 0)
                            x += (groupNameTextSize.Width + this.LegendItemSpacing);
                        y += lineHeight + this.LegendLineSpacing;
                        lineHeight = 0;
                    }

                    // Update the max size of the current line
                    lineHeight = Math.Max(lineHeight, textSize.Height);

                    if (!measureOnly)
                    {
                        seriesToRender.Add(s, new OxyRect(rect.Left + x, rect.Top + y, itemWidth, itemHeight));
                    }

                    x += itemWidth;

                    x = Math.Max(groupNameTextSize.Width, x);
                    // Update the max width and height of the legend box
                    size = new OxySize(Math.Max(size.Width, x), Math.Max(size.Height, y + textSize.Height));
                }
                else
                {
                    if (y + itemHeight > availableHeight - this.LegendPadding + Epsilon)
                    {
                        renderItems();

                        y = top + groupNameTextSize.Height;
                        x += maxItemWidth + this.LegendColumnSpacing;
                        maxItemWidth = 0;
                    }

                    if (!measureOnly)
                    {
                        seriesToRender.Add(s, new OxyRect(rect.Left + x, rect.Top + y, itemWidth, itemHeight));
                    }

                    y += itemHeight + this.LegendLineSpacing;

                    // Update the max size of the items in the current column
                    maxItemWidth = Math.Max(maxItemWidth, itemWidth);

                    // Update the max width and height of the legend box
                    size = new OxySize(Math.Max(size.Width, x + itemWidth), Math.Max(size.Height, y));
                }

                count++;
            }

            renderItems();
        }

        if (!measureOnly)
        {
            rc.PopClip();
        }

        if (size.Width > 0)
        {
            size = new OxySize(size.Width + this.LegendPadding, size.Height);
        }

        if (size.Height > 0)
        {
            size = new OxySize(size.Width, size.Height + this.LegendPadding);
        }

        if (size.Width > availableWidth)
        {
            size = new OxySize(availableWidth, size.Height);
        }

        if (size.Height > availableHeight)
        {
            size = new OxySize(size.Width, availableHeight);
        }

        if (!double.IsNaN(this.LegendMaxWidth) && size.Width > this.LegendMaxWidth)
        {
            size = new OxySize(this.LegendMaxWidth, size.Height);
        }

        if (!double.IsNaN(this.LegendMaxHeight) && size.Height > this.LegendMaxHeight)
        {
            size = new OxySize(size.Width, this.LegendMaxHeight);
        }

        return size;
    }
}