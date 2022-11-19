using Core;
using Verse3;
using Verse3.VanillaElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;


namespace MathLibrary
{
    public class JSONfromKV : BaseComp
    {
        public JSONfromKV() : base()
        {
        }
        public JSONfromKV(int x, int y) : base(x, y)
        {
        }
        
        private TextDataNode KeyNode;
        private GenericDataNode ValueNode;

        private TextDataNode JSONResult;
 

        public override void Initialize()
        {
            //EVENT NODES

            //DATA NODES
            KeyNode = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(KeyNode, "Key Node");

            ValueNode = new GenericDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(ValueNode, "Value Node");


            JSONResult = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(JSONResult, "JSON Result", true);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "JSONfromKV", "JSON", "Data");
        
        public override void Compute()
        {
            DataStructure<string> inputkey = this.ChildElementManager.GetData(KeyNode);
            DataStructure inputvalue = this.ChildElementManager.GetData(ValueNode);
            var output = new MathUtils.JsonDict();

            if (inputkey.Count > 0)
            {
                if (inputkey.Count == inputvalue.Count)
                {
                    for (int i = 0; i < inputkey.Count; i++)
                    {
                        if (inputkey[i] != null)
                        {
                            output.Add(inputkey[i].Data.ToString(), inputvalue[i]);
                        }
                    }
                }
            }
           
            
            this.ChildElementManager.SetData(output.ToString(), JSONResult);


            this.previewTextBlock.DisplayedText = $"JSON Result = {output.ToString()}";
        }
    }
}
