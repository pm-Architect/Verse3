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
    public partial class ButtonElementView : UserControl, IBaseElementView<ButtonElement>
    {
        #region IBaseElementView Members

        private ButtonElement _element;
        public ButtonElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as ButtonElement;
                }
                return _element;
            }
            private set
            {
                _element = value as ButtonElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public ButtonElementView()
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
            Element = this.DataContext as ButtonElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as ButtonElement;
            Render();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Element.ButtonClicked(sender, e);
        }
    }

    [Serializable]
    public class ButtonElement : BaseElement
    {

        #region Properties

        public event EventHandler<RoutedEventArgs> OnButtonClicked;
        
        public override Type ViewType => typeof(ButtonElementView);
        
        private object displayedText;
        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        private System.Windows.Media.Brush backgroundColor;
        public System.Windows.Media.Brush BackgroundColor { get => backgroundColor; set => SetProperty(ref backgroundColor, value); }

        #endregion

        #region Constructors

        public ButtonElement() : base()
        {
            this.DisplayedText = "Button";
            this.BackgroundColor = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff6700"));
        }

        #endregion

        internal void ButtonClicked(object sender, RoutedEventArgs e)
        {
            OnButtonClicked.Invoke(sender, e);
        }

    }
}