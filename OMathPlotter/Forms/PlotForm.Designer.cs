namespace OMathPlotter.Forms
{
    partial class PlotForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.plotTab = new System.Windows.Forms.TabPage();
            this.treeTab = new System.Windows.Forms.TabPage();
            this.xmlTab = new System.Windows.Forms.TabPage();
            this.expressionComboBox = new System.Windows.Forms.ComboBox();
            this.openOXMLFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(634, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileMenuItem,
            this.closeFileMenuItem,
            this.toolStripSeparator1,
            this.exitMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.Size = new System.Drawing.Size(124, 22);
            this.openFileMenuItem.Text = "Open";
            this.openFileMenuItem.Click += new System.EventHandler(this.openFileMenuItem_Click);
            // 
            // closeFileMenuItem
            // 
            this.closeFileMenuItem.Name = "closeFileMenuItem";
            this.closeFileMenuItem.Size = new System.Drawing.Size(124, 22);
            this.closeFileMenuItem.Text = "Close File";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(124, 22);
            this.exitMenuItem.Text = "Exit";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.plotTab);
            this.tabControl.Controls.Add(this.treeTab);
            this.tabControl.Controls.Add(this.xmlTab);
            this.tabControl.Location = new System.Drawing.Point(4, 27);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(626, 354);
            this.tabControl.TabIndex = 2;
            // 
            // plotTab
            // 
            this.plotTab.Location = new System.Drawing.Point(4, 22);
            this.plotTab.Name = "plotTab";
            this.plotTab.Padding = new System.Windows.Forms.Padding(3);
            this.plotTab.Size = new System.Drawing.Size(618, 328);
            this.plotTab.TabIndex = 0;
            this.plotTab.Text = "Plot";
            this.plotTab.UseVisualStyleBackColor = true;
            // 
            // treeTab
            // 
            this.treeTab.Location = new System.Drawing.Point(4, 22);
            this.treeTab.Name = "treeTab";
            this.treeTab.Padding = new System.Windows.Forms.Padding(3);
            this.treeTab.Size = new System.Drawing.Size(618, 328);
            this.treeTab.TabIndex = 1;
            this.treeTab.Text = "Syntax Tree";
            this.treeTab.UseVisualStyleBackColor = true;
            // 
            // xmlTab
            // 
            this.xmlTab.Location = new System.Drawing.Point(4, 22);
            this.xmlTab.Name = "xmlTab";
            this.xmlTab.Padding = new System.Windows.Forms.Padding(3);
            this.xmlTab.Size = new System.Drawing.Size(618, 328);
            this.xmlTab.TabIndex = 2;
            this.xmlTab.Text = "XML";
            this.xmlTab.UseVisualStyleBackColor = true;
            // 
            // expressionComboBox
            // 
            this.expressionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expressionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.expressionComboBox.FormattingEnabled = true;
            this.expressionComboBox.Location = new System.Drawing.Point(4, 387);
            this.expressionComboBox.Name = "expressionComboBox";
            this.expressionComboBox.Size = new System.Drawing.Size(626, 21);
            this.expressionComboBox.TabIndex = 3;
            // 
            // openOXMLFileDialog
            // 
            this.openOXMLFileDialog.FileName = "openFileDialog1";
            // 
            // PlotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 411);
            this.Controls.Add(this.expressionComboBox);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.Name = "PlotForm";
            this.Text = "PlotForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage plotTab;
        private System.Windows.Forms.TabPage treeTab;
        private System.Windows.Forms.TabPage xmlTab;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeFileMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ComboBox expressionComboBox;
        private System.Windows.Forms.OpenFileDialog openOXMLFileDialog;
    }
}