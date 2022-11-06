using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Verse3LibraryTemplate
{
    public partial class UserInputForm : Form
    {
        public string LibraryName => LibraryNameTextBox.Text;
        public string LibraryAuthor => LibraryAuthorTextBox.Text;
        public string CompName => CompNameTextBox.Text;
        public string CompGroup => CompGroupTextBox.Text;
        public string CompTab => CompTabTextBox.Text;


        public UserInputForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
