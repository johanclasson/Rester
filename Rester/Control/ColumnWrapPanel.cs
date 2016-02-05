using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Rester.Control
{
    internal class ColumnWrapPanel : Panel
    {
        private const int ColumnTargetSize = 300;

        private int GetColumnCount(Size size)
        {
            var columnCount = (int) Math.Floor(size.Width/ColumnTargetSize);
            if (Children.Count < columnCount)
                columnCount = Children.Count;
            if (columnCount == 0)
                return 1;
            return columnCount;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var columnCount = GetColumnCount(availableSize);
            var columnHeights = new double[columnCount];
            var columnWidth = availableSize.Width/columnCount;
            foreach (var child in Children)
            {
                child.Measure(new Size(columnWidth, availableSize.Height));
                var columnIndex = Array.IndexOf(columnHeights, columnHeights.Min());
                columnHeights[columnIndex] += child.DesiredSize.Height;
            }
            return new Size(availableSize.Width, columnHeights.Max());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var columnCount = GetColumnCount(finalSize);
            var columnHeights = new double[columnCount];
            var columnWidth = finalSize.Width/columnCount;
            foreach (var child in Children)
            {
                var columnIndex = Array.IndexOf(columnHeights, columnHeights.Min());
                var x1 = columnWidth*columnIndex;
                var y1 = columnHeights[columnIndex];
                var x2 = x1 + columnWidth;
                var y2 = y1 + child.DesiredSize.Height;
                columnHeights[columnIndex] = y2;
                var bounds = new Rect(
                    new Point(x1, y1),
                    new Point(x2, y2));
                child.Arrange(bounds);
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}