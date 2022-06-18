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
using System.Windows.Media;
using Verse3.VanillaElements;

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
            CompositionTarget.Rendering += CompositionTarget_Rendering;            
        }

        TimeOnly lastFrameTime = TimeOnly.FromDateTime(DateTime.Now);
        double fps = 0.0;
        double[] lfps = Array.Empty<double>();
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeOnly frameTime = TimeOnly.FromDateTime(DateTime.Now);
            fps = 1 / (frameTime - lastFrameTime).TotalSeconds;
            if (lfps.Length < 255)
            {
                lfps = lfps.Concat(new double[] { fps }).ToArray();
            }
            else
            {
                lfps = lfps.Skip(1).Concat(new double[] { fps }).ToArray();
            }
            double avgfps = lfps.Average();
            avgfps = Math.Round(avgfps, 3);
            lastFrameTime = frameTime;
            label1.Text = avgfps.ToString();
            label1.Text += "     |     ";
            label1.Text += InfiniteCanvasWPFControl.GetMouseRelPosition().ToString();
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
            if (drawstart != Point.Empty && started)
            {
                DrawBezierCurve(drawstart, InfiniteCanvasWPFControl.GetMouseRelPosition());
                started = false;
            }
        }

        Point drawstart = new Point();
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
            if (started)
            {
                drawstart = InfiniteCanvasWPFControl.GetMouseRelPosition();
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
                            Random rnd = new Random();
                            for (int i = 0; i < 10; i++)
                            {
                                int x = (int)rnd.NextInt64((long)20, (long)1000);
                                int y = (int)rnd.NextInt64((long)20, (long)1000);
                                int w = (int)rnd.NextInt64((long)250, (long)350);
                                int h = (int)rnd.NextInt64((long)250, (long)350);
                                Type[] types = { i.GetType(), i.GetType(), i.GetType(), i.GetType() };
                                IElement elInst = el.GetType().GetConstructor(types).Invoke(new object[] { x, y, w, h }) as IElement;
                                DataModel.Instance.Elements.Add(elInst);
                            }
                            //ObservableCollection<string> o;
                        }
                        //DataModel.Instance.Elements.Add(new TestElement(50, 50, 80, 150));
                        //DataModel.Instance.Elements.Add(new TestElement(550, 350, 80, 150));
                        //DataModel.Instance.Elements.Add(new TestElement(850, 850, 30, 20));
                        //DataModel.Instance.Elements.Add(new TestElement(1200, 1200, 80, 150));
                    }

                    DataViewModel.WPFControl.ExpandContent();
                }
            }
        }

        private void DrawBezierCurve(Point start, Point end)
        {
            BezierElement bezier = new BezierElement((start.X - 200), (start.Y - 200), (end.X - start.X), (end.Y - start.Y));
            DataTemplateManager.RegisterDataTemplate(bezier as IRenderable);
            DataModel.Instance.Elements.Add(bezier);
        }

        bool started = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                started = true;                
            }
            else if (started)
            {
                started = false;
            }
        }
    }
}
