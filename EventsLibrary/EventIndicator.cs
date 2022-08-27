using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class EventIndicator : BaseComp
    {
        internal int _counter = 0;
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _counter.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return /*$"Name: {name}" +*/
                    //$"\nView: {viewname}" +
                    //$"\nID: {this.ID}" +
                    //$"\nX: {this.X}" +
                    //$"\nY: {this.Y}" +
                    $"\nOutput Value: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public EventIndicator() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public EventIndicator(int x, int y, int width = 250, int height = 50) : base(x, y, width, height, CompOrientation.Horizontal)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            //this.ComputationPipelineInfo.IOManager.SetData<double>(_sliderValue, 0);
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
            ci.Name = "Event Indicator";
            ci.Group = "Event Utilities";
            ci.Tab = "Events";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal SliderElement sliderBlock = new SliderElement();
        internal ButtonClickedEventNode nodeBlock;
        internal ButtonClickedEventNode nodeBlock1;
        public override void Initialize()
        {
            //if (this.Children.Count > 0)
            //{
            //    textBlock.DisplayedText = this.ElementText;
            //    return;
            //}

            //sliderBlock = new SliderElement();
            //sliderBlock.Minimum = 0;
            //sliderBlock.Maximum = 100;
            //sliderBlock.Value = 50;
            //sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            //sliderBlock.Width = 200;
            //DataTemplateManager.RegisterDataTemplate(sliderBlock);
            //this.RenderPipelineInfo.AddChild(sliderBlock);

            //var buttonBlock = new ButtonElement();
            //buttonBlock.DisplayedText = "Trigger";
            //buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            //DataTemplateManager.RegisterDataTemplate(buttonBlock);
            //this.RenderPipelineInfo.AddChild(buttonBlock);

            nodeBlock = new ButtonClickedEventNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            DataTemplateManager.RegisterDataTemplate(nodeBlock);
            this.RenderPipelineInfo.AddChild(nodeBlock);
            this.ComputationPipelineInfo.IOManager.AddEventInputNode(nodeBlock as IEventNode);

            string? txt = this.ElementText;
            textBlock = new TextElement();
            textBlock.DisplayedText = txt;
            textBlock.TextAlignment = TextAlignment.Left;
            DataTemplateManager.RegisterDataTemplate(textBlock);
            this.RenderPipelineInfo.AddChild(textBlock);

            nodeBlock1 = new ButtonClickedEventNode(this, NodeType.Output);
            nodeBlock1.Width = 50;
            DataTemplateManager.RegisterDataTemplate(nodeBlock1);
            this.RenderPipelineInfo.AddChild(nodeBlock1);
            this.ComputationPipelineInfo.IOManager.AddEventOutputNode(nodeBlock1 as IEventNode);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            _counter++;
            nodeBlock1.EventOccured(e);
            textBlock.DisplayedText = this.ElementText;
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}
