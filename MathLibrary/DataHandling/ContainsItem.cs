using Core;
using System;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class ContainsItem : BaseComp
    {
        public ContainsItem() : base()
        {
        }
        public ContainsItem(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private GenericDataNode Value;
        
        private BooleanDataNode Output;
 

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Value = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Value, "Value");

            Output = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(Output, "Contains Value", true);

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Contains Item", "Data Details", "Data");
        
        public override void Compute()
        {
            DataStructure result = this.ChildElementManager.GetData(DataStructureNode);
            object value = this.ChildElementManager.GetData(Value, 3);
            bool contains = result.Contains(new DataStructure(value));


            this.ChildElementManager.SetData(contains, Output);
            this.previewTextBlock.DisplayedText = $"Contains Value:{contains}";
        }
    }
}
