using Core;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class RangeSelector : BaseComp
    {
        internal double? _valueStart = -100.0;
        internal double? _valueEnd = 100.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _valueStart.ToString() + " to " + _valueEnd.ToString();
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Value: {dataIN}";
            }
        }
        
        #region Constructors

        public RangeSelector() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public RangeSelector(int x, int y, int width = 250, int height = 100) : base(x, y)
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
            //_value = toggleBlock.Value;
            if (_valueStart.HasValue && _valueEnd.HasValue)
            {
                this.ChildElementManager.SetData<double>(_valueStart.Value, 0);
                this.ChildElementManager.SetData<double>(_valueEnd.Value, 1);
                textBlock.DisplayedText = this.ElementText;
            }
        }
        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Range Selector",
                Group = "Inputs",
                Tab = "Math",
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
        internal RangeSliderElement sliderBlock = new RangeSliderElement();
        internal NumberDataNode nodeBlock;
        internal NumberDataNode nodeBlock1;
        internal GenericEventNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock2 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock2, "Changed");

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Start");
            
            nodeBlock1 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock1, "End");

            sliderBlock = new RangeSliderElement();
            sliderBlock.ValuesChanged += SliderBlock_ValuesChanged;
            sliderBlock.Width = 200;
            this.ChildElementManager.AddElement(sliderBlock);
            
            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void SliderBlock_ValuesChanged(object? sender, RoutedEventArgs e)
        {
            _valueStart = sliderBlock.ValueStart;
            _valueEnd = sliderBlock.ValueEnd;
            ComputationCore.Compute(this);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(new double[] { sliderBlock.ValueStart, sliderBlock.ValueEnd })));
        }
    }
}
