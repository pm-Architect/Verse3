using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Contains : BaseComp
    {


        #region Constructors

        public Contains() : base()
        {
         
        }

        public Contains(int x, int y) : base(x, y)
        {
    
        }

        #endregion

        public override void Compute()
        {
            string a = this.ChildElementManager.GetData<string>(0, "");
            string b = this.ChildElementManager.GetData<string>(1, "");
            bool contain = a.Contains(b);
            this.ChildElementManager.SetData<bool>(contain, 0);
       
        }

 

        public override CompInfo GetCompInfo() => new CompInfo(this, "Text Contains", "Operations", "Text");

        
        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock1;
        private BooleanDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Contains");

            nodeBlock2 = new BooleanDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result");

    
        }
    }
}
