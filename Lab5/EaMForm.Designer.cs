namespace Lab5
{
    partial class EaMForm
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
            this.richTextBoxA = new System.Windows.Forms.RichTextBox();
            this.richTextBoxB = new System.Windows.Forms.RichTextBox();
            this.Run_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxA
            // 
            this.richTextBoxA.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxA.Name = "richTextBoxA";
            this.richTextBoxA.Size = new System.Drawing.Size(405, 478);
            this.richTextBoxA.TabIndex = 0;
            this.richTextBoxA.Text = "";
            // 
            // richTextBoxB
            // 
            this.richTextBoxB.Location = new System.Drawing.Point(540, 12);
            this.richTextBoxB.Name = "richTextBoxB";
            this.richTextBoxB.Size = new System.Drawing.Size(420, 478);
            this.richTextBoxB.TabIndex = 1;
            this.richTextBoxB.Text = "";
            // 
            // Run_bt
            // 
            this.Run_bt.Location = new System.Drawing.Point(442, 448);
            this.Run_bt.Name = "Run_bt";
            this.Run_bt.Size = new System.Drawing.Size(75, 23);
            this.Run_bt.TabIndex = 2;
            this.Run_bt.Text = "button1";
            this.Run_bt.UseVisualStyleBackColor = true;
            this.Run_bt.Click += new System.EventHandler(this.Run_bt_Click);
            // 
            // EaMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 502);
            this.Controls.Add(this.Run_bt);
            this.Controls.Add(this.richTextBoxB);
            this.Controls.Add(this.richTextBoxA);
            this.Name = "EaMForm";
            this.Text = "EaMForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxA;
        private System.Windows.Forms.RichTextBox richTextBoxB;
        private System.Windows.Forms.Button Run_bt;
    }
}