using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class TimeZoneProperties : BaseComp
    {

        #region Constructors

        public TimeZoneProperties() : base(0, 0)
        {
        }

        public TimeZoneProperties(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "TimeZone Properties", "Timezones", "DateTime");

        public override void Compute()
        {

            TimeZoneInfo timezone = this.ChildElementManager.GetData(nodeBlock, TimeZoneInfo.Local);

            this.ChildElementManager.SetData(timezone.Id, nodeBlock1);
            this.ChildElementManager.SetData(timezone.DisplayName, nodeBlock2);
            this.ChildElementManager.SetData(timezone.StandardName, nodeBlock3);
            this.ChildElementManager.SetData(timezone.DaylightName, nodeBlock4);

        }

        private TimeZoneDataNode nodeBlock;
        private TextDataNode nodeBlock1;
        private TextDataNode nodeBlock2;
        private TextDataNode nodeBlock3;
        private TextDataNode nodeBlock4;


        public override void Initialize()
        {
            nodeBlock = new TimeZoneDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "TimeZone");

            nodeBlock1 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock1, "ID");

            nodeBlock2 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Display Name", true);

            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Standard Name");

            nodeBlock4 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Daylight Name");

        }
    }
}
