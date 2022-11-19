using Core;
using System;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class InsertDataItem : BaseComp
    {
        public InsertDataItem() : base()
        {
        }
        public InsertDataItem(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private GenericDataNode Value;
        private NumberDataNode Index;
        private GenericDataNode ModifiedData;

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Value = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Value, "Value");

            Index = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Index, "Index");

            ModifiedData = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(ModifiedData, "Modified Value");

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Insert Data Item", "Modification", "Data");
        
        public override void Compute()
        {
            DataStructure result = this.ChildElementManager.GetData(DataStructureNode);

            object value = this.ChildElementManager.GetData(Value, 0.0);
            int index = (int)this.ChildElementManager.GetData(Index, 0.0);
            result.Insert(index, new DataStructure(value));

            this.ChildElementManager.SetData(result, ModifiedData);

  
            this.previewTextBlock.DisplayedText = $"Item at Index:{index} = {result[index]}";
        }
    }
}
