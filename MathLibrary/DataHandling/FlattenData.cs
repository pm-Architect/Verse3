using Core;
using System;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class FlattenData : BaseComp
    {
        public FlattenData() : base()
        {
        }
        public FlattenData(int x, int y) : base(x, y)
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
            this.ChildElementManager.AddDataOutputNode(Result, "Flattened Data", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Flatten Data", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure input = this.ChildElementManager.GetData(DataStructureNode);


            this.ChildElementManager.SetData(MathUtils.FlattenDataStructure(input), Result);


            this.previewTextBlock.DisplayedText = $"Grafted data = {input}";
        }
    }
}
