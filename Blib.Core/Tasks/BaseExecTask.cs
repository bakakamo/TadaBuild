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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Blib.Tasks
{
    public abstract class BaseExecTask : Task
    {
        public BaseExecTask()
        {
            LogOutput = true;
            LogError = true;
        }

        private StringBuilder _output = new StringBuilder();

        protected override void DoExecute()
        {
            string program = GetProgram();
            string args = GetArgs();

            ProcessStartInfo startInfo = new ProcessStartInfo(program, args);
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process process;

            try
            {
                process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                throw new BuildException(string.Format("Could not start external program \"{0}\" with arguments \"{1}\": {2}", program, args, ex.Message), ex);
            }


            StreamReader reader = process.StandardOutput;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _output.AppendLine(line);
                Log(LogLevel.Info, line);
            }
            if (_output.Length > Environment.NewLine.Length)  
            {
                _output.Length -= Environment.NewLine.Length;
            }

            reader = process.StandardError;
            while ((line = reader.ReadLine()) != null)
            {
                Log(LogLevel.Error, line);
            }

            while (!process.HasExited)
            {
                Thread.Sleep(10);
            }

            if (process.ExitCode != 0)
            {
                throw new BuildException(string.Format("Program exited with return with code {0} from command line: {1} {2}", process.ExitCode, program, args));
            }
        }

        protected abstract string GetArgs();

        protected abstract string GetProgram();

        public string Output
        {
            get { return _output.ToString(); }
        }

        public bool LogOutput { get; set; }
        public bool LogError { get; set; }
    }
}
