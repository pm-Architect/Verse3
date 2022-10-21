using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;
using TextElement = Verse3.VanillaElements.TextElement;

namespace MathLibrary
{
    public class RandomNumber : BaseComp
    {

        #region Constructors

        public RandomNumber() : base(0, 0)
        {
        }

        public RandomNumber(int x, int y, int width = 250, int height = 350) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0, int.MinValue);
            double b = this.ChildElementManager.GetData<double>(1, int.MaxValue);
            Random rd = new Random();
            this.ChildElementManager.SetData<double>(rd.Next((int)b, (int)a), 0);
        }


        public override CompInfo GetCompInfo() => new CompInfo(this, "Random", "Miscellaneous", "Math");
        
        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock1;
        private NumberDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Maximum");
            
            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Minimum");

            nodeBlock3 = new NumberDataNode(this, NodeType.Output);
            nodeBlock3.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Random between the limits");
        }
    }
}
