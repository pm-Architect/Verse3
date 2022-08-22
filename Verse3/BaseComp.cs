using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Core.Geometry2D;

namespace Verse3
{
    public abstract class BaseComp : IRenderable, IComputable
    {
        #region Data Members

        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal IRenderView elView;

        #endregion

        #region Properties

        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        public IRenderView RenderView
        {
            get
            {
                return elView;
            }
            set
            {
                if (ViewType.IsAssignableFrom(value.GetType()))
                {
                    elView = value;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public abstract Type ViewType { get; }
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get => boundingBox.Size.Width;
            set => boundingBox.Size.Width = value;
        }

        public double Height
        {
            get => boundingBox.Size.Height;
            set => boundingBox.Size.Height = value;
        }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; internal set; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        bool IRenderable.Visible { get; set; }
        
        private Brush background;
        public Brush Background { get => background; set => SetProperty(ref background, value); }

        private Brush backgroundTint;
        public Brush BackgroundTint { get => backgroundTint; set => SetProperty(ref backgroundTint, value); }

        public IRenderable Parent => RenderPipelineInfo.Parent;
        public ElementsLinkedList<IRenderable> Children => RenderPipelineInfo.Children;

        private ComputationPipelineInfo computationPipelineInfo;
        public ComputationPipelineInfo ComputationPipelineInfo => computationPipelineInfo;

        //private ElementsLinkedList<INode> _nodes = new ElementsLinkedList<INode>();
        //public ElementsLinkedList<INode> Nodes => _nodes;

        public ComputableElementState ComputableElementState { get; set; } = ComputableElementState.Unset;

        #endregion

        #region Constructor and Compute

        //#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public BaseComp()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
            renderPipelineInfo = new RenderPipelineInfo(this);
            computationPipelineInfo = new ComputationPipelineInfo(this);
        }

        public abstract void Initialize();
        public virtual void RenderComp()
        {
            Initialize();
        }

        public abstract void Compute();

        public virtual void CollectData()
        {
            if (this.ComputationPipelineInfo.IOManager.DataInputNodes != null && this.ComputationPipelineInfo.IOManager.DataInputNodes.Count > 1)
            {
                this.ComputationPipelineInfo.CollectData();
            }
        }
        public virtual void DeliverData()
        {
            if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0/* && computable.Nodes[0] is NodeElement*/)
            {
                this.ComputationPipelineInfo.DeliverData();
            }
        }

        //#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        #endregion

        public abstract CompInfo GetCompInfo();

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
    public interface IBaseCompView<R> : IRenderView where R : BaseComp
    {
        public new R Element { get; }
        //{
        //    get
        //    {
        //        if (Element == null)
        //        {
        //            if (this.GetType().IsAssignableTo(typeof(UserControl)))
        //            {
        //                object dc = ((UserControl)this).DataContext;
        //                if (dc.GetType().IsAssignableTo(typeof(R)))
        //                {
        //                    Element = (R)dc;
        //                }
        //            }
        //        }
        //        return Element;
        //    }
        //    private set
        //    {
        //        if (value is R)
        //        {
        //            Element = (R)value;
        //        }
        //        else
        //        {
        //            throw new InvalidCastException();
        //        }
        //    }
        //}
        public Guid? ElementGuid
        {
            get { return Element?.ID; }
        }


