namespace Lab5
{
    partial class HybridMForm
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
            this.Run_bt = new System.Windows.Forms.Button();
            this.richTextBoxA = new System.Windows.Forms.RichTextBox();
            this.richTextBoxB = new System.Windows.Forms.RichTextBox();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Run_bt
            // 
            this.Run_bt.Location = new System.Drawing.Point(391, 464);
            this.Run_bt.Name = "Run_bt";
            this.Run_bt.Size = new System.Drawing.Size(75, 23);
            this.Run_bt.TabIndex = 0;
            this.Run_bt.Text = "button1";
            this.Run_bt.UseVisualStyleBackColor = true;
            this.Run_bt.Click += new System.EventHandler(this.Run_bt_Click);
            // 
            // richTextBoxA
            // 
            this.richTextBoxA.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxA.Name = "richTextBoxA";
            this.richTextBoxA.Size = new System.Drawing.Size(373, 499);
            this.richTextBoxA.TabIndex = 1;
            this.richTextBoxA.Text = "";
            // 
            // richTextBoxB
            // 
            this.richTextBoxB.Location = new System.Drawing.Point(473, 12);
            this.richTextBoxB.Name = "richTextBoxB";
            this.richTextBoxB.Size = new System.Drawing.Size(373, 499);
            this.richTextBoxB.TabIndex = 2;
            this.richTextBoxB.Text = "";
            // 
            // comboBox
            // 
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Items.AddRange(new object[] {
            "CBC",
            "OFB",
            "CFB"});
            this.comboBox.Location = new System.Drawing.Point(391, 268);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(75, 21);
            this.comboBox.TabIndex = 3;
            // 
            // HybridMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 523);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.richTextBoxB);
            this.Controls.Add(this.richTextBoxA);
            this.Controls.Add(this.Run_bt);
            this.Name = "HybridMForm";
            this.Text = "HybridMForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Run_bt;
        private System.Windows.Forms.RichTextBox richTextBoxA;
        private System.Windows.Forms.RichTextBox richTextBoxB;
        private System.Windows.Forms.ComboBox comboBox;
    }
}