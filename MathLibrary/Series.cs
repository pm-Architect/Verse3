using Core;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Series : BaseComp
    {
        public Series() : base()
        {
        }
        public Series(int x, int y) : base(x, y)
        {
        }
        
        private NumberDataNode Start;
        private NumberDataNode Step;
        private NumberDataNode Count;
        private NumberDataNode Result;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            Start = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Start, "Start");

            Step = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Step, "Step");

            Count = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Count, "Count");

            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "AP", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Arithmetic Progression", "Series", "Data");
        
        public override void Compute()
        {
            double start = this.ChildElementManager.GetData(Start, 0.0);
            double step = this.ChildElementManager.GetData(Step, 1.0);
            int count = (int)this.ChildElementManager.GetData(Count, 10);
            DataStructure<double> result = new DataStructure<double>();
            for (int i = 0; i < count; i++)
            {
                result.Add(start + i * step);
            }
            this.ChildElementManager.SetData(result, Result);
            this.previewTextBlock.DisplayedText = $"Last number = {result.Last.Value.Data.ToString()}";
        }
    }
}
