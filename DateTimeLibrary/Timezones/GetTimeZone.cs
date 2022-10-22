using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class GetTimeZone : BaseComp
    {

        #region Constructors

        public GetTimeZone() : base()
        {
        }

        public GetTimeZone(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Get TimeZone", "Timezones", "DateTime");

        public override void Compute()
        {

            string timezoneID = this.ChildElementManager.GetData(nodeBlock1, TimeZoneInfo.Local.Id);
            TimeZoneInfo timezoneTarget = TimeZoneInfo.FindSystemTimeZoneById(timezoneID);

            this.ChildElementManager.SetData(timezoneTarget, nodeBlock2);
            this.ChildElementManager.SetData(timezoneTarget.DisplayName, nodeBlock3);

        }

        private TextDataNode nodeBlock1;
        private TimeZoneDataNode nodeBlock2;
        private TextDataNode nodeBlock3;

        public override void Initialize()
        {
            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Timezone ID");

            nodeBlock2 = new TimeZoneDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Timezone");

            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Timezone Display Name", true);
        }
    }
}
