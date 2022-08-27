using System;
using static Core.Geometry2D;

namespace Core
{
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

        //TODO: Output int
        public static bool RenderRenderable(IRenderable renderable, double xOffset, double yOffset, bool recursive = true)
        {
            //if (renderable.RenderableState == RenderableState.Rendering) return -1;
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
                                renderSuccess = renderSuccess && RenderPipeline.RenderRenderable(child, xOffset, yOffset);
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

        //TODO: Output int
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

    public class RenderPipelineInfo
    {
        private IRenderable _renderable;
        public IRenderable Renderable => _renderable;
        //private IRenderable _zPrev;
        //public IRenderable ZPrev => _zPrev;
        //private IRenderable _zNext;
        //public IRenderable ZNext => _zNext;
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
        public void SetParent(IElement parent)
        {
            if (parent is IRenderable)
            {
                this.SetParent(parent as IRenderable);
            }
            else throw new Exception("Parent is not a renderable");
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
}
