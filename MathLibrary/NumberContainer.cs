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
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = Math.Round(_sliderValue.GetValueOrDefault(), 3).ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return  $"Value: {dataIN}";
            }
        }
        
        #region Properties


        #endregion

        #region Constructors

        public NumberContainer() : base(0, 0)
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
                textBlock.DisplayedText = this.ElementText;
            }
        }
        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Number Slider",
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
        internal SliderElement sliderBlock = new SliderElement();
        internal NumberDataNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1, "Changed");

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            //nodeBlock.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Number");

            sliderBlock = new SliderElement();
            sliderBlock.Minimum = -100;
            sliderBlock.Maximum = 100;
            sliderBlock.Value = 50;
            sliderBlock.TickFrequency = 1;
            sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            sliderBlock.Width = 200;
            this.ChildElementManager.AddElement(sliderBlock);

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._sliderValue = sliderBlock.Value;
            ComputationCore.Compute(this);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_sliderValue)));
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}
