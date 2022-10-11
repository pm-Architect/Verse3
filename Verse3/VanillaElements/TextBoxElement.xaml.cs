using Core;
using System;
using System.ComponentModel;
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
    public partial class TextBoxElementView : UserControl, IBaseElementView<TextBoxElement>
    {
        #region IBaseElementView Members

        private TextBoxElement _element;
        public TextBoxElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as TextBoxElement;
                }
                return _element;
            }
            private set
            {
                _element = value as TextBoxElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public TextBoxElementView()
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
            Element = this.DataContext as TextBoxElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as TextBoxElement;
            Render();
        }

        #endregion

        private void TextBoxBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            Element.InputText = TextBoxBlock.Text;
            Element.ValueChanged.Invoke(sender, e);
        }
    }

    public class TextBoxElement : BaseElement
    {
        #region Properties

        public override Type ViewType => typeof(TextBoxElementView);

        private string inputText;
        public string InputText { get => inputText; set => SetProperty(ref inputText, value); }
        public EventHandler<TextChangedEventArgs> ValueChanged { get; set; }

        #endregion

        #region Constructors

        public TextBoxElement() : base()
        {
            this.InputText = "<empty>";
        }

        #endregion
    }
}