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
using static Core.Geometry2D;
using Verse3.VanillaElements.Utilities;
using Verse3.VanillaElements;

namespace Verse3.CanvasElements
{
    /// <summary>
    /// Interaction logic for ConnectionElement.xaml
    /// </summary>
    public partial class ConnectionElementView : UserControl, IRenderView
    {
        public ConnectionElementView()
        {
            InitializeComponent();
        }

        private ConnectionElement _element;
        private Point lastMousePoint;
        private bool isDragging;
        internal bool rendering;
        private static readonly double DragThreshold = 2;
        internal bool expanded = false;

        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = (ConnectionElement)value;
                if (((BezierElement)this.Element) != null)
                    ((BezierElement)this.Element).PropertyChanged += BezierElementView_PropertyChanged;
            }
        }
        private void BezierElementView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (!rendering)
            //    Render();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                BezierUtilities.DrawBezierCurve((ConnectionElement)this.Element, this);
            }
        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Element = this.DataContext as IRenderable;
        }

        #region Overrides

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging)
            {
                //
                // Raise the event to notify that dragging is in progress.
                //

                Point curMousePoint = e.GetPosition(DataViewModel.WPFControl.LBcontent);
                Vector offset = curMousePoint - lastMousePoint;
                if (offset.X != 0.0 &&
                    offset.Y != 0.0)
                {
                    lastMousePoint = curMousePoint;

                    //RaiseEvent(new ConnectorItemDraggingEventArgs(ConnectorDraggingEvent, this, offset.X, offset.Y));
                }

                e.Handled = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if (this.ParentNetworkView != null &&
                //    this.ParentNetworkView.EnableConnectionDragging)
                //{
                    //
                    // The user is left-dragging the connector and connection dragging is enabled,
                    // but don't initiate the drag operation until 
                    // the mouse cursor has moved more than the threshold distance.
                    //
                    Point curMousePoint = e.GetPosition(DataViewModel.WPFControl.LBcontent);
                    var dragDelta = curMousePoint - lastMousePoint;
                    double dragDistance = Math.Abs(dragDelta.Length);
                    if (dragDistance > DragThreshold)
                    {
                        //
                        // When the mouse has been dragged more than the threshold value commence dragging the node.
                        //

                        //
                        // Raise an event to notify that that dragging has commenced.
                        //
                        //var eventArgs = new ConnectorItemDragStartedEventArgs(ConnectorDragStartedEvent, this);
                        //RaiseEvent(eventArgs);

                        //if (eventArgs.Cancel)
                        //{
                        //    //
                        //    // Handler of the event disallowed dragging of the node.
                        //    //
                        //    isLeftMouseDown = false;
                        //    return;
                        //}

                        isDragging = true;
                        this.CaptureMouse();
                        e.Handled = true;
                    }
                //}
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            //TODO: Bring Parent Element to Focus
            //if (e.ChangedButton == MouseButton.Left)
            //{
            //  TODO: Trigger Parent Element Methods
            //}

            lastMousePoint = e.GetPosition(DataViewModel.WPFControl.LBcontent);
            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (isDragging)
                    {
                        //RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, this));

                        this.ReleaseMouseCapture();

                        isDragging = false;
                    }
                    else
                    {
                        //
                        // Execute mouse up selection logic only if there was no drag operation.
                        //
                        //if (this.ParentNodeItem != null)
                        //{
                        //    //
                        //    // Delegate to parent node to execute selection logic.
                        //    //
                        //    this.ParentNodeItem.LeftMouseUpSelectionLogic();
                        //}
                    }

                    e.Handled = true;
                }
            }
        }

        #endregion
    }

    public class ConnectionElement : IRenderable, IConnection
    {
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();

        #endregion

        #region Properties (Inherited from IRenderable)

        public Guid ZPrev { get; }

        public Guid ZNext { get; }

        public Guid Parent { get; }

        public Guid[] Children { get; }

        public Type ViewType => typeof(ConnectionElementView);

        public BoundingBox BoundingBox { get => boundingBox; internal set => boundingBox = value; }

        public double X { get => BoundingBox.Location.X; set => BoundingBox.Location.X = value; }
        public double Y { get => BoundingBox.Location.Y; set => BoundingBox.Location.Y = value; }

        public double Width { get => BoundingBox.Size.Width; set => BoundingBox.Size.Width = value; }
        public double Height { get => BoundingBox.Size.Height; set => BoundingBox.Size.Height = value; }

        public Guid ID { get => _id; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }

        #endregion

        #region Properties (Inherited from IConnection)

        public INode Origin { get; }
        public INode Destination { get; }
        public ConnectionType ConnectionType { get; }

        #endregion

        #region Dependency Property/Event Definitions

        public static readonly DependencyProperty OriginPointProperty =
            DependencyProperty.Register("OriginPoint", typeof(Point), typeof(ConnectionElement));

        public static readonly DependencyProperty DestinationPointProperty =
            DependencyProperty.Register("DestinationPoint", typeof(Point), typeof(ConnectionElement));

        internal static readonly DependencyProperty ParentNodeItemProperty =
            DependencyProperty.Register("ParentNodeItem", typeof(INode), typeof(ConnectionElement));

        public static double _nodeThresholdRadius = 5.0;

        public ConnectionElement(System.Drawing.Point point1, System.Drawing.Point point2, bool v)
        {
            //TODO: FindNext(ElementType t) {}
            //TODO: Check if point is in node hotspot
            foreach (INode node in DataViewModel.Instance.Elements)
            {
                if (node != null)
                {
                    if (((Math.Pow(point1.X, 2.0) + Math.Pow(node.Hotspot.Y, 2.0)) +
                        ((2 * node.Hotspot.X)) + ((2 * node.Hotspot.Y)))
                        < (Math.Pow(_nodeThresholdRadius, 2.0)))
                    {

                        this.Origin = node;

                    }
                }
            }
        }

        public ConnectionElement(INode start)
        {
            this.Origin = start;
            this.Destination = new MousePositionNode();
        }
        public bool TopToBottom
        {
            get
            {
                if (this.OriginPoint != default && this.DestinationPoint != default)
                {
                    double height = (this.DestinationPoint.X - this.OriginPoint.X);
                    double width = (this.DestinationPoint.Y - this.OriginPoint.Y);
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
        public bool LeftToRight
        {
            get
            {
                if (this.OriginPoint != default && this.DestinationPoint != default)
                {
                    double width = (this.DestinationPoint.Y - this.OriginPoint.Y);
                    return (width > 0);
                }
                return default;
            }
        }
        public BoundingBox InnerBoundingBox
        {
            get
            {
                if (!innerBoundingBox.IsValid()) return BoundingBox;
                else return innerBoundingBox;
            }
            internal set => innerBoundingBox = value;
        }
        private BoundingBox innerBoundingBox = BoundingBox.Unset;

        #endregion

        #region Properties

        public Point OriginPoint
        {
            get
            {
                return new Point(Origin.Hotspot.X, Origin.Hotspot.Y);
            }
        }

        public Point DestinationPoint
        {
            get
            {
                return new Point(Destination.Hotspot.X, Destination.Hotspot.Y);
            }
        }

        public INode ParentNodeItem
        {
            get
            {
                if (Origin != MousePositionNode.Node && Origin != null)
                {
                    return Origin;
                }
                else if (Destination != MousePositionNode.Node && Destination != null)
                {
                    return Destination;
                }
                else throw new NullReferenceException("Origin and Destination Invalid");
            }
        }

        public System.Drawing.Point End { get; internal set; }

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

        #endregion
    }

    public class MousePositionNode : INode
    {
        public static MousePositionNode Node = new MousePositionNode();

        public IElement Parent => null;

        public NodeType NodeType => NodeType.Default;

        public CanvasPoint Hotspot
        {
            get
            {
                Point pos = Mouse.GetPosition(DataViewModel.WPFControl.LBcontent);
                return new CanvasPoint(pos.X, pos.Y);
            }
        }

        public Guid ID => default;

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get => ElementType.Node; set => ElementType = ElementType.Node; }

        public double HotspotThresholdRadius { get; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
