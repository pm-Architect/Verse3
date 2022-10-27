using Core;
using System;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class ItemCount : BaseComp
    {
        public ItemCount() : base()
        {
        }
        public ItemCount(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;     
        private NumberDataNode Output;
 

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Output = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Output, "Item Count", true);

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Item Count", "Data Details", "Data");
        
        public override void Compute()
        {
            DataStructure result = this.ChildElementManager.GetData(DataStructureNode);
            int count = -1;
            if (result != null) count = result.Count;

            if (count > -1)
                this.ChildElementManager.SetData((double)count, Output);
            //this.previewTextBlock.DisplayedText = $"Item Count = {count}";
        }
    }
}
