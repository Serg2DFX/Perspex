﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Input;
using Perspex.Interactivity;
using Perspex.Metadata;

namespace Perspex.Controls.Primitives
{
    public class Track : Control
    {
        public static readonly PerspexProperty<double> MinimumProperty =
            RangeBase.MinimumProperty.AddOwner<Track>();

        public static readonly PerspexProperty<double> MaximumProperty =
            RangeBase.MaximumProperty.AddOwner<Track>();

        public static readonly PerspexProperty<double> ValueProperty =
            RangeBase.ValueProperty.AddOwner<Track>();

        public static readonly PerspexProperty<double> ViewportSizeProperty =
            ScrollBar.ViewportSizeProperty.AddOwner<Track>();

        public static readonly PerspexProperty<Orientation> OrientationProperty =
            ScrollBar.OrientationProperty.AddOwner<Track>();

        public static readonly PerspexProperty<Thumb> ThumbProperty =
            PerspexProperty.Register<Track, Thumb>("Thumb");

        static Track()
        {
            AffectsArrange(MinimumProperty);
            AffectsArrange(MaximumProperty);
            AffectsArrange(ValueProperty);
            AffectsMeasure(OrientationProperty);
        }

        public Track()
        {
            GetObservableWithHistory(ThumbProperty).Subscribe(val =>
            {
                if (val.Item1 != null)
                {
                    val.Item1.DragDelta -= ThumbDragged;
                }

                ClearVisualChildren();

                if (val.Item2 != null)
                {
                    val.Item2.DragDelta += ThumbDragged;
                    AddVisualChild(val.Item2);
                }
            });
        }

        public double Minimum
        {
            get { return GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double ViewportSize
        {
            get { return GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        [Content]
        public Thumb Thumb
        {
            get { return GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var thumb = Thumb;

            if (thumb != null)
            {
                thumb.Measure(availableSize);

                if (Orientation == Orientation.Horizontal)
                {
                    return new Size(0, thumb.DesiredSize.Height);
                }
                else
                {
                    return new Size(thumb.DesiredSize.Width, 0);
                }
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var thumb = Thumb;

            if (thumb != null)
            {
                var range = Maximum - Minimum;
                var thumbFraction = ViewportSize / range;
                var valueFraction = (Value - Minimum) / range;

                if (double.IsNaN(valueFraction) || double.IsInfinity(valueFraction))
                {
                    valueFraction = 0;
                    thumbFraction = 1;
                }
                else if (double.IsNaN(thumbFraction) || double.IsInfinity(thumbFraction))
                {
                    thumbFraction = 0;
                }

                if (Orientation == Orientation.Horizontal)
                {
                    var width = Math.Max(finalSize.Width * thumbFraction, thumb.MinWidth);
                    var x = (finalSize.Width - width) * valueFraction;
                    thumb.Arrange(new Rect(x, 0, width, finalSize.Height));
                }
                else
                {
                    var height = Math.Max(finalSize.Height * thumbFraction, thumb.MinHeight);
                    var y = (finalSize.Height - height) * valueFraction;
                    thumb.Arrange(new Rect(0, y, finalSize.Width, height));
                }
            }

            return finalSize;
        }

        private void ThumbDragged(object sender, VectorEventArgs e)
        {
            double range = Maximum - Minimum;
            double value = Value;
            double offset;

            if (Orientation == Orientation.Horizontal)
            {
                offset = e.Vector.X / ((Bounds.Size.Width - Thumb.Bounds.Size.Width) / range);
            }
            else
            {
                offset = e.Vector.Y * (range / (Bounds.Size.Height - Thumb.Bounds.Size.Height));
            }

            if (!double.IsNaN(offset) && !double.IsInfinity(offset))
            {
                value += offset;
                value = Math.Max(value, Minimum);
                value = Math.Min(value, Maximum);
                Value = value;
            }
        }
    }
}
