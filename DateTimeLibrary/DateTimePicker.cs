using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class DateTimePicker : BaseComp
    {
        internal DateTime? _value = DateTime.Now;
        //private double _inputValue = 0.0;


        
        #region Constructors

        public DateTimePicker() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public DateTimePicker(int x, int y) : base(x, y)
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


        public override CompInfo GetCompInfo() => new CompInfo(this, "DateTime Picker", "Basic UI", "DateTime");

        
        internal DateTimeElement dateTimeElement = new DateTimeElement();
        internal DateTimeDataNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1, "Changed");

            nodeBlock = new DateTimeDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "DateTime");
            
            dateTimeElement = new DateTimeElement();
            dateTimeElement.Value = _value;
            dateTimeElement.DisplayedText = dateTimeElement.Value.ToString();
            dateTimeElement.DateTimeChanged += DateTimeElement_DateTimeChanged;
            dateTimeElement.Width = 200;
            this.ChildElementManager.AddElement(dateTimeElement);
        }

        public override void Compute()
        {
            //_value = toggleBlock.Value;
            if (_value.HasValue)
            {
                this.ChildElementManager.SetData(_value.Value, nodeBlock);
                dateTimeElement.DisplayedText = dateTimeElement.Value.ToString();
            }
        }

        private void DateTimeElement_DateTimeChanged(object? sender, RoutedEventArgs e)
        {
            _value = dateTimeElement.Value;
            ComputationCore.Compute(this);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_value)));
        }
    }
}
