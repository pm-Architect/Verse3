using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Verse3.VanillaElements;
using Newtonsoft.Json;
using static Core.Geometry2D;

namespace Verse3
{
    [Serializable]
    [XmlRoot("BaseComp")]
    [XmlType("BaseComp")]
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

        private string _nameOverride;
        public string Name
        {
            get
            {
                CompInfo ci = GetCompInfo();
                if (_nameOverride == ci.Name || (_nameOverride == null))
                {
                    return ci.Name;
                }
                else
                {
                    return _nameOverride;
                }
            }
            set
            {
                _nameOverride = value;
            }
        }

        private string _metaDataCompInfo;
        public string MetadataCompInfo
        {
            get
            {
                return GetCompInfo().ToString();
            }
            set
            {
                _metaDataCompInfo = value;
                try
                {
                    CompInfo ci = CompInfo.FromString(this, value);
                    //TODO: Try and get a CompInfo from the string and match it to the current CompInfo

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    //throw ex;
                }
            }
        }
        [XmlIgnore]
        [JsonIgnore]
        public ChildElementManager ChildElementManager => _cEManager;
        [XmlIgnore]
        [JsonIgnore]
        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        [XmlIgnore]
        [JsonIgnore]
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
        [XmlIgnore]
        public Type ViewType => typeof(BaseCompView);
        [XmlIgnore]
        public object ViewKey { get; set; }
        
        public Guid ID { get => _id; set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; set => SetProperty(ref boundingBox, value); }

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
        [XmlIgnore]
        bool IRenderable.Visible { get; set; }

        private Brush background;
        [XmlIgnore]
        public Brush Background { get => background; set => SetProperty(ref background, value); }

        private Brush accent;
        [XmlIgnore]
        public Brush Accent { get => accent; set => SetProperty(ref accent, value); }

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


        [XmlIgnore]
        [JsonIgnore]
        public IRenderable Parent => RenderPipelineInfo.Parent;
        [XmlIgnore]
        [JsonIgnore]
        public ElementsLinkedList<IRenderable> Children => RenderPipelineInfo.Children;

        private ComputationPipelineInfo computationPipelineInfo;
        [XmlIgnore]
        [JsonIgnore]
        public ComputationPipelineInfo ComputationPipelineInfo => computationPipelineInfo;

        //private ElementsLinkedList<INode> _nodes = new ElementsLinkedList<INode>();
        //public ElementsLinkedList<INode> Nodes => _nodes;

        public ComputableElementState ComputableElementState { get; set; } = ComputableElementState.Unset;
        [XmlIgnore]
        [JsonIgnore]
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

        public bool RenderExpired { get; set; }
        public virtual ContextMenu ContextMenu
        {
            get
            {
                ContextMenu contextMenu = new ContextMenu();
                
                //Delete
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Delete";
                menuItem.Click += (s, e) =>
                {
                    DataViewModel.Instance.Elements.Remove(this);
                };
                contextMenu.Items.Add(menuItem);

                return contextMenu;
            }
        }

        #endregion

        #region Constructor and Compute

        //#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public BaseComp()
        {
            this.boundingBox = new BoundingBox();

            CompInfo ci = this.GetCompInfo();

            this.Accent = new SolidColorBrush(ci.Accent);
            this.Background = new SolidColorBrush(Colors.Gray);
        }

        public BaseComp(int x, int y)
        {
            _cEManager = new ChildElementManager(this);

            renderPipelineInfo = new RenderPipelineInfo(this);
            computationPipelineInfo = new ComputationPipelineInfo(this);

            this.boundingBox = new BoundingBox(x, y, 0, 0);

            CompInfo ci = this.GetCompInfo();

            this.Accent = new SolidColorBrush(ci.Accent);
            this.Background = new SolidColorBrush(Colors.Gray);
        }

        public BaseComp(SerializationInfo info, StreamingContext context)
        {
            _cEManager = new ChildElementManager(this);

            renderPipelineInfo = new RenderPipelineInfo(this);
            computationPipelineInfo = new ComputationPipelineInfo(this);

            //this.boundingBox = new BoundingBox();

            CompInfo ci = this.GetCompInfo();

            this.Accent = new SolidColorBrush(ci.Accent);
            this.Background = new SolidColorBrush(Colors.Gray);
            
            Name = info.GetString("Name");
            MetadataCompInfo = info.GetString("MetadataCompInfo");
            this.ElementType = (ElementType)info.GetValue("ElementType", typeof(ElementType));
            this.State = (ElementState)info.GetValue("State", typeof(ElementState));
            this.boundingBox = (BoundingBox)info.GetValue("BoundingBox", typeof(BoundingBox));
            this.IsSelected = info.GetBoolean("IsSelected");
            this.ElementState = (ElementState)info.GetValue("ElementState", typeof(ElementState));
        }

