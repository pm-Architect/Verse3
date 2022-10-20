using Core;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class DataAtIndex : BaseComp
    {
        public DataAtIndex() : base()
        {
        }
        public DataAtIndex(int x, int y) : base(x, y)
        {
        }
        
        private GenericDataNode DataStructureNode;
        private NumberDataNode Index;
        private GenericDataNode StartElement;
        private GenericDataNode ElementAtI;
        private GenericDataNode EndElement;
        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            DataStructureNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(DataStructureNode, "Data Structure");

            Index = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(Index, "Index");

            StartElement = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(StartElement, "Start Element");

            ElementAtI = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(ElementAtI, "Element at i", true);

            EndElement = new GenericDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(EndElement, "End Element");
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Data at Index", "Data Details", "Data");
        
        public override void Compute()
        {
            DataStructure result = this.ChildElementManager.GetData(DataStructureNode);
            
            int index = (int)this.ChildElementManager.GetData(Index, 0.0);
            
            this.ChildElementManager.SetData(result[0].Data, StartElement);
            this.ChildElementManager.SetData(result[index].Data, ElementAtI);
            this.ChildElementManager.SetData(result.Last.Value.Data, EndElement);
            this.previewTextBlock.DisplayedText = $"Item at index = {result[index]}";
        }
    }
}
