using Core;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Verse3;
using Verse3.VanillaElements;

namespace ColorLibrary
{
    public class ColorSelector : BaseComp
    {
        internal Color? _value = Color.FromArgb(255, 255, 255, 255);
        //private double _inputValue = 0.0;

        public string? ElementText
        {
            get
            {
                string? name = this.GetType().FullName;
                string? viewname = this.ViewType.FullName;
                string? dataIN = _value.ToString();
                //if (this.ComputationPipelineInfo.IOManager.DataOutputNodes != null && this.ComputationPipelineInfo.IOManager.DataOutputNodes.Count > 0)
                //dataIN = ((NumberDataNode)this.ComputationPipelineInfo.IOManager.DataOutputNodes[0])?.DataGoo.Data.ToString();
                //string? zindex = DataViewModel.WPFControl.Content.
                
                return $"Value: {dataIN}";
            }
        }
        
        #region Constructors

        public ColorSelector() : base(0, 0)
        {
            //this.background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6700"));
            //Random rng = new Random();
            //byte r = (byte)rng.Next(0, 255);
            //this.backgroundTint = new SolidColorBrush(Color.FromArgb(100, r, r, r));
        }

        public ColorSelector(int x, int y, int width = 250, int height = 100) : base(x, y)
        {
            //base.boundingBox = new BoundingBox(x, y, width, height);

            //Random rnd = new Random();
            //byte rc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte gc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //byte bc = (byte)Math.Round(rnd.NextDouble() * 255.0);
            //this.BackgroundTint = new SolidColorBrush(Color.FromRgb(rc, gc, bc));
            //this.Background = new SolidColorBrush(Colors.Gray);
        }

        #endregion

        public override void Compute()
        {
            //_value = toggleBlock.Value;
            if (_value.HasValue && b != null)
            {
                buttonBlock.BackgroundColor = b;
                buttonBlock.DisplayedText = _value.ToString();
                this.ChildElementManager.SetData<Color>(_value.Value, 0);
            }
        }
        public override CompInfo GetCompInfo()
        {
            Type[] types = { typeof(int), typeof(int), typeof(int), typeof(int) };
            CompInfo ci = new CompInfo
            {
                ConstructorInfo = this.GetType().GetConstructor(types),
                Name = "Color Picker",
                Group = "Basic UI",
                Tab = "Color",
                Description = "",
                Author = "",
                License = "",
                Repository = "",
                Version = "",
                Website = ""
            };
            return ci;
        }

        internal ColorPicker colorPicker;
        internal PopupWindow window;

        internal TextElement textBlock = new TextElement();
        internal ButtonElement buttonBlock = new ButtonElement();
        internal ColorDataNode nodeBlock;
        internal GenericEventNode nodeBlock1;
        public override void Initialize()
        {
            base.titleTextBlock.TextRotation = 0;

            nodeBlock1 = new GenericEventNode(this, NodeType.Output);
            this.ChildElementManager.AddEventOutputNode(nodeBlock1, "Changed");

            nodeBlock = new ColorDataNode(this, NodeType.Output);
            this.ChildElementManager.AddDataOutputNode(nodeBlock, "Color");

            buttonBlock = new ButtonElement();
            buttonBlock.DisplayedText = _value.GetValueOrDefault(Color.FromArgb(255, 255, 255, 255)).ToString();
            buttonBlock.BackgroundColor = new System.Windows.Media.SolidColorBrush(_value.GetValueOrDefault(Color.FromArgb(255, 255, 255, 255)));
            buttonBlock.OnButtonClicked += ButtonBlock_OnButtonClicked;
            buttonBlock.Width = 200;
            this.ChildElementManager.AddElement(buttonBlock);

            //colorPickerElement = new ColorPickerElement();
            //colorPickerElement.Value = new System.Windows.Media.SolidColorBrush(_value.GetValueOrDefault(Color.FromArgb(255, 255, 255, 255)));
            //colorPickerElement.DisplayedText = colorPickerElement.Value.ToString();
            //colorPickerElement.ColorChanged += ColorPickerElement_ColorChanged;
            //colorPickerElement.Width = 200;
            //this.ChildElementManager.AddElement(colorPickerElement);
        }

        //private void ColorPickerElement_ColorChanged(object? sender, RoutedEventArgs e)
        //{
        //    _value = colorPickerElement.Value.Color;
        //    ComputationCore.Compute(this);
        //    this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_value)));
        //}
        private void ButtonBlock_OnButtonClicked(object? sender, RoutedEventArgs e)
        {
            //colorPicker = SingleOpenHelper.CreateControl<ColorPicker>();
            //colorPicker.SelectedBrush = new SolidColorBrush(_value.GetValueOrDefault(Color.FromArgb(255, 255, 255, 255)));
            //window = new PopupWindow()
            //{
            //    PopupElement = colorPicker
            //};
            //colorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
            //colorPicker.Canceled += ColorPicker_Canceled;
            //window.Show();

            //ComputationCore.Compute(this);
            //this.ChildElementManager.EventOccured(0, new EventArgData());

            //create a winforms color dialog box
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            colorDialog.AllowFullOpen = true;
            switch (colorDialog.ShowDialog())
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    {
                        ColorPicker_SelectedColorChanged(colorDialog, colorDialog.Color);
                        break;
                    }
                case DialogResult.Cancel:
                    break;
                case DialogResult.Abort:
                    break;
                case DialogResult.Retry:
                    break;
                case DialogResult.Ignore:
                    break;
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.TryAgain:
                    break;
                case DialogResult.Continue:
                    break;
                default:
                    break;
            }
        }

        internal SolidColorBrush? b = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        private void ColorPicker_SelectedColorChanged(object? sender, System.Drawing.Color e)
        {
            _value = Color.FromArgb(e.A, e.R, e.G, e.B);
            b = new SolidColorBrush(_value.Value);
            ComputationCore.Compute(this);
            this.ChildElementManager.EventOccured(0, new EventArgData(new DataStructure(_value)));
        }
    }
}