        public abstract void Initialize();

        protected TextElement titleTextBlock = new TextElement();
        protected TextElement previewTextBlock = new TextElement();
        /// <summary>
        /// Override only if you know what you're doing
        /// </summary>
        public virtual void RenderComp()
        {
            if (this.Children.Count > 0)
            {
                //TODO: At every render
                //textBlock.DisplayedText = this.ElementText;
                this.ChildElementManager.AdjustBounds(true);
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


            previewTextBlock = new TextElement();
            previewTextBlock.TextAlignment = TextAlignment.Left;
            previewTextBlock.DisplayedText = string.Empty;
            previewTextBlock.Width = 200;
            this.ChildElementManager.AddElement(previewTextBlock);
            if (this.ComputationPipelineInfo.IOManager.PrimaryDataOutput >= 0)
            {
                IDataNode node = this.ComputationPipelineInfo.IOManager.DataOutputNodes[this.ComputationPipelineInfo.IOManager.PrimaryDataOutput];
                string primaryDataName = "Out";
                if (!string.IsNullOrEmpty(node.Name))
                {
                    primaryDataName = node.Name;
                }
                this.previewTextBlock.DisplayedText = primaryDataName + " = " + node.DataGoo.ToString();
            }

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
            this.ChildElementManager.AdjustBounds(true);
        }

        public abstract void Compute();

        public virtual void CollectData()
        {
            if (this.ComputationPipelineInfo.IOManager.DataInputNodes != null && this.ComputationPipelineInfo.IOManager.DataInputNodes.Count > 1)
            {
                this.ComputationPipelineInfo.CollectData();
                if (this.ComputationPipelineInfo.IOManager.PrimaryDataOutput > -1)
                {
                    if (previewTextBlock == null)
                    {
                        previewTextBlock = new TextElement();
                        previewTextBlock.TextAlignment = TextAlignment.Left;
                        this.ChildElementManager.AddElement(previewTextBlock);
                    }
                    IDataNode node = this.ComputationPipelineInfo.IOManager.DataOutputNodes[this.ComputationPipelineInfo.IOManager.PrimaryDataOutput];
                    string primaryDataName = "Out";
                    if (!string.IsNullOrEmpty(node.Name))
                    {
                        primaryDataName = node.Name;
                    }
                    this.previewTextBlock.DisplayedText = primaryDataName + " = " + node.DataGoo.ToString();
                }
            }
        }
        public virtual void DeliverData()
        {
            if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0/* && computable.Nodes[0] is NodeElement*/)
            {
                this.ComputationPipelineInfo.DeliverData();
            }
            if (this.ComputationPipelineInfo.IOManager.PrimaryDataOutput >= 0)
            {
                IDataNode node = this.ComputationPipelineInfo.IOManager.DataOutputNodes[this.ComputationPipelineInfo.IOManager.PrimaryDataOutput];
                string primaryDataName = "Out";
                if (!string.IsNullOrEmpty(node.Name))
                {
                    primaryDataName = node.Name;
                }
                this.previewTextBlock.DisplayedText = primaryDataName + " = " + node.DataGoo.ToString();
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
            if (this.RenderPipelineInfo != null)
            {
                if (this.RenderPipelineInfo.Children != null && this.RenderPipelineInfo.Children.Count > 0)
                {
                    foreach (var child in this.RenderPipelineInfo.Children)
                    {
                        if (child != null) child.Dispose();
                    }
                }
            }
            DataViewModel.Instance.Elements.Remove(this);
            GC.SuppressFinalize(this);
        }

        public void SetX(double x)
        {
            BoundingBox.Location.X = x;
            OnPropertyChanged("X");
        }

        public void SetY(double y)
        {
            BoundingBox.Location.Y = y;
            OnPropertyChanged("Y");
        }

        public void SetWidth(double x)
        {
            BoundingBox.Size.Width = x;
            OnPropertyChanged("Width");
        }

        public void SetHeight(double x)
        {
            BoundingBox.Size.Height = x;
            OnPropertyChanged("Height");
        }

        public void Render()
        {
            if (RenderView != null)
                RenderView.Render();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                //info.AddValue("ID", this.ID);
                CompInfo ci = GetCompInfo();
                info.AddValue("Name", ci.Name);
                info.AddValue("MetadataCompInfo", MetadataCompInfo);
                //info.AddValue("X", this.X);
                //info.AddValue("Y", this.Y);
                //info.AddValue("Width", this.Width);
                //info.AddValue("Height", this.Height);
                info.AddValue("ElementType", this.ElementType);
                info.AddValue("State", this.State);
                info.AddValue("IsSelected", this.IsSelected);
                info.AddValue("BoundingBox", this.BoundingBox);
                info.AddValue("ElementState", this.ElementState);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        public void AdjustBounds(bool forceExpand = false)
        {
            if (_owner.RenderView is BaseCompView)
            {
                BaseCompView view = _owner.RenderView as BaseCompView;

                if (_owner.Width < 50 || _owner.Height < 50 || _owner.Width == double.NaN || _owner.Height == double.NaN || forceExpand)
                {
                    _owner.Width = 10000;
                    _owner.Height = 10000;
                    view.UpdateLayout();
                    //DataViewModel.WPFControl.ExpandContent();
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
            //AdjustBounds();
            RenderingCore.Render(this._owner);
            DataViewModel.WPFControl.ExpandContent();
        }
        public void RemoveElement(IRenderable element)
        {
            switch (element.ElementType)
            {
                case ElementType.UIElement:
                    {
                        _bottomUI.Remove(element);
                        break;
                    }
                case ElementType.DisplayUIElement:
                    {
                        _center.Remove(element);
                        break;
                    }
                case ElementType.Node:
                    {
                        if (element is INode)
                        {
                            INode node = element as INode;
                            if (node.NodeType == NodeType.Input)
                            {
                                _input.Remove(element);
                            }
                            else if (node.NodeType == NodeType.Output)
                            {
                                _output.Remove(element);
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
        public int AddDataOutputNode<T>(IDataNode<T> node, string name = "", bool isPrimaryOutput = false)
        {
            node.Name = name;
            if (node is IRenderable) AddElement(node as IRenderable);
            int outInt = this._owner.ComputationPipelineInfo.IOManager.AddDataOutputNode<T>(node);
            if (isPrimaryOutput)
            {
                this._owner.ComputationPipelineInfo.IOManager.PrimaryDataOutput = outInt;
            }
            return outInt;
        }

        public int AddDataInputNode<T>(IDataNode<T> node, string name = "")
        {
            node.Name = name;
            if (node is IRenderable) AddElement(node as IRenderable);
            return this._owner.ComputationPipelineInfo.IOManager.AddDataInputNode<T>(node);
        }

        public int AddEventOutputNode(IEventNode node, string name = "")
        {
            node.Name = name;
            if (node is IRenderable) AddElement(node as IRenderable);
            return this._owner.ComputationPipelineInfo.IOManager.AddEventOutputNode(node);
        }

        public int AddEventInputNode(IEventNode node, string name = "")
        {
            node.Name = name;
            if (node is IRenderable) AddElement(node as IRenderable);
            return this._owner.ComputationPipelineInfo.IOManager.AddEventInputNode(node);
        }

        public void RemoveNode(INode node)
        {
            if (node is IRenderable) RemoveElement(node as IRenderable);
            this._owner.ComputationPipelineInfo.IOManager.RemoveNode(node);
        }

        public T GetData<T>(DataNode<T> node, T defaultValue = default)
        {
            try
            {
                DataStructure<T> ds = GetData<T>(node);
                if (ds is null) return defaultValue;
                if (ds.Data is T castData)
                {
                    return castData;
                }
                else if (ds.Data is T[] castArray && castArray.Length == 1)
                {
                    return castArray[0];
                }
                else if (ds.Data == default)
                {
                    return defaultValue;
                }
                else
                {
                    throw new Exception("Data is not a single item / type mismatch");
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
            return defaultValue;
        }

        public DataStructure<T> GetData<T>(DataNode<T> node)
        {
            if (node == null) return null;
            //if (node.DataValueType == typeof(T))
            //{
            if (node.DataGoo != null)
            {
                if (node.DataGoo.DataType.IsAssignableTo(typeof(T)))
                {
                    //return node.DataGoo.Duplicate<T>();
                    return node.DataGoo;
                }
                else if (node.DataGoo.DataType.IsAssignableTo(typeof(object)))
                {
                    return (node.DataGoo as DataStructure).DuplicateAsType<T>();
                }
                else
                {
                    throw new Exception("Data type mismatch");
                }
            }
            //}
            return null;
        }
        
        public bool SetData<T>(object data, DataNode<T> node)
        {
            if (data is null) return false;
            if (node is null) return false;
            if (data is DataStructure ds)
            {
                if (data is DataStructure<T> dsT)
                    return SetData(dsT, node);
                else
                    return SetData(ds.DuplicateAsType<T>(), node);
            }
            else if (data is T || data.GetType().IsAssignableTo(typeof(T)))
            {
                try
                {
                    return SetData(new DataStructure<T>((T)data), node);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    T tryCastData = (T)data;
                    return SetData(new DataStructure<T>(tryCastData), node);
                }
                catch (Exception ex1)
                {
                    throw ex1;
                }
            }
            return false;
        }

        private bool SetData<T>(DataStructure<T> data, DataNode<T> node)
        {
            try
            {
                if (data is null) return false;
                if (node is null) return false;
                //if (data is EventArgData eData)
                //{
                //    if (eData.Data is DataStructure eds)
                //    {
                //        node.DataGoo = eds.Duplicate<T>();
                //    }
                //}
                else
                {
                    node.DataGoo = data;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    /*else */
                    renderables.Add(renderable);
                }
            }
            return renderables;
        }

        //public void EventOccured(int v, EventArgData eventArgData)
        //{
        //    this._owner.ComputationPipelineInfo.IOManager.EventOccured(v, eventArgData);
        //}

        public void EventOccured(EventNode node, EventArgData eventArgData)
        {
            node.EventOccured(eventArgData);
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

    [Serializable]
    public readonly struct CompInfo
    {
        public CompInfo(BaseComp comp, string name, string group, string tab, Color accent = default, Type[] ArgumentTypes = default)
        {
            ConstructorInfo = comp.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
            if (ArgumentTypes != default)
            {
                ConstructorInfo = comp.GetType().GetConstructor(ArgumentTypes);
            }
            Name = name;
            Group = group;
            Tab = tab;
            Description = "DEVELOPMENT BUILD : Define values for all relevant properties before publishing";
            Author = "DEVELOPMENT_BUILD";
            Version = "0.0.0.1";
            License = "DEVELOPMENT BUILD : No warranties. Use at your own risk. Not cloud-safe";
            Website = "https://iiterate.de";
            Repository = "https://iiterate.de";
            Icon = null;
            if (accent == default)
            {
                Random rnd = new Random();
                byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
                byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
                byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
                Accent = Color.FromRgb(rc, gc, bc);
            }
            else Accent = accent;
            TypeName = comp.GetType().FullName;
            BuiltAgainst = Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            IsValid = true;
            IsDevelopmentBuild = true;
        }
        public CompInfo(IRenderable comp, string name, string group, string tab)
        {
            ConstructorInfo = comp.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
            Name = name;
            Group = group;
            Tab = tab;
            Description = "DEVELOPMENT BUILD : Define values for all relevant properties before publishing";
            Author = "DEVELOPMENT_BUILD";
            Version = "0.0.0.1";
            License = "DEVELOPMENT BUILD : No warranties. Use at your own risk. Not cloud-safe";
            Website = "https://iiterate.de";
            Repository = "https://iiterate.de";
            Icon = null;
            TypeName = comp.GetType().FullName;
            BuiltAgainst = Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            IsValid = true;
            IsDevelopmentBuild = true;
        }
        public CompInfo(BaseComp comp, string name, string group, string tab, string description, string author, string version, string license, string website, string repository, BitmapSource icon, Color accent)
        {
            ConstructorInfo = comp.GetType().GetConstructor(new Type[] { typeof(int), typeof(int) });
            Name = name;
            Group = group;
            Tab = tab;
            Description = description;
            Author = author;
            Version = version;
            License = license;
            Website = website;
            Repository = repository;
            Icon = icon;
            Accent = accent;
            TypeName = comp.GetType().FullName;
            BuiltAgainst = Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            IsValid = true;
            IsDevelopmentBuild = false;
        }
        [JsonIgnore]
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
        public string TypeName { get; }
        public string BuiltAgainst { get; }
        public bool IsValid { get; }
        public bool IsDevelopmentBuild { get; }

        //TODO: Try allowing SVGs as Icons
        [JsonIgnore]
        public BitmapSource Icon { get; init; }
        public Color Accent { get; init; }

        //public Type[] ConstructorParamTypes { get; set; }
        //public string[] ConstructorParamNames { get; set; }
        //public object[] ConstructorDefaults { get; set; }

        string ImageToBase64(BitmapSource bitmap)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        BitmapSource Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(bytes))
            {
                return BitmapFrame.Create(stream);
            }
        }

        public override string ToString()
        {
            //Serialize CompInfo to JSON string
            return JsonConvert.SerializeObject(this);
        }

        internal static CompInfo FromString(BaseComp baseComp, string value)
        {
            //Deserialize JSON string to CompInfo
            CompInfo compInfoDeserialized =  JsonConvert.DeserializeObject<CompInfo>(value);

            CompInfo compInfoOut = new CompInfo(baseComp,
                compInfoDeserialized.Name,
                compInfoDeserialized.Group,
                compInfoDeserialized.Tab,
                compInfoDeserialized.Description,
                compInfoDeserialized.Author,
                compInfoDeserialized.Version,
                compInfoDeserialized.License,
                compInfoDeserialized.Website,
                compInfoDeserialized.Repository,
                null,
                compInfoDeserialized.Accent);

            return compInfoOut;
            //        if (ci.IsValid)
            //{
            //    if (ci.Name != GetCompInfo().Name)
            //    {
            //        _nameOverride = ci.Name;
            //        CompInfo.Inject(this);
            //    }
            //}
        }
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

        [JsonIgnore]
        [IgnoreDataMember]
        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
        public abstract Type ViewType { get; }
        [JsonIgnore]
        [IgnoreDataMember]
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        [JsonIgnore]
        [IgnoreDataMember]
        public double X { get => boundingBox.Location.X; }

        [JsonIgnore]
        [IgnoreDataMember]
        public double Y { get => boundingBox.Location.Y; }

        [JsonIgnore]
        [IgnoreDataMember]
        public double Width
        {
            get => boundingBox.Size.Width;
            set => boundingBox.Size.Width = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
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


        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
        public bool RenderExpired { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public IRenderable Parent => this.RenderPipelineInfo.Parent;

        [JsonIgnore]
        [IgnoreDataMember]
        public ElementsLinkedList<IRenderable> Children => this.RenderPipelineInfo.Children;

        public void SetX(double x)
        {
            BoundingBox.Location.X = x;
            OnPropertyChanged("X");
        }

        public void SetY(double y)
        {
            BoundingBox.Location.Y = y;
            OnPropertyChanged("Y");
        }

        public void SetWidth(double x)
        {
            BoundingBox.Size.Width = x;
            OnPropertyChanged("Width");
        }

        public void SetHeight(double x)
        {
            BoundingBox.Size.Height = x;
            OnPropertyChanged("Height");
        }

        public void Render()
        {
            if (RenderView != null)
                RenderView.Render();
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
        //private object displayedText;

        #endregion

        #region Properties

        //public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        [JsonIgnore]
        [IgnoreDataMember]
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

        [JsonIgnore]
        [IgnoreDataMember]
        public ElementsLinkedList<IConnection> Connections => connections;

        private NodeType _nodeType = NodeType.Unset;
        public NodeType NodeType { get => _nodeType; }

        internal CanvasPoint _hotspot = new CanvasPoint(0, 0);
        [JsonIgnore]
        [IgnoreDataMember]
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
                        if (c.ComputationPipelineInfo.IOManager.EventOutputNodes.Count > 0)
                        {
                            for (int indexEvent = 0; indexEvent < c.ComputationPipelineInfo.IOManager.EventOutputNodes.Count; indexEvent++)
                            {
                                v += ((EventNode)c.ComputationPipelineInfo.IOManager.EventOutputNodes[indexEvent]).BoundingBox.Size.Height;
                            }
                        }
                        if (c.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.DataOutputNodes.Contains(this))
                        {
                            for (int indexData = 0; indexData < c.ComputationPipelineInfo.IOManager.DataOutputNodes.Count; indexData++)
                            {
                                if (c.ComputationPipelineInfo.IOManager.DataOutputNodes[indexData] != this)
                                {
                                    if (c.ComputationPipelineInfo.IOManager.DataOutputNodes[indexData] is IRenderable)
                                    {
                                        v += ((IRenderable)c.ComputationPipelineInfo.IOManager.DataOutputNodes[indexData]).BoundingBox.Size.Height;
                                    }
                                }
                                else
                                {
                                    //if (c.ComputationPipelineInfo.IOManager.DataOutputNodes[indexData] is IRenderable)
                                    //{
                                    //    v += this.BoundingBox.Size.Height;
                                        break;
                                    //}
                                }
                            }
                        }
                        _hotspot = this.RenderPipelineInfo.Parent.BoundingBox.Location +
                        new CanvasPoint(this.RenderPipelineInfo.Parent.BoundingBox.Size.Width,
                            ((this.BoundingBox.Size.Height / 2) + v));
                    }
                    else
                    {
                        if (c.ComputationPipelineInfo.IOManager.EventInputNodes.Count > 0)
                        {
                            for (int indexEvent = 0; indexEvent < c.ComputationPipelineInfo.IOManager.EventInputNodes.Count; indexEvent++)
                            {
                                v += ((EventNode)c.ComputationPipelineInfo.IOManager.EventInputNodes[indexEvent]).BoundingBox.Size.Height;
                            }
                        }
                        if (c.ComputationPipelineInfo.IOManager.DataInputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.DataInputNodes.Contains(this))
                        {
                            for (int indexData = 0; indexData < c.ComputationPipelineInfo.IOManager.DataInputNodes.Count; indexData++)
                            {
                                if (c.ComputationPipelineInfo.IOManager.DataInputNodes[indexData] != this)
                                {
                                    if (c.ComputationPipelineInfo.IOManager.DataInputNodes[indexData] is IRenderable)
                                    {
                                        v += ((IRenderable)c.ComputationPipelineInfo.IOManager.DataInputNodes[indexData]).BoundingBox.Size.Height;
                                    }
                                }
                                else
                                {
                                    //if (c.ComputationPipelineInfo.IOManager.DataInputNodes[indexData] is IRenderable)
                                    //{
                                    //    v += this.BoundingBox.Size.Height;
                                        break;
                                    //}
                                }
                            }
                        }
                        _hotspot = this.RenderPipelineInfo.Parent.BoundingBox.Location +
                        new CanvasPoint(0.0, ((this.BoundingBox.Size.Height / 2) + v));
                    }
                }
                return _hotspot;
            }
        }
        
        [JsonIgnore]
        [IgnoreDataMember]
        public Type DataValueType => typeof(D);

        private DataStructure<D> _dataGoo = new DataStructure<D>();
        public DataStructure<D> DataGoo
        {
            get => _dataGoo;
            set
            {
                try
                {
                    if (value != null && value is DataStructure<D>)
                        _dataGoo = value as DataStructure<D>;
                    else
                    {
                        if (value != null && value.Data != null)
                        {
                            if (value.Data.GetType().IsAssignableTo(typeof(D)))
                            {
                                _dataGoo = value.DuplicateAsType<D>();
                            }
                            else if (value.DataType.IsAssignableTo(typeof(DataStructure<D>)))
                            {
                                //TODO: Improve DataStructure depth with a while loop to iterate into branches
                                _dataGoo = ((DataStructure<D>)value.Data).DuplicateAsType<D>();
                            }
                        }
                        _dataGoo = new DataStructure<D>();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private ComputationPipelineInfo _computationPipelineInfo;
        [JsonIgnore]
        [IgnoreDataMember]
        public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;

        [JsonIgnore]
        [IgnoreDataMember]
        public ElementsLinkedList<INode> Nodes => new ElementsLinkedList<INode>() { this };
        public ComputableElementState ComputableElementState { get; set; }
        DataStructure IDataGooContainer.DataGoo
        {
            get => _dataGoo;
            set
            {
                try
                {
                    if (value != null && value is DataStructure<D>)
                        _dataGoo = value as DataStructure<D>;
                    else if (value != null && value is DataStructure)
                    {
                        if (value.DataType.IsAssignableTo(typeof(D)))
                        {
                            _dataGoo = value.DuplicateAsType<D>();
                        }
                    }
                    else
                    {
                        _dataGoo = new DataStructure<D>();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public abstract string Name { get; set; }

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
            //this.DataGoo.DataChanged += OnDataChanged;
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
                    if (conn.Destination == this && conn.Origin is IDataNode)
                    {
                        if (conn.Origin is IDataNode<D>)
                        {
                            IDataNode<D> norigin = conn.Origin as IDataNode<D>;
                            if (!this.DataGoo.IsValid)
                            {
                                this.DataGoo.Clear();
                                this.DataGoo = norigin.DataGoo;
                            }
                            else if (!this.DataGoo.Equals(norigin.DataGoo))
                            {
                                this.DataGoo = norigin.DataGoo;
                            }
                        }
                        else if ((conn.Origin as IDataNode).DataValueType.IsAssignableFrom(this.DataValueType)
                            || (conn.Origin as IDataNode).DataValueType.IsAssignableTo(this.DataValueType))
                        {
                            IDataNode noriginCAST = conn.Origin as IDataNode;
                            try
                            {
                                if (!this.DataGoo.IsValid)
                                {
                                    this.DataGoo.Clear();
                                    if (noriginCAST.DataGoo != null)
                                    {
                                        this.DataGoo = noriginCAST.DataGoo.DuplicateAsType<D>();
                                    }
                                }
                                else if (!this.DataGoo.Equals(noriginCAST.DataGoo))
                                {
                                    this.DataGoo = noriginCAST.DataGoo.DuplicateAsType<D>();
                                }
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                        }
                        else
                        {
                            throw new Exception("Data type mismatch error");
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
                    if (conn.Origin == this && conn.Destination is IDataNode)
                    {
                        if (conn.Destination is IDataNode<D> ndest)
                        {
                            if (!ndest.DataGoo.IsValid)
                            {
                                ndest.DataGoo.Clear();
                                ndest.DataGoo = this.DataGoo;
                            }
                            else if (!ndest.DataGoo.Equals(this.DataGoo))
                            {
                                ndest.DataGoo = this.DataGoo;
                            }
                        }
                        else if ((conn.Destination as IDataNode).DataValueType.IsAssignableFrom(this.DataValueType)
                            || (conn.Destination as IDataNode).DataValueType.IsAssignableTo(this.DataValueType))
                        {
                            IDataNode ndestCAST = conn.Destination as IDataNode;
                            try
                            {
                                if (!ndestCAST.DataGoo.IsValid)
                                {
                                    ndestCAST.DataGoo.Clear();
                                    //ndestCAST.DataGoo = this.DataGoo.Duplicate<ndestCAST.DataValueType>;
                                    MethodInfo mi = typeof(DataStructure).GetMethod("DuplicateAsType").MakeGenericMethod(ndestCAST.DataValueType);
                                    object result = mi.Invoke(this.DataGoo, null);
                                    if (result != null && result is DataStructure resultDs)
                                    {
                                        ndestCAST.DataGoo = resultDs.Duplicate();
                                        ndestCAST.DataGoo.ToString();
                                    }
                                }
                                else if (!ndestCAST.DataGoo.Equals(this.DataGoo))
                                {
                                    ndestCAST.DataGoo = this.DataGoo.Duplicate();
                                    ndestCAST.DataGoo.ToString();
                                }
                                else
                                {
                                    ndestCAST.DataGoo = this.DataGoo.Duplicate();
                                    ndestCAST.DataGoo.ToString();
                                }
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                        }
                        else
                        {
                            throw new Exception("Data type mismatch error");
                        }
                        //IDataNode<D> nd = conn.Destination as IDataNode<D>;
                        //if (!nd.DataGoo.IsValid)
                        //{
                        //    nd.DataGoo.Clear();
                        //    nd.DataGoo.Data = this.DataGoo.Data;
                        //}
                        //else if (!(nd.DataGoo.Data.Equals(this.DataGoo.Data)))
                        //{
                        //    nd.DataGoo.Data = this.DataGoo.Data;
                        //}
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


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                //info.AddValue("ID", this.ID);
                info.AddValue("X", this.X);
                info.AddValue("Y", this.Y);
                info.AddValue("Width", this.Width);
                info.AddValue("Height", this.Height);
                info.AddValue("ElementType", this.ElementType);
                info.AddValue("State", this.State);
                info.AddValue("IsSelected", this.IsSelected);
                info.AddValue("BoundingBox", this.BoundingBox);
                info.AddValue("ElementState", this.ElementState);
                info.AddValue("Parent", this.Parent);
                info.AddValue("Children", this.Children);
            }
            catch (Exception ex)
            {
                throw ex;
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

        [JsonIgnore]
        [IgnoreDataMember]
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

        [JsonIgnore]
        [IgnoreDataMember]
        public ElementsLinkedList<IConnection> Connections => connections;

        private NodeType _nodeType = NodeType.Unset;
        public NodeType NodeType { get => _nodeType; }

        internal CanvasPoint _hotspot = new CanvasPoint(0, 0);
        [JsonIgnore]
        [IgnoreDataMember]
        public CanvasPoint Hotspot
        {
            get
            {
                double v = 20.0;
                if ((this as INode).Parent is IComputable)
                {
                    IComputable c = (this as INode).Parent as IComputable;
                    if (this.NodeType == NodeType.Output)
                    {
                        if (c.ComputationPipelineInfo.IOManager.EventOutputNodes.Count > 1 && c.ComputationPipelineInfo.IOManager.EventOutputNodes.Contains(this))
                        {
                            int i = c.ComputationPipelineInfo.IOManager.EventOutputNodes.IndexOf(this);
                            v = v + (i * this.BoundingBox.Size.Height);
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

        //public Type DataValueType => typeof(D);

        //private DataStructure<D> _dataGoo = new DataStructure<D>();
        //public DataStructure<D> DataGoo { get => _dataGoo; set => _dataGoo = value; }

        private ComputationPipelineInfo _computationPipelineInfo;
        [JsonIgnore]
        [IgnoreDataMember]
        public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;

        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
        public RenderPipelineInfo RenderPipelineInfo => renderPipelineInfo;
        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
        public abstract Type ViewType { get; }
        [JsonIgnore]
        [IgnoreDataMember]
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        public bool IsSelected { get; set; }

        public BoundingBox BoundingBox { get => boundingBox; private set => SetProperty(ref boundingBox, value); }

        [JsonIgnore]
        [IgnoreDataMember]
        public double X { get => boundingBox.Location.X; }

        [JsonIgnore]
        [IgnoreDataMember]
        public double Y { get => boundingBox.Location.Y; }

        [JsonIgnore]
        [IgnoreDataMember]
        public double Width
        {
            get => boundingBox.Size.Width;
            set => boundingBox.Size.Width = value;
        }

        [JsonIgnore]
        [IgnoreDataMember]
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


        [JsonIgnore]
        [IgnoreDataMember]
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
        [JsonIgnore]
        [IgnoreDataMember]
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

        [JsonIgnore]
        [IgnoreDataMember]
        public EventArgData EventArgData { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public IRenderable Parent { get => this.RenderPipelineInfo.Parent; set => this.RenderPipelineInfo.SetParent(value); }
        public abstract string Name { get; set; }

        //public ElementsLinkedList<IConnection> Connections => throw new NotImplementedException();

        //public CanvasPoint Hotspot => throw new NotImplementedException();

        //public double HotspotThresholdRadius => throw new NotImplementedException();
        public bool RenderExpired { get; set; }

        //public IRenderable Parent => this.RenderPipelineInfo.Parent;

        [JsonIgnore]
        [IgnoreDataMember]
        public ElementsLinkedList<IRenderable> Children => this.RenderPipelineInfo.Children;

        public void SetX(double x)
        {
            BoundingBox.Location.X = x;
            OnPropertyChanged("X");
        }

        public void SetY(double y)
        {
            BoundingBox.Location.Y = y;
            OnPropertyChanged("Y");
        }

        public void SetWidth(double x)
        {
            BoundingBox.Size.Width = x;
            OnPropertyChanged("Width");
        }

        public void SetHeight(double x)
        {
            BoundingBox.Size.Height = x;
            OnPropertyChanged("Height");
        }

        public void Render()
        {
            if (RenderView != null)
                RenderView.Render();
        }

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

        public void TriggerEvent(EventArgData e)
        {
            NodeEvent.Invoke(this, e);
            if (this.Parent is IComputable)
            {
                ComputationCore.Compute(this.Parent as IComputable, false);
            }
        }

        public bool EventOccured(EventArgData e, bool upstreamCallback = false)
        {
            this.EventArgData = e;
            if (this.Parent is IComputable)
            {
                IComputable computable = this.Parent as IComputable;
                //TODO: Call a delegate method that triggers a call-back once complete
                if (upstreamCallback)
                {
                    foreach (IComputable compUS in this.ElementUS)
                    {
                        if (compUS != null)
                        {
                            if (compUS.ComputationPipelineInfo.IOManager.EventOutputNodes != null &&
                                compUS.ComputationPipelineInfo.IOManager.EventOutputNodes.Count > 0)
                            {
                                foreach (IEventNode en in compUS.ComputationPipelineInfo.IOManager.EventOutputNodes)
                                {
                                    if (en.Connections != null && en.Connections.Count > 0)
                                    {
                                        foreach (IConnection connection in en.Connections)
                                        {
                                            if (connection.Destination == this)
                                            {
                                                if (connection.Origin is EventNode d)
                                                {
                                                    d.TriggerEvent(e);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
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
                                                if (connection.Destination is EventNode d)
                                                {
                                                    d.TriggerEvent(e);
                                                }
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
        public bool EventOccured(EventArgData e)
        {
            this.EventArgData = e;
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
                                            if (connection.Destination is EventNode d)
                                            {
                                                d.TriggerEvent(e);
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


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                //info.AddValue("ID", this.ID);
                info.AddValue("X", this.X);
                info.AddValue("Y", this.Y);
                info.AddValue("Width", this.Width);
                info.AddValue("Height", this.Height);
                info.AddValue("ElementType", this.ElementType);
                info.AddValue("State", this.State);
                info.AddValue("IsSelected", this.IsSelected);
                info.AddValue("BoundingBox", this.BoundingBox);
                info.AddValue("ElementState", this.ElementState);
                info.AddValue("Parent", this.Parent);
                info.AddValue("Children", this.Children);
            }
            catch (Exception ex)
            {
                throw ex;
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
