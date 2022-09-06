using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using CoreInterop;

namespace Verse3InteropTestClient
{
    public partial class Form1 : Form
    {
        private InteropClient client;
        public Form1()
        {
            InitializeComponent();

            client = new InteropClient("Verse3", "token");
            client.ServerMessage += Client_ServerMessage;
        }

        private void Client_ServerMessage(object sender, DataStructure e)
        {
            label1.Text = e.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataStructure<double> value = new DataStructure<double>((double)numericUpDown1.Value);
            client.Send(value);
        }
    }
}
