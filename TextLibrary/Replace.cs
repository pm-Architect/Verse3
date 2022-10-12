﻿using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class Replace : BaseComp
    {
        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = "";
                if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                    dataIN = ((TextDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0]).DataGoo.Data;
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

        #region Properties


        #endregion

        #region Constructors

        public Replace() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public Replace(int x, int y, int width = 250, int height = 350) : base(x, y)
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
            string a = this.ChildElementManager.GetData<string>(0, "");
            string b = this.ChildElementManager.GetData<string>(1, "");
            string c = this.ChildElementManager.GetData<string>(2, "");
            this.ChildElementManager.SetData<string>(a.Replace(b,c), 0);
            textBlock.DisplayedText = this.ElementText;
        }

        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Replace",
                Group = "Operations",
                Tab = "Text",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        private TextElement textBlock = new TextElement();
        private TextDataNode nodeBlock;
        private TextDataNode nodeBlock1;
        private TextDataNode nodeBlock2;
        private TextDataNode nodeBlock3;
        public override void Initialize()
        {
            nodeBlock = new TextDataNode(this, NodeType.Input);
            nodeBlock.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock, "Text");

            nodeBlock1 = new TextDataNode(this, NodeType.Input);
            nodeBlock1.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock1, "Replace");

            nodeBlock2 = new TextDataNode(this, NodeType.Input);
            nodeBlock2.Width = 50;
            this.ChildElementManager.AddDataInputNode(nodeBlock2, "By");

            nodeBlock3 = new TextDataNode(this, NodeType.Output);
            nodeBlock3.Width = 50;
            this.ChildElementManager.AddDataOutputNode(nodeBlock3, "Result");

            textBlock = new TextElement();
            textBlock.DisplayedText = this.ElementText;
            textBlock.TextAlignment = TextAlignment.Left;
            this.ChildElementManager.AddElement(textBlock);
        }
    }
}