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
            //DataStructure<double> aDS = this.ChildElementManager.GetData(A);
            //if (aDS is null || aDS.Data == default) aDS = new DataStructure<double>(0.0);
            double b = this.ChildElementManager.GetData(B, 0.0);
            //DataStructure<double> bDS = this.ChildElementManager.GetData(B);
            //if (bDS is null || bDS.Data == default) bDS = new DataStructure<double>(0.0);
            this.ChildElementManager.SetData((a + b), Result);
            //if (aDS.Data is null || bDS.Data is null) return;
            //DataStructure<double> result = new DataStructure<double>();
            //if (aDS.Count > 0)
            //{
            //    foreach (IDataGoo<double> goo in aDS)
            //    {
            //        if (bDS.Count > 0)
            //        {
            //            foreach (IDataGoo<double> goo2 in bDS)
            //            {
            //                result.Add(goo.Data + goo2.Data);
            //            }
            //        }
            //        else if (bDS.Data is double b)
            //        {
            //            result.Add(goo.Data + b);
            //        }
            //    }
            //}
            //else if (aDS.Data is double a)
            //{
            //    if (bDS.Count > 0)
            //    {
            //        foreach (IDataGoo<double> goo2 in bDS)
            //        {
            //            result.Add(a + goo2.Data);
            //        }
            //    }
            //    else if (bDS.Data is double b)
            //    {
            //        result.Add(a + b);
            //    }
            //}
        }
    }
}
