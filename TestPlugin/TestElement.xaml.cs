using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Verse3;
using static Core.Geometry2D;

namespace TestPlugin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TestElementView : UserControl
    {
        public TestElementView()
        {
            InitializeComponent();
        }
        
        #region RectangleTEMP

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        private void TestElementView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            Rectangle rectangle = (Rectangle)sender;
            IElement myRectangle = (IElement)rectangle.DataContext;

            //myRectangle.IsSelected = true;

            //mouseButtonDown = e.ChangedButton;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                return;
            }

            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingRectangles;
            DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);

            rectangle.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        private void TestElementView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode.
                //
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            Rectangle rectangle = (Rectangle)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        private void TestElementView_MouseMove(object sender, MouseEventArgs e)
        {
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode, so don't do anything.
                //
                return;
            }

            Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

            //
            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            //

            DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

            Rectangle rectangle = (Rectangle)sender;
            IElement myRectangle = (IElement)rectangle.DataContext;
            myRectangle.SetX(myRectangle.X + rectangleDragVector.X);
            myRectangle.SetY(myRectangle.Y + rectangleDragVector.Y);

            DataViewModel.WPFControl.ExpandContent();

            e.Handled = true;
        }

        #endregion
    }

    public class TestElement : IElement
    {
        private BoundingBox boundingBox = BoundingBox.Unset;

        private static Type view = typeof(TestElementView);

        public Type View { get { return view; } }

        public bool IsSelected { get; set; }

        public TestElement()
        {
            //ElementTemplate.CreateElementTemplate(this);
        }

        public TestElement(int x, int y, int width, int height)
        {
            this.boundingBox = new BoundingBox(x, y, width, height);
        }

        public BoundingBox BoundingBox { get => boundingBox; private set => boundingBox = value; }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get
            {
                //base.Width = boundingBox.Size.Width;
                return boundingBox.Size.Width;
            }
            //set
            //{
            //    boundingBox.Size.Width = value;
            //    //base.Width = boundingBox.Size.Width;
            //}
        }

        public double Height
        {
            get
            {
                //base.Height = boundingBox.Size.Height;
                return boundingBox.Size.Height;
            }
            //set
            //{
            //    boundingBox.Size.Height = value;
            //    //base.Height = boundingBox.Size.Height;
            //}
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }        
    }
}