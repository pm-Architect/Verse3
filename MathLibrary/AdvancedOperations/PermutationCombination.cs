using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;


namespace MathLibrary
{
    public class PermutationCombination : BaseComp
    {


        #region Constructors

        public PermutationCombination() : base()
        {

        }

        public PermutationCombination(int x, int y) : base(x, y)
        {

        }

        #endregion

        public override void Compute()
        {
            double n = this.ChildElementManager.GetData(nodeBlock, 1);
            double r = this.ChildElementManager.GetData(nodeBlock1, 1);

            this.ChildElementManager.SetData(MathUtils.Permutate(n,r), nodeBlock2);
            this.ChildElementManager.SetData(MathUtils.Combinate(n, r), nodeBlock3);

        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Permutation and Combination", "Advanced Operations", "Math");

       
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock2;
        private NumberDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);         
            this.ChildElementManager.AddDataInputNode(nodeBlock, "n");

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "r");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Permutation", true);

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Combination");

        }
    }
}
