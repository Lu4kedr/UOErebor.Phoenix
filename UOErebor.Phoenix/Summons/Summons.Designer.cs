namespace Phoenix.Plugins.Summons
{
    partial class Summons
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
            this.oneTargetDetails1 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.oneTargetDetails2 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.oneTargetDetails3 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.oneTargetDetails5 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.oneTargetDetails4 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.oneTargetDetails0 = new Phoenix.Plugins.Summons.OneTargetDetails();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // oneTargetDetails1
            // 
            this.oneTargetDetails1.Location = new System.Drawing.Point(5, 148);
            this.oneTargetDetails1.Name = "oneTargetDetails1";
            this.oneTargetDetails1.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails1.TabIndex = 0;
            // 
            // oneTargetDetails2
            // 
            this.oneTargetDetails2.Location = new System.Drawing.Point(5, 248);
            this.oneTargetDetails2.Name = "oneTargetDetails2";
            this.oneTargetDetails2.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails2.TabIndex = 1;
            // 
            // oneTargetDetails3
            // 
            this.oneTargetDetails3.Location = new System.Drawing.Point(5, 348);
            this.oneTargetDetails3.Name = "oneTargetDetails3";
            this.oneTargetDetails3.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails3.TabIndex = 2;
            // 
            // oneTargetDetails5
            // 
            this.oneTargetDetails5.Location = new System.Drawing.Point(5, 546);
            this.oneTargetDetails5.Name = "oneTargetDetails5";
            this.oneTargetDetails5.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails5.TabIndex = 4;
            // 
            // oneTargetDetails4
            // 
            this.oneTargetDetails4.Location = new System.Drawing.Point(5, 446);
            this.oneTargetDetails4.Name = "oneTargetDetails4";
            this.oneTargetDetails4.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails4.TabIndex = 3;
            // 
            // oneTargetDetails0
            // 
            this.oneTargetDetails0.Location = new System.Drawing.Point(5, 12);
            this.oneTargetDetails0.Name = "oneTargetDetails0";
            this.oneTargetDetails0.Size = new System.Drawing.Size(207, 94);
            this.oneTargetDetails0.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(181, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 41);
            this.label1.TabIndex = 6;
            // 
            // Summons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(222, 640);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.oneTargetDetails0);
            this.Controls.Add(this.oneTargetDetails5);
            this.Controls.Add(this.oneTargetDetails4);
            this.Controls.Add(this.oneTargetDetails3);
            this.Controls.Add(this.oneTargetDetails2);
            this.Controls.Add(this.oneTargetDetails1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Summons";
            this.Opacity = 0.6D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.afkatt_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.afkatt_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.afkatt_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.afkatt_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private OneTargetDetails oneTargetDetails1;
        private OneTargetDetails oneTargetDetails2;
        private OneTargetDetails oneTargetDetails3;
        private OneTargetDetails oneTargetDetails5;
        private OneTargetDetails oneTargetDetails4;
        private OneTargetDetails oneTargetDetails0;
        private System.Windows.Forms.Label label1;
    }
}