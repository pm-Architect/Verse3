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

        //public static readonly DependencyProperty StartProperty =
        //        DependencyProperty.Register("Start", typeof(System.Drawing.Point), typeof(BezierElement),
        //                                    new FrameworkPropertyMetadata(System.Drawing.Point.Empty, Start_PropertyChanged));

        //private static void Start_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BezierElement b = (BezierElement)d;
        //}

        //public static readonly DependencyProperty EndProperty =
        //        DependencyProperty.Register("End", typeof(System.Drawing.Point), typeof(BezierElement),
        //                                    new FrameworkPropertyMetadata(System.Drawing.Point.Empty, End_PropertyChanged));

        //private static void End_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    BezierElement b = (BezierElement)d;
        //}

        private IRenderable _element;
        
        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value;
                if (((BezierElement)this.Element) != null)
                    ((BezierElement)this.Element).PropertyChanged += BezierElementView_PropertyChanged;
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
        
        private void BezierElementView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!rendering)
            Render();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                DrawBezierCurve(MainGrid, ((BezierElement)this.Element).Direction);
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

        private bool rendering = false;
        private bool expanded = false;
        public void DrawBezierCurve(Grid grid, BezierDirection dir = BezierDirection.Default)
        {
            rendering = true;
            BezierElement element = (BezierElement)this.Element;
            Point start = new Point(element.Start.X, element.Start.Y);
            Point end = new Point(element.End.X, element.End.Y);
            element.BoundingBox.Size.Width = Math.Abs(element.End.X - element.Start.X);
            element.BoundingBox.Size.Height = Math.Abs(element.End.Y - element.Start.Y);
            if (element.TopToBottom)
            {
                if (dir == BezierDirection.ForceRightToLeft)
                {
                    if (element.LeftToRight)
                    {
                        start = new Point(element.BoundingBox.Size.Width, element.BoundingBox.Size.Height);
                        end = new Point(0.0, 0.0);
                        //top left to bottom right (RTL)
                    }
                    else
                    {
                        start = new Point(0.0, 0.0);
                        end = new Point(element.BoundingBox.Size.Width, element.BoundingBox.Size.Height);
                        //bottom right to top left (RTL)
                    }
                }
                else if (dir == BezierDirection.ForceLeftToRight)
                {
                    if (element.LeftToRight)
                    {
                        start = new Point(0.0, 0.0);
                        end = new Point(element.BoundingBox.Size.Width, element.BoundingBox.Size.Height);
                        //top left to bottom right (LTR)
                    }
                    else
                    {
                        start = new Point(element.BoundingBox.Size.Width, element.BoundingBox.Size.Height);
                        end = new Point(0.0, 0.0);
                        //bottom right to top left (LTR)
                    }
                }
                else
                {
                    start = new Point(0.0, 0.0);
                    end = new Point(element.BoundingBox.Size.Width, element.BoundingBox.Size.Height);
                    //top left to bottom right (DEF)
                    //bottom right to top left (DEF)
                }
            }
            else
            {
                if (dir == BezierDirection.ForceRightToLeft)
                {
                    if (element.LeftToRight)
                    {
                        start = new Point(element.BoundingBox.Size.Width, 0.0);
                        end = new Point(0.0, element.BoundingBox.Size.Height);
                        //bottom left to top right (RTL)
                    }
                    else
                    {
                        start = new Point(0.0, element.BoundingBox.Size.Height);
                        end = new Point(element.BoundingBox.Size.Width, 0.0);
                        //top right to bottom left (RTL)
                    }
                }
                else if (dir == BezierDirection.ForceLeftToRight)
                {
                    if (element.LeftToRight)
                    {
                        start = new Point(0.0, element.BoundingBox.Size.Height);
                        end = new Point(element.BoundingBox.Size.Width, 0.0);
                        //bottom left to top right (LTR)
                    }
                    else
                    {
                        start = new Point(element.BoundingBox.Size.Width, 0.0);
                        end = new Point(0.0, element.BoundingBox.Size.Height);
                        //top right to bottom left (LTR)
                    }
                }
                else
                {
                    start = new Point(0.0, element.BoundingBox.Size.Height);
                    end = new Point(element.BoundingBox.Size.Width, 0.0);
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
            var b = GetBezierApproximation(ExpandBounds(points), 256);
            PathFigure pf = new PathFigure(b.Points[0], new[] { b }, false);
            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(pf);
            var pge = new PathGeometry();
            pge.Figures = pfc;
            Path p = new Path();
            p.Data = pge;
            p.Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            p.StrokeThickness = 2;
            grid.Children.Clear();
            grid.Children.Add(p);
            grid.RenderSize = new Size(element.Width, element.Height);
            
            rendering = false;
        }

        public Point[] ExpandBounds(Point[] points)
        {
            expanded = true;
            BezierElement element = (BezierElement)this.Element;
            double minx = default, maxx = default, miny = default, maxy = default;
            foreach (Point p in points)
            {
                if (minx == default) minx = p.X;
                if (maxx == default) maxx = p.X;
                if (miny == default) miny = p.Y;
                if (maxy == default) maxy = p.Y;
                if (p.X < minx) minx = p.X;
                if (p.X > maxx) maxx = p.X;
                if (p.Y < miny) miny = p.Y;
                if (p.Y > maxy) maxy = p.Y;
            }
            if (minx < points[0].X)
            {
                double dx = (element.InnerBoundingBox.Size.Width - (minx - maxx)) / 4.0;
                if (expanded)
                {
                    element.BoundingBox = element.InnerBoundingBox;
                }
                else
                {
                    element.InnerBoundingBox = element.BoundingBox;
                }
                element.BoundingBox.Inflate(new CanvasSize((dx * 2), 0.0));
                for (int i = 0; i < points.Length; i++)
                {
                    points[i].X += dx;
                }
                this.Element.SetX(element.InnerBoundingBox.Location.X - dx);
            }
            return points;
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

        public BoundingBox BoundingBox { get => boundingBox; internal set => boundingBox = value; }
        public BoundingBox InnerBoundingBox
        { 
            get
            {
                if (!innerBoundingBox.IsValid()) return BoundingBox;
                else return innerBoundingBox;
            }
            internal set => innerBoundingBox = value;
        }

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
        public BezierDirection Direction { get; private set; }
        bool IRenderable.Visible { get; set; }
        System.Drawing.Point _start;
        System.Drawing.Point _end;
        public System.Drawing.Point Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }
        public System.Drawing.Point End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
                OnPropertyChanged("End");
            }
        }

        #endregion

        #region Constructors

        public BezierElement()
        {
        }

        //private bool topToBottom = true;
        public bool TopToBottom
        {
            get
            {
                if (this.Start != System.Drawing.Point.Empty && this.End != System.Drawing.Point.Empty)
                {
                    double height = (this.End.X - this.Start.X);
                    double width = (this.End.Y - this.Start.Y);
                    if ((height < 0 && width > 0) || (width < 0 && height > 0))
                    {
                        return false;
                    }
                    else if ((height > 0 && width > 0) || (width < 0 && height < 0))
                    {
                        return true;
                    }
                }
                return default;
            }
        }
        //private bool leftToRight = true;
        public bool LeftToRight
        {
            get
            {
                if (this.Start != System.Drawing.Point.Empty && this.End != System.Drawing.Point.Empty)
                {
                    double width = (this.End.Y - this.Start.Y);
                    return (width > 0);
                }
                return default;
            }
        }
        private BoundingBox innerBoundingBox = BoundingBox.Unset;

        public BezierElement(int x, int y, int width, int height)
        {
            if (this.Start != System.Drawing.Point.Empty && this.End != System.Drawing.Point.Empty)
            {
                if (LeftToRight && TopToBottom)
                {
                    //BottomRight
                    this.boundingBox = new BoundingBox(x, y, Math.Abs(width), Math.Abs(height));
                }
                else if (!LeftToRight && !TopToBottom)
                {
                    //BottomLeft
                    this.boundingBox = new BoundingBox((x - Math.Abs(width)), y, Math.Abs(width), Math.Abs(height));
                }
                else if (LeftToRight && !TopToBottom)
                {
                    //TopRight
                    this.boundingBox = new BoundingBox(x, (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
                else if (!LeftToRight && TopToBottom)
                {
                    //TopLeft
                    this.boundingBox = new BoundingBox((x - Math.Abs(width)), (y - Math.Abs(height)), Math.Abs(width), Math.Abs(height));
                }
            }
                //if ((height < 0 && width > 0) || (width < 0 && height > 0))
                //{
                //    topToBottom = false;                
                //}
                //else if ((height > 0 && width > 0) || (width < 0 && height < 0))
                //{
                //    topToBottom = true;
                //}
                //LeftToRight = (width > 0);
        }
        internal BezierElement(int x, int y, int width, int height, bool rtl) : this(x, y, width, height)
        {
            if (rtl) this.Direction = BezierDirection.ForceRightToLeft;
            else this.Direction = BezierDirection.ForceLeftToRight;
        }
        public BezierElement(System.Drawing.Point start, System.Drawing.Point end, bool rtl) : this((start.X - 200), (start.Y - 200), (end.X - start.X), (end.Y - start.Y), rtl)
        {
            this._start = start;
            this._end = end;
            DataTemplateManager.RegisterDataTemplate(this as IRenderable);
            DataModel.Instance.Elements.Add(this);
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
    }

    public enum BezierDirection
    {
        Default = 0,
        ForceLeftToRight = 1,
        ForceRightToLeft = 2
    }
}