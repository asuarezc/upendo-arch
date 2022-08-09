using System;
using Xamarin.Forms;

namespace upendo.ContentViews
{
    public partial class AmountSlider : ContentView
    {
        public static readonly BindableProperty FieldDescriptionProperty = BindableProperty.Create(
            nameof(FieldDescription),
            typeof(string),
            typeof(AmountSlider),
            propertyChanged: FieldDescriptionPropertyChanged
        );

        public static readonly BindableProperty UnitDescriptionProperty = BindableProperty.Create(
            nameof(UnitDescription),
            typeof(string),
            typeof(AmountSlider),
            propertyChanged: UnitDescriptionPropertyChanged
        );

        public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
            nameof(MinValue),
            typeof(double),
            typeof(AmountSlider),
            0d,
            propertyChanged: MinValuePropertyChanged
        );

        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
            nameof(MaxValue),
            typeof(double),
            typeof(AmountSlider),
            10d,
            propertyChanged: MaxValuePropertyChanged
        );

        public static readonly BindableProperty CurrentValueProperty = BindableProperty.Create(
            nameof(CurrentValue),
            typeof(double),
            typeof(AmountSlider),
            5d,
            BindingMode.TwoWay,
            propertyChanged: CurrentValuePropertyChanged
        );

        public static readonly BindableProperty SteppedProperty = BindableProperty.Create(
            nameof(Stepped),
            typeof(bool),
            typeof(AmountSlider),
            false
        );

        public static readonly BindableProperty StepValueProperty = BindableProperty.Create(
            nameof(StepValue),
            typeof(int),
            typeof(AmountSlider),
            1
        );

        public string FieldDescription
        {
            get => (string)GetValue(FieldDescriptionProperty);
            set => SetValue(FieldDescriptionProperty, value);
        }

        public string UnitDescription
        {
            get => (string)GetValue(UnitDescriptionProperty);
            set => SetValue(UnitDescriptionProperty, value);
        }

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public bool Stepped
        {
            get => (bool)GetValue(SteppedProperty);
            set => SetValue(SteppedProperty, value);
        }

        public int StepValue
        {
            get => (int)GetValue(StepValueProperty);
            set => SetValue(StepValueProperty, value);
        }

        public double CurrentValue
        {
            get => (double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }

        public AmountSlider()
        {
            InitializeComponent();

            fieldDescriptionLabel.Text = FieldDescription;
            unitDescriptionLabel.Text = UnitDescription;
            slider.Minimum = MinValue;
            slider.Maximum = MaxValue;
            slider.Value = CurrentValue;
            currentValueLabel.Text = CurrentValue.ToString();
        }

        private static void FieldDescriptionPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is AmountSlider amountSlider && amountSlider.fieldDescriptionLabel != null)
                amountSlider.fieldDescriptionLabel.Text = newValue.ToString();
        }

        private static void UnitDescriptionPropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is AmountSlider amountSlider && amountSlider.unitDescriptionLabel != null)
                amountSlider.unitDescriptionLabel.Text = newValue.ToString();
        }

        private static void MinValuePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not AmountSlider amountSlider
                || amountSlider.slider == null || !double.TryParse(newValue.ToString(), out double newDoubleValue))
            {
                return;
            }

            if (amountSlider.slider.Maximum <= newDoubleValue)
                throw new InvalidOperationException();

            amountSlider.slider.Minimum = newDoubleValue;
        }

        private static void MaxValuePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject == null || bindableObject is not AmountSlider amountSlider
                || amountSlider.slider == null || !double.TryParse(newValue.ToString(), out double newDoubleValue))
            {
                return;
            }

            if (amountSlider.slider.Minimum >= newDoubleValue)
                throw new InvalidOperationException();

            amountSlider.slider.Maximum = newDoubleValue;
        }

        private static void CurrentValuePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject != null && bindableObject is AmountSlider amountSlider
                && amountSlider.currentValueLabel != null
                && amountSlider.slider != null
                && newValue != null
                && newValue is double newCurrentValue)
            {
                amountSlider.slider.Value = newCurrentValue;
                amountSlider.currentValueLabel.Text = newCurrentValue.ToString();
            }
        }

        private void LifecycleEffect_Loaded(object sender, EventArgs e)
        {
            slider.ValueChanged += Slider_ValueChanged;
            slider.DragStarted += Slider_DragStarted;
            slider.DragCompleted += Slider_DragCompleted;
        }

        private void LifecycleEffect_Unloaded(object sender, EventArgs e)
        {
            slider.ValueChanged -= Slider_ValueChanged;
            slider.DragStarted -= Slider_DragStarted;
            slider.DragCompleted -= Slider_DragCompleted;
        }

        private void Slider_DragStarted(object sender, EventArgs e)
        {
            currentValueLabel.FontAttributes = FontAttributes.Bold;
        }

        private void Slider_DragCompleted(object sender, EventArgs e)
        {
            currentValueLabel.FontAttributes = FontAttributes.None;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Stepped)
            {
                double newStep = Math.Round(e.NewValue / StepValue);

                slider.ValueChanged -= Slider_ValueChanged;
                slider.Value = newStep;
                slider.ValueChanged += Slider_ValueChanged;

                CurrentValue = newStep;
                currentValueLabel.Text = newStep.ToString();
            }
            else
            {
                CurrentValue = e.NewValue;
                currentValueLabel.Text = e.NewValue.ToString();
            }
        }
    }
}
