using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GetMediaInfoGUI
{
    public class GifImage : Image
    {
        public static readonly DependencyProperty FrameIndexProperty = DependencyProperty.Register(nameof(FrameIndex), typeof(int), typeof(GifImage), new UIPropertyMetadata(0, ChangingFrameIndex));
        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register(nameof(GifSource), typeof(string), typeof(GifImage), new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));
        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(GifImage), new UIPropertyMetadata(false, AutoStartPropertyChanged));
        private bool _isInitialized;
        private GifBitmapDecoder _gifDecoder;
        private Int32Animation _animation;

        static GifImage()
        {
            VisibilityProperty.OverrideMetadata(typeof(GifImage), new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        public string GifSource
        {
            get
            {
                return (string)GetValue(GifSourceProperty);
            }
            set
            {
                SetValue(GifSourceProperty, value);
            }
        }

        public int FrameIndex
        {
            get
            {
                return (int)GetValue(FrameIndexProperty);
            }
            set
            {
                SetValue(FrameIndexProperty, value);
            }
        }

        public bool AutoStart
        {
            get
            {
                return (bool)GetValue(AutoStartProperty);
            }
            set
            {
                SetValue(AutoStartProperty, value);
            }
        }

        private void Initialize()
        {
            _gifDecoder = new GifBitmapDecoder(new Uri("pack://application:,,," + GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            Int32Animation int32Animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, _gifDecoder.Frames.Count / 10, (int)((_gifDecoder.Frames.Count / 10.0 - _gifDecoder.Frames.Count / 10) * 1000.0))));
            int32Animation.RepeatBehavior = RepeatBehavior.Forever;
            _animation = int32Animation;
            Source = _gifDecoder.Frames[0];
            _isInitialized = true;
        }

        public void StartAnimation()
        {
            if (!_isInitialized)
                Initialize();
            BeginAnimation(FrameIndexProperty, _animation);
        }

        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }

        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GifImage).Initialize();
        }

        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                return;
            (sender as GifImage).StartAnimation();
        }

        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
                ((GifImage)sender).StartAnimation();
            else
                ((GifImage)sender).StopAnimation();
        }

        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            GifImage gifImage = obj as GifImage;
            gifImage.Source = gifImage._gifDecoder.Frames[(int)ev.NewValue];
        }
    }
}
