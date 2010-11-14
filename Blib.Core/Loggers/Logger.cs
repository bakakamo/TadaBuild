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

namespace Blib.Loggers
{
    public abstract class Logger
    {
        public abstract void Log(LogLevel level, string message);

        public abstract void BeginConfiguration(Configuration configuration);

        public virtual void EndConfiguration(Configuration configuration)
        {
            // nothing here
        }

        public abstract void BeginTarget(Target target);

        public virtual void EndTarget(Target target)
        {
            // nothing here
        }

        public abstract void BeginTask(Task task);

        public virtual void EndTask(Task task)
        {
            // nothing here
        }

        public abstract void Finish(bool success);

        public virtual void BeginThread(BuildThread thread)
        {
            // nothing here
        }
    }
}
