using Core;
using MathLibrary.Inputs;
using System;
using System.Windows;
using System.Windows.Controls;
using Verse3;
using Verse3.VanillaElements;

namespace MathLibrary
{
    public class NumberContainer : BaseComp
    {
        internal double? _sliderValue = 0.0;

        #region Constructors

        public NumberContainer() : base()
        {
   
        }

        public NumberContainer(int x, int y) : base(x, y)
        {

        }

        #endregion

        public override void Compute()
        {
            if (_sliderValue.HasValue)
            {
                this.ChildElementManager.SetData<double>(this._sliderValue.Value, 0);
                this.previewTextBlock.DisplayedText = $"Value = {_sliderValue.Value}";
            }
        }
        public override CompInfo GetCompInfo() => new CompInfo(this, "Number Slider", "Inputs", "Math");

        internal SliderElement sliderBlock = new SliderElement();
        internal NumberDataNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1, "Changed");

            nodeBlock = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Number");

            sliderBlock = new SliderElement();
            sliderBlock.Minimum = -200;
            sliderBlock.Maximum = 200;
            sliderBlock.Value = 10;
            sliderBlock.TickFrequency = 1;
            sliderBlock.ValueChanged += SliderBlock_OnValueChanged;
            this.ChildElementManager.AddElement(sliderBlock);
        }

        private void SliderBlock_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this._sliderValue = sliderBlock.Value;
            ComputationCore.Compute(this, false);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_sliderValue)));
        }

        public override ContextMenu ContextMenu
        {
            get
            {
                ContextMenu contextMenu = new ContextMenu();

                //Delete
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Delete";
                menuItem.Click += (s, e) =>
                {
                    DataViewModel.Instance.Elements.Remove(this);
                };
                contextMenu.Items.Add(menuItem);

                //Show EditNumberSliderDialog
                MenuItem menuItem1 = new MenuItem();
                menuItem1.Header = "Edit";
                menuItem1.Click += (s, e) =>
                {
                    EditNumberSliderDialog editNumberSliderDialog = new EditNumberSliderDialog(this);
                    if (editNumberSliderDialog.ShowDialog() == true)
                    {
                        
                    }
                };
                contextMenu.Items.Add(menuItem1);

                return contextMenu;
            }
        }
        //private IRenderable _parent;
        //public IRenderable Parent => _parent;
        //private ElementsLinkedList<IRenderable> _children = new ElementsLinkedList<IRenderable>();
        //public ElementsLinkedList<IRenderable> Children => _children;
    }
}
