using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class DateTimeNow : BaseComp
    {

       

        #region Constructors

        public DateTimeNow() : base(0, 0)
        {
        }

        public DateTimeNow(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "DateTime Now", "Basic UI", "DateTime");

        public override void Compute()
        {
            //ComputationCore.Compute(this);
        }

   
        private GenericEventNode nodeBlock;
        private DateTimeDataNode nodeBlock6;
        public override void Initialize()
        {
            nodeBlock = new GenericEventNode(this, NodeType.Input);
            nodeBlock.NodeEvent += NodeBlock_NodeEvent;
            this.ChildElementManager.AddEventInputNode(nodeBlock, "Refresh");

            nodeBlock6 = new DateTimeDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "Now", true);

 
        }

        private void NodeBlock_NodeEvent(IEventNode container, EventArgData e)
        {
            //Compute();
            //TODO: BUG: FIGURE OUT WHY THIS DOESN'T WORK VVVVVVVVVVV
            //ComputationCore.Compute(this);
            DateTime dtOut = DateTime.Now;
            this.ChildElementManager.SetData(dtOut, nodeBlock6);

            ComputationCore.Compute(this, false);
        }
    }
}
