using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class GraftData : BaseComp
    {
        public GraftData() : base()
        {
        }
        public GraftData(int x, int y) : base(x, y)
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
            this.ChildElementManager.AddDataOutputNode(Result, "Grafted Data", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Graft Data", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            DataStructure result = new DataStructure();

            for (int i = 0; i < input.Count; i++)
            {
                DataStructure node = new DataStructure();
                result.Add(node);
                node.Add(input[i]);
            }
            this.ChildElementManager.SetData(result, Result);


            this.previewTextBlock.DisplayedText = $"Grafted data = {result}";
        }
    }
}
