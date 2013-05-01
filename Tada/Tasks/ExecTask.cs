// Copyright 2010-2013 Bastien Hofmann <bastien@tadabuild.net>
//
// This file is part of TadaBuild.
//
// TadaBuild is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// TadaBuild is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with TadaBuild.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Tada.Types;

namespace Tada.Tasks
{
    using Tada.Types;

    public class ExecTask : BaseExecTask
    {
        public ExecTask(FilePath program)
        {
            Program = program;
            Args = string.Empty;
        }

        protected override string GetProgram()
        {
            return Program;
        }

        protected override string GetArgs()
        {
            return Args;
        }

        protected override string GetWorkingDir()
        {
            if (WorkingDir != null)
            {
                return WorkingDir;
            }
            return base.GetWorkingDir();
        }

        public FilePath Program { get; set; }

        public string Args { get; set; }

        public DirectoryPath WorkingDir { get; set; }
    }
}
