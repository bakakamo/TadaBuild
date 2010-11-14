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
using Blib.Tools;

namespace Blib.Tasks.IO
{
    public enum Overwrite
    {
        IfNewer,
        Always,
        Never
    }

    public abstract class BaseFileTransferTask : Task
    {
        public BaseFileTransferTask(BaseFileSet files)
        {
            Overwrite = Tasks.IO.Overwrite.IfNewer;
            Files = files;
            if (files != null)
            {
                Flatten = files.HasSingleItem;
            }
        }

        private void TransferFileIfOverwrite(string source, string target)
        {
            if (System.IO.File.Exists(target))
            {
                switch (Overwrite)
                {
                    case Overwrite.IfNewer:
                        if (System.IO.File.GetLastWriteTimeUtc(target) > System.IO.File.GetLastWriteTimeUtc(source))
                        {
                            Log(LogLevel.Info, "Skipping target \"{0}\" because it is newer than then source!", target);
                            return;
                        }
                        break;
                    case Overwrite.Always:
                        break;
                    case Overwrite.Never:
                        Log(LogLevel.Info, "Skipping target \"{0}\" because it already exists!", target);
                        return;
                    default:
                        throw new ArgumentException();
                }
                System.IO.File.Delete(target);
            }

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(target));
            TransferFile(source, target);
        }

        protected int _count;

        protected abstract void LogCount();

        protected override void DoExecute()
        {
            if (ToDir == null && ToFile == null)
            {
                throw new BuildArgumentMissingException("Both ToDir and ToFile arguments are missing, please specify one!", null);
            }
            else
                if (ToDir != null && ToFile != null)
                {
                    throw new BuildException("Both ToDir and ToFile arguments are set, please set only one!");
                }

            _count = IO.CountItems(Files);
            LogCount();

            if (ToFile != null)
            {
                string source = Enumerable.GetFirst(Files);
                if (source == null)
                {
                    throw new BuildArgumentException("Source fileset is empty!");
                }
                TransferFileIfOverwrite(source, ToFile.FullName);
            }
            else
            {
                string toDir = ToDir.FullName;

                string baseDir = IO.IncludeTrailingPathDelimiter(Files.Basedir);

                foreach (string filename in Files)
                {
                    string targetFilename;

                    if (Flatten || !filename.StartsWith(baseDir, StringComparison.CurrentCultureIgnoreCase))
                    {
                        targetFilename = System.IO.Path.Combine(toDir, System.IO.Path.GetFileName(filename));
                    }
                    else
                    {
                        targetFilename = System.IO.Path.Combine(toDir, filename.Substring(baseDir.Length));
                    }

                    TransferFileIfOverwrite(filename, targetFilename);
                }
            }
        }

        protected abstract void TransferFile(string source, string target);

        [Required]
        public BaseFileSet Files { get; set; }
        public DirectoryPath ToDir { get; set; }
        public FilePath ToFile { get; set; }
        public bool Flatten { get; set; }
        public Overwrite Overwrite { get; set; }
    }
}
