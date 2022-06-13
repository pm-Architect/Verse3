using InfiniteCanvas;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Verse3
{
    /// <summary>
    /// Interaction logic for InfiniteCanvasWPFControl.xaml
    /// </summary>
    public partial class InfiniteCanvasWPFControl : UserControl
    {
        private System.Windows.Forms.Cursor winFormsCursor = System.Windows.Forms.Cursors.Default;
        public System.Windows.Forms.Cursor WinFormsCursor
        {
            get
            {
                return winFormsCursor;
            }
            set
            {
                winFormsCursor = value;
            }
        }

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        public MouseHandlingMode MouseHandlingMode { get { return mouseHandlingMode; } }

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous content scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        ///// <summary>
        ///// Set to 'true' when the previous select rect is saved.
        ///// </summary>
        //private bool prevSelectRectSet = false;

        ///// <summary>
        ///// Saves the previous Select rectangle
        ///// </summary>
        //private Rect prevSelectRect;

        public InfiniteCanvasWPFControl()
        {
            InitializeComponent();
        }
        
        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            ExpandContent();
        }

        /// <summary>
        /// Expand the content area to fit the rectangles.
        /// </summary>
        private void ExpandContent()
        {
            double xOffset = 0;
            double yOffset = 0;
            Rect contentRect = new Rect(0, 0, 0, 0);
            foreach (RectangleData rectangleData in DataModel.Instance.Rectangles)
            {
                if (rectangleData.X < xOffset)
                {
                    xOffset = rectangleData.X;
                }

                if (rectangleData.Y < yOffset)
                {
                    yOffset = rectangleData.Y;
                }

                contentRect.Union(new Rect(rectangleData.X, rectangleData.Y, rectangleData.Width, rectangleData.Height));
            }

            //
            // Translate all rectangles so they are in positive space.
            //
            xOffset = Math.Abs(xOffset);
            yOffset = Math.Abs(yOffset);

            foreach (RectangleData rectangleData in DataModel.Instance.Rectangles)
            {
                rectangleData.X += xOffset;
                rectangleData.Y += yOffset;
            }

            DataModel.Instance.ContentWidth = contentRect.Width;
            DataModel.Instance.ContentHeight = contentRect.Height;
        }

        #region MouseEvents

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LBcontent.Focus();
            Keyboard.Focus(LBcontent);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(InfiniteCanvasControl1);
            origContentMouseDownPoint = e.GetPosition(LBcontent);

            if (mouseButtonDown == MouseButton.Left)
            {
                mouseHandlingMode = MouseHandlingMode.Selecting;
                if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                {
                    // Shift + left- or right-down initiates zooming mode.
                    mouseHandlingMode = MouseHandlingMode.Zooming;
                }
            }
            if (mouseButtonDown == MouseButton.Right)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                InfiniteCanvasControl1.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect();
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragSelecting)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragSelectRect();
                }

                InfiniteCanvasControl1.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void zoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(LBcontent);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                InfiniteCanvasControl1.ContentOffsetX -= dragOffset.X;
                InfiniteCanvasControl1.ContentOffsetY -= dragOffset.Y;

                this.Cursor = Cursors.SizeAll;
                this.WinFormsCursor = System.Windows.Forms.Cursors.SizeAll;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(InfiniteCanvasControl1);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                        Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragZooming;
                    this.Cursor = Cursors.SizeNWSE;
                    this.WinFormsCursor = System.Windows.Forms.Cursors.SizeNWSE;
                    Point curContentMousePoint = e.GetPosition(LBcontent);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                this.Cursor = Cursors.SizeNWSE;
                this.WinFormsCursor = System.Windows.Forms.Cursors.SizeNWSE;
                Point curContentMousePoint = e.GetPosition(LBcontent);
                InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
            }
            else if (mouseHandlingMode == MouseHandlingMode.Selecting)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(InfiniteCanvasControl1);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 5;
                if ((Math.Abs(dragOffset.X) > dragThreshold) ||
                        (Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When left-down selecting mode and the user drags beyond the drag threshold,
                    // initiate drag selecting mode where the user can drag out a rectangle to select the area
                    // to select objects from.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragSelecting;
                    this.Cursor = Cursors.Cross;
                    this.WinFormsCursor = System.Windows.Forms.Cursors.Cross;
                    Point curContentMousePoint = e.GetPosition(LBcontent);
                    InitDragSelectRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragSelecting)
            {
                this.Cursor = Cursors.Cross;
                this.WinFormsCursor = System.Windows.Forms.Cursors.Cross;
                Point curContentMousePoint = e.GetPosition(LBcontent);
                InitDragSelectRect(origContentMouseDownPoint, curContentMousePoint);
            }
            else
            {
                if (this.WinFormsCursor != System.Windows.Forms.Cursors.Default)
                {
                    this.Cursor = Cursors.Arrow;
                    this.WinFormsCursor = System.Windows.Forms.Cursors.Default;
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    //
                    // When in drag zooming mode continously update the position of the rectangle
                    // that the user is dragging out.
                    //
                    Point curContentMousePoint = e.GetPosition(LBcontent);
                    SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                    e.Handled = true;
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragSelecting)
                {
                    //
                    // When in drag zooming mode continously update the position of the rectangle
                    // that the user is dragging out.
                    //
                    Point curContentMousePoint = e.GetPosition(LBcontent);
                    SetDragSelectRect(origContentMouseDownPoint, curContentMousePoint);
                }
            }
            
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void zoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(LBcontent);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(LBcontent);
                ZoomOut(curContentMousePoint);
            }
        }

        private void zoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                Point doubleClickPoint = e.GetPosition(LBcontent);
                InfiniteCanvasControl1.AnimatedSnapTo(doubleClickPoint);
            }
        }

        #endregion

        #region ZoomFunctions

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            InfiniteCanvasControl1.ZoomAboutPoint(InfiniteCanvasControl1.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            InfiniteCanvasControl1.ZoomAboutPoint(InfiniteCanvasControl1.ContentScale + 0.1, contentZoomCenter);
        }
        
        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomIn(new Point(InfiniteCanvasControl1.ContentZoomFocusX, InfiniteCanvasControl1.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new Point(InfiniteCanvasControl1.ContentZoomFocusX, InfiniteCanvasControl1.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'JumpBackToPrevZoom' command was executed.
        /// </summary>
        private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JumpBackToPrevZoom();
        }

        /// <summary>
        /// Determines whether the 'JumpBackToPrevZoom' command can be executed.
        /// </summary>
        private void JumpBackToPrevZoom_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = prevZoomRectSet;
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            InfiniteCanvasControl1.AnimatedScaleToFit();
        }

        /// <summary>
        /// The 'OneHundredPercent' command was executed.
        /// </summary>
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            InfiniteCanvasControl1.AnimatedZoomTo(1.0);
        }

        /// <summary>
        /// Jump back to the previous zoom level.
        /// </summary>
        private void JumpBackToPrevZoom()
        {
            InfiniteCanvasControl1.AnimatedZoomTo(prevZoomScale, prevZoomRect);

            ClearPrevZoomRect();
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }
        
        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            InfiniteCanvasControl1.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();
        }
        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(InfiniteCanvasControl1.ContentOffsetX, InfiniteCanvasControl1.ContentOffsetY, InfiniteCanvasControl1.ContentViewportWidth, InfiniteCanvasControl1.ContentViewportHeight);
            prevZoomScale = InfiniteCanvasControl1.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///

        #endregion

        #region SelectFunctions


        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragSelectRect(Point pt1, Point pt2)
        {
            SetDragSelectRect(pt1, pt2);

            dragSelectCanvas.Visibility = Visibility.Visible;
            dragSelectBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragSelectRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            Canvas.SetLeft(dragSelectBorder, x);
            Canvas.SetTop(dragSelectBorder, y);
            dragSelectBorder.Width = width;
            dragSelectBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragSelectRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            //SavePrevSelectRect();

            //
            // Retreive the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragSelectBorder);
            double contentY = Canvas.GetTop(dragSelectBorder);
            double contentWidth = dragSelectBorder.Width;
            double contentHeight = dragSelectBorder.Height;

            //TODO: Actually select the items in the select rectangle

            FadeOutDragSelectRect();
        }
        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragSelectRect()
        {
            AnimationHelper.StartAnimation(dragSelectBorder, Border.OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragSelectCanvas.Visibility = Visibility.Collapsed;
                });
        }

        ////
        //// Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        ////
        //private void SavePrevSelectRect()
        //{
        //    prevSelectRect = new Rect(InfiniteCanvasControl1.ContentOffsetX, InfiniteCanvasControl1.ContentOffsetY, InfiniteCanvasControl1.ContentViewportWidth, InfiniteCanvasControl1.ContentViewportHeight);
        //    prevSelectRectSet = true;
        //}

        ///// <summary>
        ///// Clear the memory of the previous zoom level.
        ///// </summary>
        //private void ClearPrevSelectRect()
        //{
        //    prevSelectRectSet = false;
        //}

        #endregion

        #region RectangleTEMP

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LBcontent.Focus();
            Keyboard.Focus(LBcontent);

            Rectangle rectangle = (Rectangle)sender;
            RectangleData myRectangle = (RectangleData)rectangle.DataContext;

            myRectangle.IsSelected = true;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                return;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            mouseHandlingMode = MouseHandlingMode.DraggingRectangles;
            origContentMouseDownPoint = e.GetPosition(LBcontent);

            rectangle.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode.
                //
                return;
            }

            mouseHandlingMode = MouseHandlingMode.None;

            Rectangle rectangle = (Rectangle)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            {
                //
                // We are not in rectangle dragging mode, so don't do anything.
                //
                return;
            }

            Point curContentPoint = e.GetPosition(LBcontent);
            Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

            //
            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            //

            origContentMouseDownPoint = curContentPoint;

            Rectangle rectangle = (Rectangle)sender;
            RectangleData myRectangle = (RectangleData)rectangle.DataContext;
            myRectangle.X += rectangleDragVector.X;
            myRectangle.Y += rectangleDragVector.Y;

            ExpandContent();

            e.Handled = true;
        }

        #endregion
    }

    /// <summary>
    /// Defines the current state of the mouse handling logic.
    /// </summary>
    public enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// The user is left-dragging rectangles with the mouse.
        /// </summary>
        DraggingRectangles,

        /// <summary>
        /// The user is left-mouse-button-dragging to pan the viewport.
        /// </summary>
        Panning,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Zooming,

        /// <summary>
        /// The user is holding down shift and left-clicking or right-clicking to zoom in or out.
        /// </summary>
        Selecting,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragZooming,

        /// <summary>
        /// The user is holding down shift and left-mouse-button-dragging to select a region to zoom to.
        /// </summary>
        DragSelecting,

        /// <summary>
        /// The user is left-mouse-button-dragging on the viewport.
        /// </summary>
        Dragging
    }

    /// <summary>
    /// PseudoConverter to offset X and Y Positions for setting the canvas origin in the viewport
    /// https://stackoverflow.com/a/4973289
    /// </summary>
    public class CanvasSizeOffsetPseudoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double OffsetValue = DataModel.ContentCanvasMarginOffset;
            double Val = ((double)value);

            return Val + OffsetValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
