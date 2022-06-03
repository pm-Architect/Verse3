using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Verse3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //label1.Text = canvasWPFControl1.MainCanvas.Zoom.ToString();
            //label2.Text = canvasWPFControl1.MainCanvas.TranslateXform.X.ToString();
            //label3.Text = canvasWPFControl1.MainCanvas.TranslateXform.Y.ToString();
            //label4.Text = Mouse.GetPosition(canvasWPFControl1).X.ToString();
            //label5.Text = Mouse.GetPosition(canvasWPFControl1).Y.ToString();
        }
    }
}
