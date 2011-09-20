// Copyright 2010, 2011 Bastien Hofmann <kamo@cfagn.net>
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
using System.Text;
using System.Reflection;
using Microsoft.Win32;
using System.IO;

namespace Blib
{
    public class Project : ExecutableElement
    {
        public Project(params Configuration[] configurations)
        {
            BeginElement(this);
            if (Environment.Version.Major >= 4)
            {
                TargetFramework = Framework.DotNet_4_0;
            }
            else
            {
                TargetFramework = Framework.DotNet_2_0;
            }
            Add(configurations);
        }

        #region private members

        private List<Configuration> _configurations = new List<Configuration>();
        private Framework _targetFramework;
        private string _targetFrameworkToolsPath;
        private string _sdkPath;

        private int CompareConfigurations(Configuration x, Configuration y)
        {
            return x.Id.CompareTo(y.Id);
        }

        private string TrimSdkVersion(string version)
        {
            int dots = 0;
            StringBuilder result = new StringBuilder();
            foreach (char ch in version)
            {
                if (char.IsDigit(ch))
                {
                    result.Append(ch);
                }
                else
                    if (ch == '.')
                    {
                        dots++;
                        if (dots <= 3)
                        {
                            result.Append(ch);
                        }
                        else
                        {
                            break;
                        }
                    }
            }
            return result.ToString();
        }

        private void AddConfiguration(Configuration configuration)
        {
            _configurations.Add(configuration);
            configuration.Parent = this;
            configuration.Initialize();
        }

        #endregion

        #region protected members

        protected void Add(params Configuration[] configurations)
        {
            foreach (Configuration configuration in configurations)
            {
                AddConfiguration(configuration);
            }
        }

        protected C Add<C>(C configuration)
            where C : Configuration
        {
            AddConfiguration(configuration);
            return configuration;
        }

        protected C Add<C>(ref C configuration)
            where C : Configuration, new()
        {
            configuration = new C();
            AddConfiguration(configuration);
            return configuration;
        }

        #endregion

        #region public members

        public virtual IEnumerable<Configuration> Configurations
        {
            get { return _configurations; }
        }

        public override Project ParentProject
        {
            get { return this; }
        }

        public Framework TargetFramework
        {
            get { return _targetFramework; }
            set
            {
                if (_targetFramework != value)
                {
                    _targetFramework = value;
                    _targetFrameworkToolsPath = null;
                }
            }
        }

        public string TargetFrameworkToolsPath
        {
            get
            {
                if (_targetFrameworkToolsPath == null)
                {
                    RegistryKey netFrameworkKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NetFramework", false);
                    string installRoot = netFrameworkKey.GetValue("InstallRoot").ToString();

                    string prefix;
                    switch (TargetFramework)
                    {
                        case Framework.DotNet_4_0:
                            prefix = "v4.";
                            break;
                        default:
                            prefix = "v2.";
                            break;
                    }
                    foreach (DirectoryInfo dir in new DirectoryInfo(installRoot).GetDirectories())
                    {
                        if (dir.Name.StartsWith(prefix))
                        {
                            _targetFrameworkToolsPath = dir.FullName;
                            break;
                        }
                    }
                }
                return _targetFrameworkToolsPath;
            }
        }

        public string SdkPath
        {
            get
            {
                if (_sdkPath == null)
                {
                    Version maxVersion = null;

                    RegistryKey sdksKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows", false);
                    foreach (string keyName in sdksKey.GetSubKeyNames())
                    {
                        RegistryKey sdkKey = sdksKey.OpenSubKey(keyName, false);
                        Version sdkVersion = new Version(TrimSdkVersion(sdkKey.GetValue("ProductVersion").ToString()));
                        if (maxVersion == null || sdkVersion > maxVersion)
                        {
                            maxVersion = sdkVersion;
                            _sdkPath = sdkKey.GetValue("InstallationFolder").ToString();
                        }
                    }
                }
                return _sdkPath;
            }
        }

        public string Basedir
        {
            get { return base.InternalBasedir; }
            set { base.InternalBasedir = value; }
        }

        public virtual void Initialize()
        {
            // initialize properties here, before the runner overrides them with command line parameters
        }

        #endregion
    }

    public enum Framework
    {
        DotNet_1_0,
        DotNet_1_1,
        DotNet_2_0,
        DotNet_3_0,
        DotNet_3_5,
        DotNet_4_0
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class BuildOptionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class BuildOptionSuggestedValuesAttribute : Attribute
    {
        public BuildOptionSuggestedValuesAttribute(params object[] values)
        {
            Values = values;
        }

        public object[] Values { get; private set; }
    }
}
