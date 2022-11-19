using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class TextLength : BaseComp
    {


        #region Constructors

        public TextLength() : base()
        {

        }

        public TextLength(int x, int y) : base(x, y)
        {

        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData(nodeBlock, "");
            double len = (double)a.Length;
            this.ChildElementManager.SetData(len, nodeBlock2);    
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Text Length", "Operations", "Text");

        private TextDataNode nodeBlock;
 
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);      
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Length", true);
        }
    }
}
