using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        private ElementsLinkedList<INode> _nodes = new ElementsLinkedList<INode>();
        public ElementsLinkedList<INode> Nodes => _nodes;

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

        public abstract void Compute();

        public virtual void CollectData()
        {
            if (this.Nodes != null && this.Nodes.Count > 1/* && computable.Nodes[0] is NodeElement*/)
            {
                foreach (INode n in this.Nodes)
                {
                    if (n is IComputable && (n.NodeType == NodeType.Input || n.NodeType == NodeType.Unset))
                    {
                        IComputable c = (IComputable)n;
                        this.computationPipelineInfo.AddDataUpStream(c);
                        if (n.Connections != null && n.Connections.Count > 0)
                        {
                            foreach (IConnection conn in n.Connections)
                            {
                                //INCOMING CONNECTIONS
                                if (conn.Destination == n && conn.Origin is IComputable)
                                {
                                    c.ComputationPipelineInfo.AddDataDownStream(conn.Origin as IComputable);
                                    //_inputValue = n.DataGoo.Data;
                                    //RenderPipeline.RenderRenderable(conn.Origin.Parent as IRenderable);
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
                        ComputationPipeline.ComputeComputable(c);
                    }
                }
            }
        }
        public virtual void DeliverData()
        {
            if (this.Nodes != null && this.Nodes.Count > 0/* && computable.Nodes[0] is NodeElement*/)
            {
                foreach (INode n in this.Nodes)
                {
                    if (n is IComputable && (n.NodeType == NodeType.Output || n.NodeType == NodeType.Unset))
                    {
                        IComputable c = (IComputable)n;
                        this.computationPipelineInfo.AddDataDownStream(c);
                        if (n.Connections != null && n.Connections.Count > 0)
                        {
                            foreach (IConnection conn in n.Connections)
                            {
                                //INCOMING CONNECTIONS
                                //if (conn.Destination == n/* && conn.Origin is NodeElement*/)
                                //{
                                //_inputValue = n.DataGoo.Data;
                                //RenderPipeline.RenderRenderable(conn.Origin.Parent as IRenderable);
                                //}
                                //OUTGOING CONNECTIONS
                                /*else */
                                if (conn.Origin == n/* && conn.Destination is NodeElement*/)
                                {
                                    c.ComputationPipelineInfo.AddDataUpStream(conn.Destination as IComputable);
                                    //NodeElement nd = (NodeElement)conn.Destination;
                                    //nd.DataGoo.Data = _sliderValue + _inputValue;
                                    //RenderPipeline.RenderRenderable(conn.Destination.Parent as IRenderable);
                                }
                            }
                        }
                        ComputationPipeline.ComputeComputable(c);
                    }
                }
            }
        }

        //#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
    public interface IBaseCompView<R> : IRenderView where R : BaseComp
    {
        public new R Element
        {
            get
            {
                if (Element == null)
                {
                    if (this.GetType().IsAssignableTo(typeof(UserControl)))
                    {
                        object dc = ((UserControl)this).DataContext;
                        if (dc.GetType().IsAssignableTo(typeof(R)))
                        {
                            Element = (R)dc;
                        }
                    }
                }
                return Element;
            }
            private set
            {
                if (value is R)
                {
                    Element = (R)value;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        public Guid? ElementGuid
        {
            get { return Element?.ID; }
        }


        public new void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.RenderView != this) this.Element.RenderView = this;
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
}
