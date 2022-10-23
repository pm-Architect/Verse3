using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Verse3.VanillaElements;

namespace Verse3
{
    //TODO: Create a better Editor Form that can pop up in an MDI container form
    public partial class EditorForm : Form
    {
        private InfiniteCanvasWPFControl infiniteCanvasWPFControl;
        
        public static Dictionary<CompInfo, object[]> compsPendingInst = new Dictionary<CompInfo, object[]>();
        public static List<CompInfo> compsPendingAddToArsenal = new List<CompInfo>();
        public static List<BezierElement> connectionsPending = new List<BezierElement>();

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
        public EditorForm()
        {
            InitializeComponent();
            //TODO: Remove dev open here
            InfiniteCanvasWPFControl = new InfiniteCanvasWPFControl();
            elementHost1.Child = InfiniteCanvasWPFControl;
            InfiniteCanvasWPFControl.MouseDown += Canvas_MouseDown;
            InfiniteCanvasWPFControl.MouseUp += Canvas_MouseUp;
            InfiniteCanvasWPFControl.MouseMove += Canvas_MouseMove;
            //InfiniteCanvasWPFControl.MouseMove += AddToCanvas_OnCall;
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
            if (compsPendingInst.Count > 0)
            {
                AddToCanvas_OnCall(sender, e);
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


        private Dictionary<string, CompInfo> _loadedLibraries = new Dictionary<string, CompInfo>();
        public Dictionary<string, CompInfo> LoadedLibraries { get => _loadedLibraries; private set => _loadedLibraries = value; }
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
            catch (Exception ex)
            {
                //throw;
                //throw ex;
                System.Diagnostics.Debug.WriteLine(ex.Message);
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
                                        if (compInfo.Group == "_CanvasElements" &&
                                            compInfo.Tab == "_CanvasElements")
                                        {
                                            if (compInfo.Name == "Extent")
                                            {
                                                LoadedLibraries.Add(path + "._" + compInfo.Name, compInfo);
                                                //Create two instances of CanvasExtent Element to make the initial/min canvas size 10000x10000
                                                int x = -5000;
                                                int y = -5000;
                                                IElement elInst = compInfo.ConstructorInfo.Invoke(new object[] { x, y }) as IElement;
                                                DataViewModel.Instance.Elements.Add(elInst);
                                                x = 9990;
                                                y = 9990;
                                                elInst = compInfo.ConstructorInfo.Invoke(new object[] { x, y }) as IElement;
                                                DataViewModel.Instance.Elements.Add(elInst);
                                                DataViewModel.WPFControl.ExpandContent();
                                                DataViewModel.WPFControl.InfiniteCanvasControl1.AnimatedSnapTo(new System.Windows.Point(5000.0, 5000.0));
                                                continue;
                                            }
                                            else if (compInfo.Name == "Search")
                                            {
                                                DataViewModel.SearchBarCompInfo = compInfo;
                                                LoadedLibraries.Add(path + "._" + compInfo.Name, compInfo);
                                                continue;
                                            }
                                        }
                                        else if (compInfo.Group == "`" && compInfo.Tab == "`")
                                        {
                                            if (compInfo.Name == "Callback")
                                            {
                                                LoadedLibraries.Add(path + "._" + compInfo.Name, compInfo);
                                                continue;
                                            }
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

        public void AddToArsenal(CompInfo compInfo)
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
                    //tp.AutoScroll = true;
                    //tp.HorizontalScroll.Enabled = true;
                    //tp.HorizontalScroll.Visible = true;
                    //tp.Size = new System.Drawing.Size(1005, 116);
                    tp.TabIndex = 0;
                    tp.Text = compInfo.Tab;
                    tp.UseVisualStyleBackColor = true;

                    this.tabControl1.Controls.Add(tp);
                    this.Tabs.Add(compInfo.Tab, tp);

                    if (tp.Controls.Count == 0)
                    {
                        flp.Dock = System.Windows.Forms.DockStyle.Fill;
                        flp.FlowDirection = FlowDirection.LeftToRight;
                        flp.Margin = new System.Windows.Forms.Padding(0);
                        flp.Name = tp.Name + "_FLP";
                        flp.MaximumSize = new System.Drawing.Size(0, 125);
                        flp.WrapContents = false;
                        flp.AutoScroll = true;
                        //flp.HorizontalScroll.Enabled = true;
                        //flp.HorizontalScroll.Visible = true;
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
                    gb.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    //gb.Location = new System.Drawing.Point(3, 3);
                    gb.MinimumSize = new System.Drawing.Size(100, 100);
                    gb.MaximumSize = new System.Drawing.Size(350, 100);
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
                        flp1.FlowDirection = FlowDirection.LeftToRight;
                        flp1.MaximumSize = new System.Drawing.Size(0, 100);
                        flp1.AutoScroll = true;
                        flp1.AutoSize = true;
                        flp1.AutoSizeMode = AutoSizeMode.GrowOnly;
                        flp1.WrapContents = true;
                        //flp1.Location = new System.Drawing.Point(0, 16);
                        flp1.Margin = new System.Windows.Forms.Padding(0);
                        flp1.Name = gb.Name + "_FLP";
                        //flp1.Size = new System.Drawing.Size(100, 84);
                        flp1.TabIndex = 0;

                        gb.Controls.Add(flp1);
                    }
                }
                if (!this.Buttons.ContainsKey(compInfo.Tab + ".." + compInfo.Group + ".." + compInfo.Name))
                {
                    btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    btn.Name = gb.Name + "_BTN" + this.Buttons.Count.ToString();
                    btn.MinimumSize = new System.Drawing.Size(30, 30);
                    btn.MaximumSize = new System.Drawing.Size(0, 30);
                    btn.Size = new System.Drawing.Size(30, 30);
                    btn.Text = compInfo.Name;
                    btn.AutoSize = true;
                    btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    btn.TabIndex = 0;
                    btn.UseVisualStyleBackColor = true;
                    btn.Tag = compInfo;

                    btn.Click += AddToCanvas_OnCall;

                    flp1.Controls.Add(btn);
                    this.Buttons.Add((compInfo.Tab + ".." + compInfo.Group + ".." + compInfo.Name), btn);
                }

                //if (gb.Width > 300 && gb.Width < 350)
                //{
                //    gb.MaximumSize = new System.Drawing.Size(350, 100);
                //}
                //else if (gb.Width >= 350 && gb.Width < 550)
                //{
                //    gb.MaximumSize = new System.Drawing.Size(550, 100);
                //}
                //else if (gb.Width >= 550)
                //{
                //    gb.MaximumSize = new System.Drawing.Size()
                //}
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

        public void AddToCanvas_OnCall(object sender, EventArgs e)
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
                            ////TODO: Invoke constructor based on <PluginName>.cfg json file
                            ////TODO: Allow user to place the comp with MousePosition
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
                                DataViewModel.Instance.Elements.Add(elInst);
                                if (elInst is BaseComp) ComputationCore.Compute(elInst as BaseComp);
                                //DataViewModel.WPFControl.ExpandContent();
                            }
                            else
                            {
                                //throw new Exception("Constructor parameters not provided");
                                //DataViewModel.WPFControl.ExpandContent();
                            }
                            //IElement elInst = ci.ConstructorInfo.Invoke(new object[] { x, y, w, h }) as IElement;
                            //DataModel.Instance.Elements.Add(elInst);
                            //DataViewModel.WPFControl.ExpandContent();
                        }
                    }
                }
            }
            else
            {
                if (EditorForm.compsPendingInst.Count > 0)
                {
                    try
                    {
                        foreach (CompInfo compInfo in EditorForm.compsPendingInst.Keys)
                        {
                            if (compInfo.ConstructorInfo != null)
                            {
                                BaseComp elInst = compInfo.ConstructorInfo.Invoke(EditorForm.compsPendingInst[compInfo]) as BaseComp;
                                DataTemplateManager.RegisterDataTemplate(elInst);
                                DataViewModel.Instance.Elements.Add(elInst);
                                //EditorForm.compsPendingInst.Remove(compInfo);
                                ComputationCore.Compute(elInst);
                            }
                        }
                        EditorForm.compsPendingInst.Clear();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
                if (EditorForm.connectionsPending.Count > 0)
                {
                    try
                    {
                        foreach (BezierElement b in EditorForm.connectionsPending)
                        {
                            DataTemplateManager.RegisterDataTemplate(b);
                            DataViewModel.Instance.Elements.Add(b);
                            //EditorForm.connectionsPending.Remove(b);
                            b.RedrawBezier(b.Origin, b.Destination);
                        }
                        EditorForm.connectionsPending.Clear();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
                if (EditorForm.compsPendingAddToArsenal.Count > 0)
                {
                    try
                    {
                        foreach (CompInfo compInfo in EditorForm.compsPendingAddToArsenal)
                        {
                            AddToArsenal(compInfo);
                            EditorForm.compsPendingAddToArsenal.Remove(compInfo);
                        }
                        EditorForm.compsPendingAddToArsenal.Clear();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
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

        private void exportCanvasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(elementHost1.Size.Width, elementHost1.Size.Height);
            elementHost1.DrawToBitmap(bmp, elementHost1.Bounds);
            //save bmp to AppData/CanvasExports
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Verse3\\CanvasExports\\");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            { 
                bmp.Save(Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmss") + "_ProjectVerseCanvasExport.png"));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Core.Core.GetUser() != null)
            {
                Core.Core.Logout();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VFSerializable VFfile = new VFSerializable((DataViewModel)DataViewModel.Instance);
            //show a save file dialog with default file extension *.vf or *.vfx
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Verse3 JSON File (*.vfj)|*.vfj|Verse3 File Extended (*.vfx)|*.vfx|Verse3 File (*.vf)|*.vf";
            saveFileDialog.DefaultExt = "vfj";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //save the file
                if (saveFileDialog.FileName.EndsWith(".vf"))
                {
                    VFfile.Serialize(saveFileDialog.FileName);
                }
                else if (saveFileDialog.FileName.EndsWith(".vfx"))
                {
                    //Serialize to xml
                    string xml = VFfile.ToXMLString();
                    File.WriteAllText(saveFileDialog.FileName, xml);
                }
                else if (saveFileDialog.FileName.EndsWith(".vfj"))
                {
                    //Serialize to xml
                    string xml = VFfile.ToJSONString();
                    File.WriteAllText(saveFileDialog.FileName, xml);
                }
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //show an open file dialog to pick a *.vf or *.vfx file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Verse3 JSON File (*.vfj)|*.vfj|Verse3 File Extended (*.vfx)|*.vfx|Verse3 File (*.vf)|*.vf";
            openFileDialog.DefaultExt = "vfj";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //load the file
                try
                {
                    if (openFileDialog.FileName.EndsWith(".vf"))
                    {
                        VFSerializable VFfile = VFSerializable.Deserialize(openFileDialog.FileName);
                        DataViewModel.Instance = VFfile.DataViewModel;
                    }
                    else if (openFileDialog.FileName.EndsWith(".vfx"))
                    {
                        VFSerializable VFfile = VFSerializable.DeserializeXML(openFileDialog.FileName);
                        DataViewModel.Instance = VFfile.DataViewModel;
                    }
                    else if (openFileDialog.FileName.EndsWith(".vfj"))
                    {
                        VFSerializable VFfile = VFSerializable.DeserializeJSON(openFileDialog.FileName);
                        DataViewModel.Instance = VFfile.DataViewModel;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
