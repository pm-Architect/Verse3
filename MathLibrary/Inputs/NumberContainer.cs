using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class NumberContainer : BaseComp
    {
        internal double? _sliderValue = 0.0;

        #region Constructors

        public NumberContainer() : base()
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public NumberContainer(int x, int y, int width = 250, int height = 300) : base(x, y)
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
            if (_sliderValue.HasValue)
            {
                this.ChildElementManager.SetData<double>(this._sliderValue.Value, 0);
                this.previewTextBlock.DisplayedText = $"Value = {_sliderValue.Value}";
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Number Slider", "Inputs", "Math");

        internal SliderElement sliderBlock = new SliderElement();
        internal NumberDataNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1, "Changed");

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Number");

            sliderBlock = new SliderElement();
            sliderBlock.Minimum = -100;
            sliderBlock.Maximum = 100;
            sliderBlock.Value = 50;
            sliderBlock.TickFrequency = 1;
            sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            this.ChildElementManager.AddElement(sliderBlock);
        }

        private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._sliderValue = sliderBlock.Value;
            ComputationCore.Compute(this, false);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_sliderValue)));
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}
