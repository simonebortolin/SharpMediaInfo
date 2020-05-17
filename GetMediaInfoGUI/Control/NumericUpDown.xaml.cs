using GetMediaInfoGUI.Control.Converter;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GetMediaInfoGUI.Control
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {

        #region Dependency objects
        /// <summary>
        /// Dependency Object for the value of the UpDown Control
        /// </summary>
        public static readonly DependencyProperty _value = DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(0.0, ValuePropertyChangeCallback), new ValidateValueCallback(ValidateValue));

        /// <summary>
        /// Dependency Object for the Minimal Value of the UpDown Control
        /// </summary>
        public static readonly DependencyProperty _minvalue = DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MinValue, MinValueCallback));

        /// <summary>
        /// Dependency Object for the Maximal Value of the UpDown Control
        /// </summary>
        public static readonly DependencyProperty _maxvalue = DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(double.MaxValue, MaxValueCallback));

        /// <summary>
        /// Dependency Object for the Maximal Value of the UpDown Control
        /// </summary>
        public static readonly DependencyProperty _decimation = DependencyProperty.Register("Decimation", typeof(uint), typeof(NumericUpDown), new FrameworkPropertyMetadata(0U, DecimationCallback));

        /// <summary>
        /// Dependency Object for the Step Value of the UpDown Control
        /// </summary>
        public static readonly DependencyProperty _step = DependencyProperty.Register("Step", typeof(double), typeof(NumericUpDown), new FrameworkPropertyMetadata(1.0));
        
        /// <summary>
        /// Dependency Object for the state of visibility of the UpDown Buttons
        /// </summary>
        public static readonly DependencyProperty _showButtons = DependencyProperty.Register("ShowButtons", typeof(bool), typeof(NumericUpDown), new FrameworkPropertyMetadata(true));
        #endregion

        #region Events
        /// <summary>
        /// Event Definition Value Change
        /// </summary>
        public static RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericUpDown));

        /// <summary>
        /// Event fired when value changes
        /// </summary>
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Event Helper Function when Value is changed
        /// </summary>
        protected virtual void OnValueChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ValueChangedEvent;
            RaiseEvent(args);
        }
        #endregion

        #region Static fields
        /// <summary>
        /// Minimal possible value of the control
        /// </summary>
        static double minValueLimit = double.MinValue;

        /// <summary>
        /// Maximum possible value of the control
        /// </summary>
        static double maxValueLimit = double.MaxValue;

        /// <summary>
        /// Reference for static Functions
        /// </summary>
        static NumericUpDown thisReference = null;
        #endregion

        #region Constructor and Deconstructor
        /// <summary>
        /// Default Constructor (nothing special here x) )
        /// </summary>
        public NumericUpDown()
        {
            InitializeComponent();
            thisReference = this;
        }

        /// <summary>
        /// Destroys the Class (sets thispointer null)
        /// </summary>
        ~NumericUpDown()
        {
            thisReference = null;
        }
        #endregion

        #region Public fields
        /// <summary>
        /// Specifies / Reads the number of digits shown after the decimal point
        /// </summary>
        public uint Decimation
        {
            get
            {
                return (uint)GetValue(_decimation);
            }
            set
            {
                SetValue(_decimation, value) ;
                SetDecimationBinding(value);
            }
        }

        /// <summary>
        /// Gets / Sets the value that the control is showing
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <remarks>If Value exceeds <see cref="MaxValue"/> or falls below <see cref="MinValue"/>, an <see cref="ArgumentException"/> is thrown</remarks>
        public double Value
        {
            get
            {
                return (double)GetValue(_value);
            }
            set
            {
                SetValue(_value, value);
            }
        }

        /// <summary>
        /// Specifies / Reads weather the UpDown Buttons are to be shown
        /// </summary>
        public bool ShowButtons
        {
            get
            {
                return (bool)GetValue(_value);
            }
            set
            {
                SetValue(_value, value);
            }
        }

        /// <summary>
        /// Gets / Sets the minimal value of the control's value
        /// </summary>
        public double MinValue
        {
            get
            {
                return (double)GetValue(_minvalue);
            }
            set
            {
                SetValue(_minvalue, value);
            }
        }

        /// <summary>
        /// Gets / Sets the maximal value of the control's value
        /// </summary>
        public double MaxValue
        {
            get
            {
                return (double)GetValue(_maxvalue);
            }
            set
            {
                SetValue(_maxvalue, value);
            }
        }

        /// <summary>
        /// Gets / Sets the step size (increment / decrement size) of the control's value
        /// </summary>
        public double Step
        {
            get
            {
                return (double)GetValue(_step);
            }
            set
            {
                SetValue(_step, value);
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Increments the control's value by the value defined by <see cref="Step"/>
        /// </summary>
        /// <remarks>The value doesn't increment over MaxValue or under MinValue</remarks>
        public void Increment()
        {
            try
            {
                Value += Step;
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// Decrements the control's value by the value defined by <see cref="Step"/>
        /// </summary>
        /// <remarks>The value doesn't increment over MaxValue or under MinValue</remarks>
        public void Decrement()
        {
            try
            {
                Value -= Step;
            }
            catch (ArgumentException)
            {
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Handler for the Up Button Click.
        /// Increments the <see cref="Value"/> by <see cref="Step"/>
        /// </summary>
        /// <param name="sender">The Up Button Control</param>
        /// <param name="e"></param>
        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Increment();
        }

        /// <summary>
        /// Handler for the Down Button Click.
        /// Decrements the <see cref="Value"/> by <see cref="Step"/>
        /// </summary>
        /// <param name="sender">The Down Button Control</param>
        /// <param name="e"></param>
        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Decrement();
        }

        /// <summary>
        /// Handles the Loaded event of the ucNumericUpDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UcNumericUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            SetDecimationBinding(Decimation);
        }
        #endregion

        #region helpers
        /// <summary>
        /// Validation function for the value.
        /// Checks weather Value is inbetween <see cref="MinValue"/> and <see cref="MaxValue"/>
        /// </summary>
        /// <param name="value">The current value of the Dependency Property</param>
        /// <returns><list type="bullet"><item><term>true</term><description>The Value is inbetween <see cref="MinValue"/> and <see cref="MaxValue"/></description></item><item><term>false</term><description>The value is out of bounds</description></item></list></returns>
        static bool ValidateValue(object value)
        {
            double val = Convert.ToDouble(value);
            return (val >= minValueLimit && val <= maxValueLimit);
        }

        /// <summary>
        /// Sets the decimation binding.
        /// </summary>
        /// <param name="decimation">The decimation.</param>
        private void SetDecimationBinding(uint decimation)
        {
            Binding bindingValue = new Binding("Value")
            {
                Converter = new DecimationConverter(),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ConverterParameter = decimation
            };
            bindingValue.ValidationRules.Add(new ExceptionValidationRule());

            tbValue.SetBinding(TextBox.TextProperty, bindingValue);
        }
        #endregion

        #region Static dependency objects callbacks
        /// <summary>
        /// Change Event for Value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ValuePropertyChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (thisReference != null)
            {
                thisReference.OnValueChanged();
            }
        }

        /// <summary>
        /// Change Event for Min Value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void MinValueCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                minValueLimit = Convert.ToDouble(e.NewValue);
            }
            finally
            {
                //minValueLimit
            }
        }

        /// <summary>
        /// Change Event for Max Value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void MaxValueCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                maxValueLimit = Convert.ToDouble(e.NewValue);
            } finally
            {
                //maxValueLimit
            } 
        }

        /// <summary>
        /// Change Event for Decimation
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DecimationCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //DecimationConverter.decimation = Convert.ToUInt32(e.NewValue);
            if (thisReference != null)
            {
                thisReference.SetDecimationBinding(Convert.ToUInt32(e.NewValue));
            }

            // Hack to update the control's binding.
            if (thisReference != null)
            {
                thisReference.Value++;
                thisReference.Value--;
            }
        }
        #endregion

    }
}