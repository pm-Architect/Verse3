using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class SortData : BaseComp
    {
        public SortData() : base()
        {
        }
        public SortData(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private BooleanDataNode Descending;
        private GenericDataNode Result;
        //private IEnumerable<object> sortedData;

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Descending = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Descending, "Descending");

            Result = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Sorted Data", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Sort Data", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            bool desc = this.ChildElementManager.GetData(Descending, false);

            DataStructure result = MathUtils.SortData(input, desc);
            this.ChildElementManager.SetData(result, Result);


            this.previewTextBlock.DisplayedText = $"Sorted data = {result}";
        }
    }
}
