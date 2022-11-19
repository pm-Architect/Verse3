using Core;
using MS.WindowsAPICodePack.Internal;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class GetRange : BaseComp
    {
        public GetRange() : base()
        {
        }
        public GetRange(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;

        private NumberDataNode Result;
        //private IEnumerable<object> sortedData;

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Result = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Result, "Range", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Get Range", "Computation", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            if (input == null || input.Count == 0) return;

            if (input[0].Data is double)
            {
                double? range = (double)input.Last.Value.Data - (double)input.First.Value.Data;
                this.ChildElementManager.SetData(range, Result);
                this.previewTextBlock.DisplayedText = $"Range = {range}";
            }

        }
    }
}
