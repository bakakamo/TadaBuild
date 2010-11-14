// Copyright 2010 Bastien Hofmann <kamo@cfagn.net>
//
// This file is part of Blib.
//
// Blib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Blib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Blib.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blib;
using System.Threading;
using Blib.Loggers;
using System.Reflection;

namespace BlibGui
{
    public partial class MainForm : Form
    {
        public MainForm(string[] args)
        {
            InitializeComponent();
            _mainBrowser = AddBrowser("Runner");
            _args = args;
        }

        #region nested types

        private delegate WebBrowser AddBrowserDelegate(string name);

        private class GuiLogger : BaseHtmlLogger
        {
            public GuiLogger(MainForm owner)
            {
                _owner = owner;
            }

            private MainForm _owner;

            private void AppendOutput(object data, string output)
            {
                ((WebBrowser)data).Invoke(new AppendOutputDelegate(DoAppendOutput), data, output);
            }

            private void DoAppendOutput(object data, string output)
            {
                ((WebBrowser)data).Document.Write(output);
            }

            protected override KeyValuePair<object, BaseHtmlLogger.AppendOutputDelegate> CreateOutput(BuildThread buildThread)
            {
                WebBrowser browser;
                if (Thread.CurrentThread == _owner._runnerThread)
                {
                    browser = _owner._mainBrowser;
                }
                else
                {
                    browser = (WebBrowser)_owner.Invoke(new AddBrowserDelegate(_owner.AddBrowser), buildThread != null ? buildThread.Name : string.Format("Unknown thread: {0}", Thread.CurrentThread.Name));
                }
                KeyValuePair<object, BaseHtmlLogger.AppendOutputDelegate> result = new KeyValuePair<object, AppendOutputDelegate>(browser, AppendOutput);
                return result;
            }
        }

        private class GuiRunner : Runner
        {
            public GuiRunner(Options options)
                : base(options)
            {
            }

            protected override void Sleep()
            {
                Application.DoEvents();
            }
        }

        #endregion

