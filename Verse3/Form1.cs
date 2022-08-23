using Core;
using System;
using System.Collections.Generic;
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
            InfiniteCanvasWPFControl.Loaded += LoadLibraries;
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

        private Dictionary<string, CompInfo> LoadedLibraries = new Dictionary<string, CompInfo>();
        public void HotLoadLibraryFolder(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(".verse"))
                    {
                        if (!LoadedLibraries.ContainsKey(file))
                        {
                            HotLoadLibrary(file);
                        }
                    }
                }
            }
        }

        private Dictionary<string, IEnumerable<IElement>> Elements = new Dictionary<string, IEnumerable<IElement>>();
        public void HotLoadLibrary(string path)
        {
            try
            {
                if (DataViewModel.WPFControl == null) return;
                if (File.Exists(path))
                {
                    if (!LoadedLibraries.ContainsKey(path))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            File.OpenRead(path).CopyTo(ms);
                            var es = AssemblyLoader.Load(ms);
                            if (!Elements.ContainsValue(es))
                            {
                                Elements.Add(path, es);
                                LoadElements(path, es);

                            }
                        }
                    }
                    else
                    {
                        if (Elements.ContainsKey(path))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                File.OpenRead(path).CopyTo(ms);
                                var es = AssemblyLoader.Load(ms);
                                Elements[path] = es;
                            }
                            //LoadElements(Elements[path], path);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
                //throw ex;
            }
        }

        private void LoadElements(string path, IEnumerable<IElement> es)
        {
            if (es != null)
            {
                foreach (IElement el in es)
                {
                    if (el is IRenderable)
                    {
                        DataTemplateManager.RegisterDataTemplate(el as IRenderable);
                        //TODO: Check for other types of constructors
                        //TODO: Get LibraryInfo
                        MethodInfo mi = el.GetType().GetRuntimeMethod("GetCompInfo", new Type[] { });
                        if (mi != null)
                        {
                            if (mi.ReturnType == typeof(CompInfo))
                            {
                                CompInfo compInfo = (CompInfo)mi.Invoke(el, null);
                                if (compInfo.ConstructorInfo != null)
                                {
                                    if (!LoadedLibraries.ContainsValue(compInfo))
                                    {
                                        if (compInfo.Name == "Extent" && 
                                            compInfo.Group == "_CanvasElements" && 
                                            compInfo.Tab == "_CanvasElements")
                                        {
                                            LoadedLibraries.Add(path + "._" + compInfo.Name, compInfo);
                                            //Create two instances of CanvasExtent Element to make the initial/min canvas size 10000x10000
                                            int x = -5000;
                                            int y = -5000;
                                            int w = 10;
                                            int h = 10;
                                            IElement elInst = compInfo.ConstructorInfo.Invoke(new object[] { x, y, w, h }) as IElement;
                                            DataModel.Instance.Elements.Add(elInst);
                                            DataViewModel.WPFControl.ExpandContent();
                                            x = 9990;
                                            y = 9990;
                                            w = 10;
                                            h = 10;
                                            elInst = compInfo.ConstructorInfo.Invoke(new object[] { x, y, w, h }) as IElement;
                                            DataModel.Instance.Elements.Add(elInst);
                                            DataViewModel.WPFControl.ExpandContent();
                                            continue;
                                        }
                                        //TODO: Check for validity / scan library info
                                        AddToArsenal(compInfo);
                                        LoadedLibraries.Add(path + "._" + compInfo.Name, compInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<string, TabPage> Tabs = new Dictionary<string, TabPage>();
        private Dictionary<string, GroupBox> Groups = new Dictionary<string, GroupBox>();
        private Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        private void AddToArsenal(CompInfo compInfo)
        {
            if ((compInfo.ConstructorInfo != null) &&
                (compInfo.Name != String.Empty) &&
                (compInfo.Group != String.Empty) &&
                (compInfo.Tab != String.Empty))
            {
                this.tabControl1.SuspendLayout();
                TabPage tp = new TabPage(compInfo.Tab);
                tp.SuspendLayout();
                FlowLayoutPanel flp = new FlowLayoutPanel();
                flp.SuspendLayout();
                GroupBox gb = new GroupBox();
                gb.SuspendLayout();
                FlowLayoutPanel flp1 = new FlowLayoutPanel();
                flp1.SuspendLayout();
                Button btn = new Button();
                btn.SuspendLayout();
                if (this.Tabs.ContainsKey(compInfo.Tab))
                {
                    tp = this.Tabs[compInfo.Tab];

                    if (tp.Controls.Count == 1) flp = tp.Controls[0] as FlowLayoutPanel;
                    //TODO: LOG else return;
                }
                else
                {
                    //tp.Location = new System.Drawing.Point(4, 24);
                    tp.Name = "Tab" + this.tabControl1.Controls.Count.ToString();
                    //tp.Size = new System.Drawing.Size(1005, 116);
                    tp.TabIndex = 0;
                    tp.Text = compInfo.Tab;
                    tp.UseVisualStyleBackColor = true;

                    this.tabControl1.Controls.Add(tp);
                    this.Tabs.Add(compInfo.Tab, tp);

                    if (tp.Controls.Count == 0)
                    {
                        flp.Dock = System.Windows.Forms.DockStyle.Fill;
                        //flp.Location = new System.Drawing.Point(0, 0);
                        flp.Margin = new System.Windows.Forms.Padding(0);
                        flp.Name = tp.Name + "_FLP";
                        //flp.Size = new System.Drawing.Size(1005, 116);
                        flp.TabIndex = 0;

                        tp.Controls.Add(flp);
                    }
                }
                if (this.Groups.ContainsKey(compInfo.Tab + ".." + compInfo.Group))
                {
                    gb = this.Groups[compInfo.Tab + ".." + compInfo.Group];

                    if (gb.Controls.Count == 1) flp1 = gb.Controls[0] as FlowLayoutPanel;
                }
                else
                {
                    gb.AutoSize = true;
                    //gb.Location = new System.Drawing.Point(3, 3);
                    gb.MinimumSize = new System.Drawing.Size(100, 100);
                    gb.Name = tp.Name + "_GRP" + flp.Controls.Count.ToString();
                    gb.Padding = new System.Windows.Forms.Padding(0);
                    //gb.Size = new System.Drawing.Size(100, 100);
                    gb.TabIndex = 0;
                    gb.TabStop = false;
                    gb.Text = compInfo.Group;

                    flp.Controls.Add(gb);
                    this.Groups.Add((compInfo.Tab + ".." + compInfo.Group), gb);

                    if (gb.Controls.Count == 0)
                    {
                        flp1.Dock = System.Windows.Forms.DockStyle.Fill;
                        //flp1.Location = new System.Drawing.Point(0, 16);
                        flp1.Margin = new System.Windows.Forms.Padding(0);
                        flp1.Name = gb.Name + "_FLP";
                        //flp1.Size = new System.Drawing.Size(100, 84);
                        flp1.TabIndex = 0;

                        gb.Controls.Add(flp1);
                    }
                }
                if (this.Buttons.ContainsKey(compInfo.Tab + ".." + compInfo.Group + ".." + compInfo.Name))
                {
                    //TODO: Throw error, log to console
                }
                else
                {
                    btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    //btn.Location = new System.Drawing.Point(3, 3);
                    btn.Name = gb.Name + "_BTN" + this.Buttons.Count.ToString();
                    btn.MinimumSize = new System.Drawing.Size(30, 30);
                    btn.MaximumSize = new System.Drawing.Size(30, 30);
                    btn.Size = new System.Drawing.Size(30, 30);
                    btn.TabIndex = 0;
                    btn.UseVisualStyleBackColor = true;
                    btn.Text = this.Buttons.Count.ToString();
                    btn.Tag = compInfo;

                    btn.Click += Btn_Click;

                    flp1.Controls.Add(btn);
                    this.Buttons.Add((compInfo.Tab + ".." + compInfo.Group + ".." + compInfo.Name), btn);
                }
                btn.ResumeLayout(false);
                flp1.ResumeLayout(false);
                gb.ResumeLayout(false);
                flp.ResumeLayout(false);
                tp.ResumeLayout(false);
                this.tabControl1.ResumeLayout(false);
                this.PerformLayout();
            }
        }
        // 
        // tabControl1
        // 
        //this.tabControl1.Controls.Add(this.tabPage1);

        // 
        // tabPage1
        // 
        //this.tabPage1.Controls.Add(this.flowLayoutPanel1);
        //this.tabPage1.Location = new System.Drawing.Point(4, 24);
        //this.tabPage1.Name = "tabPage1";
        //this.tabPage1.Size = new System.Drawing.Size(1005, 116);
        //this.tabPage1.TabIndex = 0;
        //this.tabPage1.Text = "tabPage1";
        //this.tabPage1.UseVisualStyleBackColor = true;
        // 
        // flowLayoutPanel1
        // 
        //this.flowLayoutPanel1.Controls.Add(this.groupBox1);
        //this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        //this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        //this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
        //this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        //this.flowLayoutPanel1.Size = new System.Drawing.Size(1005, 116);
        //this.flowLayoutPanel1.TabIndex = 0;

        // 
        // groupBox1
        // 
        //this.groupBox1.AutoSize = true;
        //this.groupBox1.Controls.Add(this.flowLayoutPanel2);
        //this.groupBox1.Location = new System.Drawing.Point(3, 3);
        //this.groupBox1.MinimumSize = new System.Drawing.Size(100, 100);
        //this.groupBox1.Name = "groupBox1";
        //this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
        //this.groupBox1.Size = new System.Drawing.Size(100, 100);
        //this.groupBox1.TabIndex = 0;
        //this.groupBox1.TabStop = false;
        //this.groupBox1.Text = "groupBox1";
        // 
        // flowLayoutPanel2
        // 
        //this.flowLayoutPanel2.Controls.Add(this.button1);
        //this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
        //this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 16);
        //this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
        //this.flowLayoutPanel2.Name = "flowLayoutPanel2";
        //this.flowLayoutPanel2.Size = new System.Drawing.Size(100, 84);
        //this.flowLayoutPanel2.TabIndex = 0;

        // 
        // button1
        // 
        //this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
        //this.button1.Location = new System.Drawing.Point(3, 3);
        //this.button1.Name = "button1";
        //this.button1.Size = new System.Drawing.Size(30, 30);
        //this.button1.TabIndex = 0;
        //this.button1.UseVisualStyleBackColor = true;

        private void Btn_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button btn = sender as Button;
                if (btn.Tag != null && btn.Tag is CompInfo)
                {
                    if (DataViewModel.WPFControl != null)
                    {
                        CompInfo ci = (CompInfo)btn.Tag;
                        if (ci.ConstructorInfo != null)
                        {
                            //Random rnd = new Random();
                            ////TODO: Invoke constructor based on <PluginName>.cfg json file
                            ////TODO: Allow user to place the comp with MousePosition
                            //int x = InfiniteCanvasWPFControl.GetMouseRelPosition().X;
                            //int y = InfiniteCanvasWPFControl.GetMouseRelPosition().Y;
                            //int w = ci.
                            //int h = (int)rnd.NextInt64((long)250, (long)350);
                            if (ci.ConstructorInfo.GetParameters().Length > 0)
                            {
                                ParameterInfo[] pi = ci.ConstructorInfo.GetParameters();
                                object[] args = new object[pi.Length];
                                for (int i = 0; i < pi.Length; i++)
                                {
                                    if (!(pi[i].DefaultValue is DBNull)) args[i] = pi[i].DefaultValue;
                                    else
                                    {
                                        if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "x")
                                            args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().X;
                                        else if (pi[i].ParameterType == typeof(int) && pi[i].Name.ToLower() == "y")
                                            args[i] = InfiniteCanvasWPFControl.GetMouseRelPosition().Y;
                                    }
                                }
                                IElement elInst = ci.ConstructorInfo.Invoke(args) as IElement;
                                DataModel.Instance.Elements.Add(elInst);
                                DataViewModel.WPFControl.ExpandContent();
                            }
                            else
                            {
                                //throw new Exception("Constructor parameters not provided");
                                DataViewModel.WPFControl.ExpandContent();
                            }
                            //IElement elInst = ci.ConstructorInfo.Invoke(new object[] { x, y, w, h }) as IElement;
                            //DataModel.Instance.Elements.Add(elInst);
                            //DataViewModel.WPFControl.ExpandContent();
                        }
                    }
                }
            }
        }

        private void LoadLibraries(object sender, EventArgs e)
        {
            HotLoadLibraryFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Verse3\\Libraries\\"));
        }

        private void loadLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                HotLoadLibrary(openFileDialog.FileName);
            }
        }
    }
}
