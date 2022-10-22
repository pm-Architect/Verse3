using Core;
using System;
using System.Windows;
using Verse3;
using Verse3.VanillaElements;

namespace TextLibrary
{
    public class TextContainer : BaseComp
    {
        internal string _inputText = "";

        #region Constructors

        public TextContainer() : base()
        {
        }

        public TextContainer(int x, int y) : base(x, y)
        {
        }

        #endregion

        public override void Compute()
        {
            this._inputText = textBoxBlock.InputText;
            this.ChildElementManager.SetData<string>(this._inputText, 0);
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Text Input", "Inputs", "Text");

        internal TextBoxElement textBoxBlock = new TextBoxElement();
        internal TextDataNode nodeBlock;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            textBoxBlock = new TextBoxElement();
            textBoxBlock.InputText = "";
            textBoxBlock.ValueChanged += TextBoxBlock_OnValueChanged;
            this.ChildElementManager.AddElement(textBoxBlock);

            nodeBlock = new TextDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Text");
        }
        
        private void TextBoxBlock_OnValueChanged(object? sender, EventArgs e)
        {
            this._inputText = textBoxBlock.InputText;
            ComputationPipeline.ComputeComputable(this);
        }
    }
}
