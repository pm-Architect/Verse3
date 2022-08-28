using Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Verse3;
using static Core.Geometry2D;

namespace CanvasElements
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CanvasExtent : UserControl, IRenderView
    {
        public CanvasExtent()
        {
            InitializeComponent();
        }

        public IRenderable Element => throw new System.NotImplementedException();

        public void Render()
        {
            throw new System.NotImplementedException();
        }
    }

    public class CanvasExtentElement : IRenderable
    {
        #region Data Members

        private RenderPipelineInfo renderPipelineInfo;
        protected BoundingBox boundingBox = BoundingBox.Unset;
        private Guid _id = Guid.NewGuid();
        internal IRenderView? elView;

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
        public Type ViewType { get => typeof(CanvasExtent); }
        public object ViewKey { get; set; }

        public Guid ID { get => _id; private set => _id = value; }

        private bool sel = false;
        public bool IsSelected { get => sel; set => sel = false; }

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

        public ElementState ElementState { get; set; }
        public ElementType ElementType { get => ElementType.CanvasElement; set => ElementType = ElementType.CanvasElement; }
        bool IRenderable.Visible { get; set; }

        public IRenderable Parent => RenderPipelineInfo.Parent;
        public ElementsLinkedList<IRenderable> Children => RenderPipelineInfo.Children;

        #endregion

        public CanvasExtentElement() : base()
        {
            renderPipelineInfo = new RenderPipelineInfo(this);
        }

        public CanvasExtentElement(int x, int y, int width = 10, int height = 10) : base()
        {
            renderPipelineInfo = new RenderPipelineInfo(this);
            this.boundingBox = new BoundingBox(x, y, width, height);

            //this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
        }

        public CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Extent",
                Group = "_CanvasElements",
                Tab = "_CanvasElements",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler? PropertyChanged;

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
            GC.SuppressFinalize(this);
        }
        ~CanvasExtentElement() => Dispose();
    }
}