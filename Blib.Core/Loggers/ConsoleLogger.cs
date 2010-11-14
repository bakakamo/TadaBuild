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
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger()
        {
        }

        public ConsoleLogger(string options)
        {
        }

        public override void Log(LogLevel level, string message)
        {
            Console.WriteLine(message);
        }

        public override void Finish(bool success)
        {
            Console.WriteLine(success ? "Build successful" : "Build failed!");
        }

        public override void BeginConfiguration(Configuration configuration)
        {
            Console.WriteLine(configuration.Name);
        }

        public override void BeginTarget(Target target)
        {
            Console.WriteLine(target.Name);
        }

        public override void BeginTask(Task task)
        {
            Console.WriteLine(task.Name);
        }
    }
}
