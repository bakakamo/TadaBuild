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
using Tada.Types;

namespace Tada.Tasks.IO
{
    using Tada.Types;

    public class MoveTask : BaseFileTransferTask
    {
        public MoveTask(BaseFileSet files)
            : base(files)
        {
        }

        protected override void TransferFile(string source, string target)
        {
            Log(_count == 1 ? LogLevel.Info : LogLevel.Verbose, "Moving file \"{0}\" to \"{1}\"...", source, target);

            System.IO.File.Move(source, target);
        }

        protected override void LogCount()
        {
            if (_count == 0)
            {
                Log(LogLevel.Warning, "No files to move!");
            }
            else
            {
                Log(_count == 1 ? LogLevel.Verbose : LogLevel.Info, "Moving {0} items...", _count);
            }
        }
    }
}
