using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class SubtractDateTime : BaseComp
    {

        #region Constructors

        public SubtractDateTime() : base(0, 0)
        {
        }

        public SubtractDateTime(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Subtract DateTime", "Primitives", "DateTime");

        public override void Compute()
        {

            DateTime dateTime = (DateTime)this.ChildElementManager.GetData(nodeBlock0, DateTime.Now);
            DateTime dateTime2 = (DateTime)this.ChildElementManager.GetData(nodeBlock1, DateTime.Now);
            TimeSpan timespan0 = (TimeSpan)this.ChildElementManager.GetData(nodeBlock2);

            TimeSpan timespan = dateTime.Subtract(dateTime2);
            DateTime dateTime3 = dateTime.Subtract(timespan0);

            this.ChildElementManager.SetData(timespan.Days, nodeBlock3);
            this.ChildElementManager.SetData(timespan.Hours, nodeBlock4);
            this.ChildElementManager.SetData(timespan.Minutes, nodeBlock5);
            this.ChildElementManager.SetData(timespan, nodeBlock6);
            this.ChildElementManager.SetData(dateTime3, nodeBlock7);

        }

        private DateTimeDataNode nodeBlock0;
        private DateTimeDataNode nodeBlock1;
        private TimeSpanDataNode nodeBlock2;

        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private NumberDataNode nodeBlock5;
        private TimeSpanDataNode nodeBlock6;
        private DateTimeDataNode nodeBlock7;




        public override void Initialize()
        {
            nodeBlock0 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock0, "DateTime A");

            nodeBlock1 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "DateTime B");

            nodeBlock2 = new TimeSpanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Timespan");

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Days", true);

            nodeBlock4 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Hours");

            nodeBlock5 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock5, "Minutes");

            nodeBlock6 = new TimeSpanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "TimeSpan");

            nodeBlock7 = new DateTimeDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock7, "New Date");

        }
    }
}
