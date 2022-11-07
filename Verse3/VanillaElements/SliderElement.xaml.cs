using Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class SliderElementView : UserControl, IBaseElementView<SliderElement>
    {

        #region IBaseElementView Members

        private SliderElement _element;
        public SliderElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as SliderElement;
                }
                return _element;
            }
            private set
            {
                _element = value as SliderElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public SliderElementView()
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
            Element = this.DataContext as SliderElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as SliderElement;
            Render();
        }

        #endregion

        private void SliderBlock_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Element.OnValueChanged(sender, e);
            //ComputationPipeline.ComputeComputable(this.Element.RenderPipelineInfo.Parent as IComputable);
            //RenderPipeline.Render();
        }
    }

    //[Serializable]
    public class SliderElement : BaseElement
    {
        #region Properties

        public override Type ViewType => typeof(SliderElementView);
        
        private double minimum;
        public double Minimum { get => minimum; set => SetProperty(ref minimum, value); }

        private double maximum;
        public double Maximum { get => maximum; set => SetProperty(ref maximum, value); }

        private double _value;
        public double Value { get => _value;
            set => SetProperty(ref _value, value); }

        private double tickFrequency;
        public double TickFrequency { get => tickFrequency; set => SetProperty(ref tickFrequency, value); }

        #endregion
        
        #region Constructors

        public SliderElement() : base()
        {
            this.Minimum = 0;
            this.Maximum = 100;
            this.Value = 50;
            this.TickFrequency = 0.001;
        }

        #endregion

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;
        public void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Value = (double)e.NewValue;
            this.ValueChanged.Invoke(sender, e);
        }
    }
}