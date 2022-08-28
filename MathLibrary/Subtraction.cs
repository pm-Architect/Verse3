using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
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


        #endregion

        #region Constructors

        public Subtraction() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public Subtraction(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
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
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Subtraction",
                Group = "Operations",
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
