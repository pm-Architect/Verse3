using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class CompareDateTime : BaseComp
    {

        #region Constructors

        public CompareDateTime() : base(0, 0)
        {
        }

        public CompareDateTime(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Compare DateTime", "Primitives", "DateTime");

        public override void Compute()
        {

            DateTime dateTime = (DateTime)this.ChildElementManager.GetData(nodeBlock0, DateTime.Now);
            DateTime dateTime2 = (DateTime)this.ChildElementManager.GetData(nodeBlock1, DateTime.Now);


            this.ChildElementManager.SetData(dateTime > dateTime2, nodeBlock2);
            this.ChildElementManager.SetData(dateTime >= dateTime2, nodeBlock3);
            this.ChildElementManager.SetData(dateTime == dateTime2, nodeBlock4);
            this.ChildElementManager.SetData(dateTime <= dateTime2, nodeBlock5);
            this.ChildElementManager.SetData(dateTime < dateTime2, nodeBlock6);

        }

        private DateTimeDataNode nodeBlock0;
        private DateTimeDataNode nodeBlock1;

        private BooleanDataNode nodeBlock2;
        private BooleanDataNode nodeBlock3;
        private BooleanDataNode nodeBlock4;
        private BooleanDataNode nodeBlock5;
        private BooleanDataNode nodeBlock6;


        public override void Initialize()
        {
            nodeBlock0 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock0, "DateTime A");

            nodeBlock1 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "DateTime B");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Greater Than/ Equal To", true);

            nodeBlock3 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Greater Than");

            nodeBlock4 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Equal To");

            nodeBlock5 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock5, "Less Than/ Equal To");

            nodeBlock6 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "Less Than");

        }
    }
}
