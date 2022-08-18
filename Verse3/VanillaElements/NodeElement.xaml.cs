using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Core.Geometry2D;

namespace Verse3.VanillaElements
{
    /// <summary>
    /// Visual Interaction logic for TestElement.xaml
    /// </summary>
    public partial class NodeElementView : UserControl, IBaseElementView<NodeElement>
    {
        #region IBaseElementView Members

        private NodeElement _element;
        public NodeElement Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as NodeElement;
                }
                return _element;
            }
            private set
            {
                _element = value as NodeElement;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render

        public NodeElementView()
        {
            InitializeComponent();
        }

        public void Render()
        {
            if (this.Element != null)
            {
                if (Element.RenderView != this) Element.RenderView = this;
                if (this.Element.Connections != null)
                {
                    foreach (BezierElement bezier in this.Element.Connections)
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
                this.Element = this.DataContext as NodeElement;
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
            BezierElement b = (BezierElement)DataViewModel.ActiveConnection;
            if (DataViewModel.ActiveConnection == default)
            {
                DataViewModel.ActiveNode = this.Element as INode;
                DataViewModel.ActiveConnection = DataViewModel.CreateConnection(DataViewModel.ActiveNode);
                b = (BezierElement)DataViewModel.ActiveConnection;
                if (b != null)
                {
                    this.Element.RenderPipelineInfo.AddChild(b);
                    this.Element.Connections.Add(b);
                    MousePositionNode.Instance.Connections.Add(b);
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            else
            {
                if (b != null)
                {
                    if (DataViewModel.ActiveNode is IComputable && DataViewModel.ActiveNode != this.Element)
                    {
                        this.Element.ComputationPipelineInfo.AddDataUpStream(DataViewModel.ActiveNode as IComputable);
                    }
                    DataViewModel.ActiveNode = this.Element as INode;
                    b.SetDestination(DataViewModel.ActiveNode);
                    if (MousePositionNode.Instance.Connections.Contains(b))
                        MousePositionNode.Instance.Connections.Remove(b);
                    this.Element.RenderPipelineInfo.AddChild(b);
                    this.Element.Connections.Add(b);
                    DataViewModel.ActiveConnection = default;
                    DataViewModel.ActiveNode = default;
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            ComputationPipeline.Compute();
            //RenderPipeline.Render();
            //this.Element.OnPropertyChanged("BoundingBox");
        }
    }

    public class NodeElement : BaseElement, INode, IDataGooContainer<double>
    {
        #region Data Members

        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();
        internal IRenderable parentElement = default;
        private object displayedText;

        #endregion

        #region Properties

        public override Type ViewType => typeof(NodeElementView);

        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        IElement INode.Parent { get => this.RenderPipelineInfo.Parent; }

        public ElementsLinkedList<IConnection> Connections => connections;

        public NodeType NodeType { get => NodeType.Unset; }

        public CanvasPoint Hotspot
        {
            get
            {
                CanvasPoint center = this.BoundingBox.Center;
                //center.X += (this.BoundingBox.Size.Width / 2);
                //center.Y -= (this.BoundingBox.Size.Height / 2);
                return center;
            }
        }

        public double HotspotThresholdRadius { get; }

        public Type DataValueType => typeof(double);

        private DataStructure<double> _dataGoo = new DataStructure<double>();
        public DataStructure<double> DataGoo { get => _dataGoo; set => _dataGoo = value; }

        private ComputationPipelineInfo _computationPipelineInfo;
        public ComputationPipelineInfo ComputationPipelineInfo => _computationPipelineInfo;

        public ElementsLinkedList<INode> Nodes => new ElementsLinkedList<INode>() { this };
        public ComputableElementState ComputableElementState { get; set; }

        #endregion

        #region Constructor and Compute

        public NodeElement(IRenderable parent, NodeType type = NodeType.Unset) : base()
        {
            _computationPipelineInfo = new ComputationPipelineInfo(this);
            parentElement = parent as IRenderable;
            double x = DataViewModel.ContentCanvasMarginOffset + parentElement.X;
            double y = DataViewModel.ContentCanvasMarginOffset + parentElement.Y;
            base.boundingBox = new BoundingBox(x, y, parentElement.Width, 50);
            (this as IRenderable).RenderPipelineInfo.SetParent(parentElement);
            this.DisplayedText = "Node";
            this.PropertyChanged += NodeElement_PropertyChanged;
        }
        public void Compute()
        {
            //if (Nodes[0] == this)
            //{
            //    if (this.Connections != null && this.Connections.Count > 0)
            //    {
            //        if (this.Connections[0].Destination is NodeElement && this.Connections[0].Origin == this)
            //        {
            //            NodeElement des = this.Connections[0].Destination as NodeElement;
            //            if (this.DataGoo != null)
            //            {
            //                //TODO: Restructure \ Refine Compute steps
            //                //this.DataGoo = des.DataGoo;
            //                //if (this.DataGoo == null)
            //                //{
            //                    //des.DataGoo = new DataStructure<double>(0.0);
            //                //}
            //            }
            //        }
            //    }
            //}
        }
        public void CollectData()
        {
            if (this.Connections != null && this.Connections.Count > 0)
            {
                foreach (IConnection conn in this.Connections)
                {
                    //INCOMING CONNECTIONS
                    if (conn.Destination == this/* && conn.Origin is NodeElement*/)
                    {
                        
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
            //if (this.Nodes != null && this.Nodes.Count > 1/* && computable.Nodes[0] is NodeElement*/)
            //{
            //    foreach (INode n in this.Nodes)
            //    {
            //        if (n is IComputable)
            //        {
            //            IComputable c = (IComputable)n;
            //            c.Compute();
            //        }
            //        if (n.Connections != null && n.Connections.Count > 0)
            //        {
            //            foreach (IConnection conn in n.Connections)
            //            {
            //                //INCOMING CONNECTIONS
            //                //if (conn.Destination == n/* && conn.Origin is NodeElement*/)
            //                //{
            //                //_inputValue = n.DataGoo.Data;
            //                //RenderPipeline.RenderRenderable(conn.Origin.Parent as IRenderable);
            //                //}
            //                //OUTGOING CONNECTIONS
            //                /*else */
            //                if (conn.Origin == n/* && conn.Destination is NodeElement*/)
            //                {
            //                    //NodeElement nd = (NodeElement)conn.Destination;
            //                    //nd.DataGoo.Data = _sliderValue + _inputValue;
            //                    //RenderPipeline.RenderRenderable(conn.Destination.Parent as IRenderable);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        #endregion

        private void NodeElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (IRenderable renderable in this.Connections)
            {
                //renderable.Render();
            }
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
                        SetProperty(ref horizontalAlignment, HorizontalAlignment.Left);
                    }
                }
                else if (this.NodeType == NodeType.Output)
                {
                    if (horizontalAlignment != HorizontalAlignment.Right)
                    {
                        horizontalAlignment = HorizontalAlignment.Right;
                        SetProperty(ref horizontalAlignment, HorizontalAlignment.Right);
                    }
                }
                return horizontalAlignment;
            }
            set => SetProperty(ref horizontalAlignment, value);
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
                            b.RenderView.Render();
                        }
                    }
                }
            }
        }
        
        public IElement Parent { get; }

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