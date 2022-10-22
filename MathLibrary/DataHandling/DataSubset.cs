using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class DataSubset : BaseComp
    {
        public DataSubset() : base()
        {
        }
        public DataSubset(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private NumberDataNode StartIndex;
        private NumberDataNode EndIndex;
        private GenericDataNode Result;
 

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            StartIndex = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(StartIndex, "Start Index");

            EndIndex = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(EndIndex, "End Index");

            Result = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Data Subset", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Data Subset", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            int start = (int)this.ChildElementManager.GetData(StartIndex, 0);
            int end = (int)this.ChildElementManager.GetData(EndIndex, 1);

            DataStructure result = new DataStructure();


            if (input != null)
            {
                if (start < 0)
                {
                    start = 0;
                }
                if (end > input.Count)
                {
                    end = input.Count;
                }

                for (int i = start; i < end; i++)
                {
                    result.Add(input[i]);
                }
            }
            
            this.ChildElementManager.SetData(result, Result);


            this.previewTextBlock.DisplayedText = $"Data Subset = {result}";
        }
    }
}
