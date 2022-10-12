using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class DateTimeNow : BaseComp
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
                    dataIN = (((DateTimeDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data).ToString();
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

        #endregion

        #region Constructors

        public DateTimeNow() : base(0, 0)
        {
        }

        public DateTimeNow(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "DateTime Now",
                Group = "Primitive",
                Tab = "DateTime",
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
            DateTime dtOut = DateTime.Now;
            this.ChildElementManager.SetData<DateTime>(dtOut, 0);
            textBlock.DisplayedText = this.ElementText;
        }
        
        private TextElement textBlock = new TextElement();
        private GenericEventNode nodeBlock;
        private DateTimeDataNode nodeBlock6;
        public override void Initialize()
        {
            nodeBlock = new GenericEventNode(this, NodeType.Input);
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(nodeBlock, "Refresh");

            nodeBlock6 = new DateTimeDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "Now");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            //TODO: BUG: FIGURE OUT WHY THIS DOESN'T WORK VVVVVVVVVVV
            //ComputationPipeline.ComputeComputable(this);
            Compute();
        }
    }
}
