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
    public partial class NodeElementView : UserControl, IRenderView
    {
        private NodeElement _element;
        
        public IRenderable Element
        {
            get { return _element; }
            private set
            {
                _element = value as NodeElement;
                _element.RenderView = this;
                //Update();
            }
        }
        public Guid? ElementGuid
        {
            get { return _element?.ID; }
        }

        public NodeElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                if (_element.RenderView != this) _element.RenderView = this;
                if (this._element.Connections != null)
                {
                    foreach (BezierElement bezier in this._element.Connections)
                    {
                        if (bezier != null)
                        {
                            if (bezier.Origin == this._element)
                            {

                            }
                            else if (bezier.Destination == this._element)
                            {

                            }
                            bezier.RedrawBezier(bezier.Origin, bezier.Destination);
                        }
                    }
                }
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

            //NodeElementView rectangle = (NodeElementView)sender;
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

            //NodeElementView rectangle = (NodeElementView)sender;
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

            //NodeElementView rectangle = (NodeElementView)sender;
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
                this._element = this.DataContext as NodeElement;
                Render();
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            if (this.DataContext != null)
            {
                //this._element = this.DataContext as NodeElement;
                //Render();
            }
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Set as active Node
            DataViewModel.ActiveNode = this._element as INode;
            BezierElement b = (BezierElement)DataViewModel.ActiveConnection;
            if (DataViewModel.ActiveConnection == default)
            {
                DataViewModel.ActiveConnection = DataViewModel.CreateConnection(DataViewModel.ActiveNode);
                b = (BezierElement)DataViewModel.ActiveConnection;
                if (b != null)
                {
                    this._element.AddChild(b);
                    this._element.Connections.Add(b);
                }
            }
            else
            {
                if (b != null)
                {
                    b.SetDestination(DataViewModel.ActiveNode);
                    this._element.AddChild(b);
                    this._element.Connections.Add(b);
                    DataViewModel.ActiveConnection = default;
                    DataViewModel.ActiveNode = default;
                }
            }
        }
    }

    public class NodeElement : IRenderable, INode
    {        
        #region Data Members

        private BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        private static Type view = typeof(NodeElementView);
        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();
        internal IRenderable parentElement = default;
        internal NodeElementView elView;
        public IRenderView RenderView
        {
            get
            {
                return elView;
            }
            set
            {
                if (value is NodeElementView)
                {
                    elView = (NodeElementView)value;
                }
            }
        }

        #endregion

        #region Properties

        public Type ViewType { get { return view; } }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox
        {
            get => boundingBox;
            internal set => SetProperty(ref boundingBox, value);
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

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        bool IRenderable.Visible { get; set; }

        #endregion

        #region Constructors

        public NodeElement(IElement parent)
        {
            parentElement = parent as IRenderable;
            double x = DataViewModel.ContentCanvasMarginOffset + parentElement.X;
            double y = DataViewModel.ContentCanvasMarginOffset + parentElement.Y;
            this.BoundingBox = new BoundingBox(x, y, parentElement.Width, 50);
            this.SetParent(parentElement);
            this.DisplayedText = "Node";
            this.PropertyChanged += NodeElement_PropertyChanged;
        }

        private void NodeElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (IRenderable renderable in this.Connections)
            {
                renderable.Render();
            }
        }

        //public NodeElement(int x, int y, int width, int height)
        //{
        //    this.boundingBox = new BoundingBox(x, y, width, height);

        //    this.DisplayedText = "Button";
        //}

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

        private object displayedText;

        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        IElement INode.Parent { get; }

        public ElementsLinkedList<IConnection> Connections => connections;

        public NodeType NodeType { get => NodeType.Output; }

        public CanvasPoint Hotspot
        {
            get
            {
                CanvasPoint center = this.BoundingBox.Location;
                //center.X += (this.BoundingBox.Size.Width / 2);
                //center.Y -= (this.BoundingBox.Size.Height / 2);
                return center;
            }
        }

        public double HotspotThresholdRadius { get; }
        public object ViewKey { get; set; }
    }

    public class MousePositionNode : INode
    {
        public static readonly MousePositionNode Instance = new MousePositionNode();
        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();

        private MousePositionNode()
        {

        }
        
        public IElement Parent { get; }

        public ElementsLinkedList<IConnection> Connections => connections;

        public NodeType NodeType => NodeType.Unset;

        public CanvasPoint Hotspot
        {
            get
            {
                System.Drawing.Point p = DataViewModel.WPFControl.GetMouseRelPosition();
                if (this.Connections != null)
                {
                    if (this.Connections.Count > 0)
                    {
                        foreach (BezierElement bezier in this.Connections)
                        {
                            bezier.RedrawBezier(bezier.Origin, bezier.Destination);
                        }
                    }
                }

                return new CanvasPoint(p.X, p.Y);
            }
        }

        public double HotspotThresholdRadius { get; }

        public Guid ID { get; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get => ElementType.Node; set => ElementType = ElementType.Node; }

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
    }
}