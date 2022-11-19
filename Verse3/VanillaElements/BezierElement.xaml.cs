using Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
    public partial class BezierElementView : UserControl, IBaseElementView<BezierElement>
    {
        #region IBaseElementView Members

        private BezierElement _element;
        public BezierElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as BezierElement;
                }
                return _element;
            }
            private set
            {
                _element = value as BezierElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

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
                if (this.Element.RenderView != this) this.Element.RenderView = this;
                DrawBezierCurve(MainGrid, (this.Element).Direction);
            }

            this._rendering = false;
        }

        #endregion

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
            Cursor = Cursors.Hand;
            //IRenderable myRectangle = (IRenderable)rectangle.DataContext;

            ////myRectangle.IsSelected = true;

            ////mouseButtonDown = e.ChangedButton;

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

            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                try
                {
                    //TODO: Delete connection
                    IRenderable start = this.Element.Origin as IRenderable;
                    IRenderable end = this.Element.Destination as IRenderable;
                    this.Element.Remove();
                    RenderPipeline.RenderRenderable(start);
                    RenderPipeline.RenderRenderable(end);
                    //return;
                }
                catch (Exception ex)
                {
                    CoreConsole.Log(ex);
                }
            }

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
            //if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            //{
            //    Cursor = Cursors.Hand;
                //TODO: Prep to delete connection

                //DataViewModel.WPFControl.ExpandContent();

                //e.Handled = true;

                //return;
            //}
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
                this.Element = this.DataContext as BezierElement;
                this.Element.RenderView = this;
                //Render();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            if (this.DataContext != null)
            {
                this.Element = this.DataContext as BezierElement;
                this.Element.RenderView = this;
                //Render();
            }
        }

        #endregion

        Point start = default;
        Point end = default;
        public void DrawBezierCurve(Grid grid, BezierDirection dir = BezierDirection.Default)
        {
            //TODO: Adjustable x and y pull settings
            double xPull = 0.5;
            double yPull = 0.75;
            double hmargin = 5.0, vmargin = 5.0;
            double pull = yPull * this.Element.InnerBoundingBox.Size.Height;
            if ((yPull * this.Element.InnerBoundingBox.Size.Height)
                < (Math.Abs(xPull * this.Element.InnerBoundingBox.Size.Width)))
            {
                pull = xPull * this.Element.InnerBoundingBox.Size.Width;
            }
            //TODO: Calculate required width without having to calculate the curve first
            //Math.Abs((end.X - pull) - (start.X + pull)) CONTROL POINTS WIDTH
            //Math.Abs((end.X - (pull * 0.5)) - (start.X + (pull * 0.5))) FINAL WIDTH
            double sx = 0.0, sy = 0.0, ex = 0.0, ey = 0.0;
            double dx = (4 * pull / 5) + (2 * hmargin);
            double dy = (2 * vmargin);
            double newBBWidth = this.Element.InnerBoundingBox.Size.Width + dx;
            double newBBHeight = this.Element.InnerBoundingBox.Size.Height + dy;
            if (this.Element.InnerBoundingBox.Size.Width < pull)
            {
                if (this.Element.LeftToRight)
                {
                    sx = (dx / 2);
                    ex = this.Element.InnerBoundingBox.Size.Width + (dx / 2);
                }
                else
                {
                    sx = this.Element.InnerBoundingBox.Size.Width + (dx / 2);
                    ex = (dx / 2);
                }
                if (!this.Element.inflatedX)
                {
                    (this.Element as IRenderable).SetWidth(newBBWidth);
                    (this.Element as IRenderable).SetX(this.Element.X - (dx / 2));
                    this.Element.inflatedX = true;
                }
            }
            else
            {
                if (this.Element.LeftToRight)
                {
                    dx = (2 * hmargin);
                    sx = (dx / 2);
                    ex = this.Element.InnerBoundingBox.Size.Width + (dx / 2);
                }
                else
                {
                    sx = this.Element.InnerBoundingBox.Size.Width + (dx / 2);
                    ex = (dx / 2);
                }
                if (!this.Element.inflatedX)
                {
                    (this.Element as IRenderable).SetWidth(newBBWidth);
                    (this.Element as IRenderable).SetX(this.Element.X - (dx / 2));
                    this.Element.inflatedX = true;
                }
            }

            if (this.Element.TopToBottom)
            {
                ey = this.Element.InnerBoundingBox.Size.Height + (dy / 2);
                sy = (dy / 2);
                if (!this.Element.inflatedY)
                {
                    (this.Element as IRenderable).SetHeight(newBBHeight);
                    (this.Element as IRenderable).SetY(this.Element.Y - (dy / 2));
                    this.Element.inflatedY = true;
                }
            }
            else
            {
                sy = this.Element.InnerBoundingBox.Size.Height + (dy / 2);
                ey = (dy / 2);
                if (!this.Element.inflatedY)
                {
                    (this.Element as IRenderable).SetHeight(newBBHeight);
                    (this.Element as IRenderable).SetY(this.Element.Y - (dy / 2));
                    this.Element.inflatedY = true;
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
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //p.Stroke = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            p.Stroke = new SolidColorBrush(Colors.White);
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

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }

    [Serializable]
    public class BezierElement : BaseElement, IConnection
    {
        #region Data Members

        private BoundingBox innerBoundingBox = BoundingBox.Unset;
        private INode origin;
        private INode destination;
        internal bool inflatedX = false;
        internal bool inflatedY = false;

        #endregion

        #region Properties

        [JsonIgnore]
        [IgnoreDataMember]
        public BoundingBox InnerBoundingBox { get => this.innerBoundingBox; private set => this.innerBoundingBox = value; }
        [JsonIgnore]
        [IgnoreDataMember]
        public Point StartPoint => new Point(this.Origin.Hotspot.X, this.Origin.Hotspot.Y);
        [JsonIgnore]
        [IgnoreDataMember]
        public Point EndPoint => new Point(this.Destination.Hotspot.X, this.Destination.Hotspot.Y);
        [JsonIgnore]
        [IgnoreDataMember]
        public override Type ViewType => typeof(BezierElementView);
        [JsonIgnore]
        [IgnoreDataMember]
        public BezierDirection Direction { get; private set; }
        //[JsonIgnore]
        //[IgnoreDataMember]
        public INode Origin { get => this.origin; }
        //[JsonIgnore]
        //[IgnoreDataMember]
        public INode Destination { get => this.destination; }
        public ConnectionType ConnectionType { get; }
        [JsonIgnore]
        [IgnoreDataMember]
        public bool TopToBottom => (this.origin.Hotspot.Y < this.destination.Hotspot.Y);
        [JsonIgnore]
        [IgnoreDataMember]
        public bool LeftToRight => (this.origin.Hotspot.X < this.destination.Hotspot.X);
        
        #endregion

        public bool SetDestination(INode destination)
        {
            //TODO: LOOP WARNING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //this.destination.Connections.Remove(this);
            if (this.destination != MousePositionNode.Instance && this.origin == MousePositionNode.Instance)
            {
                //TODO: Check whether destination is valid
                bool check = (destination.GetType().BaseType == this.destination.GetType().BaseType);
                check = check && (destination.NodeType != this.destination.NodeType);
                check = NodeUtilities.CheckCompatibility(destination, this.destination);
                if (!check) return false;
                this.origin = destination;
            }
            else
            {
                //TODO: Check whether destination is valid
                bool check = (destination.GetType().BaseType == this.origin.GetType().BaseType);
                check = check && (destination.NodeType != this.origin.NodeType);
                check = NodeUtilities.CheckCompatibility(this.origin, destination);
                if (!check) return false;
                this.destination = destination;
            }
            //this.destination.Connections.Add(this);
            RedrawBezier(this.origin, this.destination);
            if (this.RenderView != null)
            {
                this.RenderView.Render();
            }
            return true;
        }

        public void Remove()
        {
            try
            {
                this.origin.Connections.Remove(this);
                this.destination.Connections.Remove(this);
                //this.origin = null;
                //this.destination = null;
                DataViewModel.Instance.Elements.Remove(this);
                this.Dispose();
            }
            catch (Exception ex)
            {
                CoreConsole.Log(ex);
            }
        }

        #region Constructors

        public BezierElement() : base()
        {
        }
        
        public BezierElement(INode start, INode end) : base()
        {
            if (this.origin != start) this.origin = start;
            if (this.destination != end) this.destination = end;
            //if (this.origin.NodeType == NodeType.Input) this.Direction = BezierDirection.ForceRightToLeft;
            //else this.Direction = BezierDirection.ForceLeftToRight;
            if (this.origin.NodeType == NodeType.Input)
            {
                this.destination = start;
                this.origin = end;
            }
            RedrawBezier(this.origin, this.destination);
        }

        public BezierElement(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            INode o = (INode)info.GetValue("origin", typeof(INode));
            INode d = (INode)info.GetValue("destination", typeof(INode));
            if (o != null) this.origin = o;
            if (d != null) this.destination = d;
            if (this.origin != null && this.Destination != null) RedrawBezier(this.origin, this.destination);
            else throw new NullReferenceException("BezierElement: Origin and/or Destination is null");
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("origin", this.origin);
            info.AddValue("destination", this.destination);
        }

        #endregion

        public void RedrawBezier(INode start, INode end)
        {
            if (this.origin != start) this.origin = start;
            if (this.destination != end) this.destination = end;
            //RedrawBezier(((start.Hotspot.X) - 200), ((start.Hotspot.Y) - 200), ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
            //RedrawBezier(start.Hotspot.X, start.Hotspot.Y, ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
            if (LeftToRight && TopToBottom)
            {
                //BottomRight
                base.boundingBox = new BoundingBox(start.Hotspot.X, start.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (!LeftToRight && !TopToBottom)
            {
                //TopLeft
                base.boundingBox = new BoundingBox(end.Hotspot.X, end.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (LeftToRight && !TopToBottom)
            {
                //TopRight
                base.boundingBox = new BoundingBox(start.Hotspot.X, end.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            else if (!LeftToRight && TopToBottom)
            {
                //BottomLeft
                base.boundingBox = new BoundingBox(end.Hotspot.X, start.Hotspot.Y, Math.Abs((end.Hotspot.X) - (start.Hotspot.X)), Math.Abs((end.Hotspot.Y) - (start.Hotspot.Y)));
            }
            this.OnPropertyChanged("BoundingBox");
            this.InnerBoundingBox = this.BoundingBox;
            this.inflatedX = false;
            this.inflatedY = false;
        }
    }

    public enum BezierDirection
    {
        Default = 0,
        ForceLeftToRight = 1,
        ForceRightToLeft = 2
    }
}