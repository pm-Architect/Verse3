using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using TextElement = Verse3.VanillaElements.TextElement;

namespace MathLibrary
{
    public class Division : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = (Math.Round((((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data), 2)).ToString();
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

        public Division() : base(0, 0)
        {
        }

        public Division(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0, 0);
            double b = this.ChildElementManager.GetData<double>(1, 1);
            if (b == 0) return;
            this.ChildElementManager.SetData<double>((a / b), 0);
            textBlock.DisplayedText = this.ElementText;
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Division",
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

        private TextElement textBlock = new TextElement();
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");
            
            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
