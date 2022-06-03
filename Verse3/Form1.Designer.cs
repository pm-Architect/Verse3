namespace Verse3
{
    partial class Form1
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
            this.canvasControl2 = new Verse3.CanvasCore.CanvasControl();
            this.SuspendLayout();
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(107, 52);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(454, 313);
            this.elementHost2.TabIndex = 1;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.canvasControl2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 547);
            this.Controls.Add(this.elementHost2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private CanvasCore.CanvasControl canvasControl2;

        #endregion

        //private System.Windows.Forms.Integration.ElementHost elementHost1;
        //private CanvasControl canvasControl1;
    }
}

