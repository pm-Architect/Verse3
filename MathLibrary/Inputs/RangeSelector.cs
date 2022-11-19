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
        
        #region Constructors

        public RangeSelector() : base()
        {
        }

        public RangeSelector(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            if (_valueStart.HasValue && _valueEnd.HasValue)
            {
                this.ChildElementManager.SetData<double>(_valueStart.Value, nodeBlock);
                this.ChildElementManager.SetData<double>(_valueEnd.Value, nodeBlock1);
                this.previewTextBlock.DisplayedText = $"Range = {_valueStart.Value} to {_valueEnd.Value}";
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Range Selector", "Inputs", "Math");
        
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
            sliderBlock.TickFrequency = 1;
            if (_valueStart.HasValue) sliderBlock.ValueStart = _valueStart.Value;
            else sliderBlock.ValueStart = 0;
            if (_valueEnd.HasValue) sliderBlock.ValueEnd = _valueEnd.Value;
            else sliderBlock.ValueEnd = 100;
            this.ChildElementManager.AddElement(sliderBlock);
        }

        private void SliderBlock_ValuesChanged(object? sender, RoutedEventArgs e)
        {
            _valueStart = sliderBlock.ValueStart;
            _valueEnd = sliderBlock.ValueEnd;
            ComputationCore.Compute(this, false);
            this.ChildElementManager.EventOccured(nodeBlock2, new EventArgData(new DataStructure<double>(new double[] { sliderBlock.ValueStart, sliderBlock.ValueEnd })));
        }
    }
}
