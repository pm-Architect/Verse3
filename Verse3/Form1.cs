using Core;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Verse3
{
    //TODO: Create a better Editor Form that can pop up in an MDI container form
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
            if (DataViewModel.ActiveConnection != default)
            {
                //DataViewModel.ActiveConnection.Destination.
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
            //if (DataViewModel.ActiveNode != default /*&& started*/)
            //{
            //    //DrawBezierCurve(drawstart, InfiniteCanvasWPFControl.GetMouseRelPosition(), rtl);

            //    if (DataViewModel.ActiveConnection == default)
            //    {
            //        DataViewModel.ActiveConnection = CreateConnection(DataViewModel.ActiveNode);
            //    }
            //    else
            //    {
            //        ((BezierElement)DataViewModel.ActiveConnection).SetDestination(DataViewModel.ActiveNode);
            //        DataViewModel.ActiveConnection = default;
            //        DataViewModel.ActiveNode = default;
            //    }
            //    //started = false;
            //}
        }

        //public INode drawstart = default;
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is InfiniteCanvasWPFControl)
            {
                InfiniteCanvasWPFControl infiniteCanvas = (InfiniteCanvasWPFControl)sender;
                if (infiniteCanvas.MouseHandlingMode == MouseHandlingMode.Panning)
                {
                    this.Cursor = Cursors.SizeAll;
                }
                //if (started)
                //{
                //}
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

                                //TODO: Check for other types of constructors

                                ConstructorInfo ci = el.GetType().GetConstructor(types);

                                if (ci != null)
                                {
                                    //TODO: Invoke constructor based on <PluginName>.cfg json file
                                    IElement elInst = ci.Invoke(new object[] { x, y, w, h }) as IElement;
                                    DataModel.Instance.Elements.Add(elInst);
                                }
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

        //private void DrawBezierCurve(Point start, Point end, bool rtl)
        //{
        //    BezierElement bezier = new BezierElement((start.X - 200), (start.Y - 200), (end.X - start.X), (end.Y - start.Y), rtl);
        //    DataTemplateManager.RegisterDataTemplate(bezier as IRenderable);
        //    DataModel.Instance.Elements.Add(bezier);
        //}


        //bool started = false;
        //bool rtl = false;
        private void button2_Click(object sender, EventArgs e)
        {
            //if (!started)
            //{
            //    started = true;
            //    rtl = false;
            //}
            //else if (started)
            //{
            //    started = false;
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //if (!started)
            //{
            //    started = true;
            //    rtl = true;
            //}
            //else if (started)
            //{
            //    started = false;
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = InfiniteCanvasWPFControl.AverageFPS.ToString();
            label2.Text = InfiniteCanvasWPFControl.GetMouseRelPosition().ToString();
        }
    }
}
