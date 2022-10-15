using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class OrGate : BaseComp
    {

        #region Properties

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = (((BooleanDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data).ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Output Value: {dataIN}";
            }
        }

        #endregion

        #region Constructors

        public OrGate() : base(0, 0)
        {
        }

        public OrGate(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "OR Gate",
                Group = "Gates",
                Tab = "Boolean",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        public override void Compute()
        {
            bool a = this.ChildElementManager.GetData<bool>(0, false);
            bool b = this.ChildElementManager.GetData<bool>(1, false);
            this.ChildElementManager.SetData<bool>((a || b), 0);
            textBlock.DisplayedText = this.ElementText;
        }
        
        private TextElement textBlock = new TextElement();
        private BooleanDataNode nodeBlock;
        private BooleanDataNode nodeBlock1;
        private BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new BooleanDataNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "A");

            nodeBlock1 = new BooleanDataNode(this, NodeType.Input);
            //nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "B");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            //nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
