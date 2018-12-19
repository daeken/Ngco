using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ngco.Widgets
{
    public class Grid : BaseContainer
    {
        List<Section> layoutRows    = new List<Section>();
        List<Section> layoutColumns = new List<Section>();

        int rows;
        int columns;
        bool autoRows;
        bool autoColumns;

        public bool AutoColumns { get => autoColumns || Columns == 0; set => autoColumns = value; }
        public bool AutoRows { get => autoRows || Rows == 0; set => autoRows = value; }
        public int Columns { get => columns; set => columns = value; }
        public int Rows { get => rows; set => rows = value; }

        public override string[] PropertyKeys { get; } = new string[] { "rows", "columns" };

        public override string[] ChildPropertyKeys => new string[] { "grid.row", "grid.column" };

        public override void Load(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("rows", out string rows))
            {
                if (rows != "auto")
                {
                    if (!int.TryParse(rows, out int Row))
                    {
                        var dimensions = rows.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        foreach(var dimension in dimensions)
                        {
                            Section row = new Section();

                            if (int.TryParse(dimension, out int length))
                                row.Length = int.Parse(dimension.Replace("dp", string.Empty));
                            else if (dimension != "auto" && dimension != "*")
                                throw new NotSupportedException($"Value '{dimension}' is not valid for property rows");

                            layoutRows.Add(row);
                        }

                        Rows = layoutRows.Count;
                    }
                    else
                        Rows = Row;
                }
                else
                    AutoRows = true;
            }
            if (properties.TryGetValue("columns", out string columns))
            {
                if (columns != "auto")
                {
                    if (!int.TryParse(columns, out int Column))
                    {
                        var dimensions = rows.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        foreach (var dimension in dimensions)
                        {
                            Section column = new Section();

                            if (int.TryParse(dimension, out int length))
                                column.Length = int.Parse(dimension.Replace("dp", string.Empty));
                            else if (dimension != "auto" && dimension != "*")
                                throw new NotSupportedException($"Value '{dimension}' is not valid for property rows");

                            layoutColumns.Add(column);
                        }

                        Columns = layoutColumns.Count;
                    }
                    else
                        Columns = Column;
                }
                else
                    AutoColumns = true;
            }
        }

        public override void OnLayout(Rect region)
        {
            Point currentRowPosition = region.TopLeft;

            var firstRow = layoutRows.FirstOrDefault();

            if (firstRow != null)
            {
                var currentColumn = layoutColumns.FirstOrDefault();

                currentColumn.Bounds = new Rect(region.TopLeft, currentColumn.Bounds.Size);

                for (int i = 1; i < layoutColumns.Count; i++)
                {
                    var column = layoutColumns[i];

                    var topLeft = new Point(currentColumn.Bounds.TopLeft.X + currentColumn.Bounds.Size.Width, 
                        currentColumn.Bounds.TopLeft.Y);

                    column.Bounds = new Rect(topLeft, column.Bounds.Size);

                    currentColumn = column;
                }

                foreach (var row in layoutRows)
                {
                    foreach (var widget in row.Widgets)
                    {
                        var column = layoutColumns.Find(x => x.Widgets.Contains(widget));

                        Point currentWidgetPosition = new Point(column.Bounds.TopLeft.X, currentRowPosition.Y);

                        switch (widget.Layout.Alignment.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                currentWidgetPosition.X += widget.Layout.Margin.Left;
                                break;
                            case HorizontalAlignment.Right:
                                currentWidgetPosition.X += column.Bounds.Size.Width - widget.BoundingBox.Size.Width - widget.Layout.Margin.Right;
                                break;
                            case HorizontalAlignment.Center:
                                currentWidgetPosition.X += widget.Layout.Margin.Left;
                                break;
                        }

                        switch (widget.Layout.Alignment.VerticalAlignment)
                        {
                            case VerticalAlignment.Up:
                            case VerticalAlignment.Center:
                                currentWidgetPosition.Y += widget.Layout.Margin.Up;
                                break;
                            case VerticalAlignment.Down:
                                currentWidgetPosition.Y += column.Bounds.Size.Height - widget.BoundingBox.Size.Height - widget.Layout.Margin.Down;
                                break;
                        }

                        widget.SetPosition(currentWidgetPosition);
                        widget.BoundingBox.ClipTo(region);
                        widget.OnLayout(widget.BoundingBox);
                    }

                    currentRowPosition.Y += row.Bounds.Size.Height;
                }
            }
        }

        public override void OnMeasure(Size region)
        {
            int columnsToPlot = (int)(AutoColumns ? Math.Sqrt(Children.Count) : Columns);
            int rowsToPlot    = (int)(AutoRows ? Math.Ceiling((double)Children.Count / columnsToPlot) : Rows);

            while (columnsToPlot > layoutColumns.Count)
                layoutColumns.RemoveAt(layoutColumns.Count - 1);

            while (rowsToPlot > layoutRows.Count)
                layoutRows.RemoveAt(layoutRows.Count - 1);

            layoutColumns.AddRange(Enumerable.Repeat(new Section(), columnsToPlot - layoutColumns.Count).Select(x => new Section()).ToList());
            layoutRows   .AddRange(Enumerable.Repeat(new Section(), rowsToPlot - layoutRows.Count)      .Select(x => new Section()).ToList());
            

            if(columnsToPlot > 0)
            {
                int columnIndex = 0;
                int rowIndex    = 0;

                foreach(var child in Children)
                {
                    bool rowPlaced = false;
                    bool columnPlaced = false;

                    if(child.TryGetPropertyValue("grid.row", out string gridRow))
                    {
                        if(int.TryParse(gridRow, out int childRow))
                        {
                            if (childRow < rowsToPlot)
                            {
                                layoutRows[childRow].Add(child);

                                rowPlaced = true;
                            }
                        }
                    }
                    else
                        layoutRows[rowIndex].Add(child);

                    if (child.TryGetPropertyValue("grid.column", out string gridColumn))
                    {
                        if (int.TryParse(gridColumn, out int childColumn))
                        {
                            if (childColumn < columns)
                            {
                                layoutColumns[childColumn].Add(child);
                                columnPlaced = true;
                            }
                        }
                    }
                    else
                        layoutColumns[columnIndex % columnsToPlot].Add(child);

                    if (!columnPlaced)
                        columnIndex++;

                    if (!rowPlaced)
                        if (columnIndex % columnsToPlot == 0)
                            rowIndex++;
                }
            }

            int divisionHeight = region.Height / layoutRows.Count;

            foreach(var row in layoutRows)
            {
                foreach (var widget in row.Widgets)
                {
                    var column = layoutColumns.Find(x => x.Widgets.Contains(widget));

                    widget.OnMeasure(new Size(region.Width, divisionHeight));

                    Size widgetSize = widget.BoundingBox.Size;

                    switch (widget.Layout.Alignment.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            widgetSize.Width += widget.Layout.Margin.Left;
                            break;
                        case HorizontalAlignment.Right:
                            widgetSize.Width += widget.Layout.Margin.Right;
                            break;
                        case HorizontalAlignment.Center:
                            widgetSize.Width += widget.Layout.Margin.Right + widget.Layout.Margin.Left;
                            break;
                    }

                    switch (widget.Layout.Alignment.VerticalAlignment)
                    {
                        case VerticalAlignment.Up:
                            widgetSize.Height += widget.Layout.Margin.Up;
                            break;
                        case VerticalAlignment.Down:
                            widgetSize.Height += widget.Layout.Margin.Down;
                            break;
                        case VerticalAlignment.Center:
                            widgetSize.Height += widget.Layout.Margin.Up + widget.Layout.Margin.Down;
                            break;
                    }

                    row.Bounds = new Rect(new Point(),new Size(row.Bounds.Size.Width + widgetSize.Width,
                        Math.Max(row.Bounds.Size.Height, widgetSize.Height)));

                    if (row.Length > 0)
                    {
                        row.Bounds = new Rect(row.Bounds.TopLeft, new Size()
                        {
                            Height = row.Length,
                            Width = row.Bounds.Size.Width
                        });
                    }

                    column.Bounds = new Rect(new Point(), new Size(Math.Max(column.Bounds.Size.Width, widgetSize.Width),
                        column.Bounds.Size.Height + widgetSize.Height));

                    if (column.Length > 0)
                    {
                        column.Bounds = new Rect(column.Bounds.TopLeft, new Size()
                        {
                            Height = row.Bounds.Size.Height,
                            Width = row.Length
                        });
                    }
                }

                BoundingBox = new Rect(BoundingBox.TopLeft, new Size(Math.Max(BoundingBox.Size.Width, row.Bounds.Size.Width),
                        BoundingBox.Size.Height + row.Bounds.Size.Height));
            }

            ApplyLayoutSize();
        }

        public override void Render(RICanvas canvas)
        {
            foreach (var widget in this)
            {
                widget.Render(canvas);
            }
        }
    }
}
