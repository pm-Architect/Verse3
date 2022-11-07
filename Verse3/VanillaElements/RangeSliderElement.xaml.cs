using Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class RangeSliderElementView : UserControl, IBaseElementView<RangeSliderElement>
    {
        #region IBaseElementView Members

        private RangeSliderElement _element;
        public RangeSliderElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as RangeSliderElement;
                }
                return _element;
            }
            private set
            {
                _element = value as RangeSliderElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public RangeSliderElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
            }
        }

        #endregion

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
            Element = this.DataContext as RangeSliderElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as RangeSliderElement;
            Render();
        }

        #endregion
        
        private void SliderBlock_ValueChanged(object sender, RoutedPropertyChangedEventArgs<HandyControl.Data.DoubleRange> e)
        {
            this.Element.OnValuesChanged(sender, e);
        }
    }

    //[Serializable]
    public class RangeSliderElement : BaseElement
    {
        public event EventHandler<RoutedEventArgs> ValuesChanged;

        #region Properties
        
        public override Type ViewType => typeof(RangeSliderElementView);
        
        #endregion

        #region Constructors

        public RangeSliderElement() : base()
        {
            this.minimum = -200.0;
            this.maximum = 200.0;
            this.valueStart = -100.0;
            this.valueEnd = 100.0;
            this.tickFrequency = 0.001;
        }

        #endregion

        public void OnValuesChanged(object sender, RoutedPropertyChangedEventArgs<HandyControl.Data.DoubleRange> e)
        {
            this.valueStart = e.NewValue.Start;
            this.valueEnd = e.NewValue.End;
            ValuesChanged?.Invoke(sender, e);
        }

        private double valueStart;

        public double ValueStart { get => valueStart; set => SetProperty(ref valueStart, value); }

        private double valueEnd;

        public double ValueEnd { get => valueEnd; set => SetProperty(ref valueEnd, value); }

        private double tickFrequency;

        public double TickFrequency { get => tickFrequency; set => SetProperty(ref tickFrequency, value); }

        private double minimum;

        public double Minimum { get => minimum; set => SetProperty(ref minimum, value); }

        private double maximum;

        public double Maximum { get => maximum; set => SetProperty(ref maximum, value); }

    }
}