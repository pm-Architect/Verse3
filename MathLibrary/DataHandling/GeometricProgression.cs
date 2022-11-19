using Core;
using Verse3;
using Verse3.VanillaElements;
using System;

namespace MathLibrary
{
    public class GeometricProgression : BaseComp
    {
        public GeometricProgression() : base()
        {
        }
        public GeometricProgression(int x, int y) : base(x, y)
        {
        }
        
        private NumberDataNode Start;
        private NumberDataNode Multiplier;
        private NumberDataNode Count;
        private NumberDataNode Result;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            Start = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Start, "Start");

            Multiplier = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Multiplier, "Multiplier");

            Count = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Count, "Count");

            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "GP", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Geometric Progression", "Series", "Data");
        
        public override void Compute()
        {
            double start = this.ChildElementManager.GetData(Start, 0.0);
            double multiplier = this.ChildElementManager.GetData(Multiplier, 1.0);
            int count = (int)Math.Abs(this.ChildElementManager.GetData(Count, 10));
            DataStructure<double> result = new DataStructure<double>();
            for (int i = 0; i < count; i++)
            {
                result.Add(start * Math.Pow(multiplier, i));
            }
            this.ChildElementManager.SetData(result, Result);
            if (count > 0)
            this.previewTextBlock.DisplayedText = $"Last number = {result.Last.Value.Data.ToString()}";
        }
    }
}
