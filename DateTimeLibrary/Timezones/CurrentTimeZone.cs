using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class CurrentTimeZone : BaseComp
    {

        #region Constructors

        public CurrentTimeZone() : base()
        {
        }

        public CurrentTimeZone(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Current TimeZone", "Timezones", "DateTime");

        public override void Compute()
        {

            TimeZoneInfo localZone = TimeZoneInfo.Local;
            TimeZoneInfo utcZone = TimeZoneInfo.Utc;

            this.ChildElementManager.SetData(localZone, nodeBlock1);
            this.ChildElementManager.SetData(utcZone, nodeBlock2);

        }

  
        private TimeZoneDataNode nodeBlock1;
        private TimeZoneDataNode nodeBlock2;


        public override void Initialize()
        {
            nodeBlock1 = new TimeZoneDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock1, "Local Time", true);


            nodeBlock2 = new TimeZoneDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "UTC Time");

        }
    }
}
