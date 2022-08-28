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
using Verse3.VanillaElements;
using static Core.Geometry2D;

namespace Verse3
{
    public abstract class BaseComp : IRenderable, IComputable
    {
        #region Data Members

        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal BaseCompView elView;
        protected ChildElementManager _cEManager;

        #endregion

        #region Properties

        public ChildElementManager ChildElementManager => _cEManager;
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
                    elView = value as BaseCompView;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public Type ViewType => typeof(BaseCompView);
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        public double X { get => boundingBox.Location.X; }

        public double Y { get => boundingBox.Location.Y; }

        public double Width
        {
            get => boundingBox.Size.Width;
            set
            {
                this.BoundingBox.Size.Width = value;
                OnPropertyChanged("Width");
            }
        }

        public double Height
        {
            get => boundingBox.Size.Height;
            set
            {
                this.BoundingBox.Size.Height = value;
                OnPropertyChanged("Height");
            }
        }

        public ElementState State { get; set; }

        //public IRenderView ElementView { get; internal set; }

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get => ElementType.BaseComp; set => ElementType = ElementType.BaseComp; }
        bool IRenderable.Visible { get; set; }

        private Brush background;
        public Brush Background { get => background; set => SetProperty(ref background, value); }

        private Brush backgroundTint;
        public Brush BackgroundTint { get => backgroundTint; set => SetProperty(ref backgroundTint, value); }

        //internal CompOrientation _orientation = CompOrientation.Vertical;
        //public string Orientation
        //{
        //    get => _orientation.ToString();
        //    set
        //    {
        //        if (Enum.TryParse(value, out CompOrientation orientation))
        //        {
        //            _orientation = orientation;
        //        }
        //    }
        //}


        public IRenderable Parent => RenderPipelineInfo.Parent;
        public ElementsLinkedList<IRenderable> Children => RenderPipelineInfo.Children;

        private ComputationPipelineInfo computationPipelineInfo;

        public ComputationPipelineInfo ComputationPipelineInfo => computationPipelineInfo;

        //private ElementsLinkedList<INode> _nodes = new ElementsLinkedList<INode>();
        //public ElementsLinkedList<INode> Nodes => _nodes;

        public ComputableElementState ComputableElementState { get; set; } = ComputableElementState.Unset;
        IRenderView IRenderable.RenderView
        {
            get => this.RenderView as IRenderView;
            set
            {
                if (value is BaseCompView)
                {
                    this.RenderView = value as BaseCompView;
                }
            }
        }


        #endregion

        #region Constructor and Compute

