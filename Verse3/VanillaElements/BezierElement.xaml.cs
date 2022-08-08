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
        private double _boundx = 0.0;
        private double _expBoundx = 0.0;
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
                this._element.bezView = this;
                //if (!this._element.requestedRedraw)
                //{
                //    this._element.RedrawBezier(this._element.Origin, this._element.Destination);
                //}
                DrawBezierCurve(MainGrid, (this._element).Direction);
            }

            this._rendering = false;
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
                Render();
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

        Point start = default;
        Point end = default;
        public void DrawBezierCurve(Grid grid, BezierDirection dir = BezierDirection.Default)
        {
            if (this._element.topToBottom)
            {
                if (dir == BezierDirection.ForceRightToLeft)
                {
                    if (this._element.leftToRight)
                    {
                        start = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                        end = new Point(0.0, 0.0);
                        //top left to bottom right (RTL)
                    }
                    else
                    {
                        start = new Point(0.0, 0.0);
                        end = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                        //bottom right to top left (RTL)
                    }
                }
                else if (dir == BezierDirection.ForceLeftToRight)
                {
                    if (this._element.leftToRight)
                    {
                        start = new Point(0.0, 0.0);
                        end = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                        //top left to bottom right (LTR)
                    }
                    else
                    {
                        start = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                        end = new Point(0.0, 0.0);
                        //bottom right to top left (LTR)
                    }
                }
                else
                {
                    start = new Point(0.0, 0.0);
                    end = new Point(this.Element.BoundingBox.Size.Width, this.Element.BoundingBox.Size.Height);
                    //top left to bottom right (DEF)
                    //bottom right to top left (DEF)
                }
            }
            else
            {
                if (dir == BezierDirection.ForceRightToLeft)
                {
                    if (this._element.leftToRight)
                    {
                        start = new Point(this.Element.BoundingBox.Size.Width, 0.0);
                        end = new Point(0.0, this.Element.BoundingBox.Size.Height);
                        //bottom left to top right (RTL)
                    }
                    else
                    {
                        start = new Point(0.0, this.Element.BoundingBox.Size.Height);
                        end = new Point(this.Element.BoundingBox.Size.Width, 0.0);
                        //top right to bottom left (RTL)
                    }
                }
                else if (dir == BezierDirection.ForceLeftToRight)
                {
                    if (this._element.leftToRight)
                    {
                        start = new Point(0.0, this.Element.BoundingBox.Size.Height);
                        end = new Point(this.Element.BoundingBox.Size.Width, 0.0);
                        //bottom left to top right (LTR)
                    }
                    else
                    {
                        start = new Point(this.Element.BoundingBox.Size.Width, 0.0);
                        end = new Point(0.0, this.Element.BoundingBox.Size.Height);
                        //top right to bottom left (LTR)
                    }
                }
                else
                {
                    start = new Point(0.0, this.Element.BoundingBox.Size.Height);
                    end = new Point(this.Element.BoundingBox.Size.Width, 0.0);
                    //bottom left to top right (DEF)
                    //top right to bottom left (DEF)
                }
            }
            double pull = 0.75 * Math.Abs(end.Y - start.Y);
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

            //EXPAND BOUNDING BOX------------------------------------------------
            double minx = default, maxx = default, miny = default, maxy = default;
            foreach (Point pt in points)
            {
                if (minx == default) minx = pt.X;
                if (maxx == default) maxx = pt.X;
                if (miny == default) miny = pt.Y;
                if (maxy == default) maxy = pt.Y;
                if (pt.X < minx) minx = pt.X;
                if (pt.X > maxx) maxx = pt.X;
                if (pt.Y < miny) miny = pt.Y;
                if (pt.Y > maxy) maxy = pt.Y;
            }
            if (minx < points[0].X)
            {
                _boundx = this.Element.BoundingBox.Size.Width;
                _expBoundx = Math.Abs(this.Element.BoundingBox.Size.Width - Math.Abs(maxx - minx)) / 4.0;
                this.Element.BoundingBox.Inflate(new CanvasSize((_expBoundx * 2), 0.0));
                //TODO:Inflate along Y axis to account for curve thickness

                //this._element.bezView = this;
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X += _expBoundx;
                }
                this.Element.SetX(this.Element.X - _expBoundx);
            }


            //++++
            this.Element.OnPropertyChanged("BoundingBox");
            //DataViewModel.WPFControl.ExpandContent();



            var b = GetBezierApproximation(points, 256);
            PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);
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
    }

    public class BezierElement : IRenderable, IConnection
    {        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
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

        public Guid ZPrev { get; }

        public Guid ZNext { get; }

        public Guid Parent { get; }

        public Guid[] Children { get; }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        public BezierDirection Direction { get; private set; }
        bool IRenderable.Visible { get; set; }

        public INode Origin { get => this.origin; }

        public INode Destination { get => this.destination; }

        public ConnectionType ConnectionType { get; }

        #endregion

        public void SetDestination(INode destination)
        {
            this.destination = destination;
            RedrawBezier(this.origin, this.destination);
        }

        #region Constructors

        //public BezierElement()
        //{
        //}

        internal bool topToBottom = true;
        internal bool leftToRight = true;

        public void RedrawBezier(double x, double y, double width, double height)
        {
            //requestedRedraw = true;
            if (this.bezView == null || !this.bezView._rendering)
            {
                if ((height < 0.0 && width > 0.0) || (width < 0 && height > 0.0))
                {
                    topToBottom = false;
                }
                else if ((height > 0.0 && width > 0.0) || (width < 0.0 && height < 0.0))
                {
                    topToBottom = true;
                }
                leftToRight = (width > 0.0);
                if (leftToRight && topToBottom)
                {
                    //BottomRight
                    this.BoundingBox = new BoundingBox(x, y, Math.Abs(width), Math.Abs(height));
                }
                else if (!leftToRight && !topToBottom)
                {
                    //BottomLeft
                    this.BoundingBox = new BoundingBox((x - Math.Abs(width)), y, Math.Abs(width), Math.Abs(height));
                }
                else if (leftToRight && !topToBottom)
                {
                    //TopRight
                    this.BoundingBox = new BoundingBox(x, (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
                else if (!leftToRight && topToBottom)
                {
                    //TopLeft
                    this.BoundingBox = new BoundingBox((x - Math.Abs(width)), (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
                OnPropertyChanged("BoundingBox");

                if (bezView != null)
                    bezView.Render();

                //requestedRedraw = false;
            }
        }

        public void RedrawBezier(INode start, INode end)
        {
            //RedrawBezier(((start.Hotspot.X) - 200), ((start.Hotspot.Y) - 200), ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
            RedrawBezier(start.Hotspot.X, start.Hotspot.Y, ((end.Hotspot.X) - (start.Hotspot.X)), ((end.Hotspot.Y) - (start.Hotspot.Y)));
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
            this.origin = start;
            this.destination = end;
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