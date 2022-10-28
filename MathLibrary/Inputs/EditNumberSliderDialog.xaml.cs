using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MathLibrary.Inputs
{
    /// <summary>
    /// Interaction logic for EditNumberSliderDialog.xaml
    /// </summary>
    public partial class EditNumberSliderDialog : Window
    {
        NumberContainer _owner;
        public EditNumberSliderDialog(NumberContainer owner)
        {
            InitializeComponent();
            _owner = owner;
            if (_owner != null && _owner._sliderValue.HasValue) Value = _owner._sliderValue.Value;
            else
            {
                Exception ex = new ArgumentNullException("owner");
                CoreConsole.Log(ex);
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string ValueAsText
        {
            get { return Value.ToString(); }
            set
            {
                double.TryParse(value, out double result);
                if (result != Value) Value = result;
            }
        }
        
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(double), typeof(EditNumberSliderDialog), new PropertyMetadata(0.0));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_owner != null && _owner.sliderBlock != null)
                _owner.sliderBlock.Value = Value;
            else
            {
                Exception ex = new ArgumentNullException("owner");
                CoreConsole.Log(ex);
            }
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValueAsText = ValueTextBox.Text;
        }
    }
}
