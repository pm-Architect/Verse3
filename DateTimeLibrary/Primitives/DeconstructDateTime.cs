using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class DeconstructDateTime : BaseComp
    {

        

        #region Constructors

        public DeconstructDateTime() : base()
        {
        }

        public DeconstructDateTime(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Deconstruct DateTime", "Primitives", "DateTime");

        public override void Compute()
        {

            DateTime dateTime = (DateTime)this.ChildElementManager.GetData(nodeBlock6, DateTime.Now);

            this.ChildElementManager.SetData(dateTime.Year, nodeBlock);
            this.ChildElementManager.SetData(dateTime.Month, nodeBlock1);
            this.ChildElementManager.SetData(dateTime.Day, nodeBlock2);
            this.ChildElementManager.SetData(dateTime.Hour, nodeBlock3);
            this.ChildElementManager.SetData(dateTime.Minute, nodeBlock4);
            this.ChildElementManager.SetData(dateTime.Second, nodeBlock5);
            
        }

        private DateTimeDataNode nodeBlock6;
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private NumberDataNode nodeBlock5;
        
        public override void Initialize()
        {
            nodeBlock6 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock6, "DateTime");

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Year");

            nodeBlock1 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock1, "Month");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Day", true);

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Hour");

            nodeBlock4 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Minute");

            nodeBlock5 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock5, "Second");

            //nodeBlock7 = new DateTimeDataNode(this, NodeType.Output);
            ////nodeBlock7.Width = 50;
            //this.ChildElementManager.AddDataOutputNode(nodeBlock7, "Now");
        }
    }
}
