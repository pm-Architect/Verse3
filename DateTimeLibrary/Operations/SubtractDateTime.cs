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

        public SubtractDateTime() : base()
        {
        }

        public SubtractDateTime(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Subtract DateTime", "Operations", "DateTime");

        public override void Compute()
        {

            DateTime dateTime = (DateTime)this.ChildElementManager.GetData(a, DateTime.Now);
            DateTime dateTime2 = (DateTime)this.ChildElementManager.GetData(b, DateTime.Now);

            TimeSpan timespan = dateTime2.Subtract(dateTime);

            this.ChildElementManager.SetData(timespan, timeSpan);
            this.ChildElementManager.SetData((double)timespan.Days, days);
            this.ChildElementManager.SetData((double)timespan.Hours, hours);
            this.ChildElementManager.SetData((double)timespan.Minutes, mins);
            this.ChildElementManager.SetData((double)timespan.Seconds, secs);
            this.ChildElementManager.SetData((double)timespan.Milliseconds, msecs);

        }

        private DateTimeDataNode a;
        private DateTimeDataNode b;

        private TimeSpanDataNode timeSpan;
        private NumberDataNode days;
        private NumberDataNode hours;
        private NumberDataNode mins;
        private NumberDataNode secs;
        private NumberDataNode msecs;




        public override void Initialize()
        {
            a = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(a, "DateTime A");

            b = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(b, "DateTime B");

            timeSpan = new TimeSpanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(timeSpan, "TimeSpan", true);

            days = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(days, "Days");

            hours = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(hours, "Hours");

            mins = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(mins, "Minutes");

            secs = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(secs, "Seconds");

            msecs = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(msecs, "Milliseconds");
        }
    }
}
