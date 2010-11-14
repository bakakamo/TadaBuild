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
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tpOptions = new System.Windows.Forms.TabPage();
            this.tlpTargets = new System.Windows.Forms.TableLayoutPanel();
            this.btnRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAssembly = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbConfigurations = new System.Windows.Forms.ComboBox();
            this.tvTargets = new System.Windows.Forms.TreeView();
            this.tlpProjectOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tlpConfigurationOptions = new System.Windows.Forms.TableLayoutPanel();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.lblTargets = new System.Windows.Forms.Label();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.tcTabs.SuspendLayout();
            this.tpOptions.SuspendLayout();
            this.tlpTargets.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcTabs
            // 
            this.tcTabs.Controls.Add(this.tpOptions);
            this.tcTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTabs.Location = new System.Drawing.Point(0, 0);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(682, 439);
            this.tcTabs.TabIndex = 0;
            this.tcTabs.SelectedIndexChanged += new System.EventHandler(this.tcTabs_SelectedIndexChanged);
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.tlpTargets);
            this.tpOptions.Location = new System.Drawing.Point(4, 22);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpOptions.Size = new System.Drawing.Size(674, 413);
            this.tpOptions.TabIndex = 0;
            this.tpOptions.Text = "Options";
            this.tpOptions.UseVisualStyleBackColor = true;
            // 
            // tlpTargets
            // 
            this.tlpTargets.ColumnCount = 3;
            this.tlpTargets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTargets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTargets.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpTargets.Controls.Add(this.btnRun, 2, 6);
            this.tlpTargets.Controls.Add(this.label1, 0, 0);
            this.tlpTargets.Controls.Add(this.tbAssembly, 1, 0);
            this.tlpTargets.Controls.Add(this.btnBrowse, 2, 0);
            this.tlpTargets.Controls.Add(this.label2, 0, 2);
            this.tlpTargets.Controls.Add(this.cbConfigurations, 1, 2);
            this.tlpTargets.Controls.Add(this.tvTargets, 1, 4);
            this.tlpTargets.Controls.Add(this.tlpProjectOptions, 1, 1);
            this.tlpTargets.Controls.Add(this.tlpConfigurationOptions, 1, 3);
            this.tlpTargets.Controls.Add(this.btnCheckAll, 2, 4);
            this.tlpTargets.Controls.Add(this.lblTargets, 0, 4);
            this.tlpTargets.Controls.Add(this.btnUncheckAll, 2, 5);
            this.tlpTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTargets.Location = new System.Drawing.Point(3, 3);
            this.tlpTargets.Name = "tlpTargets";
            this.tlpTargets.RowCount = 7;
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTargets.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpTargets.Size = new System.Drawing.Size(668, 407);
            this.tlpTargets.TabIndex = 0;
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(590, 381);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
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
            this.tbAssembly.Size = new System.Drawing.Size(503, 20);
            this.tbAssembly.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(590, 2);
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
            this.cbConfigurations.Size = new System.Drawing.Size(503, 21);
            this.cbConfigurations.TabIndex = 4;
            this.cbConfigurations.SelectedIndexChanged += new System.EventHandler(this.cbConfigurations_SelectedIndexChanged);
            // 
            // tvTargets
            // 
            this.tvTargets.CheckBoxes = true;
            this.tvTargets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTargets.Location = new System.Drawing.Point(81, 70);
            this.tvTargets.Name = "tvTargets";
            this.tlpTargets.SetRowSpan(this.tvTargets, 2);
            this.tvTargets.Size = new System.Drawing.Size(503, 305);
            this.tvTargets.TabIndex = 5;
            this.tvTargets.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvTargets_AfterCheck);
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
            this.tlpProjectOptions.Size = new System.Drawing.Size(503, 0);
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
            this.tlpConfigurationOptions.Size = new System.Drawing.Size(503, 0);
            this.tlpConfigurationOptions.TabIndex = 8;
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(590, 70);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnCheckAll.TabIndex = 6;
            this.btnCheckAll.Text = "Check all";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // lblTargets
            // 
            this.lblTargets.AutoSize = true;
            this.lblTargets.Location = new System.Drawing.Point(3, 73);
            this.lblTargets.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblTargets.Name = "lblTargets";
            this.lblTargets.Size = new System.Drawing.Size(46, 13);
            this.lblTargets.TabIndex = 9;
            this.lblTargets.Text = "Targets:";
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Location = new System.Drawing.Point(590, 99);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.btnUncheckAll.TabIndex = 7;
            this.btnUncheckAll.Text = "Uncheck all";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 439);
            this.Controls.Add(this.tcTabs);
            this.Name = "MainForm";
            this.Text = "Blib Gui";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tcTabs.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
            this.tlpTargets.ResumeLayout(false);
            this.tlpTargets.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tpOptions;
        private System.Windows.Forms.TableLayoutPanel tlpTargets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAssembly;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbConfigurations;
        private System.Windows.Forms.TreeView tvTargets;
        private System.Windows.Forms.TableLayoutPanel tlpProjectOptions;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.TableLayoutPanel tlpConfigurationOptions;
        private System.Windows.Forms.Label lblTargets;
    }
}

