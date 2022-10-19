using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class ConstructDateTime : BaseComp
    {

        

        #region Constructors

        public ConstructDateTime() : base(0, 0)
        {
        }

        public ConstructDateTime(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Construct DateTime", "Primitives", "DateTime");

        public override void Compute()
        {
            int yr = (int)this.ChildElementManager.GetData(nodeBlock, DateTime.Now.Year);
            int mnt = (int)this.ChildElementManager.GetData(nodeBlock1, DateTime.Now.Month);
            int dat = (int)this.ChildElementManager.GetData(nodeBlock2, DateTime.Now.Day);
            int hr = (int)this.ChildElementManager.GetData(nodeBlock3, DateTime.Now.Hour);
            int min = (int)this.ChildElementManager.GetData(nodeBlock4, DateTime.Now.Minute);
            int sec = (int)this.ChildElementManager.GetData(nodeBlock5, DateTime.Now.Second);
            DateTime dateTime = new DateTime(yr, mnt, dat, hr, min, sec);
            this.ChildElementManager.SetData(dateTime, nodeBlock6);
            
        }
        
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private NumberDataNode nodeBlock5;
        private DateTimeDataNode nodeBlock6;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            //nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Year");

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
            this.ChildElementManager.AddDataOutputNode(nodeBlock6, "DateTime", true);

            //nodeBlock7 = new DateTimeDataNode(this, NodeType.Output);
            ////nodeBlock7.Width = 50;
            //this.ChildElementManager.AddDataOutputNode(nodeBlock7, "Now");
        }
    }
}
