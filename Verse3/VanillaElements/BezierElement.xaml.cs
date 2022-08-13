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
        private BezierElement _element;
        //BoundingBox innerBounds = default;
        //private double _boundx = 0.0;
        //private double _expBoundx = 0.0;
        //private double _boundy = 0.0;
        //private double _expBoundy = 0.0;

        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value as BezierElement;
                this._element.bezView = this;
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

        internal bool _rendering = false;
        public void Render()
        {
            if (this._rendering) return;
            this._rendering = true;
            if (this.Element != null)
            {
                //++
                if (this._element.bezView != this) this._element.bezView = this;
                //if (!this._element.requestedRedraw)
                //{
                //    this._element.RedrawBezier(this._element.Origin, this._element.Destination);
                //}
                DrawBezierCurve(MainGrid, (this._element).Direction);
            }

            this._rendering = false;
        }

        Point start = default;
        Point end = default;
        //double _expBoundx = 0.0;
        public void DrawBezierCurve(Grid grid, BezierDirection dir = BezierDirection.Default)
        {
            grid.UpdateLayout();
            double sx = 0.0, sy = 0.0, ex = 0.0, ey = 0.0;
            if (this._element.LeftToRight) ex = this._element.InnerBoundingBox.Size.Width;
            else sx = this._element.InnerBoundingBox.Size.Width;
            if (this._element.TopToBottom) ey = this._element.InnerBoundingBox.Size.Height;
            else sy = this._element.InnerBoundingBox.Size.Height;

            start = new Point(sx, sy);
            end = new Point(ex, ey);

            //NOTE TO DEV: PointToScreen DOES NOT WORK WELL - VERY JANKY. DO NOT USE!!!
            //start = DataViewModel.WPFControl.PointToScreen(new Point(this._element.Origin.Hotspot.X, this._element.Origin.Hotspot.Y));
            //end = DataViewModel.WPFControl.PointToScreen(new Point(this._element.Destination.Hotspot.X, this._element.Destination.Hotspot.Y));

            //start = this.PointFromScreen(start);
            //end = this.PointFromScreen(end);

            //TODO: Check the following if statement for correctness
            //if (dir == BezierDirection.ForceLeftToRight)
            //{
            //    start = new Point(sx, sy);
            //    end = new Point(ex, ey);
            //}
            //else
            //{
            //    end = new Point(sx, sy);
            //    start = new Point(ex, ey);
            //}

            double pull = 0.5 * Math.Abs(end.Y - start.Y);
            pull = Math.Max(pull, (Math.Abs(0.5 * (end.X - start.X))));
            Point[] points = new[] {
                //new Point(0, 0), START
                start,
                //new Point(190, 0), TANGENT START
                new Point((start.X + pull), start.Y),
                //new Point(200, 100), MID
                new Point(((end.X + start.X) / 2), ((end.Y + start.Y) / 2)),
                //new Point(210, 200), TANGENT END
                new Point((end.X - pull), end.Y),
                //new Point(400, 200) END
                end
            };
            Point[] curvePoints = GetBezierApproximation(points, 256);

            double minx = default, maxx = default, miny = default, maxy = default;
            int left = -1, right = -1, top = -1, bottom = -1;
            int j = 0;
            foreach (Point pt in curvePoints)
            {
                if ((minx == default) || (pt.X < minx))
                {
                    minx = pt.X;
                    left = j;
                }
                if ((maxx == default) || (pt.X > maxx))
                {
                    maxx = pt.X;
                    right = j;
                }
                if ((miny == default) || (pt.Y < miny))
                {
                    miny = pt.Y;
                    top = j;
                }
                if ((maxy == default) || (pt.Y > maxy))
                {
                    maxy = pt.Y;
                    bottom = j;
                }
                j++;
            }

            //TODO: FIX BEZIER BUG
            //TODO: Add canvas expansion offset
            //if (!this._element.inflatedX || this._element.inflatedY)
            //{
            if (!this._element.inflatedX)
            {
                if (Math.Abs(maxx - minx) != this._element.InnerBoundingBox.Size.Width)
                {
                    this.Element.SetWidth(Math.Abs(maxx - minx));
                    //this.Element.BoundingBox.Inflate(new CanvasSize((Math.Abs(maxx - minx) - this._element.InnerBoundingBox.Size.Width), 0.0));
                    this._element.inflatedX = true;
                    this.Element.SetX(this._element.InnerBoundingBox.Location.X + minx);
                    //this.Element.SetX(this._element.InnerBoundingBox.Location.X - ((Math.Abs(maxx - minx) - this._element.InnerBoundingBox.Size.Width) / 4));
                    for (int i = 0; i < curvePoints.Length; i++)
                    {
                        curvePoints[i].X -= minx;
                    }
                }
            }
            if (!this._element.inflatedY)
            {
                if (Math.Abs(maxy - miny) != this._element.InnerBoundingBox.Size.Height)
                {
                    this.Element.SetHeight(Math.Abs(maxy - miny));
                    //this.Element.BoundingBox.Inflate(new CanvasSize(0.0, (Math.Abs(maxy - miny) - this._element.InnerBoundingBox.Size.Height)));
                    this._element.inflatedY = true;
                    this.Element.SetY(this._element.InnerBoundingBox.Location.Y + miny);
                    //this.Element.SetY(this._element.InnerBoundingBox.Location.Y - (Math.Abs(maxy - miny) - this._element.InnerBoundingBox.Size.Height));
                    for (int i = 0; i < curvePoints.Length; i++)
                    {
                        curvePoints[i].Y -= miny;
                    }
                }
            }
            //if (this._element.inflatedX)
            //{
            //    //this.Element.SetX(this._element.InnerBoundingBox.Location.X - (Math.Abs(maxx - minx) / 4.0));
            //    this.Element.SetX(this._element.InnerBoundingBox.Location.X + minx);
            //    for (int i = 0; i < curvePoints.Length; i++)
            //    {
            //        curvePoints[i].X -= minx;
            //    }
            //}
            //if (this._element.inflatedY)
            //{
            //    //this.Element.SetY(this._element.InnerBoundingBox.Location.Y - (Math.Abs(maxy - miny) / 4.0));
            //    this.Element.SetY(this._element.InnerBoundingBox.Location.Y + miny);
            //    for (int i = 0; i < curvePoints.Length; i++)
            //    {
            //        curvePoints[i].Y -= miny;
            //    }
            //}
            //this._element.OnPropertyChanged("BoundingBox");
            //}
            //_boundx = this.Element.BoundingBox.Size.Width;
            //this._element.InnerBoundingBox = this._element.BoundingBox;
            //_expBoundx = Math.Abs(this._element.InnerBoundingBox.Size.Width - Math.Abs(maxx - minx)) / 4.0;
            //if (_expBoundx != 0.0)
            //{
            //    this._element.BoundingBox.Inflate(new CanvasSize(Math.Abs(_expBoundx * 2), 0.0));
            //    //TODO:Inflate along Y axis to account for curve thickness

            //    this.Element.SetX(this._element.InnerBoundingBox.Left + _expBoundx);
            //    //if (minx < points[0].X)
            //    //{
            //    for (int i = 0; i < points.Length; i++)
            //    {
            //        points[i].X += minx;
            //    }
            //    //}
            //    //++++
            //    this._element.OnPropertyChanged("BoundingBox");
            //    //DataViewModel.WPFControl.ExpandContent();
            //}
            //}

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
            grid.Children.Add(p);
        }

        Point[] GetBezierApproximation(Point[] controlPoints, int outputSegmentCount)
        {
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
                //OnPropertyChanged("Width");
                //--
                //if (bezView != null)
                //    bezView.Render();
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
                //if (bezView != null) bezView.Render();
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
                //if (bezView != null) bezView.Render();
            }
        }

        //public Guid ZPrev { get; }

        //public Guid ZNext { get; }

        //public Guid Parent { get; }

        //public Guid[] Children { get; }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; }

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
        }

        #endregion

        public void SetDestination(INode destination)
        {
            //this.destination.Connections.Remove(this);
            this.destination = destination;
            //this.destination.Connections.Add(this);
            RedrawBezier(this.origin, this.destination);
        }

        #region Constructors

        //public BezierElement()
        //{
        //}

        //internal bool topToBottom = true;
        //internal bool leftToRight = true;

        public bool TopToBottom => (this.origin.Hotspot.Y < this.destination.Hotspot.Y);
        public bool LeftToRight => (this.origin.Hotspot.X < this.destination.Hotspot.X);

        public void RedrawBezier(double x, double y, double width, double height)
        {
            //requestedRedraw = true;
            if (this.bezView == null || !this.bezView._rendering)
            {
                //if ((height < 0.0 && width > 0.0) || (width < 0 && height > 0.0))
                //{
                //    topToBottom = false;
                //}
                //else if ((height > 0.0 && width > 0.0) || (width < 0.0 && height < 0.0))
                //{
                //    topToBottom = true;
                //}
                //leftToRight = (width > 0.0);
                if (LeftToRight && TopToBottom)
                {
                    //BottomRight
                    this.BoundingBox = new BoundingBox(x, y, Math.Abs(width), Math.Abs(height));
                }
                else if (!LeftToRight && !TopToBottom)
                {
                    //BottomLeft
                    this.BoundingBox = new BoundingBox((x - Math.Abs(width)), y, Math.Abs(width), Math.Abs(height));
                }
                else if (LeftToRight && !TopToBottom)
                {
                    //TopRight
                    this.BoundingBox = new BoundingBox(x, (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
                else if (!LeftToRight && TopToBottom)
                {
                    //TopLeft
                    this.BoundingBox = new BoundingBox((x - Math.Abs(width)), (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
                //OnPropertyChanged("BoundingBox");

                if (!this.inflatedX || this.inflatedY)
                {
                    this.InnerBoundingBox = this.BoundingBox;
                    if (bezView != null)
                        bezView.Render();
                }
                else
                {
                    if (this.InnerBoundingBox != null)
                    {
                        if (this.BoundingBox != this.InnerBoundingBox)
                        {
                            this.inflatedX = false;
                            this.inflatedY = false;
                            this.InnerBoundingBox = this.BoundingBox;
                            if (bezView != null)
                                bezView.Render();
                        }
                    }
                }


                //requestedRedraw = false;
            }
        }

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
            if (bezView != null)
                bezView.Render();
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