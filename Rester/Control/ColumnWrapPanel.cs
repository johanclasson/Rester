using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using static Rester.Control.Constants;

namespace Rester.Control
{
    internal static class Constants
    {
        //TODO: Remove constants construct and make them settable from Xaml on the ColumnWrapPanel
        public const double ButtonMaxSize = 110.0;
        public const double AdditionalMargin = 80;
        public const double ButtonMargin = 4;
        public const int ColumnTargetSize = 300;
    }

    internal class UpdateButtonSizeMessage
    {
        public double Size { get; }
        public double ColumnWidth { get; }

        public UpdateButtonSizeMessage(double size, double columnWidth)
        {
            Size = size;
            ColumnWidth = columnWidth;
        }
    }

    internal class ButtonSizeCalculator
    {
        internal class ButtonSizePoint
        {
            public ButtonSizePoint(double columnWidth, int buttonCount)
            {
                ColumnWidth = columnWidth;
                ButtonCount = buttonCount;
                Size = (columnWidth - GetMargin(buttonCount))/buttonCount;
            }

            public double Size { get; }
            public double ColumnWidth { get; }
            public int ButtonCount { get; }
        }

        private List<ButtonSizePoint> Points { get; } = new List<ButtonSizePoint>();

        public ButtonSizeCalculator()
        {
            AddFirstPoint();
        }

        private void AddFirstPoint()
        {
            Points.Add(new ButtonSizePoint(GetMargin(1), 1));
        }

        private void AddPoints()
        {
            ButtonSizePoint lastPoint = Points.Last();
            double columnWidth = GetMargin(lastPoint.ButtonCount) + lastPoint.ButtonCount*ButtonMaxSize;
            Points.Add(new ButtonSizePoint(columnWidth, lastPoint.ButtonCount));
            Points.Add(new ButtonSizePoint(columnWidth, lastPoint.ButtonCount + 1));
        }

        private static double GetMargin(int buttonCount)
        {
            return AdditionalMargin + buttonCount*ButtonMargin;
        }

        public double GetButtonSize(double columnWidth)
        {
            while (Points.Last().ColumnWidth <= columnWidth)
            {
                AddPoints();
            }
            ButtonSizePoint p1 = null;
            ButtonSizePoint p2 = null;
            foreach (ButtonSizePoint point in Points)
            {
                if (point.ColumnWidth > columnWidth)
                {
                    p2 = point;
                    break;
                }
                p1 = point;
            }
            if (p1 == null)
                return 0.0;
            Debug.Assert(p2 != null, "p2 != null");
            double k = (p2.Size - p1.Size)/(p2.ColumnWidth - p1.ColumnWidth);
            double m = p1.Size - k*p1.ColumnWidth;
            return k*columnWidth + m;
        }
    }

    internal class ColumnWrapPanel : Panel
    {
        private ButtonSizeCalculator Calculator { get; } = new ButtonSizeCalculator();

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
            double buttonSize = Calculator.GetButtonSize(columnWidth);
            Messenger.Default.Send(new UpdateButtonSizeMessage(buttonSize, columnWidth));
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