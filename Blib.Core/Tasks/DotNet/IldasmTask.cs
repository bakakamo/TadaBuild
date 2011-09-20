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
    public class IldasmTask : BaseExecTask
    {
        protected override string GetProgram()
        {
            return System.IO.Path.Combine(ParentProject.SdkPath, @"bin\ildasm.exe");
        }

        protected override string GetArgs()
        {
            StringBuilder args = new StringBuilder();
            args.Append("/nobar");
            if (UTF8)
            {
                args.Append(" /utf8");
            }
            if (All)
            {
                args.Append(" /all");
            }
            args.AppendFormat(" /output=\"{0}\"", System.IO.Path.Combine(ToDir, System.IO.Path.ChangeExtension(File.Name, ".il")));
            args.AppendFormat(" \"{0}\"", File);
            return args.ToString();
        }

        public bool UTF8 { get; set; }
        public bool All { get; set; }
        [Required]
        public DirectoryPath ToDir { get; set; }
        [Required]
        public FilePath File { get; set; }
    }
}
