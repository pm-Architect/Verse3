using Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class BezierElementView : UserControl, IRenderView
    {
        private BezierElement _element;

        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value as BezierElement;
                this._element.bezView = this;
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

        internal bool _rendering = false;
        public void Render()
        {
            if (this._rendering) return;
            this._rendering = true;
            if (this.Element != null)
            {
                if (this._element.bezView != this) this._element.bezView = this;
                DrawBezierCurve(MainGrid, (this._element).Direction);
            }

            this._rendering = false;
        }

        Point start = default;
        Point end = default;
        public void DrawBezierCurve(Grid grid, BezierDirection dir = BezierDirection.Default)
        {
            //TODO: Adjustable x and y pull settings
            double xPull = 0.5;
            double yPull = 0.75;
            double hmargin = 5.0, vmargin = 5.0;
            double pull = yPull * this._element.InnerBoundingBox.Size.Height;
            if ((yPull * this._element.InnerBoundingBox.Size.Height)
                < (Math.Abs(xPull * this._element.InnerBoundingBox.Size.Width)))
            {
                pull = xPull * this._element.InnerBoundingBox.Size.Width;
            }
            //TODO: Calculate required width without having to calculate the curve first
            //Math.Abs((end.X - pull) - (start.X + pull)) CONTROL POINTS WIDTH
            //Math.Abs((end.X - (pull * 0.5)) - (start.X + (pull * 0.5))) FINAL WIDTH
            double sx = 0.0, sy = 0.0, ex = 0.0, ey = 0.0;
            double dx = (4 * pull / 5) + (2 * hmargin);
            double dy = (2 * vmargin);
            double newBBWidth = this._element.InnerBoundingBox.Size.Width + dx;
            double newBBHeight = this._element.InnerBoundingBox.Size.Height + dy;
            if (this._element.InnerBoundingBox.Size.Width < pull)
            {
                if (this._element.LeftToRight)
                {
                    sx = (dx / 2);
                    ex = this._element.InnerBoundingBox.Size.Width + (dx / 2);
                }
                else
                {
                    sx = this._element.InnerBoundingBox.Size.Width + (dx / 2);
                    ex = (dx / 2);
                }
                if (!this._element.inflatedX)
                {
                    //this.Element.TransformBoundsTo(new BoundingBox(
                    //        (this._element.InnerBoundingBox.Location.X - (dx / 2)),
                    //        (this._element.InnerBoundingBox.Location.Y - (dy / 2)),
                    //        (this._element.InnerBoundingBox.Size.Width + dx),
                    //        (this._element.InnerBoundingBox.Size.Height + dy)
                    //    ));
                    this.Element.SetWidth(newBBWidth);
                    this.Element.SetX(this.Element.X - (dx / 2));
                    this._element.inflatedX = true;
                }
            }
            else
            {
                if (this._element.LeftToRight)
                {
                    dx = (2 * hmargin);
                    sx = (dx / 2);
                    ex = this._element.InnerBoundingBox.Size.Width + (dx / 2);
                }
                else
                {
                    sx = this._element.InnerBoundingBox.Size.Width + (dx / 2);
                    ex = (dx / 2);
                }
                if (!this._element.inflatedX)
                {
                    //this.Element.TransformBoundsTo(new BoundingBox(
                    //        (this._element.InnerBoundingBox.Location.X - (dx / 2)),
                    //        (this._element.InnerBoundingBox.Location.Y - (dy / 2)),
                    //        (this._element.InnerBoundingBox.Size.Width + dx),
                    //        (this._element.InnerBoundingBox.Size.Height + dy)
                    //    ));
                    this.Element.SetWidth(newBBWidth);
                    this.Element.SetX(this.Element.X - (dx / 2));
                    this._element.inflatedX = true;
                }
            }

            if (this._element.TopToBottom)
            {
                ey = this._element.InnerBoundingBox.Size.Height + (dy / 2);
                sy = (dy / 2);
                if (!this._element.inflatedY)
                {
                    //this.Element.TransformBoundsTo(new BoundingBox(
                    //        (this._element.InnerBoundingBox.Location.X - (dx / 2)),
                    //        (this._element.InnerBoundingBox.Location.Y - (dy / 2)),
                    //        (this._element.InnerBoundingBox.Size.Width + dx),
                    //        (this._element.InnerBoundingBox.Size.Height + dy)
                    //    ));
                    this.Element.SetHeight(newBBHeight);
                    this.Element.SetY(this.Element.Y - (dy / 2));
                    this._element.inflatedY = true;
                }
            }
            else
            {
                sy = this._element.InnerBoundingBox.Size.Height + (dy / 2);
                ey = (dy / 2);
                if (!this._element.inflatedY)
                {
                    //this.Element.TransformBoundsTo(new BoundingBox(
                    //        (this._element.InnerBoundingBox.Location.X - (dx / 2)),
                    //        (this._element.InnerBoundingBox.Location.Y - (dy / 2)),
                    //        (this._element.InnerBoundingBox.Size.Width + dx),
                    //        (this._element.InnerBoundingBox.Size.Height + dy)
                    //    ));
                    this.Element.SetHeight(newBBHeight);
                    this.Element.SetY(this.Element.Y - (dy / 2));
                    this._element.inflatedY = true;
                }
            }

            start = new Point(sx, sy);
            end = new Point(ex, ey);

            Point[] curvePoints = GetBezierApproximation(start, end, pull, 256);

            PolyLineSegment segment = new PolyLineSegment(curvePoints, true);
            PathFigure pf = new PathFigure(segment.Points[0], new[] { segment }, false);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(pf);
            var pge = new PathGeometry();
            pge.Figures = pfc;
            Path p = new Path();
            p.Data = pge;
            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            p.Stroke = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            p.StrokeThickness = 3.0;
            grid.Children.Clear();
            //TODO: CREATE ENV VARIABLE for Debug Mode
            //if (false)
            //{
            //    LineGeometry lg = new LineGeometry(new Point((start.X + pull), start.Y), new Point((end.X - pull), end.Y));
            //    Path lp = new Path();
            //    lp.Data = lg;
            //    lp.Stroke = new SolidColorBrush(Color.FromRgb(bc, rc, gc));
            //    lp.StrokeThickness = 3.0;
            //    lp.StrokeStartLineCap = PenLineCap.Round;
            //    lp.StrokeEndLineCap = PenLineCap.Triangle;
            //    grid.Children.Add(lp);

            //    RectangleGeometry rg = new RectangleGeometry(new Rect(start, end));
            //    Path rp = new Path();
            //    rp.Data = rg;
            //    rp.Stroke = new SolidColorBrush(Color.FromRgb(gc, bc, rc));
            //    rp.StrokeThickness = 2.0;
            //    grid.Children.Add(rp);

            //}
            grid.Children.Add(p);
        }

        Point[] GetBezierApproximation(Point start, Point end, double pull, int outputSegmentCount)
        {
            Point[] controlPoints = new[] {
                // START
                start,
                // TANGENT START
                new Point((start.X + pull), start.Y),
                // MID
                new Point(((end.X + start.X) / 2), ((end.Y + start.Y) / 2)),
                // TANGENT END
                new Point((end.X - pull), end.Y),
                // END
                end
            };

            Point[] points = new Point[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                double t = (double)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        Point GetBezierPoint(double t, Point[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Point((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
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
            if (this.DataContext != null)
            {
                this._element = this.DataContext as BezierElement;
                this._element.bezView = this;
                //Render();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            if (this.DataContext != null)
            {
                this._element = this.DataContext as BezierElement;
                this._element.bezView = this;
                //Render();
            }
        }

        #endregion

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }

    public class BezierElement : IRenderable, IConnection
    {
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private BoundingBox innerBoundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(BezierElementView);
        private INode origin;
        private INode destination;
        internal BezierElementView bezView;

        #endregion

        #region Properties

        public Point StartPoint
        {
            get
            {
                return new Point(this.Origin.Hotspot.X, this.Origin.Hotspot.Y);
            }
        }
        public Point EndPoint
        {
            get
            {
                return new Point(this.Destination.Hotspot.X, this.Destination.Hotspot.Y);
            }
        }
        public Type ViewType { get { return view; } }
        public object ViewKey { get; set; }
        public IRenderView RenderView
        {
            get
            {
                return bezView;
            }
            set
            {
                if (value is BezierElementView)
                {
                    bezView = (BezierElementView)value;
                }
            }
        }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
            private set
            {
                SetProperty(ref boundingBox, value);
                //OnPropertyChanged("BoundingBox");
            }
        }

        public BoundingBox InnerBoundingBox
        {
            get
            {
                return innerBoundingBox;
            }
            internal set
            {
                innerBoundingBox = value;
                //inflatedX = false;
                //inflatedY = false;
            }
        }

        public bool inflatedX = false;
        public bool inflatedY = false;

        public double X { get => BoundingBox.Location.X; }

        public double Y { get => BoundingBox.Location.Y; }

        public double Width
        {
            get
            {
                return BoundingBox.Size.Width;
            }
            set
            {
                BoundingBox.Size.Width = value;
                //OnPropertyChanged("Width");
            }
        }

        public double Height
        {
            get
            {
                return BoundingBox.Size.Height;
            }
            set
            {
                BoundingBox.Size.Height = value;
                //OnPropertyChanged("Height");
            }
        }

        public ElementState State { get; set; }
        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        public BezierDirection Direction { get; private set; }
        bool IRenderable.Visible { get; set; }
        public INode Origin { get => this.origin; }
        public INode Destination { get => this.destination; }
        public ConnectionType ConnectionType { get; }

        private IRenderable _zPrev;
        public IRenderable ZPrev => _zPrev;
        private IRenderable _zNext;
        public IRenderable ZNext => _zNext;
        private IRenderable _parent;
        public IRenderable Parent => _parent;
        private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> Children => _children;

        public void AddChild(IRenderable child)
        {
            if (!this.Children.Contains(child))
            {
                this.Children.Add(child);
                child.SetParent(this);
            }
        }

        public void SetParent(IRenderable parent)
        {
            this._parent = parent;
            if (this._parent != null)
            {
                if (!this._parent.Children.Contains(this))
                {
                    this._parent.Children.Add(this);
                }
            }
        }

        #endregion

        public void SetDestination(INode destination)
        {
            //this.destination.Connections.Remove(this);
            this.destination = destination;
            //this.destination.Connections.Add(this);
            RedrawBezier(this.origin, this.destination);
            if (this.RenderView != null)
            {
                this.RenderView.Render();
            }
        }

        #region Constructors

        public BezierElement()
        {
        }

        public bool TopToBottom => (this.origin.Hotspot.Y < this.destination.Hotspot.Y);
        public bool LeftToRight => (this.origin.Hotspot.X < this.destination.Hotspot.X);

        public void RedrawBezier(INode start, INode end)
        {
            if (this.origin != start) this.origin = start;
            if (this.destination != end) this.destination = end;
            //RedrawBezier(((start.Hotspot.X) - 200), ((start.Hotspot.Y) - 200), ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
            //RedrawBezier(start.Hotspot.X, start.Hotspot.Y, ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
            if (LeftToRight && TopToBottom)
            {
                //BottomRight
                this.BoundingBox = new BoundingBox(start.Hotspot.X, start.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (!LeftToRight && !TopToBottom)
            {
                //TopLeft
                this.BoundingBox = new BoundingBox(end.Hotspot.X, end.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (LeftToRight && !TopToBottom)
            {
                //TopRight
                this.BoundingBox = new BoundingBox(start.Hotspot.X, end.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (!LeftToRight && TopToBottom)
            {
                //BottomLeft
                this.BoundingBox = new BoundingBox(end.Hotspot.X, start.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            this.InnerBoundingBox = this.BoundingBox;
            this.inflatedX = false;
            this.inflatedY = false;
        }


        //public BezierElement(double x, double y, double width, double height)
        //{
        //    RedrawBezier(x, y, width, height);
        //}
        //public BezierElement(double x, double y, double width, double height, bool rtl) : this(x, y, width, height)
        //{
        //    if (rtl) this.Direction = BezierDirection.ForceRightToLeft;
        //    else this.Direction = BezierDirection.ForceLeftToRight;
        //}

        //internal bool requestedRedraw = false;
        public BezierElement(INode start, INode end)
        {
            if (this.origin != start) this.origin = start;
            if (this.destination != end) this.destination = end;
            if (this.origin.NodeType == NodeType.Input) this.Direction = BezierDirection.ForceRightToLeft;
            else this.Direction = BezierDirection.ForceLeftToRight;
            RedrawBezier(start, end);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                //this.boundingBox.PropertyChanged += this.PropertyChanged;
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
    }

    public enum BezierDirection
    {
        Default = 0,
        ForceLeftToRight = 1,
        ForceRightToLeft = 2
    }
}