        private string[] _args;
        private Options _options;
        private Runner _runner;
        private Thread _runnerThread;
        private bool _newRunnerNeeded;
        private WebBrowser _mainBrowser;
        private Dictionary<string, Control> _projectOptions = new Dictionary<string, Control>();
        private Dictionary<string, object> _projectOptionsValues = new Dictionary<string, object>();
        private Dictionary<string, Control> _configurationOptions = new Dictionary<string, Control>();
        private Dictionary<string, object> _configurationOptionsValues = new Dictionary<string, object>();
        private Dictionary<string, bool> _checkedTargets = new Dictionary<string, bool>();

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Assemblies and buildfiles (*.dll;*.blib)|*.dll;*.blib";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OpenAssemblyOrBuildFile(dialog.FileName);
                }
            }
        }

        private void OpenAssemblyOrBuildFile(string filename)
        {
            tbAssembly.Text = filename;

           _options = new Options();
           _options.Parse(new string[] { filename });
           CreateRunner();
        }

        private void CreateRunner()
        {
            _options.DisabledTargets = null;
            _runnerThread = Thread.CurrentThread;
            _runner = new GuiRunner(_options);
            _runner.AddLogger(new GuiLogger(this));
            FillProjectOptions();
            FillConfigurations();
        }

        private void FillProjectOptions()
        {
            FillOptions(_runner.Project, _projectOptions, _projectOptionsValues, tlpProjectOptions);
        }

        private void FillOptions(object item, Dictionary<string, Control> options, Dictionary<string, object> values, TableLayoutPanel panel)
        {
            tlpTargets.SuspendLayout();
            try
            {
                panel.SuspendLayout();
                try
                {
                    panel.Controls.Clear();

                    int count = 0;

                    foreach (var member in item.GetType().GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (member.GetCustomAttributes(typeof(BuildOptionAttribute), true).Length > 0)
                        {
                            count++;

                            Type type;
                            object currentValue;

                            FieldInfo field = member as FieldInfo;
                            if (field != null)
                            {
                                type = field.FieldType;
                                currentValue = field.GetValue(item);
                            }
                            else
                            {
                                PropertyInfo property = member as PropertyInfo;
                                if (property != null)
                                {
                                    type = property.PropertyType;
                                    currentValue = property.GetValue(item, null);
                                }
                                else
                                {
                                    throw new ArgumentException();
                                }
                            }

                            object previousValue;
                            if (values.TryGetValue(member.Name, out previousValue))
                            {
                                currentValue = previousValue;
                            }

                            Label label = null;
                            Control control = null;

                            if (type == typeof(bool))
                            {
                                CheckBox checkBox = new CheckBox();
                                checkBox.Text = member.Name;
                                checkBox.Checked = (bool)currentValue;
                                control = checkBox;
                            }
                            else
                            {
                                label = new Label();
                                label.Text = member.Name;
                                label.AutoSize = true;

                                if (type == typeof(int) || type == typeof(byte) || type == typeof(short) || type == typeof(long))
                                {
                                    NumericUpDown upDown = new NumericUpDown();
                                    upDown.DecimalPlaces = 0;
                                    upDown.Value = (int)currentValue;
                                    control = upDown;
                                }
                                else
                                {
                                    object[] attributes = member.GetCustomAttributes(typeof(BuildOptionSuggestedValuesAttribute), true);
                                    if (attributes.Length > 0)
                                    {
                                        ComboBox comboBox = new ComboBox();
                                        comboBox.Items.AddRange(((BuildOptionSuggestedValuesAttribute)attributes[0]).Values);
                                        comboBox.SelectedItem = currentValue;
                                        control = comboBox;
                                    }
                                    else
                                    {
                                        TextBox textBox = new TextBox();
                                        textBox.Text = currentValue == null ? string.Empty : currentValue.ToString();
                                        control = textBox;
                                    }
                                }
                            }

                            panel.RowCount = count;
                            while (panel.RowStyles.Count < count)
                            {
                                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }

                            if (label != null)
                            {
                                panel.Controls.Add(label, 0, panel.RowCount - 1);
                            }
                            control.Dock = DockStyle.Top;
                            panel.Controls.Add(control, 1, panel.RowCount - 1);

                            options[member.Name] = control;
                        }
                    }
                }
                finally
                {
                    panel.ResumeLayout(true);
                }
            }
            finally
            {
                tlpTargets.ResumeLayout(true);
            }
        }

        private WebBrowser AddBrowser(string name)
        {
            TabPage tabPage = new TabPage(name);
            tcTabs.TabPages.Add(tabPage);
            WebBrowser browser = new WebBrowser();
            browser.Dock = DockStyle.Fill;
            browser.Parent = tabPage;
            browser.Navigate("about:blank");
            while (browser.IsBusy)
            {
                Application.DoEvents();
            }
            return browser;
        }

        private void FillConfigurations()
        {
            cbConfigurations.BeginUpdate();
            try
            {
                cbConfigurations.Items.Clear();

                foreach (Configuration configuration in _runner.Configurations)
                {
                    cbConfigurations.Items.Add(configuration);
                }
            }
            finally
            {
                cbConfigurations.EndUpdate();
            }

            if (cbConfigurations.Items.Count > 0)
            {
                cbConfigurations.SelectedIndex = 0;
            }

            btnRun.Enabled = cbConfigurations.Items.Count > 0;
        }

        private void cbConfigurations_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillConfigurationOptions();
            FillTargets();
        }

        private void FillConfigurationOptions()
        {
            FillOptions((Configuration)cbConfigurations.SelectedItem, _configurationOptions, _configurationOptionsValues, tlpConfigurationOptions);
        }

        private void FillTargets()
        {
            tvTargets.BeginUpdate();
            try
            {
                tvTargets.Nodes.Clear();

                if (cbConfigurations.SelectedItem != null)
                {
                    AddTargets(tvTargets.Nodes, (Configuration)cbConfigurations.SelectedItem);
                }
            }
            finally
            {
                tvTargets.EndUpdate();
            }
        }

        private void AddTargets(TreeNodeCollection nodes, TargetGroup targetGroup)
        {
            foreach (Target target in targetGroup.Targets)
            {
                TreeNode node = nodes.Add(target.Name);
                node.Tag = target;
                bool enabled;
                if (!_checkedTargets.TryGetValue(node.FullPath, out enabled))
                {
                    enabled = target.Enabled;
                }
                node.Checked = enabled;
                if (target is TargetGroup)
                {
                    AddTargets(node.Nodes, (TargetGroup)target);
                }
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            SetChecked(tvTargets.Nodes, true);
        }

        private void SetChecked(TreeNodeCollection nodes, bool value)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = value;
                SetChecked(node.Nodes, value);
            }
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            SetChecked(tvTargets.Nodes, false);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                _checkedTargets.Clear();
                EnableTargets(tvTargets.Nodes);
                EnableControls(tpOptions.Controls, false);
                Configuration configuration = (Configuration)cbConfigurations.SelectedItem;

                SetOptions(_projectOptions, _projectOptionsValues, _options.ProjectProperties);
                SetOptions(_configurationOptions, _configurationOptionsValues, _options.ConfigurationProperties);

                while (tcTabs.TabPages.Count > 2)
                {
                    tcTabs.TabPages[tcTabs.TabPages.Count - 1].Dispose();
                }
                tcTabs.SelectedIndex = 1;
                _runner.Run(configuration);

                EnableControls(tpOptions.Controls, true);

                if (tcTabs.SelectedIndex == 0)
                {
                    CreateRunner();
                }
                else
                {
                    _newRunnerNeeded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.GetType().FullName + ' ' + ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetOptions(Dictionary<string, Control> controls, Dictionary<string, object> values, Dictionary<string, string> dictionary)
        {
            foreach (var option in controls)
            {
                values[option.Key] = GetOptionValue(option.Value);
                dictionary[option.Key] = GetOptionValue(option.Value).ToString();
            }
        }

        private object GetOptionValue(Control control)
        {
            if (control is CheckBox)
            {
                return ((CheckBox)control).Checked;
            }
            if (control is NumericUpDown)
            {
                return (int)((NumericUpDown)control).Value;
            }
            if (control is ComboBox)
            {
                if (((ComboBox)control).DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    return ((ComboBox)control).SelectedItem;
                }
                else
                {
                    return ((ComboBox)control).Text;
                }
            }
            return ((TextBox)control).Text;
        }

        private void EnableControls(Control.ControlCollection controls, bool value)
        {
            foreach (Control control in controls)
            {
                control.Enabled = value;
                EnableControls(control.Controls, value);
            }
        }

        private void EnableTargets(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                _checkedTargets[node.FullPath] = node.Checked;
                ((Target)node.Tag).Enabled = node.Checked;
                EnableTargets(node.Nodes);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_args.Length > 0)
            {
                _options = new Options();
                _options.Parse(_args);

                tbAssembly.Text = _options.BuildFile ?? _options.DllFilename;

                CreateRunner();
            }
        }

        private void tvTargets_AfterCheck(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in e.Node.Nodes)
            {
                node.Checked = e.Node.Checked;
            }
        }

        private void tcTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcTabs.SelectedIndex == 0 && _newRunnerNeeded)
            {
                _newRunnerNeeded = false;
                CreateRunner();
            }
        }
    }
}
