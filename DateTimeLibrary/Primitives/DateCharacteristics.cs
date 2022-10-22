using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class DateCharacteristics : BaseComp
    {

        #region Constructors

        public DateCharacteristics() : base()
        {
        }

        public DateCharacteristics(int x, int y) : base(x, y)
        {
        }

        #endregion


        public override CompInfo GetCompInfo() => new CompInfo(this, "Date Characteristics", "Primitives", "DateTime");

        public override void Compute()
        {

            DateTime dateTime = (DateTime)this.ChildElementManager.GetData(nodeBlock1, DateTime.Now);
            
            this.ChildElementManager.SetData(DateTime.IsLeapYear(dateTime.Year), nodeBlock2);
            this.ChildElementManager.SetData(dateTime.DayOfYear, nodeBlock3);
            this.ChildElementManager.SetData(DateTime.DaysInMonth(dateTime.Year, dateTime.Month), nodeBlock4);
            this.ChildElementManager.SetData(dateTime.DayOfWeek.ToString(), nodeBlock5);

        }

  
        private DateTimeDataNode nodeBlock1;

        private BooleanDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        private NumberDataNode nodeBlock4;
        private TextDataNode nodeBlock5;



        public override void Initialize()
        {
            nodeBlock1 = new DateTimeDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "DateTime A");


            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Is Leap Year");

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Day of the Year", true);

            nodeBlock4 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock4, "Days in Month");

            nodeBlock5 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock5, "Weekday");

        }
    }
}
