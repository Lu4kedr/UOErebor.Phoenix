namespace Phoenix.Plugins.Summons
{
    partial class OneTargetDetails
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_DIsp = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_hp = new System.Windows.Forms.Label();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.btn_Rename = new System.Windows.Forms.Button();
            this.lbl_ActualHP = new System.Windows.Forms.Label();
            this.lbl_Dist = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // btn_DIsp
            // 
            this.btn_DIsp.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_DIsp.ForeColor = System.Drawing.Color.DarkRed;
            this.btn_DIsp.Location = new System.Drawing.Point(178, 33);
            this.btn_DIsp.Name = "btn_DIsp";
            this.btn_DIsp.Size = new System.Drawing.Size(25, 25);
            this.btn_DIsp.TabIndex = 0;
            this.btn_DIsp.Text = "D";
            this.btn_DIsp.UseVisualStyleBackColor = true;
            this.btn_DIsp.Click += new System.EventHandler(this.btn_DIsp_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(49, 33);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(77, 25);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 1;
            // 
            // lbl_hp
            // 
            this.lbl_hp.Location = new System.Drawing.Point(7, 33);
            this.lbl_hp.Name = "lbl_hp";
            this.lbl_hp.Size = new System.Drawing.Size(36, 25);
            this.lbl_hp.TabIndex = 2;
            this.lbl_hp.Text = "Hits:";
            this.lbl_hp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Name
            // 
            this.lbl_Name.Location = new System.Drawing.Point(7, 5);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(165, 25);
            this.lbl_Name.TabIndex = 3;
            this.lbl_Name.Text = "label1";
            this.lbl_Name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_Rename
            // 
            this.btn_Rename.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_Rename.ForeColor = System.Drawing.Color.DarkRed;
            this.btn_Rename.Location = new System.Drawing.Point(178, 66);
            this.btn_Rename.Name = "btn_Rename";
            this.btn_Rename.Size = new System.Drawing.Size(25, 25);
            this.btn_Rename.TabIndex = 4;
            this.btn_Rename.Text = "R";
            this.btn_Rename.UseVisualStyleBackColor = true;
            this.btn_Rename.Click += new System.EventHandler(this.btn_Rename_Click);
            // 
            // lbl_ActualHP
            // 
            this.lbl_ActualHP.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lbl_ActualHP.Location = new System.Drawing.Point(132, 35);
            this.lbl_ActualHP.Name = "lbl_ActualHP";
            this.lbl_ActualHP.Size = new System.Drawing.Size(40, 20);
            this.lbl_ActualHP.TabIndex = 5;
            this.lbl_ActualHP.Text = "100";
            this.lbl_ActualHP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbl_Dist
            // 
            this.lbl_Dist.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lbl_Dist.Location = new System.Drawing.Point(132, 66);
            this.lbl_Dist.Name = "lbl_Dist";
            this.lbl_Dist.Size = new System.Drawing.Size(30, 20);
            this.lbl_Dist.TabIndex = 8;
            this.lbl_Dist.Text = "100";
            this.lbl_Dist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Dist:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(49, 64);
            this.progressBar2.Maximum = 18;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(77, 25);
            this.progressBar2.Step = 1;
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 6;
            // 
            // OneTargetDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_Dist);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.lbl_ActualHP);
            this.Controls.Add(this.btn_Rename);
            this.Controls.Add(this.lbl_Name);
            this.Controls.Add(this.lbl_hp);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_DIsp);
            this.Name = "OneTargetDetails";
            this.Size = new System.Drawing.Size(207, 94);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_DIsp;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbl_hp;
        private System.Windows.Forms.Label lbl_Name;
        private System.Windows.Forms.Button btn_Rename;
        private System.Windows.Forms.Label lbl_ActualHP;
        private System.Windows.Forms.Label lbl_Dist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar2;
    }
}