        public new virtual void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.RenderView != this) this.Element.RenderView = this;
                this.Element.RenderComp();
            }
        }

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        public void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        public void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse wheel is moved.
        /// </summary>
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        //public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.GetType().IsAssignableTo(typeof(UserControl)))
        //    {
        //        UserControl uc = this as UserControl;
        //        if (uc.DataContext is R)
        //        {
        //            Element = (R)uc.DataContext;
        //        }
        //    }
        //    Render();
        //}

        //public void OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    if (this.GetType().IsAssignableTo(typeof(UserControl)))
        //    {
        //        UserControl uc = this as UserControl;
        //        if (uc.DataContext is R)
        //        {
        //            Element = (R)uc.DataContext;
        //        }
        //    }
        //    Render();
        //}

        #endregion
    }

    //public abstract partial class BaseCompView<R> : UserControl, IBaseCompView<R> where R : BaseComp
    //{
    //    public R Element
    //    {
    //        get
    //        {
    //            if (Element == null)
    //            {
    //                if (this.GetType().IsAssignableTo(typeof(UserControl)))
    //                {
    //                    object dc = ((UserControl)this).DataContext;
    //                    if (dc.GetType().IsAssignableTo(typeof(R)))
    //                    {
    //                        Element = (R)dc;
    //                    }
    //                }
    //            }
    //            return Element;
    //        }
    //        set
    //        {
    //            if (value is R)
    //            {
    //                Element = (R)value;
    //            }
    //            else
    //            {
    //                throw new InvalidCastException();
    //            }
    //        }
    //    }

    //    IRenderable IRenderView.Element
    //    {
    //        get => Element;
    //        //set
    //        //{
    //        //    if (value is R) Element = value as R;
    //        //    else throw new InvalidCastException();
    //        //}
    //    }

    //    #region Constructor and Render


    //    //TODO: Log to Console if this.Element is still null
    //    public BaseCompView()
    //    {
    //        if (this.DataContext is R) this.Element = (R)this.DataContext;
    //        //InitializeComponent();
    //        Render();
    //    }

    //    public void Render()
    //    {
    //        if (this.Element != null)
    //        {
    //            if (this.Element.RenderView != this) this.Element.RenderView = this;
    //            this.Element.RenderComp();

    //            //InputsList.ItemsSource = this.Element.Children;
    //        }
    //    }

    //    #endregion


    //    #region MouseEvents

    //    /// <summary>
    //    /// Event raised when a mouse button is clicked down over a Rectangle.
    //    /// </summary>
    //    void OnMouseDown(object sender, MouseButtonEventArgs e)
    //    {
    //        //MouseButtonEventArgs
    //        DataViewModel.WPFControl.ContentElements.Focus();
    //        Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

    //        BaseCompView<R> rectangle = (BaseCompView<R>)sender;
    //        IRenderable myRectangle = (IRenderable)rectangle.DataContext;

    //        //myRectangle.IsSelected = true;

    //        //mouseButtonDown = e.ChangedButton;

    //        if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
    //        {
    //            //
    //            // When the shift key is held down special zooming logic is executed in content_MouseDown,
    //            // so don't handle mouse input here.
    //            //
    //            return;
    //        }

    //        if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
    //        {
    //            //
    //            // We are in some other mouse handling mode, don't do anything.
    //            return;
    //        }

    //        DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingElements;
    //        DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);

    //        rectangle.CaptureMouse();

    //        e.Handled = true;
    //    }

    //    /// <summary>
    //    /// Event raised when a mouse button is released over a Rectangle.
    //    /// </summary>
    //    void OnMouseUp(object sender, MouseButtonEventArgs e)
    //    {
    //        //MouseButtonEventArgs
    //        if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
    //        {
    //            //
    //            // We are not in rectangle dragging mode.
    //            //
    //            return;
    //        }

    //        DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

    //        BaseCompView<R> rectangle = (BaseCompView<R>)sender;
    //        rectangle.ReleaseMouseCapture();

    //        e.Handled = true;
    //    }

    //    /// <summary>
    //    /// Event raised when the mouse cursor is moved when over a Rectangle.
    //    /// </summary>
    //    void OnMouseMove(object sender, MouseEventArgs e)
    //    {
    //        //MouseEventArgs
    //        if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
    //        {
    //            //
    //            // We are not in rectangle dragging mode, so don't do anything.
    //            //
    //            return;
    //        }

    //        Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
    //        Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

    //        //
    //        // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
    //        //

    //        DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

    //        RenderPipeline.RenderRenderable(this.Element, rectangleDragVector.X, rectangleDragVector.Y);

    //        DataViewModel.WPFControl.ExpandContent();

    //        e.Handled = true;
    //    }

    //    void OnMouseWheel(object sender, MouseWheelEventArgs e)
    //    {
    //        //MouseWheelEventArgs
    //    }

    //    #endregion

    //    #region UserControlEvents

    //    void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        //DependencyPropertyChangedEventArgs
    //    }

    //    void OnLoaded(object sender, RoutedEventArgs e)
    //    {
    //        //RoutedEventArgs
    //        Render();
    //    }

    //    #endregion
    //}

    public struct CompInfo
    {
        public ConstructorInfo ConstructorInfo { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Tab { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string License { get; set; }
        public string Website { get; set; }
        public string Repository { get; set; }
        public string Icon { get; set; }
        //public Type[] ConstructorParamTypes { get; set; }
        //public string[] ConstructorParamNames { get; set; }
        //public object[] ConstructorDefaults { get; set; }
    }

    public abstract class DataNode<D> : IRenderable, IDataNode<D>
    {
        #region Data Members

        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal IRenderView elView;

        #endregion

        #region Properties

        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        public IRenderView RenderView
        {
            get
            {
                return elView;
            }
            set
            {
                if (ViewType.IsAssignableFrom(value.GetType()))
                {
                    elView = value;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public abstract Type ViewType { get; }
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get => boundingBox.Size.Width;
            set => boundingBox.Size.Width = value;
        }

        public double Height
        {
            get => boundingBox.Size.Height;
            set => boundingBox.Size.Height = value;
        }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; internal set; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get; set; }
        bool IRenderable.Visible { get; set; }


        public IEnumerable<IElement> ElementDS
        {
            get
            {
                List<IElement> elements = new List<IElement>();
                if (this.NodeType == NodeType.Input) return elements;
                if (this.Connections != null && this.Connections.Count > 0)
                {
                    foreach (IConnection connection in this.Connections)
                    {
                        if (connection.Origin == this)
                        {
                            elements.Add(connection.Destination.Parent);
                        }
                    }
                }
                return elements;
            }
        }
        public IEnumerable<IElement> ElementUS
        {
            get
            {
                List<IElement> elements = new List<IElement>();
                if (this.NodeType == NodeType.Output) return elements;
                if (this.Connections != null && this.Connections.Count > 0)
                {
                    foreach (IConnection connection in this.Connections)
                    {
                        if (connection.Destination == this)
                        {
                            elements.Add(connection.Origin.Parent);
                        }
                    }
                }
                return elements;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public event IDataNode<D>.NodeDataChangedEventHandler NodeDataChanged;

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

        #region Data Members

        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();
        //internal IRenderable parentElement = default;
        private object displayedText;

        #endregion

        #region Properties

        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        IElement INode.Parent
        {
            get => this.RenderPipelineInfo.Parent;
            set
            {
                if (value is IRenderable)
                    this.RenderPipelineInfo.Parent = value as IRenderable;
                //else
                //    this.RenderPipelineInfo.Parent = null;
            }
        }

        public ElementsLinkedList<IConnection> Connections => connections;

        private NodeType _nodeType = NodeType.Unset;
        public NodeType NodeType { get => _nodeType; }

        internal CanvasPoint _hotspot = new CanvasPoint(0,0);
        public CanvasPoint Hotspot
        {
            get
            {
                double v = 0.0;
                if ((this as INode).Parent is IComputable)
                {
                    IComputable c = (this as INode).Parent as IComputable;
                    if (this.NodeType == NodeType.Output)
                    {
                        if (c.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.DataOutputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.DataOutputNodes.IndexOf(this);
                            v = i * this.BoundingBox.Size.Height;
                        }
                        _hotspot = this.RenderPipelineInfo.Parent.BoundingBox.Location +
                        new CanvasPoint(this.RenderPipelineInfo.Parent.BoundingBox.Size.Width,
                            ((this.BoundingBox.Size.Height / 2) + v));
                    }
                    else
                    {
                        if (c.ComputationPipelineInfo.IOManager.DataInputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.DataInputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.DataInputNodes.IndexOf(this);
                            v = i * this.BoundingBox.Size.Height;
                        }
                        _hotspot = this.RenderPipelineInfo.Parent.BoundingBox.Location +
                        new CanvasPoint(0.0, ((this.BoundingBox.Size.Height / 2) + v));
                    }
                }
                return _hotspot;
            }
        }

        public double HotspotThresholdRadius { get; }

        public Type DataValueType => typeof(D);

        private DataStructure<D> _dataGoo = new DataStructure<D>();
        public DataStructure<D> DataGoo { get => _dataGoo; set => _dataGoo = value; }

        private ComputationPipelineInfo _computationPipelineInfo;
        public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;

        public ElementsLinkedList<INode> Nodes => new ElementsLinkedList<INode>() { this };
        public ComputableElementState ComputableElementState { get; set; }
        DataStructure IDataGooContainer.DataGoo
        {
            get => this.DataGoo;
            set
            {
                if (value is DataStructure<D>)
                    this.DataGoo = value as DataStructure<D>;
                else
                    throw new InvalidCastException();
            }
        }

        #endregion


        private void OnDataChanged(DataStructure<D> sender, DataChangedEventArgs<D> e)
        {
            this.NodeDataChanged.Invoke(this, e);
        }
        public DataNode(IRenderable parent, NodeType type = NodeType.Unset) : base()
        {
            this.renderPipelineInfo = new RenderPipelineInfo(this);
            _computationPipelineInfo = new ComputationPipelineInfo(this);
            this.RenderPipelineInfo.Parent = parent as IRenderable;
            this.DataGoo.DataChanged += OnDataChanged;
            this._nodeType = type;
            //this.DataGoo.DataChanged += DataChanged;
            //double x = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.X;
            //double y = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.Y;
            //base.boundingBox = new BoundingBox(x, y, this.RenderPipelineInfo.Parent.Width, 50);
            //(this as IRenderable).RenderPipelineInfo.SetParent(this.RenderPipelineInfo.Parent);
            //this.DisplayedText = "Node";
            //this.PropertyChanged += NodeElement_PropertyChanged;
            //if (type == NodeType.Input)
            //{
            //    this.HorizontalAlignment = HorizontalAlignment.Left;
            //}
            //else if (type == NodeType.Output)
            //{
            //    this.HorizontalAlignment = HorizontalAlignment.Right;
            //}
            //else
            //{
            //    this.HorizontalAlignment = HorizontalAlignment.Center;
            //}
        }

        //event EventHandler<DataChangedEventArgs> IDataNode.DataChanged
        //{
        //    add => DataChanged += value;
        //    remove => DataChanged -= value;
        //}

        //event EventHandler<DataChangedEventArgs<D>> IDataNode<D>.DataChanged
        //{
        //    add => DataChanged += value;
        //    remove => DataChanged -= value;
        //}

        //public new event EventHandler<DataChangedEventArgs> DataChanged;


        public void Compute()
        {
            this.ComputableElementState = ComputableElementState.Computed;
        }
        public void CollectData()
        {
            if (this.Connections != null && this.Connections.Count > 0)
            {
                foreach (IConnection conn in this.Connections)
                {
                    //INCOMING CONNECTIONS
                    if (conn.Destination == this && conn.Origin is IDataNode<D>)
                    {
                        IDataNode<D> no = conn.Origin as IDataNode<D>;
                        if (!this.DataGoo.IsValid)
                        {
                            this.DataGoo.Clear();
                            this.DataGoo.Data = no.DataGoo.Data;
                        }
                        else if (!this.DataGoo.Data.Equals(no.DataGoo.Data))
                        {
                            this.DataGoo.Data = no.DataGoo.Data;
                        }
                        //this.NodeContentColor = System.Windows.Media.Brushes.White;
                        //break;
                    }
                    //OUTGOING CONNECTIONS
                    //else if (conn.Origin == n/* && conn.Destination is NodeElement*/)
                    //{
                    //NodeElement nd = (NodeElement)conn.Destination;
                    //nd.DataGoo.Data = _sliderValue + _inputValue;
                    //RenderPipeline.RenderRenderable(conn.Destination.Parent as IRenderable);
                    //}
                }
            }
        }
        public void DeliverData()
        {
            if (this.Connections != null && this.Connections.Count > 0)
            {
                foreach (IConnection conn in this.Connections)
                {
                    if (conn.Origin == this && conn.Destination is IDataNode<D>)
                    {
                        IDataNode<D> nd = conn.Destination as IDataNode<D>;
                        if (!nd.DataGoo.IsValid)
                        {
                            nd.DataGoo.Clear();
                            nd.DataGoo.Data = this.DataGoo.Data;
                        }
                        else if (!(nd.DataGoo.Data.Equals(this.DataGoo.Data)))
                        {
                            nd.DataGoo.Data = this.DataGoo.Data;
                        }
                        //this.NodeContentColor = System.Windows.Media.Brushes.White;
                        //break;
                    }
                    //OUTGOING CONNECTIONS
                    //else if (conn.Origin == n/* && conn.Destination is NodeElement*/)
                    //{
                    //NodeElement nd = (NodeElement)conn.Destination;
                    //nd.DataGoo.Data = _sliderValue + _inputValue;
                    //RenderPipeline.RenderRenderable(conn.Destination.Parent as IRenderable);
                    //}
                }
            }
        }
    }

}
