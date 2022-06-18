using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
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
    public partial class BezierElementView : UserControl, IRenderView
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

        public BezierElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                Point start = new Point(0.0, 0.0);
                Point end = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                DrawBezierCurve(MainGrid, start, end);
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
            //MouseButtonEventArgs
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            BezierElementView rectangle = (BezierElementView)sender;
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

            rectangle.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            //if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            //{
            //    //
            //    // We are not in rectangle dragging mode.
            //    //
            //    return;
            //}

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            BezierElementView rectangle = (BezierElementView)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            //MouseEventArgs
            //if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingRectangles)
            //{
            //    //
            //    // We are not in rectangle dragging mode, so don't do anything.
            //    //
            //    return;
            //}

            //Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            //System.Windows.Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

            ////
            //// When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            ////

            //DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

            //BezierElementView rectangle = (BezierElementView)sender;
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

        PolyLineSegment GetBezierApproximation(Point[] controlPoints, int outputSegmentCount)
        {
            Point[] points = new Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return new PolyLineSegment(points, true);
        }

        Point GetBezierPoint(double t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Point((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }

        public void DrawBezierCurve(Grid grid, Point start, Point end)
        {
            double pull = 0.9;
            Point[] points = new[] {
                //new Point(0, 0),
                start,
                //new Point(190, 0),
                new Point((((end.X + start.X) / 2) * pull), start.Y),
                //new Point(200, 100),
                new Point(((end.X + start.X) / 2), ((end.Y + start.Y) / 2)),
                //new Point(210, 200),
                new Point((end.X - (((end.X + start.X) / 2) * pull)), end.Y),
                //new Point(400, 200)
                end
            };
            var b = GetBezierApproximation(points, 256);
            PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(pf);
            var pge = new PathGeometry();
            pge.Figures = pfc;
            Path p = new Path();
            p.Data = pge;
            p.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            grid.Children.Add(p);
        }
    }

    public class BezierElement : IRenderable
    {        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(BezierElementView);

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

        public IRenderView ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }

        #endregion

        #region Constructors

        public BezierElement()
        {
        }

        public BezierElement(int x, int y, int width, int height)
        {
            this.boundingBox = new BoundingBox(x, y, width, height);

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

        #endregion

        private void DrawBezier()
        {
            //private void DrawSpline(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint,
            //    Color color)
            //{
            //    var strokeThickness = 2f;

            //    // Draw the spline
            //    using (var pathBuilder = new CanvasPathBuilder(sender))
            
            //    {
            //        pathBuilder.BeginFigure(startPoint);
            //        pathBuilder.AddCubicBezier(controlPoint1, controlPoint2, endPoint);
            //        pathBuilder.EndFigure(CanvasFigureLoop.Open);

            //        var geometry = CanvasGeometry.CreatePath(pathBuilder);
            //        ds.DrawGeometry(geometry, Vector2.Zero, color, strokeThickness);
            //    }

            //    // Draw Control Points
            //    if (_showControlPoints)
            //    {
            //        var strokeStyle = new CanvasStrokeStyle() { DashStyle = CanvasDashStyle.Dot };
            //        ds.DrawLine(startPoint, controlPoint1, color, strokeThickness, strokeStyle);
            //        var rect1 = new Rect(controlPoint1.X - 3, controlPoint1.Y - 3, 6, 6);
            //        ds.FillRectangle(rect1, Colors.Beige);
            //        ds.DrawRectangle(rect1, color, strokeThickness);

            //        ds.DrawLine(endPoint, controlPoint2, color, strokeThickness, strokeStyle);
            //        var rect2 = new Rect(controlPoint2.X - 3, controlPoint2.Y - 3, 6, 6);
            //        ds.FillRectangle(rect2, Colors.Beige);
            //        ds.DrawRectangle(rect2, color, strokeThickness);
            //    }

            //    // Draw EndPoints
            //    ds.DrawCircle(startPoint, 5, color, strokeThickness);
            //    ds.FillCircle(startPoint, 5, Colors.Beige);
            //    ds.DrawCircle(endPoint, 5, color, strokeThickness);
            //    ds.FillCircle(endPoint, 5, Colors.Beige);

            //}
        }
    }
}