        //#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public BaseComp(int x, int y)
        {
            _cEManager = new ChildElementManager(this);
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));


            renderPipelineInfo = new RenderPipelineInfo(this);
            computationPipelineInfo = new ComputationPipelineInfo(this);

            //if (this.RenderView == null) return;
            //_orientation = orientation;

            this.boundingBox = new BoundingBox(x, y, 0, 0);

            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            this.Background = new SolidColorBrush(Colors.Gray);
        }

        public abstract void Initialize();

        protected TextElement titleTextBlock = new TextElement();
        /// <summary>
        /// Override only if you know what you're doing
        /// </summary>
        public virtual void RenderComp()
        {
            if (this.Children.Count > 0)
            {
                //TODO: At every render
                //textBlock.DisplayedText = this.ElementText;
                this.ChildElementManager.AdjustBounds();
                return;
            }

            //textBlock.DisplayedText = this.ElementText;
            titleTextBlock.DisplayedText = this.GetCompInfo().Name;
            titleTextBlock.TextAlignment = TextAlignment.Left;
            titleTextBlock.TextRotation = 90;
            titleTextBlock.ElementType = ElementType.DisplayUIElement;
            //double h = titleTextBlock.Height;
            //titleTextBlock.Height = titleTextBlock.Width;
            //titleTextBlock.Width = h;
            this.ChildElementManager.AddElement(titleTextBlock);

            //this.ChildElementManager.AddElement()


            Initialize();

            //if (this.RenderView is BaseCompView)
            //{
            //    BaseCompView view = this.RenderView as BaseCompView;

            //    if (this.Width != (view.InputsList.ActualWidth + view.OutputsList.ActualWidth + view.CenterBar.Width))
            //    {
            //        this.Width = (view.InputsList.ActualWidth + view.OutputsList.ActualWidth + view.CenterBar.Width);
            //    }
            //    if (this.Height != view.MainStackPanel.ActualHeight) this.Height = view.MainStackPanel.ActualHeight;

            //    //TODO: Add Center Bar Elements (Title, Icon, etc)
            //    //this.ChildElementManager.AddElement()
            //}
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

        public void Dispose()
        {
            if (this.RenderPipelineInfo.Children != null && this.RenderPipelineInfo.Children.Count > 0)
            {
                foreach (var child in this.RenderPipelineInfo.Children)
                {
                    if (child != null) child.Dispose();
                }
            }
            DataViewModel.Instance.Elements.Remove(this);
            GC.SuppressFinalize(this);
        }
        ~BaseComp() => Dispose();
    }

    public class ChildElementManager
    {
        private BaseComp _owner;

        public ChildElementManager(BaseComp owner)
        {
            this._owner = owner;
        }

        public void AdjustBounds()
        {
            if (_owner.RenderView is BaseCompView)
            {
                BaseCompView view = _owner.RenderView as BaseCompView;

                if (_owner.Width < 50 || _owner.Height < 50 || _owner.Width == double.NaN || _owner.Height == double.NaN)
                {
                    _owner.Width = 500;
                    _owner.Height = 500;
                    view.UpdateLayout();
                    //view.Render();
                }
                //if (view.MainStackPanel.ActualWidth < 50 || view.MainStackPanel.ActualHeight < 50) return;
                //if (view.MainStackPanel.ActualWidth > 500 || view.MainStackPanel.ActualHeight > 500) return;
                if (_owner.Width != view.MainStackPanel.ActualWidth) _owner.Width = view.MainStackPanel.ActualWidth;
                if (_owner.Height != view.MainStackPanel.ActualHeight) _owner.Height = view.MainStackPanel.ActualHeight;
                //_owner.OnPropertyChanged("BoundingBox");
                //RenderPipeline.RenderRenderable(_owner);
                if (view.CenterBar.ActualWidth < view.ActualWidth - (view.InputsList.ActualWidth + view.OutputsList.ActualWidth))
                {
                    if (view.ActualWidth > (view.InputsList.ActualWidth + view.OutputsList.ActualWidth))
                    {
                        double targetWidth = _owner.Width;
                        if ((view.InputsList.ActualWidth + view.OutputsList.ActualWidth + view.CenterBar.ActualWidth) >
                            view.BottomUI.ActualWidth)
                            targetWidth = (view.InputsList.ActualWidth + view.OutputsList.ActualWidth + view.CenterBar.ActualWidth);
                        else targetWidth = view.BottomUI.ActualWidth;
                        if (_owner.Width != targetWidth)
                        {
                            _owner.Width = targetWidth;
                        }
                        view.CenterBar.Width = _owner.Width - (view.InputsList.ActualWidth + view.OutputsList.ActualWidth);
                    }
                }
                view.UpdateLayout();
            }
        }

        public void AddElement(IRenderable element)
        {
            DataTemplateManager.RegisterDataTemplate(element);
            this._owner.RenderPipelineInfo.AddChild(element);
            switch (element.ElementType)
            {
                case ElementType.UIElement:
                    {
                        _bottomUI.Add(element);
                        break;
                    }
                case ElementType.DisplayUIElement:
                    {
                        _center.Add(element);
                        break;
                    }
                case ElementType.Node:
                    {
                        if (element is INode)
                        {
                            INode node = element as INode;
                            if (node.NodeType == NodeType.Input)
                            {
                                _input.Add(element);
                            }
                            else if (node.NodeType == NodeType.Output)
                            {
                                _output.Add(element);
                            }
                        }
                        break;
                    }
                default:
                    {

                        break;
                    }
            }
        }
        public void RemoveElement(IRenderable element)
        {
            this._owner.RenderPipelineInfo.Children.Remove(element);
        }
        public void AddDataOutputNode<T>(IDataNode<T> node)
        {
            if (node is IRenderable) AddElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.AddDataOutputNode<T>(node);
        }

        public void AddDataInputNode<T>(IDataNode<T> node)
        {
            if (node is IRenderable) AddElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.AddDataInputNode<T>(node);
        }

        public void AddEventOutputNode(IEventNode node)
        {
            if (node is IRenderable) AddElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.AddEventOutputNode(node);
        }

        public void AddEventInputNode(IEventNode node)
        {
            if (node is IRenderable) AddElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.AddEventInputNode(node);
        }

        public void RemoveNode(INode node)
        {
            if (node is IRenderable) RemoveElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.RemoveNode(node);
        }

        public T GetData<T>(int v)
        {
            return this._owner.ComputationPipelineInfo.IOManager.GetData<T>(v);
        }

        public void SetData<T>(T v1, int v2)
        {
            this._owner.ComputationPipelineInfo.IOManager.SetData<T>(v1, v2);
        }

        public T GetData<T>(int v, T defaultValue)
        {
            T a = this._owner.ComputationPipelineInfo.IOManager.GetData<T>(v);
            if (a is null) a = defaultValue;
            return a;
        }

        public ElementsLinkedList<IRenderable> FilterChildElementsByType(ElementType elementType)
        {
            ElementsLinkedList<IRenderable> renderables = new ElementsLinkedList<IRenderable>();
            foreach (IRenderable renderable in _owner.Children)
            {
                if (renderable.ElementType == elementType)
                {
                    //TODO: Log to console
                    //if (renderable is INode) continue;
                    /*else */renderables.Add(renderable);
                }
            }
            return renderables;
        }

        private ElementsLinkedList<IRenderable> _input = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> InputSide
        {
            get
            {
                //foreach (IRenderable renderable in _owner.Children)
                //{
                //    if (renderable.ElementType == ElementType.Node)
                //    {
                //        if (renderable is INode)
                //        {
                //            INode node = renderable as INode;
                //            if (node.NodeType == NodeType.Input)
                //            {
                //                _input.Add(renderable);
                //            }
                //        }
                //    }
                //}
                return _input;
            }
        }

        private ElementsLinkedList<IRenderable> _output = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> OutputSide
        {
            get
            {
                //foreach (IRenderable renderable in _owner.Children)
                //{
                //    if (renderable.ElementType == ElementType.Node)
                //    {
                //        if (renderable is INode)
                //        {
                //            INode node = renderable as INode;
                //            if (node.NodeType == NodeType.Output)
                //            {
                //                _output.Add(renderable);
                //            }
                //        }
                //    }
                //}
                return _output;
            }
        }

        private ElementsLinkedList<IRenderable> _bottomUI = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> BottomUIItems
        {
            get
            {
                //foreach (IRenderable renderable in _owner.Children)
                //{
                //    if (renderable.ElementType == ElementType.UIElement)
                //    {
                //        //TODO: Log to console
                //        if (renderable is INode) continue;
                //        else _bottomUI.Add(renderable);
                //    }
                //}
                return _bottomUI;
            }
        }

        private ElementsLinkedList<IRenderable> _center = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> CenterBarItems
        {
            get
            {
                //foreach (IRenderable renderable in _owner.Children)
                //{
                //    if (renderable.ElementType == ElementType.DisplayUIElement)
                //    {
                //        //TODO: Log to console
                //        if (renderable is INode) continue;
                //        else _center.Add(renderable);
                //    }
                //}
                return _center;
            }
        }
    }

    //string? txt = this.ElementText;
    //textBlock = new TextElement();
    //textBlock.DisplayedText = txt;
    //    textBlock.TextAlignment = TextAlignment.Left;
    //    DataTemplateManager.RegisterDataTemplate(textBlock);
    //    this.RenderPipelineInfo.AddChild(textBlock);

    //    sliderBlock = new SliderElement();
    //sliderBlock.Minimum = 0;
    //    sliderBlock.Maximum = 100;
    //    sliderBlock.Value = 50;
    //    sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
    //    DataTemplateManager.RegisterDataTemplate(sliderBlock);
    //    this.RenderPipelineInfo.AddChild(sliderBlock);

    //    var buttonBlock = new ButtonElement();
    //buttonBlock.DisplayedText = "Click me";
    //    buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
    //    DataTemplateManager.RegisterDataTemplate(buttonBlock);
    //    this.RenderPipelineInfo.AddChild(buttonBlock);

    //    var textBoxBlock = new TextBoxElement();
    //textBoxBlock.InputText = "Enter text";
    //    DataTemplateManager.RegisterDataTemplate(textBoxBlock);
    //    this.RenderPipelineInfo.AddChild(textBoxBlock);

    //nodeBlock2 = new NumberDataNode(this, NodeType.Output);
    //DataTemplateManager.RegisterDataTemplate(nodeBlock2);
    //    this.RenderPipelineInfo.AddChild(nodeBlock2);
    //    this.ComputationPipelineInfo.IOManager.AddDataOutputNode<double>(nodeBlock2 as IDataNode<double>);

    //    string? txt = this.ElementText;
    //textBlock = new TextElement();
    //textBlock.DisplayedText = txt;
    //    textBlock.TextAlignment = TextAlignment.Left;
    //    DataTemplateManager.RegisterDataTemplate(textBlock);
    //    this.RenderPipelineInfo.AddChild(textBlock);

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

    
    public readonly struct CompInfo
    {
        public ConstructorInfo ConstructorInfo { get; init; }
        public string Name { get; init; }
        public string Group { get; init; }
        public string Tab { get; init; }
        public string Description { get; init; }
        public string Author { get; init; }
        public string Version { get; init; }
        public string License { get; init; }
        public string Website { get; init; }
        public string Repository { get; init; }
        public string Icon { get; init; }
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
        public ElementType ElementType { get => ElementType.Node; set => ElementType = ElementType.Node; }
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
                //TODO: Turn into a converter for BaseComp
                double v = 20.0;
                if ((this as INode).Parent is IComputable)
                {
                    IComputable c = (this as INode).Parent as IComputable;
                    if (this.NodeType == NodeType.Output)
                    {
                        if (c.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.DataOutputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.DataOutputNodes.IndexOf(this);
                            v = v + (i * this.BoundingBox.Size.Height);
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
                            v = v + (i * this.BoundingBox.Size.Height);
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

        public abstract void ToggleActive();
        
        public void Dispose()
        {
            if (this.RenderPipelineInfo.Children != null && this.RenderPipelineInfo.Children.Count > 0)
            {
                foreach (var child in this.RenderPipelineInfo.Children)
                {
                    if (child != null) child.Dispose();
                }
                foreach (var connection in this.Connections)
                {
                    if (connection != null) connection.Dispose();
                }
            }
            DataViewModel.Instance.Elements.Remove(this);
            GC.SuppressFinalize(this);
        }
        ~DataNode() => Dispose();
    }

    public abstract class EventNode : IRenderable, IEventNode
    {
        #region Data Members

        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();
        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal IRenderView elView;
        //public ComputableElementState ComputableElementState { get; set; }

        #endregion

        #region Properties

        //public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

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

        internal CanvasPoint _hotspot = new CanvasPoint(0, 0);
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
                        if (c.ComputationPipelineInfo.IOManager.EventOutputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.EventOutputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.EventOutputNodes.IndexOf(this);
                            v = i * this.BoundingBox.Size.Height;
                        }
                        _hotspot = this.RenderPipelineInfo.Parent.BoundingBox.Location +
                        new CanvasPoint(this.RenderPipelineInfo.Parent.BoundingBox.Size.Width,
                            ((this.BoundingBox.Size.Height / 2) + v));
                    }
                    else
                    {
                        if (c.ComputationPipelineInfo.IOManager.EventInputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.EventInputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.EventInputNodes.IndexOf(this);
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

        //public Type DataValueType => typeof(D);

        //private DataStructure<D> _dataGoo = new DataStructure<D>();
        //public DataStructure<D> DataGoo { get => _dataGoo; set => _dataGoo = value; }

        private ComputationPipelineInfo _computationPipelineInfo;
        public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;

        public ElementsLinkedList<INode> Nodes => new ElementsLinkedList<INode>() { this };
        public ComputableElementState ComputableElementState { get; set; }
        //DataStructure IDataGooContainer.DataGoo
        //{
        //    get => this.DataGoo;
        //    set
        //    {
        //        if (value is DataStructure<D>)
        //            this.DataGoo = value as DataStructure<D>;
        //        else
        //            throw new InvalidCastException();
        //    }
        //}

        #endregion

        public EventNode(IRenderable parent, NodeType type = Core.NodeType.Unset)
        {
            this.renderPipelineInfo = new RenderPipelineInfo(this);
            _computationPipelineInfo = new ComputationPipelineInfo(this);
            this.RenderPipelineInfo.Parent = parent as IRenderable;
            //this.DataGoo.DataChanged += OnDataChanged;
            this._nodeType = type;
        }


        #region Properties


        //private NodeType _nodeType = Core.NodeType.Unset;
        //public NodeType NodeType { get => _nodeType; }
        //private ComputationPipelineInfo _computationPipelineInfo;
        //public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;
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
        public ElementType ElementType { get => ElementType.Node; set => ElementType = ElementType.Node; }
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

        public EventArgData EventArgData { get; set; }
        public IElement Parent { get => this.RenderPipelineInfo.Parent; set => this.RenderPipelineInfo.SetParent(value); }

        //public ElementsLinkedList<IConnection> Connections => throw new NotImplementedException();

        //public CanvasPoint Hotspot => throw new NotImplementedException();

        //public double HotspotThresholdRadius => throw new NotImplementedException();

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public event IEventNode.NodeEventHandler NodeEvent;

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

        public void TriggerEvent()
        {
            NodeEvent.Invoke(this, new EventArgData());
            if (this.Parent is IComputable)
            {
                ComputationPipeline.ComputeComputable(this.Parent as IComputable);
            }
        }

        public bool EventOccured(EventArgData e)
        {
            if (this.Parent is IComputable)
            {
                IComputable computable = this.Parent as IComputable;
                //TODO: Call a delegate method that triggers a call-back once complete
                foreach (IComputable compDS in this.ElementDS)
                {
                    if (compDS != null)
                    {
                        if (compDS.ComputationPipelineInfo.IOManager.EventInputNodes != null &&
                            compDS.ComputationPipelineInfo.IOManager.EventInputNodes.Count > 0)
                        {
                            foreach (IEventNode en in compDS.ComputationPipelineInfo.IOManager.EventInputNodes)
                            {
                                if (en.Connections != null && en.Connections.Count > 0)
                                {
                                    foreach (IConnection connection in en.Connections)
                                    {
                                        if (connection.Origin == this)
                                        {
                                            if (connection.Destination is EventNode)
                                            {
                                                EventNode d = connection.Destination as EventNode;
                                                d.TriggerEvent();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //if (computable.ComputationPipelineInfo.IOManager.EventInputNodes.Contains(this))
                //{
                //    int i = computable.ComputationPipelineInfo.IOManager.EventInputNodes.IndexOf(this);
                //    computable.ComputationPipelineInfo.IOManager.EventDelegates[i].Invoke(this, new EventArgData());
                //    return true;
                //}
                //ComputationPipeline.ComputeComputable(computable);
            }
            return false;
        }
        public abstract void ToggleActive();
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
                    if (conn.Destination == this && conn.Origin is IEventNode)
                    {
                        IEventNode no = conn.Origin as IEventNode;
                        //if (!this.DataGoo.IsValid)
                        //{
                        //    this.DataGoo.Clear();
                        //    this.DataGoo.Data = no.DataGoo.Data;
                        //}
                        //else if (!this.DataGoo.Data.Equals(no.DataGoo.Data))
                        //{
                        //    this.DataGoo.Data = no.DataGoo.Data;
                        //}
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
                    if (conn.Origin == this && conn.Destination is IEventNode)
                    {
                        IEventNode no = conn.Origin as IEventNode;
                        //if (!nd.DataGoo.IsValid)
                        //{
                        //    nd.DataGoo.Clear();
                        //    nd.DataGoo.Data = this.DataGoo.Data;
                        //}
                        //else if (!(nd.DataGoo.Data.Equals(this.DataGoo.Data)))
                        //{
                        //    nd.DataGoo.Data = this.DataGoo.Data;
                        //}
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
        
        public void Dispose()
        {
            if (this.RenderPipelineInfo.Children != null && this.RenderPipelineInfo.Children.Count > 0)
            {
                foreach (var child in this.RenderPipelineInfo.Children)
                {
                    if (child != null) child.Dispose();
                }
                foreach (var connection in this.Connections)
                {
                    if (connection != null) connection.Dispose();
                }
            }
            DataViewModel.Instance.Elements.Remove(this);
            GC.SuppressFinalize(this);
        }
        ~EventNode() => Dispose();
    }

    //public enum CompOrientation
    //{
    //    Horizontal,
    //    Vertical
    //}
}
