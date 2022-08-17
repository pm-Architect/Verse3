using Core;
using System;
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
            DataViewModel.ActiveNode = this.Element as INode;
            BezierElement b = (BezierElement)DataViewModel.ActiveConnection;
            if (DataViewModel.ActiveConnection == default)
            {
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
                    if (MousePositionNode.Instance.Connections.Contains(b))
                        MousePositionNode.Instance.Connections.Remove(b);
                    b.SetDestination(DataViewModel.ActiveNode);
                    this.Element.RenderPipelineInfo.AddChild(b);
                    this.Element.Connections.Add(b);
                    DataViewModel.ActiveConnection = default;
                    DataViewModel.ActiveNode = default;
                    //b.RedrawBezier(b.Origin, b.Destination);
                    //b.RenderView.Render();
                }
            }
            this.Element.OnPropertyChanged("BoundingBox");
        }
    }

    public class NodeElement : BaseElement, INode/*, IDataGooContainer*/
    {
        #region Data Members

        private ElementsLinkedList<IConnection> connections = new ElementsLinkedList<IConnection>();
        internal IRenderable parentElement = default;
        
        #endregion

        #region Properties

        public override Type ViewType => typeof(NodeElementView);

        #endregion

        #region Constructors

        public NodeElement(IRenderable parent) : base()
        {
            parentElement = parent as IRenderable;
            double x = DataViewModel.ContentCanvasMarginOffset + parentElement.X;
            double y = DataViewModel.ContentCanvasMarginOffset + parentElement.Y;
            base.boundingBox = new BoundingBox(x, y, parentElement.Width, 50);
            (this as IRenderable).RenderPipelineInfo.SetParent(parentElement);
            this.DisplayedText = "Node";
            this.PropertyChanged += NodeElement_PropertyChanged;
        }

        #endregion

        //public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    base.OnPropertyChanged(propertyName);
        //}

        private void NodeElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (IRenderable renderable in this.Connections)
            {
                //renderable.Render();
            }
        }

        //public NodeElement(int x, int y, int width, int height)
        //{
        //    this.boundingBox = new BoundingBox(x, y, width, height);

        //    this.DisplayedText = "Button";
        //}


        private object displayedText;

        public object DisplayedText { get => displayedText; set => SetProperty(ref displayedText, value); }

        IElement INode.Parent { get => this.parentElement; }

        public ElementsLinkedList<IConnection> Connections => connections;

        public NodeType NodeType { get => NodeType.Output; }

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