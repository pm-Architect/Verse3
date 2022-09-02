using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace Verse3
{
    public partial class Main_Verse3 : Form
    {
        private int childFormNumber = 0;

        public Main_Verse3()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form1();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        //}

        //private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        //}

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void Main_Verse3_Load(object sender, EventArgs e)
        {
            //#if DEBUG
            if (Debugger.IsAttached)
            {
                Form childForm = new Form1();
                childForm.MdiParent = this;
                childForm.Text = "Window " + childFormNumber++;
                childForm.Show();
                toolStripButton1.Visible = false;
                toolStripButton2.Enabled = false;
            }
            //#endif
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is Form1)
            {
                toolStripStatusLabel.Text = (this.ActiveMdiChild as Form1).InfiniteCanvasWPFControl.AverageFPS.ToString();
                toolStripStatusLabel1.Text = (this.ActiveMdiChild as Form1).InfiniteCanvasWPFControl.GetMouseRelPosition().ToString();
            }
            else
            {
                if (loggedIn && !toolStripButton2.Enabled)
                {
                    toolStripButton1.Visible = false;
                    toolStripButton2.Enabled = true;
                    if (Core.Core.GetUser() != null)
                    {
                        Form childForm = new Form1();
                        childForm.MdiParent = this;
                        childForm.Text = Core.Core.GetUser().Email;
                        childForm.Show();
                        toolStripButton2.Enabled = false;
                    }
                }
            }
            //label1.Text = InfiniteCanvasWPFControl.AverageFPS.ToString();
            //label2.Text = InfiniteCanvasWPFControl.GetMouseRelPosition().ToString();
        }

        private static string GetStandardBrowserPath()
        {
            const string currentUserSubKey =
            @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(currentUserSubKey, false))
            {
                string progId = (userChoiceKey.GetValue("ProgId").ToString());
                using (RegistryKey kp =
                       Registry.ClassesRoot.OpenSubKey(progId + @"\shell\open\command", false))
                {
                    // Get default value and convert to EXE path.
                    // It's stored as:
                    //    "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
                    // So we want the first quoted string only
                    string rawValue = (string)kp.GetValue("");
                    Regex reg = new Regex("(?<=\").*?(?=\")");
                    Match m = reg.Match(rawValue);
                    return m.Success ? m.Value : "";
                }
            }
        }

        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (Core.Core.GetUser() == null)
            {
                string uri = await Core.Core.Login();
                
                //Supabase.Gotrue.Session s = await Core.Core.SetAuth(uri);

                //EmbeddedWPFBrowser browser = new EmbeddedWPFBrowser(uri);
                //if (browser.ShowDialog() == true)
                //{
                //    Core.Core.SetAuth(browser.token);
                //}

                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = GetStandardBrowserPath();
                p.StartInfo.Arguments = uri;
                //p.OutputDataReceived += P_OutputDataReceived;
                p.Start();
                Core.Core.UserLoggedIn += OnUserLoggedIn;
                //p.BeginOutputReadLine();
                //while (!p.StandardOutput.EndOfStream)
                //{
                //    string t = p.StandardOutput.ReadLine();
                //    Core.Core.SetAuth(t);
                //}
                //System.Diagnostics.Process.Start("iexplore.exe", uri);
                //await p.WaitForExitAsync();
                //if (p.HasExited)
                //{
                //    MessageBox.Show(Core.Core.GetUser().Email);
                //}
            }
            else
            {
                //TODO:Splash screen?
                //Form childForm = new Form1();
                //childForm.MdiParent = this;
                //childForm.Text = "Window " + childFormNumber++;
                //childForm.Show();
                //MessageBox.Show("Logged in as: " + Core.Core.GetUser().Email);
                //Core.Core.Logout();
            }
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (Core.Core.GetUser() != null)
            {
                //TODO:Splash screen?
                Form childForm = new Form1();
                childForm.MdiParent = this;
                childForm.Text = Core.Core.GetUser().Email;
                childForm.Show();
                toolStripButton2.Enabled = false;
            }
            else
            {
                string uri = await Core.Core.Login();
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = GetStandardBrowserPath();
                p.StartInfo.Arguments = uri;
                p.Start();
                Core.Core.UserLoggedIn += OnUserLoggedIn;
            }
        }

        bool loggedIn = false;

        public void OnUserLoggedIn(object sender, EventArgs e)
        {
            loggedIn = true;
        }

        //private async void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    if (e.Data is string)
        //    {
        //        string t = e.Data as string;
        //        Supabase.Gotrue.Session s = await Core.Core.SetAuth(t);
        //    }
        //}
    }
}
