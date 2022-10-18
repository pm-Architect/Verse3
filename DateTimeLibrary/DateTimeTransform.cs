using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class DateTimeTransform : BaseComp
    {

        #region Constructors

        public DateTimeTransform() : base(0, 0)
        {
        }

        public DateTimeTransform(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "DateTime transform", "Operations", "DateTime");

        public override void Compute()
        {
            
            DateTime dateTime = this.ChildElementManager.GetData(nodeBlock, DateTime.Now);
            int yr = (int)this.ChildElementManager.GetData(nodeBlock0, 0);
            int mnt = (int)this.ChildElementManager.GetData(nodeBlock1, 0);
            int dat = (int)this.ChildElementManager.GetData(nodeBlock2, 0);
            int hr = (int)this.ChildElementManager.GetData(nodeBlock3, 0);
            int min = (int)this.ChildElementManager.GetData(nodeBlock4, 0);
            int sec = (int)this.ChildElementManager.GetData(nodeBlock5, 0);
            DateTime newdateTime = dateTime.AddYears(yr);
            newdateTime = newdateTime.AddMonths(mnt);
            newdateTime = newdateTime.AddDays(dat);
            newdateTime = newdateTime.AddHours(hr);
            newdateTime = newdateTime.AddMinutes(min);
            newdateTime = newdateTime.AddSeconds(sec);

            this.ChildElementManager.SetData(newdateTime, nodeBlock6);

        }

        private DateTimeDataNode nodeBlock;
        private NumberDataNode nodeBlock0;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private NumberDataNode nodeBlock5;
        private DateTimeDataNode nodeBlock6;

        public override void Initialize()
        {
            nodeBlock = new DateTimeDataNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "DateTime");

            nodeBlock0 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock0.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock0, "Year");

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Month");

            nodeBlock2 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Day");

            nodeBlock3 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock3.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock3, "Hour");

            nodeBlock4 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock4.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock4, "Minute");

            nodeBlock5 = new NumberDataNode(this, NodeType.Input);
            //nodeBlock5.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock5, "Second");

            nodeBlock6 = new DateTimeDataNode(this, NodeType.Output);
            //nodeBlock6.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "New DateTime", true);

        }
    }
}
