using Core;
using Verse3;
using Verse3.VanillaElements;
using System;

namespace MathLibrary
{
    public class FibonacciSequence : BaseComp
    {
        public FibonacciSequence() : base()
        {
        }
        public FibonacciSequence(int x, int y) : base(x, y)
        {
        }
        
       
        private NumberDataNode Count;
        private NumberDataNode Result;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            Count = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Count, "Count");

            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Fibonacci Sequence", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Fibonacci Sequence", "Series", "Data");
        
        public override void Compute()
        {
            int count = (int)Math.Abs(this.ChildElementManager.GetData(Count, 10));

            DataStructure<double> result = new DataStructure<double>();
            result = MathUtils.FibonacciSequence(count);

            this.ChildElementManager.SetData(result, Result);
            if (count > 0)
            this.previewTextBlock.DisplayedText = $"Last number = {result.Last.Value.Data.ToString()}";
        }
    }
}
