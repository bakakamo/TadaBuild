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
    public class DeleteTask : Task
    {
        public DeleteTask(string path)
        {
            Path = path;
        }

        public DeleteTask(FileSet files)
        {
            Files = files;
        }

        public string Path { get; set; }
        public FileSet Files { get; set; }

        private void DeleteDir(string path)
        {
            if (IO.IsFileSystemRoot(path))
            {
                throw new BuildException("Trying to delete the filesystem root, probably not a good idea!");
            }

            Log(LogLevel.Verbose, "Deleting directory \"{0}\"...", path);

            System.IO.Directory.Delete(path, true);
        }

        private void DeleteFile(string path)
        {
            if (IO.IsFileSystemRoot(System.IO.Path.GetDirectoryName(path)))
            {
                throw new BuildException("Trying to delete the filesystem root, probably not a good idea!");
            }

            Log(LogLevel.Verbose, "Deleting file \"{0}\"...", path);

            System.IO.File.Delete(path);
        }

        protected override void DoExecute()
        {
            if (string.IsNullOrEmpty(Path) && Files == null)
            {
                throw new ArgumentException("Path or Files need to be set!");
            }

            int count = IO.CountItems(Path, Files);
            if (count == 0)
            {
                Log(Blib.LogLevel.Warning, "No files to delete!");
            }
            else
            {
                Log(LogLevel.Info, "Deleting {0} item(s)...", count);
            }

            if (!string.IsNullOrEmpty(Path))
            {
                string path = GetFullPath(Path);
                if (System.IO.File.Exists(path))
                {
                    DeleteFile(path);
                }
                else
                    if (System.IO.Directory.Exists(path))
                    {
                        DeleteDir(path);
                    }
            }
            if (Files != null)
            {
                foreach (string file in Files)
                {
                    if (System.IO.File.Exists(file))
                    {
                        DeleteFile(file);
                    }
                }
            }
        }
    }
}
