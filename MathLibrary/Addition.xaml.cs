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
    /// Visual Interaction logic for Addition.xaml
    /// </summary>
    public partial class AdditionView : UserControl, IBaseCompView<Addition>
    {
        #region IBaseElementView Members

        private Addition? _element;
        public Addition Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as Addition;
                }
                //TODO: Log to Console if this.Element is still null
#pragma warning disable CS8603 // Possible null reference return.
                return _element;
#pragma warning restore CS8603 // Possible null reference return.
            }
            private set
            {
                _element = value as Addition;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion


        #region Constructor and Render


        //TODO: Log to Console if this.Element is still null
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AdditionView()
        {
            if (this.DataContext is Addition) this.Element = (Addition)this.DataContext;
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

        //private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        //{
            //this.Element._sliderValue = sliderBlock.Value;
            //if (nodeBlock.Connections != null)
            //{
            //    if (nodeBlock.Connections.Count > 0)
            //    {
            //        IConnection c = nodeBlock.Connections[0];
            //        if (c.Origin == nodeBlock)
            //        {
            //            if (c.Destination != MousePositionNode.Instance && c.Destination != null)
            //            {
            //                if (c.Destination.Parent is Add)
            //                {
            //                    Add te = (Add)c.Destination.Parent;
            //                    if (te != null)
            //                    {
            //                        te._sliderValue = sliderBlock.Value;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        //}

        #endregion


        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            RenderPipeline.Render();
            ComputationPipeline.Compute();
        }

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            AdditionView rectangle = (AdditionView)sender;
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

            AdditionView rectangle = (AdditionView)sender;
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

    public class Addition : BaseComp
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

        public override Type ViewType => typeof(AdditionView);

        #endregion

        #region Constructors

        public Addition() : base()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public Addition(int x, int y, int width, int height) : base()
        {
            base.boundingBox = new BoundingBox(x, y, width, height);

            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
        }

        #endregion

        public override void Compute()
        {
            double a = this.ComputationPipelineInfo.IOManager.GetData<double>(0);
            if (a == default) return;
            double b = this.ComputationPipelineInfo.IOManager.GetData<double>(1);
            if (b == default) return;
            this.ComputationPipelineInfo.IOManager.SetData<double>((a + b), 0);
            //if (this.ComputationPipelineInfo.IOManager.DataInputNodes != null && this.ComputationPipelineInfo.IOManager.DataInputNodes.Count > 1/* && this.Nodes[0] is NodeElement*/)
            //{
            //    double sum = 0.0;
            //    foreach (NodeElement n in this.ComputationPipelineInfo.IOManager.DataInputNodes)
            //    {
            //        if (n != null) sum += n.DataGoo.Data;
            //    }
            //    if (this.ComputationPipelineInfo.IOManager.DataOutputNodes[0] is NodeElement)
            //        ((NodeElement)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data = sum;
            //}
        }

        public override CompInfo GetCompInfo()
        {
            CompInfo ci = new CompInfo();
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            ci.ConstructorInfo = this.GetType().GetConstructor(types);
            ci.ConstructorParams = types;
            ci.Name = "Addition";
            ci.Group = "Operations";
            ci.Tab = "Math";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal NodeElement nodeBlock;
        internal NodeElement nodeBlock1;
        internal NodeElement nodeBlock2;
        public override void Initialize()
        {
            if (this.Children.Count > 0)
            {
                textBlock.DisplayedText = this.ElementText;
                return;
            }

            nodeBlock = new NodeElement(this, NodeType.Input);
            DataTemplateManager.RegisterDataTemplate(nodeBlock);
            this.RenderPipelineInfo.AddChild(nodeBlock);
            this.ComputationPipelineInfo.IOManager.AddDataInputNode<double>(nodeBlock as IDataNode<double>);
            //Subscribe to NodeElement PropertyChanged Event
            //nodeBlock.PropertyChanged += NodeBlock_PropertyChanged;

            nodeBlock1 = new NodeElement(this, NodeType.Input);
            DataTemplateManager.RegisterDataTemplate(nodeBlock1);
            this.RenderPipelineInfo.AddChild(nodeBlock1);
            this.ComputationPipelineInfo.IOManager.AddDataInputNode<double>(nodeBlock1 as IDataNode<double>);

            nodeBlock2 = new NodeElement(this, NodeType.Output);
            DataTemplateManager.RegisterDataTemplate(nodeBlock2);
            this.RenderPipelineInfo.AddChild(nodeBlock2);
            this.ComputationPipelineInfo.IOManager.AddDataOutputNode<double>(nodeBlock2 as IDataNode<double>);

            string? txt = this.ElementText;
            textBlock = new TextElement();
            textBlock.DisplayedText = txt;
            textBlock.TextAlignment = TextAlignment.Left;
            DataTemplateManager.RegisterDataTemplate(textBlock);
            this.RenderPipelineInfo.AddChild(textBlock);
        }
    }
}