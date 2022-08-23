using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class NodeElementView : UserControl, IBaseElementView<IRenderable>
    {
        public class GenericObjectFactoryExtension : MarkupExtension
        {
            public Type Type { get; set; }
            public Type T { get; set; }

            public override object ProvideValue(IServiceProvider serviceProvider)
            {
                var genericType = Type.MakeGenericType(T);
                return Activator.CreateInstance(genericType);
            }
        }

        #region IBaseElementView Members

        private Type Y = default;
        //DEV NOTE: CAUTION! DYNAMIC TYPE IS USED
        private dynamic _element;
        public IRenderable Element
        {
            get
            {
                if (this._element == null)
                {
                    //if (this.DataContext.GetType().GenericTypeArguments.Length == 1)
                    //{
                    if (this.DataContext != null)
                    {
                        Y = this.DataContext.GetType();
                        if (Y.BaseType.Name == (typeof(NodeElement<>).Name))
                        {
                            //TODO: Log to Console and process
                            //Y = this.DataContext.GetType().MakeGenericType(Y);
                            //_element = Convert.ChangeType(this.DataContext, U) as IRenderable;
                            Y = this.DataContext.GetType()/*.MakeGenericType(this.DataContext.GetType().GenericTypeArguments[0].GetType())*/;
                            _element = this.DataContext;
                            return _element;
                        }
                    }
                    //}
                }
                return _element;
            }
            private set
            {
                if (Y != default)
                {
                    if (value.GetType().IsAssignableTo(Y))
                    {
                        _element = value;
                    }
                }
            }
        }
        IRenderable IRenderView.Element => Element;
        //public static T ForceCast<T>(object obj)
        //{
        //    try
        //    {
        //        return
        //    }
        //    catch/* (Exception ex)*/
        //    {
        //        //throw ex;
        //    }
        //}

        #endregion

        #region Constructor and Render

        public NodeElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null && this.Element is IDataNode)
            {
                IDataNode node = this.Element as IDataNode;
                if (Element.RenderView != this) Element.RenderView = this;
                if (node.Connections != null)
                {
                    foreach (BezierElement bezier in node.Connections)
                    {
                        if (bezier != null)
                        {
                            if (bezier.Origin == this.Element)
                            {

                            }
                            else if (bezier.Destination == this.Element)
                            {

                            }
                            bezier.RedrawBezier(bezier.Origin, bezier.Destination);
                        }
                    }
                }
            }
        }

        #endregion

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
            if (this.DataContext != null)
            {
                this.Element = this.DataContext as IRenderable;
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
            if (this.Element != null)
            {
                (this.Element as IDataNode).ToggleActive();
            }
            //DEV NOTE: CAUTION! DYNAMIC TYPE IS USED
            //try
            //{
            //    dynamic element = Convert.ChangeType(this.Element, Y);
            //    element.ToggleActive(this, e);
            //}
            //catch (Exception)
            //{
            //    //Log to Console and process
            //    throw;
            //}
        }
    }

    public class NodeElement<T> : DataNode<T>
    {
        public override Type ViewType => typeof(NodeElementView);

        #region Constructor and Compute

        public NodeElement(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
            //_computationPipelineInfo = new ComputationPipelineInfo(this);
            //this.RenderPipelineInfo.Parent = parent as IRenderable;
            double x = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.X;
            double y = DataViewModel.ContentCanvasMarginOffset + this.RenderPipelineInfo.Parent.Y;
            base.boundingBox = new BoundingBox(x, y, this.RenderPipelineInfo.Parent.Width, 50);
            (this as IRenderable).RenderPipelineInfo.SetParent(this.RenderPipelineInfo.Parent);
            this.DisplayedText = "Node";
            this.PropertyChanged += NodeElement_PropertyChanged;
            if (type == NodeType.Input)
            {
                this.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (type == NodeType.Output)
            {
                this.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else
            {
                this.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        #endregion

        private void NodeElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (IRenderable renderable in this.Connections)
            {
                //renderable.Render();
            }
        }

        public override void ToggleActive()
        {
            //Set as active Node
            BezierElement b = (BezierElement)DataViewModel.ActiveConnection;
            if (DataViewModel.ActiveConnection == default)
            {
                DataViewModel.ActiveNode = this as INode;
                DataViewModel.ActiveConnection = DataViewModel.CreateConnection(DataViewModel.ActiveNode);
                b = (BezierElement)DataViewModel.ActiveConnection;
                if (b != null)
                {
                    this.RenderPipelineInfo.AddChild(b);
                    this.Connections.Add(b);
                    MousePositionNode.Instance.Connections.Add(b);
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            else
            {
                if (b != null)
                {
                    if (DataViewModel.ActiveNode is IComputable && DataViewModel.ActiveNode != this && DataViewModel.ActiveNode.NodeType == NodeType.Output)
                    {
                        this.ComputationPipelineInfo.AddDataUpStream(DataViewModel.ActiveNode as IComputable);
                    }
                    DataViewModel.ActiveNode = this as INode;
                    b.SetDestination(DataViewModel.ActiveNode);
                    if (MousePositionNode.Instance.Connections.Contains(b))
                        MousePositionNode.Instance.Connections.Remove(b);
                    this.RenderPipelineInfo.AddChild(b);
                    this.Connections.Add(b);
                    DataViewModel.ActiveConnection = default;
                    DataViewModel.ActiveNode = default;
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            //ComputationPipeline.Compute();
            //RenderPipeline.Render();
            //this.Element.OnPropertyChanged("BoundingBox");
        }

        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (this.NodeType == NodeType.Input)
                {
                    if (horizontalAlignment != HorizontalAlignment.Left)
                    {
                        horizontalAlignment = HorizontalAlignment.Left;
                        //SetProperty(ref horizontalAlignment, HorizontalAlignment.Left);
                        OnPropertyChanged("HorizontalAlignment");
                    }
                }
                else if (this.NodeType == NodeType.Output)
                {
                    if (horizontalAlignment != HorizontalAlignment.Right)
                    {
                        horizontalAlignment = HorizontalAlignment.Right;
                        //SetProperty(ref horizontalAlignment, HorizontalAlignment.Right);
                        OnPropertyChanged("HorizontalAlignment");
                    }
                }
                return horizontalAlignment;
            }
             private set => SetProperty(ref horizontalAlignment, value);
        }

        private System.Windows.Media.Brush nodeContentColor = System.Windows.Media.Brushes.White;

        public System.Windows.Media.Brush NodeContentColor
        {
            get
            {
                if (this.Connections != null && this.Connections.Count > 0)
                {
                    if (nodeContentColor != System.Windows.Media.Brushes.White)
                    {
                        nodeContentColor = System.Windows.Media.Brushes.White;
                        //SetProperty(ref nodeContentColor, System.Windows.Media.Brushes.White);
                        OnPropertyChanged("NodeContentColor");
                    }
                }
                else
                {
                    if (nodeContentColor != System.Windows.Media.Brushes.Transparent)
                    {
                        nodeContentColor = System.Windows.Media.Brushes.Transparent;
                        //SetProperty(ref nodeContentColor, System.Windows.Media.Brushes.Transparent);
                        OnPropertyChanged("NodeContentColor");
                    }
                }
                return nodeContentColor;
            }
            private set => SetProperty(ref nodeContentColor, value);
        }

    }

    public class MousePositionNode : INode
    {
        public static readonly MousePositionNode Instance = new MousePositionNode();
        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();

        private MousePositionNode()
        {
        }

        public static void RefreshPosition()
        {
            System.Drawing.Point p = DataViewModel.WPFControl.GetMouseRelPosition();
            Instance._hotspot = new CanvasPoint(p.X, p.Y);

            if (MousePositionNode.Instance.Connections != null)
            {
                if (MousePositionNode.Instance.Connections.Count > 0)
                {
                    //RenderPipeline.Render();
                    foreach (IConnection c in MousePositionNode.Instance.Connections)
                    {
                        if (c is BezierElement)
                        {
                            BezierElement b = c as BezierElement;
                            b.RedrawBezier(b.Origin, b.Destination);
                            if (b.RenderView != null) b.RenderView.Render();
                        }
                    }
                }
            }
        }

        public IElement Parent { get; set; } = default;

        public ElementsLinkedList<IConnection> Connections => connections;

        public NodeType NodeType => NodeType.Unset;

        private CanvasPoint _hotspot = CanvasPoint.Unset;
        public CanvasPoint Hotspot
        {
            get
            {
                return _hotspot;
            }
            private set
            {
                _hotspot = value;
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