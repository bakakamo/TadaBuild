namespace BlibGui
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tpOptions = new System.Windows.Forms.TabPage();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAssembly = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbConfigurations = new System.Windows.Forms.ComboBox();
            this.tlpProjectOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tlpConfigurationOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tvTargets = new System.Windows.Forms.TreeView();
            this.ilThreadImages = new System.Windows.Forms.ImageList(this.components);
            this.ilStateImages = new System.Windows.Forms.ImageList(this.components);
            this.tsCommands = new System.Windows.Forms.ToolStrip();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tsTargets = new System.Windows.Forms.ToolStrip();
            this.btnCheckAll = new System.Windows.Forms.ToolStripButton();
            this.btnUncheckAll = new System.Windows.Forms.ToolStripButton();
            this.tcTabs.SuspendLayout();
            this.tpOptions.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.tsCommands.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tsTargets.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcTabs
            // 
            this.tcTabs.Controls.Add(this.tpOptions);
            this.tcTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTabs.Location = new System.Drawing.Point(0, 0);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(510, 504);
            this.tcTabs.TabIndex = 0;
            this.tcTabs.SelectedIndexChanged += new System.EventHandler(this.tcTabs_SelectedIndexChanged);
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.tlpOptions);
            this.tpOptions.Location = new System.Drawing.Point(4, 22);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpOptions.Size = new System.Drawing.Size(502, 478);
            this.tpOptions.TabIndex = 0;
            this.tpOptions.Text = "Options";
            this.tpOptions.UseVisualStyleBackColor = true;
            // 
            // tlpOptions
            // 
            this.tlpOptions.ColumnCount = 3;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOptions.Controls.Add(this.label1, 0, 0);
            this.tlpOptions.Controls.Add(this.tbAssembly, 1, 0);
            this.tlpOptions.Controls.Add(this.btnBrowse, 2, 0);
            this.tlpOptions.Controls.Add(this.label2, 0, 2);
            this.tlpOptions.Controls.Add(this.cbConfigurations, 1, 2);
            this.tlpOptions.Controls.Add(this.tlpProjectOptions, 1, 1);
            this.tlpOptions.Controls.Add(this.tlpConfigurationOptions, 1, 3);
            this.tlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOptions.Location = new System.Drawing.Point(3, 3);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 7;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpOptions.Size = new System.Drawing.Size(496, 472);
            this.tlpOptions.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project:";
            // 
            // tbAssembly
            // 
            this.tbAssembly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAssembly.Location = new System.Drawing.Point(81, 3);
            this.tbAssembly.Name = "tbAssembly";
            this.tbAssembly.ReadOnly = true;
            this.tbAssembly.Size = new System.Drawing.Size(331, 20);
            this.tbAssembly.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(418, 2);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Configuration:";
            // 
            // cbConfigurations
            // 
            this.cbConfigurations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbConfigurations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConfigurations.FormattingEnabled = true;
            this.cbConfigurations.Location = new System.Drawing.Point(81, 37);
            this.cbConfigurations.Name = "cbConfigurations";
            this.cbConfigurations.Size = new System.Drawing.Size(331, 21);
            this.cbConfigurations.TabIndex = 4;
            this.cbConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbConfigurations_SelectedIndexChanged);
            // 
            // tlpProjectOptions
            // 
            this.tlpProjectOptions.AutoSize = true;
            this.tlpProjectOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpProjectOptions.ColumnCount = 2;
            this.tlpProjectOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProjectOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProjectOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpProjectOptions.Location = new System.Drawing.Point(81, 31);
            this.tlpProjectOptions.Name = "tlpProjectOptions";
            this.tlpProjectOptions.RowCount = 1;
            this.tlpProjectOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpProjectOptions.Size = new System.Drawing.Size(331, 0);
            this.tlpProjectOptions.TabIndex = 2;
            // 
            // tlpConfigurationOptions
            // 
            this.tlpConfigurationOptions.AutoSize = true;
            this.tlpConfigurationOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpConfigurationOptions.ColumnCount = 2;
            this.tlpConfigurationOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpConfigurationOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpConfigurationOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpConfigurationOptions.Location = new System.Drawing.Point(81, 64);
            this.tlpConfigurationOptions.Name = "tlpConfigurationOptions";
            this.tlpConfigurationOptions.RowCount = 1;
            this.tlpConfigurationOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpConfigurationOptions.Size = new System.Drawing.Size(331, 0);
            this.tlpConfigurationOptions.TabIndex = 8;
            // 
            // tvTargets
            // 
            this.tvTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTargets.ImageIndex = 0;
            this.tvTargets.ImageList = this.ilThreadImages;
            this.tvTargets.Location = new System.Drawing.Point(0, 0);
            this.tvTargets.Name = "tvTargets";
            this.tvTargets.SelectedImageIndex = 0;
            this.tvTargets.Size = new System.Drawing.Size(257, 479);
            this.tvTargets.StateImageList = this.ilStateImages;
            this.tvTargets.TabIndex = 5;
            this.tvTargets.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvTargets_NodeMouseClick);
            // 
            // ilThreadImages
            // 
            this.ilThreadImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilThreadImages.ImageSize = new System.Drawing.Size(16, 16);
            this.ilThreadImages.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilStateImages
            // 
            this.ilStateImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStateImages.ImageStream")));
            this.ilStateImages.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStateImages.Images.SetKeyName(0, "Unchecked.png");
            this.ilStateImages.Images.SetKeyName(1, "Checked.png");
            this.ilStateImages.Images.SetKeyName(2, "Indeterminate.png");
            this.ilStateImages.Images.SetKeyName(3, "Run");
            this.ilStateImages.Images.SetKeyName(4, "Error");
            this.ilStateImages.Images.SetKeyName(5, "Warning");
            this.ilStateImages.Images.SetKeyName(6, "Success");
            // 
            // tsCommands
            // 
            this.tsCommands.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRun});
            this.tsCommands.Location = new System.Drawing.Point(0, 0);
            this.tsCommands.Name = "tsCommands";
            this.tsCommands.Size = new System.Drawing.Size(771, 25);
            this.tsCommands.TabIndex = 1;
            this.tsCommands.Text = "toolStrip1";
            // 
            // btnRun
            // 
            this.btnRun.Image = global::BlibGui.Properties.Resources.Run;
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(48, 22);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvTargets);
            this.splitContainer1.Panel1.Controls.Add(this.tsTargets);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tcTabs);
            this.splitContainer1.Size = new System.Drawing.Size(771, 504);
            this.splitContainer1.SplitterDistance = 257;
            this.splitContainer1.TabIndex = 2;
            // 
            // tsTargets
            // 
            this.tsTargets.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsTargets.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsTargets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCheckAll,
            this.btnUncheckAll});
            this.tsTargets.Location = new System.Drawing.Point(0, 479);
            this.tsTargets.Name = "tsTargets";
            this.tsTargets.Size = new System.Drawing.Size(257, 25);
            this.tsTargets.TabIndex = 6;
            this.tsTargets.Text = "toolStrip1";
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Image = global::BlibGui.Properties.Resources.Checked;
            this.btnCheckAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 22);
            this.btnCheckAll.Text = "Check all";
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Image = global::BlibGui.Properties.Resources.Unchecked;
            this.btnUncheckAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(88, 22);
            this.btnUncheckAll.Text = "Uncheck all";
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 529);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tsCommands);
            this.Name = "MainForm";
            this.Text = "Blib Gui";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tcTabs.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.tsCommands.ResumeLayout(false);
            this.tsCommands.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tsTargets.ResumeLayout(false);
            this.tsTargets.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tpOptions;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAssembly;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbConfigurations;
        private System.Windows.Forms.TreeView tvTargets;
        private System.Windows.Forms.TableLayoutPanel tlpProjectOptions;
        private System.Windows.Forms.TableLayoutPanel tlpConfigurationOptions;
        private System.Windows.Forms.ToolStrip tsCommands;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip tsTargets;
        private System.Windows.Forms.ToolStripButton btnCheckAll;
        private System.Windows.Forms.ToolStripButton btnUncheckAll;
        private System.Windows.Forms.ImageList ilThreadImages;
        private System.Windows.Forms.ImageList ilStateImages;
    }
}

