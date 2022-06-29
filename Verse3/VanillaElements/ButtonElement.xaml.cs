using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Verse3;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class ButtonElementView : UserControl, IRenderView
    {
        private IRenderable _element;
        
        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value;
                //Update();
            }
        }
        public Guid? ElementGuid
        {
            get { return _element?.ID; }
        }

        public ButtonElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                //this.Element.BoundingBox.Size.Height = SliderBlock.ActualHeight;
                //this.Element.BoundingBox.Size.Width = SliderBlock.ActualWidth;
                //this.Element.OnPropertyChanged("Width");
                //this.Element.OnPropertyChanged("Height");
            }
        }

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ////MouseButtonEventArgs
            //DataViewModel.WPFControl.ContentElements.Focus();
            //Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            //ButtonElementView rectangle = (ButtonElementView)sender;
            //IRenderable myRectangle = (IRenderable)rectangle.DataContext;

            ////myRectangle.IsSelected = true;

            ////mouseButtonDown = e.ChangedButton;

            //if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            //{
            //    //
            //    // When the shift key is held down special zooming logic is executed in content_MouseDown,
            //    // so don't handle mouse input here.
            //    //
            //    return;
            //}

            //if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
            //{
            //    //
            //    // We are in some other mouse handling mode, don't do anything.
            //    return;
            //}

            //DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingRectangles;
            //DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);

            //rectangle.CaptureMouse();

            //e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ////MouseButtonEventArgs
            //if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            //{
            //    //
            //    // We are not in rectangle dragging mode.
            //    //
            //    return;
            //}

            //DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            //ButtonElementView rectangle = (ButtonElementView)sender;
            //rectangle.ReleaseMouseCapture();

            //e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            ////MouseEventArgs
            //if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            //{
            //    //
            //    // We are not in rectangle dragging mode, so don't do anything.
            //    //
            //    return;
            //}

            //Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            //Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

            ////
            //// When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            ////

            //DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

            //ButtonElementView rectangle = (ButtonElementView)sender;
            //IRenderable myRectangle = (IRenderable)rectangle.DataContext;
            //myRectangle.SetX(myRectangle.X + rectangleDragVector.X);
            //myRectangle.SetY(myRectangle.Y + rectangleDragVector.Y);

            //DataViewModel.WPFControl.ExpandContent();

            //e.Handled = true;
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //MouseWheelEventArgs
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
            Element = this.DataContext as IRenderable;
            Render();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as IRenderable;
            Render();
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ButtonElement)this.Element).ButtonClicked(sender, e);
        }
    }

    public class ButtonElement : IRenderable
    {        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(ButtonElementView);

        #endregion

        #region Properties

        public Type ViewType { get { return view; } }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => boundingBox = value; }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get
            {
                return boundingBox.Size.Width;
            }
            set
            {
                boundingBox.Size.Width = value;
            }
        }

        public double Height
        {
            get
            {
                return boundingBox.Size.Height;
            }
            set
            {
                boundingBox.Size.Height = value;
            }
        }

        public Guid ZPrev { get; }

        public Guid ZNext { get; }

        public Guid Parent { get; }

        public Guid[] Children { get; }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        bool IRenderable.Visible { get; set; }

        #endregion

        #region Constructors

        public ButtonElement()
        {
            this.DisplayedText = "Button";
        }

        public ButtonElement(int x, int y, int width, int height)
        {
            this.boundingBox = new BoundingBox(x, y, width, height);

            this.DisplayedText = "Button";
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        internal void ButtonClicked(object sender, RoutedEventArgs e)
        {
            OnButtonClicked.Invoke(sender, e);
        }

        private object displayedText;

        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }
        public event EventHandler<RoutedEventArgs> OnButtonClicked;

        #endregion
    }
}