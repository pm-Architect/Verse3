using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Verse3
{
    public partial class Form1 : Form
    {
        private InfiniteCanvasWPFControl infiniteCanvasWPFControl;

        public InfiniteCanvasWPFControl InfiniteCanvasWPFControl
        {
            get
            {
                return infiniteCanvasWPFControl;
            }
            private set
            {
                infiniteCanvasWPFControl = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            //TODO: Remove dev open here
            InfiniteCanvasWPFControl = new InfiniteCanvasWPFControl();
            elementHost1.Child = InfiniteCanvasWPFControl;
            InfiniteCanvasWPFControl.MouseDown += Canvas_MouseDown;
            InfiniteCanvasWPFControl.MouseUp += Canvas_MouseUp;
            InfiniteCanvasWPFControl.MouseMove += Canvas_MouseMove;
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is InfiniteCanvasWPFControl)
            {
                InfiniteCanvasWPFControl infiniteCanvas = (InfiniteCanvasWPFControl)sender;
                this.Cursor = infiniteCanvas.WinFormsCursor;
            }
        }

        private void Canvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is InfiniteCanvasWPFControl)
            {
                InfiniteCanvasWPFControl infiniteCanvas = (InfiniteCanvasWPFControl)sender;
                if (infiniteCanvas.MouseHandlingMode == MouseHandlingMode.None)
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is InfiniteCanvasWPFControl)
            {
                InfiniteCanvasWPFControl infiniteCanvas = (InfiniteCanvasWPFControl)sender;
                if (infiniteCanvas.MouseHandlingMode == MouseHandlingMode.Panning)
                {
                    this.Cursor = Cursors.SizeAll;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    File.OpenRead(openFileDialog.FileName).CopyTo(ms);
                    var elements = AssemblyLoader.Load(ms);
                    
                    foreach (IElement el in elements)
                    {
                        if (el is IRenderable)
                        {
                            DataTemplateManager.RegisterDataTemplate(el as IRenderable);
                            int x = 0;
                            Type[] types = { x.GetType(), x.GetType(), x.GetType(), x.GetType() };
                            IElement elInst = el.GetType().GetConstructor(types).Invoke(new object[] { 50, 50, 150, 150 }) as IElement;
                            DataModel.Instance.Elements.Add(elInst);
                            //ObservableCollection<string> o;
                        }
                        //DataModel.Instance.Elements.Add(new TestElement(50, 50, 80, 150));
                        //DataModel.Instance.Elements.Add(new TestElement(550, 350, 80, 150));
                        //DataModel.Instance.Elements.Add(new TestElement(850, 850, 30, 20));
                        //DataModel.Instance.Elements.Add(new TestElement(1200, 1200, 80, 150));
                    }
                }
            }
        }
    }
}
