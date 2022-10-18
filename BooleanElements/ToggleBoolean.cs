using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace EventsLibrary
{
    public class ToggleBoolean : BaseComp
    {
        internal bool? _value = false;
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _value.ToString();
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Value: {dataIN}";
            }
        }

        #region Properties


        #endregion

        #region Constructors

        public ToggleBoolean() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public ToggleBoolean(int x, int y, int width = 250, int height = 100) : base(x, y)
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
            if (_value.HasValue)
            {
                this.ChildElementManager.SetData<bool>(_value.Value, 0);
                toggleBlock.DisplayedText = _value.ToString();
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Toggle Boolean", "Basic UI", "Boolean");

        internal TextElement textBlock = new TextElement();
        internal ToggleElement toggleBlock = new ToggleElement();
        internal GenericEventNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        internal BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            toggleBlock = new ToggleElement();
            toggleBlock.Value = _value;
            toggleBlock.DisplayedText = _value.ToString();
            toggleBlock.ToggleChecked += ButtonBlock_ToggleChecked;
            toggleBlock.ToggleUnchecked += ButtonBlock_ToggleUnchecked;
            toggleBlock.Width = 200;
            this.ChildElementManager.AddElement(toggleBlock);

            nodeBlock = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock as IEventNode, "Checked");

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1 as IEventNode, "Unchecked");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode<bool>(nodeBlock2 as IDataNode<bool>, "Value");
        }

        private void ButtonBlock_ToggleChecked(object? sender, RoutedEventArgs e)
        {
            _value = true;
            ComputationCore.Compute(this, false);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_value)));
        }

        private void ButtonBlock_ToggleUnchecked(object? sender, RoutedEventArgs e)
        {
            _value = false;
            ComputationCore.Compute(this, false);
            this.ChildElementManager.EventOccured(1, new EventArgData(new DataStructure(_value)));
        }
    }
}
