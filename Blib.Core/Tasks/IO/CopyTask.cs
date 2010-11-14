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

namespace Blib.Tasks.IO
{
    public class CopyTask : BaseFileTransferTask
    {
        public CopyTask(BaseFileSet files)
            : base(files)
        {
        }

        protected override void TransferFile(string source, string target)
        {
            Log(_count == 1 ? LogLevel.Info : LogLevel.Verbose, "Copying file \"{0}\" to \"{1}\"...", source, target);

            System.IO.File.Copy(source, target, true);
        }

        protected override void LogCount()
        {
            if (_count == 0)
            {
                Log(Blib.LogLevel.Warning, "No files to copy!");
            }
            else
            {
                Log(_count == 1 ? LogLevel.Verbose : LogLevel.Info, "Copying {0} item(s)...", _count);
            }
        }
    }
}
