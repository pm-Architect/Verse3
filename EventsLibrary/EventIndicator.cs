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
        internal string _argstring = "";
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
                    $"Counter: {dataIN}" +
                    $"\nArgs: {_argstring}";
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

        public EventIndicator(int x, int y, int width = 250, int height = 100) : base(x, y)
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
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Event Indicator",
                Group = "Event Utilities",
                Tab = "Events",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal SliderElement sliderBlock = new SliderElement();
        internal GenericEventNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        internal GenericDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock = new GenericEventNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(nodeBlock as IEventNode);

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddEventOutputNode(nodeBlock1 as IEventNode);

            nodeBlock2 = new GenericDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2);

            string? txt = this.ElementText;
            textBlock = new TextElement();
            textBlock.DisplayedText = txt;
            textBlock.TextAlignment = TextAlignment.Center;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            //TODO: FIX EventArgData Null problem
            _counter++;
            nodeBlock1.EventOccured(e);
            if (e.Count > 0)
            {
                _argstring = e[0].Data.ToString();
                object dataOut = e[0].Data;
                this.ChildElementManager.SetData(dataOut, 0);
                textBlock.DisplayedText = this.ElementText;
                ComputationCore.Compute(this);
            }
            textBlock.DisplayedText = this.ElementText;
        }
    }
}
