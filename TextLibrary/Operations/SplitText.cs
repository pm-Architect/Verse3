using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class SplitText : BaseComp
    {


        #region Constructors

        public SplitText() : base()
        {

        }

        public SplitText(int x, int y) : base(x, y)
        {

        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData(nodeBlock, "Hello, You");
            string splitter = this.ChildElementManager.GetData(nodeBlock2, ",");
            

            string[] splitArray = a.Split(splitter);
            DataStructure result = new DataStructure(); 
            foreach (string item in splitArray)
            {
                result.Add(item);
            }
            this.ChildElementManager.SetData(result, nodeBlock2);    
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Split Text", "Operations", "Text");

        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock2;
        private TextDataNode nodeBlock3;

        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);      
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock2 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "Split by");

            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Split Text", true);
        }
    }
}
