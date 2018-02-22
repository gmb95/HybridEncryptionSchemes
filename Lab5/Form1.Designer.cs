namespace Lab5
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
            this.Hybrid = new System.Windows.Forms.Button();
            this.EaM_bt = new System.Windows.Forms.Button();
            this.HybridM_bt = new System.Windows.Forms.Button();
            this.OCB_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Hybrid
            // 
            this.Hybrid.Location = new System.Drawing.Point(93, 34);
            this.Hybrid.Name = "Hybrid";
            this.Hybrid.Size = new System.Drawing.Size(75, 23);
            this.Hybrid.TabIndex = 0;
            this.Hybrid.Text = "Hybrid";
            this.Hybrid.UseVisualStyleBackColor = true;
            this.Hybrid.Click += new System.EventHandler(this.Hybrid_Click);
            // 
            // EaM_bt
            // 
            this.EaM_bt.Location = new System.Drawing.Point(93, 128);
            this.EaM_bt.Name = "EaM_bt";
            this.EaM_bt.Size = new System.Drawing.Size(75, 23);
            this.EaM_bt.TabIndex = 1;
            this.EaM_bt.Text = "EaM";
            this.EaM_bt.UseVisualStyleBackColor = true;
            this.EaM_bt.Click += new System.EventHandler(this.EaM_bt_Click);
            // 
            // HybridM_bt
            // 
            this.HybridM_bt.Location = new System.Drawing.Point(93, 79);
            this.HybridM_bt.Name = "HybridM_bt";
            this.HybridM_bt.Size = new System.Drawing.Size(75, 23);
            this.HybridM_bt.TabIndex = 2;
            this.HybridM_bt.Text = "HybridM";
            this.HybridM_bt.UseVisualStyleBackColor = true;
            this.HybridM_bt.Click += new System.EventHandler(this.HybridM_bt_Click);
            // 
            // OCB_bt
            // 
            this.OCB_bt.Location = new System.Drawing.Point(93, 174);
            this.OCB_bt.Name = "OCB_bt";
            this.OCB_bt.Size = new System.Drawing.Size(75, 23);
            this.OCB_bt.TabIndex = 3;
            this.OCB_bt.Text = "OCB";
            this.OCB_bt.UseVisualStyleBackColor = true;
            this.OCB_bt.Click += new System.EventHandler(this.OCB_bt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.OCB_bt);
            this.Controls.Add(this.HybridM_bt);
            this.Controls.Add(this.EaM_bt);
            this.Controls.Add(this.Hybrid);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Hybrid;
        private System.Windows.Forms.Button EaM_bt;
        private System.Windows.Forms.Button HybridM_bt;
        private System.Windows.Forms.Button OCB_bt;
    }
}

