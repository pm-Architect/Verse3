using System;
using System.ComponentModel;
using static Core.Geometry2D;

namespace Core
{

    #region Dev-Singletons and Pipelines

    /// <summary>
    /// A simple example of a data-model.  
    /// The purpose of this data-model is to share display data between the main window and overview window.
    /// </summary>
    public class DataModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// </summary>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region Data Members

        /// <summary>
        /// The singleton instance.
        /// This is a singleton for convenience.
        /// </summary>
        protected static DataModel instance = new DataModel();

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        protected ElementsLinkedList<IElement> elements = new ElementsLinkedList<IElement>();

        ///
        /// The current scale at which the content is being viewed.
        /// 
        protected double contentScale = 1;

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        protected double contentOffsetX = 0;

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        protected double contentOffsetY = 0;

        ///
        /// The width of the content (in content coordinates).
        /// 
        protected double contentWidth = 0;

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        protected double contentHeight = 0;

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        protected double contentViewportWidth = 0;

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        protected double contentViewportHeight = 0;

        #endregion Data Members

        #region Constructors

        /// <summary>
        /// Retreive the singleton instance.
        /// </summary>
        public static DataModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataModel();
                }
                return DataModel.instance;
            }
            protected set
            {
                instance = value;
            }
        }

        public static double ContentCanvasMarginOffset = 10.0;

        public DataModel() : base()
        {
            //
            // Initialize the data model.
            //
            DataModel.instance = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The list of rectangles that is displayed both in the main window and in the overview window.
        /// </summary>
        public ElementsLinkedList<IElement> Elements
        {
            get
            {
                return elements;
            }
            protected set
            {
                elements = value;
            }
        }

        public IElement GetElementWithGuid(Guid guid)
        {
            return this.Elements.Find(guid).Value;
        }

        ///
        /// The current scale at which the content is being viewed.
        /// 
        public double ContentScale
        {
            get
            {
                return contentScale;
            }
            set
            {
                contentScale = value;

                OnPropertyChanged("ContentScale");
            }
        }

        public CanvasPoint ContentOffset
        {
            get
            {
                return new CanvasPoint(ContentOffsetX, ContentOffsetY);
            }
            set
            {
                ContentOffsetX = value.X;
                ContentOffsetY = value.Y;
            }
        }

        ///
        /// The X coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetX
        {
            get
            {
                return contentOffsetX;
            }
            set
            {
                contentOffsetX = value;

                OnPropertyChanged("ContentOffsetX");
            }
        }

        ///
        /// The Y coordinate of the offset of the viewport onto the content (in content coordinates).
        /// 
        public double ContentOffsetY
        {
            get
            {
                return contentOffsetY;
            }
            set
            {
                contentOffsetY = value;

                OnPropertyChanged("ContentOffsetY");
            }
        }

        public CanvasSize ContentSize
        {
            get
            {
                return new CanvasSize(ContentWidth, ContentHeight);
            }
            set
            {
                ContentWidth = value.Width;
                ContentHeight = value.Height;
            }
        }

        ///
        /// The width of the content (in content coordinates).
        /// 
        public double ContentWidth
        {
            get
            {
                return contentWidth;
            }
            set
            {
                contentWidth = value + (ContentCanvasMarginOffset * 2);

                OnPropertyChanged("ContentWidth");
            }
        }

        ///
        /// The heigth of the content (in content coordinates).
        /// 
        public double ContentHeight
        {
            get
            {
                return contentHeight;
            }
            set
            {
                contentHeight = value + (ContentCanvasMarginOffset * 2);

                OnPropertyChanged("ContentHeight");
            }
        }


        public CanvasSize ContentViewportSize
        {
            get
            {
                return new CanvasPoint(ContentViewportWidth, ContentViewportHeight);
            }
            set
            {
                ContentViewportWidth = value.Width;
                ContentViewportHeight = value.Height;
            }
        }

        ///
        /// The width of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportWidth
        {
            get
            {
                return contentViewportWidth;
            }
            set
            {
                contentViewportWidth = value;

                OnPropertyChanged("ContentViewportWidth");
            }
        }

        ///
        /// The heigth of the viewport onto the content (in content coordinates).
        /// The value for this is actually computed by the main window's ZoomAndPanControl and update in the
        /// data model so that the value can be shared with the overview window.
        /// 
        public double ContentViewportHeight
        {
            get
            {
                return contentViewportHeight;
            }
            set
            {
                contentViewportHeight = value;

                OnPropertyChanged("ContentViewportHeight");
            }
        }

        #endregion
    }
    public class RenderPipeline
    {
        private static RenderPipeline instance = new RenderPipeline();
        public static RenderPipeline Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RenderPipeline();
                }
                return RenderPipeline.instance;
            }
            protected set
            {
                instance = value;
            }
        }
        internal IRenderable _current;
        public IRenderable Current => _current;
        private RenderPipeline()
        {
            this._current = default;
        }
        public static int Render()
        {
            int count = 0;
            try
            {
                //Parallel.ForEach(DataModel.Instance.Elements, e => { });
                foreach (IElement e in DataModel.Instance.Elements)
                {
                    if (e != null && e is IRenderable)
                    {
                        IRenderable renderable = e as IRenderable;
                        //if (RenderPipeline.Instance._current != default)
                        //{
                        //    RenderPipeline.Instance._current.ZNext = renderable;
                        //}
                        RenderPipeline.Instance._current = renderable;
                        if (RenderRenderable(renderable))
                        {
                            count++;
                        }
                        //renderable.Render();
                        //count++;
                        //if (renderable.Children.Count > 0)
                        //{
                        //    foreach (IRenderable child in renderable.Children)
                        //    {
                        //        child.Render();
                        //        count++;
                        //    }
                        //}
                    }
                }
            }
            catch /*(Exception e)*/
            {
                //TODO: Log to console
            }
            return count;
        }
        public static bool TranslateOffsetRenderable(IRenderable renderable, double xOffset, double yOffset, bool recursive = true)
        {
            bool renderSuccess = true;
            try
            {
                if (renderable != null)
                {
                    renderable.SetX(renderable.X + xOffset);
                    renderable.SetY(renderable.Y + yOffset);
                    renderSuccess = renderSuccess && RenderPipeline.RenderRenderable(renderable);
                    if (recursive)
                    {
                        if (renderable.Children != null && renderable.Children.Count > 0)
                        {
                            foreach (IRenderable child in renderable.Children)
                            {
                                renderSuccess = renderSuccess && RenderPipeline.TranslateOffsetRenderable(child, xOffset, yOffset);
                            }
                        }
                    }
                }
            }
            catch /*(Exception e)*/
            {
                //TODO: Log to console
                return false;
            }
            return renderSuccess;
        }
        public static bool RenderRenderable(IRenderable renderable, bool recursive = true)
        {
            bool renderSuccess = true;
            try
            {
                if (renderable != null)
                {
                    renderable.Render();
                    if (recursive)
                    {
                        if (renderable.Children != null && renderable.Children.Count > 0)
                        {
                            foreach (IRenderable child in renderable.Children)
                            {
                                renderSuccess = renderSuccess && RenderRenderable(child);
                            }
                        }
                    }
                }
            }
            catch /*(Exception e)*/
            {
                //TODO: Log to console
                return false;
            }
            return renderSuccess;
        }
    }

    #endregion

    #region Element Interface

    public interface IElement : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// GUID of the element.
        /// </summary>
        public Guid ID { get; }

        public ElementState ElementState { get; set; }

        public ElementType ElementType { get; set; }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the 'PropertyChanged' event when the value of a property of the data model has changed.
        /// Be sure to Define a 'PropertyChanged' event that is raised when the value of a property of the data model has changed.
        /// eg. <code>public new abstract event PropertyChangedEventHandler PropertyChanged;</code>
        /// </summary>
        public abstract void OnPropertyChanged(string name);

        #endregion
    }

    #endregion

    #region Renderables

    public class RenderPipelineInfo
    {
        private IRenderable _renderable;
        public IRenderable Renderable => _renderable;
        private IRenderable _zPrev;
        public IRenderable ZPrev => _zPrev;
        private IRenderable _zNext;
        public IRenderable ZNext => _zNext;
        private IRenderable _parent;
        public IRenderable Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        public ElementsLinkedList<IRenderable> Children => _children;

        public RenderPipelineInfo(IRenderable renderable)
        {
            this._renderable = renderable;
        }

        public void AddChild(IRenderable child)
        {
            if (!this.Children.Contains(child))
            {
                this.Children.Add(child);
                child.RenderPipelineInfo.SetParent(Renderable);
            }
        }
        public void SetParent(IRenderable parent)
        {
            this.Parent = parent;
            if (this.Parent != null)
            {
                if (!this.Parent.Children.Contains(Renderable))
                {
                    this.Parent.Children.Add(Renderable);
                }
            }
        }
    }

    public interface IRenderable : IElement
    {
        #region Render Pipeline Info

        public RenderPipelineInfo RenderPipelineInfo { get; }
        //TODO: GUID Lookup in DataModel Instance
        //public IRenderable ZPrev { get; }
        //public IRenderable ZNext { get; }
        public IRenderable Parent { get => RenderPipelineInfo.Parent; }
        public ElementsLinkedList<IRenderable> Children { get => RenderPipelineInfo.Children; }

        #endregion

        #region Properties

#nullable enable
        public Type? ViewType { get; }
#nullable restore
        object ViewKey { get; set; }
        IRenderView RenderView { get; set; }

        public bool Visible { get; set; }

        #endregion

        #region BoundingBox

        /// <summary>
        /// Bounding Box of the Element
        /// </summary>
        public abstract BoundingBox BoundingBox { get; }

        /// <summary>
        /// The X coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        double X { get; }
        /// <summary>
        /// Set the X coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetX(double x)
        {
            BoundingBox.Location.X = x;
            OnPropertyChanged("X");
        }

        /// <summary>
        /// The Y coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        double Y { get; }
        /// <summary>
        /// Set the Y coordinate of the location of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetY(double x)
        {
            BoundingBox.Location.Y = x;
            OnPropertyChanged("Y");
        }

        /// <summary>
        /// The width of the element Bounding Box (in content coordinates).
        /// </summary>
        double Width { get; set; }
        /// <summary>
        /// Set the width of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetWidth(double x) { BoundingBox.Size.Width = x; OnPropertyChanged("Width"); }

        /// <summary>
        /// The height of the element Bounding Box (in content coordinates).
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Set the height of the element Bounding Box (in content coordinates).
        /// </summary>
        void SetHeight(double x) { BoundingBox.Size.Height = x; OnPropertyChanged("Height"); }

        #endregion
        
        void Render()
        {
            if (RenderView != null)
                RenderView.Render();
        }
    }

    public interface IRenderView
    {
        abstract IRenderable Element { get; }
        void Render();
    }

    #endregion

    #region Computables

    public interface IComputable : IElement
    {
        public ElementsLinkedList<IComputable> DataDS { get; }
        public ElementsLinkedList<IComputable> DataUS { get; }
        public ElementsLinkedList<IComputable> EventDS { get; }
        public ElementsLinkedList<IComputable> EventUS { get; }

        public ElementsLinkedList<INode> Nodes { get; }
        public ComputableElementState ComputableElementState { get; set; }
        public ElementConsole Console { get; }
        public bool Enabled { get; set; }
        void ClearData();
        void CollectData()
        {
            //TODO: Populate DataDS and DataUS and Collect data from nodes
        }
        void ComputeData();
    }

    #endregion

    #region Nodes and Connections

    public interface INode : IElement
    {
        public IElement Parent { get; }
        public ElementsLinkedList<IConnection> Connections { get; }
        public NodeType NodeType { get; }
        public CanvasPoint Hotspot { get; }
        public double HotspotThresholdRadius { get; }
        public new ElementType ElementType { get => ElementType.Node; }

    }

    public interface IConnection : IElement
    {
        public INode Origin { get; }
        public INode Destination { get; }
        public ConnectionType ConnectionType { get; }
        public new ElementType ElementType { get => ElementType.Connection; }
    }

    #endregion

    #region Enums

    public enum ElementState
    {
        /// <summary>
        /// No state.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default state.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Hidden state.
        /// </summary>
        Hidden = 1,
        /// <summary>
        /// Disabled state.
        /// </summary>
        Disabled = 2
    }
    public enum ComputableElementState
    {
        /// <summary>
        /// No state.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default state.
        /// </summary>
        Default = 0,
        Computing = 1,
        Computed = 2,
        Failed = 3
    }
    public enum ElementType
    {
        /// <summary>
        /// No type.
        /// </summary>
        Unset = -1,
        /// <summary>
        /// Default type.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Default type.
        /// </summary>
        Connection = 1,
        /// <summary>
        /// Default type.
        /// </summary>
        Node = 2
    }
    public enum NodeType
    {
        Unset = -1,
        Default = 0,
        Input = 1,
        Output = 2
    }
    public enum ConnectionType
    {
        Unset = -1,
        Default = 0,
        Data = 1,
        Event = 2
    }

    #endregion
}
