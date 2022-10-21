using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class Replace : BaseComp
    {

        #region Constructors

        public Replace() : base(0, 0)
        {
            
        }

        public Replace(int x, int y) : base(x, y)
        {
      
        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(0, "");
            string b = this.ChildElementManager.GetData<string>(1, "");
            string c = this.ChildElementManager.GetData<string>(2, "");
            this.ChildElementManager.SetData<string>(a.Replace(b,c), 0);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Replace", "Operations", "Text");
        
        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock1;
        private TextDataNode nodeBlock2;
        private TextDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Replace");

            nodeBlock2 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "By");
            
            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Result", true);
        }
    }
}
