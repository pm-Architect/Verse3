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
    public partial class ToggleElementView : UserControl, IBaseElementView<ToggleElement>
    {
        #region IBaseElementView Members

        private ToggleElement _element;
        public ToggleElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as ToggleElement;
                }
                return _element;
            }
            private set
            {
                _element = value as ToggleElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public ToggleElementView()
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
            Element = this.DataContext as ToggleElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as ToggleElement;
            Render();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Element.OnUnchecked(sender, e);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.Element.OnChecked(sender, e);
        }

        #endregion
    }

    [Serializable]
    public class ToggleElement : BaseElement
    {

        #region Properties

        public event EventHandler<RoutedEventArgs> ToggleChecked;
        public event EventHandler<RoutedEventArgs> ToggleUnchecked;

        public override Type ViewType => typeof(ToggleElementView);
        
        private object displayedText;
        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }
        
        #endregion

        #region Constructors

        public ToggleElement() : base()
        {
            this.DisplayedText = "Toggle";
        }

        #endregion

        internal void OnChecked(object sender, RoutedEventArgs e)
        {
            Value = true;
            this.ToggleChecked?.Invoke(sender, e);
        }
        internal void OnUnchecked(object sender, RoutedEventArgs e)
        {
            Value = false;
            this.ToggleUnchecked?.Invoke(sender, e);
        }

        private bool? value1;
        public bool? Value { get => value1; set => SetProperty(ref value1, value); }
    }
}