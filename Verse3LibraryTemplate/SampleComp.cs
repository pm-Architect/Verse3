using System;
using System.Collections.Generic;
$
if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;

namespace $rootnamespace$
{
	public class $safecompname$ : BaseComp
    {
        #region Constructors
        public $safecompname$() : base()
        {
        }

        public $safecompname$(int x, int y) : base(x, y)
        {
            
        }
        #endregion

        public override CompInfo GetCompInfo() => new CompInfo(this, "$compname$", "$compgroup$", "$comptab$");

        private GenericEventNode EventAInputNode;
        private GenericEventNode EventBInputNode;
        private NumberDataNode NumberCInputNode;
        private BooleanDataNode BooleanDInputNode;

        private GenericEventNode EventEOutputNode;
        private NumberDataNode NumberFOutputNode;

        private ButtonElement ButtonX;
        private SliderElement SliderY;
        private TextBoxElement TextBoxZ;
        public override void Initialize()
        {
            //EVENT NODES
            //INPUTS
            EventAInputNode = new GenericEventNode(this, NodeType.Input);
            EventAInputNode.NodeEvent += EventAInputNode_NodeEvent;
            this.ChildElementManager.AddEventInputNode(EventAInputNode, "A");

            EventBInputNode = new GenericEventNode(this, NodeType.Input);
            EventBInputNode.NodeEvent += EventBInputNode_NodeEvent;
            this.ChildElementManager.AddEventInputNode(EventBInputNode, "B");

            //OUTPUTS
            EventEOutputNode = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(EventEOutputNode, "E");

            //DATA NODES
            //INPUTS
            NumberCInputNode = new NumberDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(NumberCInputNode, "C");

            BooleanDInputNode = new BooleanDataNode(this, NodeType.Input);
            this.ChildElementManager.AddDataInputNode(BooleanDInputNode, "D");

            //OUTPUTS
            NumberFOutputNode = new NumberDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(NumberFOutputNode, "F", true);

            //UI ELEMENTS
            //BUTTON
            ButtonX = new ButtonElement();
            ButtonX.DisplayedText = "Sample Button";
            ButtonX.OnButtonClicked += ButtonX_OnButtonClicked;
            this.ChildElementManager.AddElement(ButtonX);

            //SLIDER
            SliderY = new SliderElement();
            SliderY.Minimum = 0;
            SliderY.Maximum = 100;
            SliderY.Value = 50;
            SliderY.ValueChanged += SliderY_ValueChanged;
            this.ChildElementManager.AddElement(SliderY);

            //TEXTBOX
            TextBoxZ = new TextBoxElement();
            TextBoxZ.InputText = "Sample Text";
            TextBoxZ.ValueChanged += TextBoxZ_ValueChanged;
        }

        public override void Compute()
        {
            DataStructure<double> numbersC = this.ChildElementManager.GetData(NumberCInputNode);
            DataStructure<bool> booleansD = this.ChildElementManager.GetData(BooleanDInputNode);
            if (numbersC is null || booleansD is null) return;
            this.ChildElementManager.SetData(numbersC, NumberFOutputNode);
        }

        private void EventAInputNode_NodeEvent(IEventNode container, EventArgData e)
        {
            this.previewTextBlock.DisplayedText = "Event A was triggered";
        }
        private void EventBInputNode_NodeEvent(IEventNode container, EventArgData e)
        {
            this.previewTextBlock.DisplayedText = "Event B was triggered";
        }

        private void ButtonX_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            this.previewTextBlock.DisplayedText = "Button X was clicked";
        }
        private void SliderY_ValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.previewTextBlock.DisplayedText = "Slider Y was changed to " + SliderY.Value;
        }
        private void TextBoxZ_ValueChanged(object? sender, TextChangedEventArgs e)
        {
            this.previewTextBlock.DisplayedText = "Text Box Z was changed to " + TextBoxZ.InputText;
        }
    }
}
