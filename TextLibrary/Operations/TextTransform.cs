using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class TextTransform : BaseComp
    {

        #region Constructors

        public TextTransform() : base()
        {
            
        }

        public TextTransform(int x, int y) : base(x, y)
        {
            
        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(0, "");
            this.ChildElementManager.SetData<string>(a.ToUpper(), 0);
            this.ChildElementManager.SetData<string>(a.ToLower(), 1);
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Text Transform", "Operations", "Text");
        
        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock2;
        private TextDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");


            nodeBlock2 = new TextDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Upper", true);

            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            nodeBlock3.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Lower");
        }
    }
}
