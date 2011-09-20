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
using Blib.Types;

namespace Blib.Tasks.DotNet
{
    public class MSBuildTask : BaseExecTask
    {
        public MSBuildTask()
        {
            Verbosity = MSBuildVerbosity.Normal;
            Properties = new Dictionary<string, string>();
        }

        protected override string GetProgram()
        {
            return System.IO.Path.Combine(ParentProject.TargetFrameworkToolsPath, "msbuild.exe");
        }

        protected override string GetArgs()
        {
            StringBuilder args = new StringBuilder();
            args.AppendFormat("/nologo /verbosity:{0}", Verbosity.ToString().ToLowerInvariant());

            Dictionary<string, string> tempProperties = new Dictionary<string, string>(Properties);

            if (!string.IsNullOrEmpty(Configuration))
            {
                tempProperties["Configuration"] = Configuration;
            }
            if (!string.IsNullOrEmpty(Platform))
            {
                tempProperties["Platform"] = Platform;
            }

            foreach (var property in tempProperties)
            {
                args.AppendFormat(" /p:\"{0}\"=\"{1}\"", property.Key, property.Value);
            }

            if (!string.IsNullOrEmpty(Target))
            {
                args.AppendFormat(" /target:{0}", Target);
            }

            args.AppendFormat(" \"{0}\"", ProjectFile);
            return args.ToString();
        }

        public MSBuildVerbosity Verbosity { get; set; }

        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string Target { get; set; }
        [Required]
        public FilePath ProjectFile { get; set; }

        public Dictionary<string, string> Properties { get; private set; }
    }

    public enum MSBuildVerbosity
    {
        Quiet,
        Minimal,
        Normal,
        Detailed,
        Diagnostic
    }
}
