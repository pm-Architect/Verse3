using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class SumData : BaseComp
    {
        public SumData() : base()
        {
        }
        public SumData(int x, int y) : base(x, y)
        {
        }
        
        private NumberDataNode DataStructureNode;
  
        private NumberDataNode Result;
        private NumberDataNode Averaged;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");


            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Sum of Data", true);

            Averaged = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Averaged, "Average");
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Sum & Average", "Series", "Data");
        
        public override void Compute()
        {
            DataStructure result = this.ChildElementManager.GetData(DataStructureNode);
            double sum = 0;

            for (int i = 0; i < result.Count; i++)
            {
                sum += (double)result[i].Data;
            }
            this.ChildElementManager.SetData(sum, Result);
            this.ChildElementManager.SetData(sum/result.Count, Averaged);

            this.previewTextBlock.DisplayedText = $"Sum of data = {sum}";
        }
    }
}
