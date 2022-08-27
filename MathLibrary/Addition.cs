using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;
using static Core.Geometry2D;

namespace MathLibrary
{
    public class Addition : BaseComp
    {

        #region Properties

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                //TODO: Z Index control for IRenderable
                return $"Name: {name}" +
                    $"\nView: {viewname}" +
                    $"\nID: {this.ID}" +
                    $"\nX: {this.X}" +
                    $"\nY: {this.Y}" +
                    $"\nOutput Value: {dataIN}";
            }
        }

        #endregion

        #region Constructors

        public Addition() : base(0,0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public Addition(int x, int y, int width = 250, int height = 350) : base(x, y, width, height)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 125.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            double a = this.ChildElementManager.GetData<double>(0);
            if (a == default) a = 0;
            double b = this.ChildElementManager.GetData<double>(1);
            if (b == default) b = 0;
            this.ChildElementManager.SetData<double>((a + b), 0);
            textBlock.DisplayedText = this.ElementText;
        }

        public override CompInfo GetCompInfo()
        {
            CompInfo ci = new CompInfo();
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            ci.ConstructorInfo = this.GetType().GetConstructor(types);
            ci.Name = "Addition";
            ci.Group = "Operations";
            ci.Tab = "Math";
            return ci;
        }

        internal TextElement textBlock = new TextElement();
        internal NumberDataNode nodeBlock;
        internal NumberDataNode nodeBlock1;
        internal NumberDataNode nodeBlock2;
        public override void Initialize()
        {
            nodeBlock = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock);

            nodeBlock1 = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(nodeBlock1);

            nodeBlock2 = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock2);

            string? txt = this.ElementText;
            textBlock = new TextElement();
            textBlock.DisplayedText = txt;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}
