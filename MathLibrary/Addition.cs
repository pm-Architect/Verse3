using Core;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Addition : BaseComp
    {
        public Addition() : base()
        {
        }
        public Addition(int x, int y) : base(x, y)
        {
        }

        private NumberDataNode A;
        private NumberDataNode B;
        private NumberDataNode Result;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            A = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(A, "A");

            B = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(B, "B");

            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Result", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Addition", "Operations", "Math");

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData(A, 0.0);
            double b = this.ChildElementManager.GetData(B, 0.0);
            this.ChildElementManager.SetData((a + b), Result);
        }
    }
}
