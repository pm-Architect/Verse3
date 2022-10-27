using Core;
using MS.WindowsAPICodePack.Internal;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class SampleStandardDeviation : BaseComp
    {
        public SampleStandardDeviation() : base()
        {
        }
        public SampleStandardDeviation(int x, int y) : base(x, y)
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
            this.ChildElementManager.AddDataOutputNode(Result, "Sample Standard Deviation", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Sample Standard Deviation", "Computation", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);
   
            double? result = MathUtils.SampleStandardDeviation(input);


            this.ChildElementManager.SetData(result, Result);


            this.previewTextBlock.DisplayedText = $"Sample Standard Deviation = {result}";
        }
    }
}
