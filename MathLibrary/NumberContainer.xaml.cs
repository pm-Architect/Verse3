using Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;
using static Core.Geometry2D;
using TextElement = Verse3.VanillaElements.TextElement;

namespace MathLibrary
{
    /// <summary>
    /// Visual Interaction logic for NumberContainer.xaml
    /// </summary>
    public partial class NumberContainerView : UserControl, IBaseCompView<NumberContainer>
    {
        #region IBaseElementView Members

        private NumberContainer? _element;
        public NumberContainer Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as NumberContainer;
                }
                //TODO: Log to Console if this.Element is still null
#pragma warning disable CS8603 // Possible null reference return.
                return _element;
#pragma warning restore CS8603 // Possible null reference return.
            }
            private set
            {
                _element = value as NumberContainer;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render


        //TODO: Log to Console if this.Element is still null
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public NumberContainerView()
        {
            if (this.DataContext is NumberContainer) this.Element = (NumberContainer)this.DataContext;
            InitializeComponent();
            Render();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Render()
        {
            if (this.Element != null)
            {
                if (this.Element.RenderView != this) this.Element.RenderView = this;
                this.Element.RenderComp();
                
                InputsList.ItemsSource = this.Element.Children;
            }
        }

        #endregion

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            NumberContainerView rectangle = (NumberContainerView)sender;
            IRenderable myRectangle = (IRenderable)rectangle.DataContext;

            //myRectangle.IsSelected = true;

            //mouseButtonDown = e.ChangedButton;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                //
                // When the shift key is held down special zooming logic is executed in content_MouseDown,
                // so don't handle mouse input here.
                //
                return;
            }

            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.None)
            {
                //
                // We are in some other mouse handling mode, don't do anything.
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.DraggingElements;
            DataViewModel.WPFControl.origContentMouseDownPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);

            rectangle.CaptureMouse();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when a mouse button is released over a Rectangle.
        /// </summary>
        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
            {
                //
                // We are not in rectangle dragging mode.
                //
                return;
            }

            DataViewModel.WPFControl.MouseHandlingMode = MouseHandlingMode.None;

            NumberContainerView rectangle = (NumberContainerView)sender;
            rectangle.ReleaseMouseCapture();

            e.Handled = true;
        }

        /// <summary>
        /// Event raised when the mouse cursor is moved when over a Rectangle.
        /// </summary>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            //MouseEventArgs
            if (DataViewModel.WPFControl.MouseHandlingMode != MouseHandlingMode.DraggingElements)
            {
                //
                // We are not in rectangle dragging mode, so don't do anything.
                //
                return;
            }

            Point curContentPoint = e.GetPosition(DataViewModel.WPFControl.ContentElements);
            Vector rectangleDragVector = curContentPoint - DataViewModel.WPFControl.origContentMouseDownPoint;

            //
            // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
            //

            DataViewModel.WPFControl.origContentMouseDownPoint = curContentPoint;

            RenderPipeline.RenderRenderable(this.Element, rectangleDragVector.X, rectangleDragVector.Y);

            DataViewModel.WPFControl.ExpandContent();

            e.Handled = true;
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //MouseWheelEventArgs
        }

        #endregion

        #region UserControlEvents

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //DependencyPropertyChangedEventArgs
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //RoutedEventArgs
            Render();
        }

        #endregion
    }

    public class NumberContainer : BaseComp
    {
        internal double _sliderValue = 0.0;
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((NodeElement)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}" +
                    $"\nOutput Value: {dataIN}";
            }
        }

        #region Properties

        public override Type ViewType => typeof(NumberContainerView);

        #endregion

        #region Constructors

        public NumberContainer() : base()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public NumberContainer(int x, int y, int width = 250, int height = 50) : base()
        {
            base.boundingBox = new BoundingBox(x, y, width, height);

            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            this.ComputationPipelineInfo.IOManager.SetData<double>(_sliderValue, 0);
            //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count == 1)
            //{
            //    if (this.ComputationPipelineInfo.IOManager.DataOutputNodes[0] is NodeElement)
            //        ((NodeElement)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data = _sliderValue;
            //}
        }
        public override CompInfo GetCompInfo()
        {
            CompInfo ci = new CompInfo();
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            ci.ConstructorInfo = this.GetType().GetConstructor(types);
            ci.Name = "Number Slider";
            ci.Group = "Inputs";
            ci.Tab = "Math";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal SliderElement sliderBlock = new SliderElement();
        internal NodeElement nodeBlock;
        public override void Initialize()
        {
            if (this.Children.Count > 0)
            {
                textBlock.DisplayedText = this.ElementText;
                return;
            }

            sliderBlock = new SliderElement();
            sliderBlock.Minimum = 0;
            sliderBlock.Maximum = 100;
            sliderBlock.Value = 50;
            sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            sliderBlock.Width = 200;
            DataTemplateManager.RegisterDataTemplate(sliderBlock);
            this.RenderPipelineInfo.AddChild(sliderBlock);
            
            nodeBlock = new NodeElement(this, NodeType.Output);
            nodeBlock.Width = 50;
            DataTemplateManager.RegisterDataTemplate(nodeBlock);
            this.RenderPipelineInfo.AddChild(nodeBlock);
            this.ComputationPipelineInfo.IOManager.AddDataOutputNode<double>(nodeBlock as IDataNode<double>);
        }
        
        private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._sliderValue = sliderBlock.Value;
            ComputationPipeline.ComputeComputable(this);
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}