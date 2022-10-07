using System.ComponentModel;

namespace BatchCompress
{
    partial class ConfigMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.Apply = new System.Windows.Forms.Button();
            this.openMenu = new System.Windows.Forms.CheckBox();
            this.useDedicatedOutputPath = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Apply
            // 
            this.Apply.Location = new System.Drawing.Point(256, 641);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(267, 83);
            this.Apply.TabIndex = 0;
            this.Apply.Text = "Apply";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.button1_Click);
            // 
            // openMenu
            // 
            this.openMenu.Location = new System.Drawing.Point(44, 33);
            this.openMenu.Name = "openMenu";
            this.openMenu.Size = new System.Drawing.Size(293, 56);
            this.openMenu.TabIndex = 1;
            this.openMenu.Text = "Open this menu on start";
            this.openMenu.UseVisualStyleBackColor = true;
            // 
            // useDedicatedOutputPath
            // 
            this.useDedicatedOutputPath.Location = new System.Drawing.Point(44, 95);
            this.useDedicatedOutputPath.Name = "useDedicatedOutputPath";
            this.useDedicatedOutputPath.Size = new System.Drawing.Size(355, 86);
            this.useDedicatedOutputPath.TabIndex = 2;
            this.useDedicatedOutputPath.Text = "Use Dedicated Output Path";
            this.useDedicatedOutputPath.UseVisualStyleBackColor = true;
            // 
            // ConfigMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 736);
            this.ControlBox = false;
            this.Controls.Add(this.useDedicatedOutputPath);
            this.Controls.Add(this.openMenu);
            this.Controls.Add(this.Apply);
            this.Name = "ConfigMenu";
            this.Text = "ConfigMenu";
            this.Load += new System.EventHandler(this.ConfigMenu_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox useDedicatedOutputPath;

        private System.Windows.Forms.CheckBox openMenu;

        private System.Windows.Forms.Button Apply;

        #endregion
    }
}