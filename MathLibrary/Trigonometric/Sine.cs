using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Sine : BaseComp
    {


   

        #region Constructors

        public Sine() : base(0, 0)
        {
       
        }

        public Sine(int x, int y) : base(x, y)
        {
      
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData(nodeBlock, 0);

            this.ChildElementManager.SetData(Math.Sin(a), nodeBlock2);
            
        }



        public override CompInfo GetCompInfo() => new CompInfo(this, "Sine", "Trigonometry", "Math 2");

    
        private NumberDataNode nodeBlock;

        private NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Radians");

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock2, "Result", true);

        
        }
    }
}
