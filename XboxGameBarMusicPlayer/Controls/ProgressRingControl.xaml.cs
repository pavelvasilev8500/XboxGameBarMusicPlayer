using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace XboxGameBarMusicPlayer.Controls
{
    public sealed partial class ProgressRingControl : UserControl
    {
        public ProgressRingControl()
        {
            InitializeComponent();
            Loaded += CircularProgressRing_Loaded;
        }

        private void CircularProgressRing_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateArc();
        }

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(double),
                typeof(ProgressRingControl),
                new PropertyMetadata(0d, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressRingControl)d).UpdateArc();
        }

        public ImageSource InnerImageSource
        {
            get => (ImageSource)GetValue(InnerImageSourceProperty);
            set => SetValue(InnerImageSourceProperty, value);
        }

        public static readonly DependencyProperty InnerImageSourceProperty =
            DependencyProperty.Register(nameof(InnerImageSource),
                typeof(ImageSource),
                typeof(ProgressRingControl),
                new PropertyMetadata(null));


        private void UpdateArc()
        {
            double angle = 360 * (Progress / 100.0);
            angle = Math.Min(Math.Max(angle, 0), 360);

            double radius = Root.Width / 2 - 4;
            double center = Root.Width / 2;

            double rad = (Math.PI / 180) * (angle - 90);

            double x = center + radius * Math.Cos(rad);
            double y = center + radius * Math.Sin(rad);

            bool largeArc = angle > 180;

            var pathFigure = new PathFigure
            {
                StartPoint = new Point(center, 4)
            };

            var arc = new ArcSegment
            {
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = largeArc,
                Size = new Size(radius, radius),
                Point = new Point(x, y)
            };

            var geometry = new PathGeometry();
            pathFigure.Segments.Add(arc);
            geometry.Figures.Add(pathFigure);

            ProgressPath.Data = geometry;
        }

        public event TappedEventHandler RingTapped;

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            RingTapped?.Invoke(this, e);
        }

        public void AnimateTo(double newValue)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                To = newValue,
                EnableDependentAnimation = true
            };

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, "Progress");

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

    }
}
