using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class ArcSine : BaseComp
    {


        #region Constructors

        public ArcSine() : base()
        {

        }

        public ArcSine(int x, int y) : base(x, y)
        {
        
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData(nodeBlock, 0);
            this.ChildElementManager.SetData(Math.Asin(a), nodeBlock2);
  
        }

        public override CompInfo GetCompInfo() => new CompInfo(this, "Arc Sine", "Trigonometry", "Math 2");


        private NumberDataNode nodeBlock;
        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Number");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Radians", true);

  
        }
    }
}
