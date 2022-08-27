using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class NumberContainer : BaseComp
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


        #endregion

        #region Constructors

        public NumberContainer() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public NumberContainer(int x, int y, int width = 250, int height = 50) : base(x, y, width, height, CompOrientation.Horizontal)
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
            this.ComputationPipelineInfo.IOManager.SetData<double>(_sliderValue, 0);
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
            ci.Name = "Number Slider";
            ci.Group = "Inputs";
            ci.Tab = "Math";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal SliderElement sliderBlock = new SliderElement();
        internal NumberDataNode nodeBlock;
        public override void Initialize()
        {
            if (this.Children.Count > 0)
            {
                textBlock.DisplayedText = this.ElementText;
                return;
            }

            sliderBlock = new SliderElement();
            sliderBlock.Minimum = 0;
            sliderBlock.Maximum = 100;
            sliderBlock.Value = 50;
            sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            sliderBlock.Width = 200;
            DataTemplateManager.RegisterDataTemplate(sliderBlock);
            this.RenderPipelineInfo.AddChild(sliderBlock);

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            nodeBlock.Width = 50;
            DataTemplateManager.RegisterDataTemplate(nodeBlock);
            this.RenderPipelineInfo.AddChild(nodeBlock);
            this.ComputationPipelineInfo.IOManager.AddDataOutputNode<double>(nodeBlock as IDataNode<double>);
        }

        private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._sliderValue = sliderBlock.Value;
            this.ComputationPipelineInfo.IOManager.SetData<double>(this._sliderValue, 0);
            //ComputationPipeline.ComputeComputable(this);
        }

        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }

    public class NumberDataNode : DataNodeElement<double>
    {
        public NumberDataNode(IRenderable parent, NodeType type = NodeType.Unset) : base(parent, type)
        {
        }
    }
}
