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
    /// Visual Interaction logic for Subtraction.xaml
    /// </summary>
    public partial class SubtractionView : UserControl, IBaseCompView<Subtraction>
    {
        #region IBaseElementView Members

        private Subtraction? _element;
        public Subtraction Element
        {
            get
            {
                if (this._element == null)
                {
                    _element = this.DataContext as Subtraction;
                }
                //TODO: Log to Console if this.Element is still null
#pragma warning disable CS8603 // Possible null reference return.
                return _element;
#pragma warning restore CS8603 // Possible null reference return.
            }
            private set
            {
                _element = value as Subtraction;
            }
        }
        IRenderable IRenderView.Element => Element;

        #endregion

        #region Constructor and Render


        //TODO: Log to Console if this.Element is still null
        public SubtractionView()
        {
            if (this.DataContext is Subtraction) this.Element = (Subtraction)this.DataContext;
            InitializeComponent();
            Render();
        }

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

        #region MouseEvents

        /// <summary>
        /// Event raised when a mouse button is clicked down over a Rectangle.
        /// </summary>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //MouseButtonEventArgs
            DataViewModel.WPFControl.ContentElements.Focus();
            Keyboard.Focus(DataViewModel.WPFControl.ContentElements);

            SubtractionView rectangle = (SubtractionView)sender;
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

            SubtractionView rectangle = (SubtractionView)sender;
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


    public class Subtraction : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
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

        public override Type ViewType => typeof(SubtractionView);

        #endregion

        #region Constructors

        public Subtraction() : base()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public Subtraction(int x, int y, int width = 250, int height = 350) : base()
        {
            base.boundingBox = new BoundingBox(x, y, width, height);

            Random rnd = new Random();
            byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            double a = this.ComputationPipelineInfo.IOManager.GetData<double>(0);
            if (a == default) a = 0;
            double b = this.ComputationPipelineInfo.IOManager.GetData<double>(1);
            if (b == default) b = 0;
            this.ComputationPipelineInfo.IOManager.SetData<double>((a - b), 0);
            textBlock.DisplayedText = this.ElementText;
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
            ci.Name = "Subtraction";
            ci.Group = "Operations";
            ci.Tab = "Math";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal NumberDataNode nodeBlock;
        internal NumberDataNode nodeBlock1;
        internal NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            if (this.Children.Count > 0)
            {
                textBlock.DisplayedText = this.ElementText;
                return;
            }

            nodeBlock = new NumberDataNode(this, NodeType.Input);
            DataTemplateManager.RegisterDataTemplate(nodeBlock);
            this.RenderPipelineInfo.AddChild(nodeBlock);
            this.ComputationPipelineInfo.IOManager.AddDataInputNode<double>(nodeBlock as IDataNode<double>);
            //Subscribe to NodeElement PropertyChanged Event
            //nodeBlock.PropertyChanged += NodeBlock_PropertyChanged;

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            DataTemplateManager.RegisterDataTemplate(nodeBlock1);
            this.RenderPipelineInfo.AddChild(nodeBlock1);
            this.ComputationPipelineInfo.IOManager.AddDataInputNode<double>(nodeBlock1 as IDataNode<double>);

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
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