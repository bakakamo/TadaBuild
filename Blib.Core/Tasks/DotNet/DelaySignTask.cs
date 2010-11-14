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
using System.Text;
using Blib.Types;

namespace Blib.Tasks.DotNet
{
    public class DelaySignTask : BaseExecTask
    {
        private string _assembly;

        protected override string GetProgram()
        {
            return ParentProject.SdkPath + @"bin\sn.exe";
        }

        protected override string GetArgs()
        {
            return string.Format("-q -R \"{0}\" \"{1}\"", _assembly, KeyFile);
        }

        protected override void DoExecute()
        {
            foreach (string filename in Assemblies)
            {
                _assembly = filename;
                base.DoExecute();
            }
        }

        public FilePath KeyFile { get; set; }
        public FileSet Assemblies { get; set; }
    }
}
