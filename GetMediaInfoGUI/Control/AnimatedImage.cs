using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GetMediaInfoGUI.Control
{
    /// <summary>
    /// Animated image
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Image" />
    public class AnimatedImage : Image
    {
        #region Properties
        private bool _isInitialized;
        private GifBitmapDecoder _gifDecoder;
        private Int32Animation _animation;
        public static readonly DependencyProperty FrameIndexProperty = DependencyProperty.Register("FrameIndex", typeof(int), typeof(AnimatedImage), new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));
        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(string), typeof(AnimatedImage), new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));
        public static readonly DependencyProperty AutoStartProperty = DependencyProperty.Register("AutoStart", typeof(bool), typeof(AnimatedImage), new UIPropertyMetadata(false, AutoStartPropertyChanged));

        /// <summary>
        /// Initializes the <see cref="AnimatedImage"/> class.
        /// </summary>
        static AnimatedImage()
        {
            VisibilityProperty.OverrideMetadata(typeof(AnimatedImage), new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        /// <summary>
        /// Gets or sets the GIF source.
        /// </summary>
        /// <value>
        /// The GIF source.
        /// </value>
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

        /// <summary>
        /// Gets or sets the index of the frame of the animation.
        /// </summary>
        /// <value>
        /// The index of the frame currently shown.
        /// </value>
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

        /// <summary>
        /// Defines whether the animation starts on it's own
        /// </summary>
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
        #endregion

        #region MainMethods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _gifDecoder = new GifBitmapDecoder(new Uri("pack://application:,,," + this.GifSource), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            _animation = new Int32Animation(0, _gifDecoder.Frames.Count - 1, new Duration(new TimeSpan(0, 0, 0, _gifDecoder.Frames.Count / 10, (int)((_gifDecoder.Frames.Count / 10.0 - _gifDecoder.Frames.Count / 10) * 1000))))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            this.Source = _gifDecoder.Frames[0];

            _isInitialized = true;
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            if (!_isInitialized)
                this.Initialize();

            BeginAnimation(FrameIndexProperty, _animation);
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
        #endregion

        #region PropertyChanged Management
        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as AnimatedImage).Initialize();
        }
        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                (sender as AnimatedImage).StartAnimation();
        }
        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility)e.NewValue == Visibility.Visible)
            {
                ((AnimatedImage)sender).StartAnimation();
            }
            else
            {
                ((AnimatedImage)sender).StopAnimation();
            }
        }
        private static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var gifImage = obj as AnimatedImage;
            gifImage.Source = gifImage._gifDecoder.Frames[(int)ev.NewValue];
        }
        #endregion
    }
}
