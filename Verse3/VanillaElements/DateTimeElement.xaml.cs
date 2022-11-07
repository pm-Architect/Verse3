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
    public partial class DateTimeElementView : UserControl, IBaseElementView<DateTimeElement>
    {
        #region IBaseElementView Members

        private DateTimeElement _element;
        public DateTimeElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as DateTimeElement;
                }
                return _element;
            }
            private set
            {
                _element = value as DateTimeElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public DateTimeElementView()
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
            Element = this.DataContext as DateTimeElement;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as DateTimeElement;
            Render();
        }
        private void DateTimePicker_SelectedDateTimeChanged(object sender, HandyControl.Data.FunctionEventArgs<DateTime?> e)
        {
            this.Element.OnSelectedDateTimeChanged(sender, e);
        }

        #endregion

    }

    //[Serializable]
    public class DateTimeElement : BaseElement
    {
        public event EventHandler<RoutedEventArgs> DateTimeChanged;

        #region Properties
        
        public override Type ViewType => typeof(DateTimeElementView);
        
        private object displayedText;
        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }
        
        #endregion

        #region Constructors

        public DateTimeElement() : base()
        {
            this.DisplayedText = "Toggle";
        }

        #endregion

        public void OnSelectedDateTimeChanged(object sender, HandyControl.Data.FunctionEventArgs<DateTime?> e)
        {
            Value = e.Info;
            DateTimeChanged?.Invoke(sender, e);
        }

        
        private DateTime? value1;
        public DateTime? Value { get => value1; set => SetProperty(ref value1, value); }
    }
}