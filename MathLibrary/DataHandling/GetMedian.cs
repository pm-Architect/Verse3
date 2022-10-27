using Core;
using MS.WindowsAPICodePack.Internal;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class GetMedian : BaseComp
    {
        public GetMedian() : base()
        {
        }
        public GetMedian(int x, int y) : base(x, y)
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
            this.ChildElementManager.AddDataOutputNode(Result, "Median", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Get Median", "Computation", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
            if (input == null || input.Count == 0) return;
            double? median = MathUtils.Median(input);


            this.ChildElementManager.SetData(median, Result);


            this.previewTextBlock.DisplayedText = $"Median = {median}";
        }
    }
}
