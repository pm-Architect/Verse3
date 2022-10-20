using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class ReverseData : BaseComp
    {
        public ReverseData() : base()
        {
        }
        public ReverseData(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private GenericDataNode Result;
 

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Result = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Reversed Data", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Reverse Data", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            DataStructure result = new DataStructure();

            for (int i = input.Count; i > 0; i--)
            {
                result.Add(input[i-1]);
            }
            this.ChildElementManager.SetData(result, Result);


            this.previewTextBlock.DisplayedText = $"Reversed data = {result}";
        }
    }
}
