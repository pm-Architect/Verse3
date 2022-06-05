namespace Verse3
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.infiniteCanvasWPFControl1 = new Verse3.InfiniteCanvasWPFControl();
            this.SuspendLayout();
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(62, 60);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(789, 394);
            this.elementHost2.TabIndex = 0;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.infiniteCanvasWPFControl1;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 518);
            this.Controls.Add(this.elementHost2);
            this.Name = "MainWindow";
            this.Text = "Verse3";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private InfiniteCanvasWPFControl infiniteCanvasWPFControl1;
    }
}

