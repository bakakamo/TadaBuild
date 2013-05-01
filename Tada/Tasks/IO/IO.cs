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
using Tada.Tools;

namespace Tada.Tasks.IO
{
    using Tada.Tools;
    using Tada.Types;

    public static class IO
    {
        #region internal members

        internal static int CountItems(params object[] items)
        {
            int count = 0;
            foreach (object item in items)
            {
                BasePathSet pathSet = item as BasePathSet;
                if (pathSet != null)
                {
                    count += Enumerable.Count(pathSet);
                }
                else
                    if (item is string)
                    {
                        if (!string.IsNullOrEmpty((string)item))
                        {
                            count++;
                        }
                    }
                    else
                        if (item != null)
                        {
                            throw new ArgumentException("Unknown item type: {0}", item.GetType().FullName);
                        }
            }
            return count;
        }

        #endregion

        #region functions

        public static bool IsFileSystemRoot(string path)
        {
            return System.IO.Path.GetPathRoot(path).Equals(path, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string IncludeTrailingPathDelimiter(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            char ch = path[path.Length - 1];
            if (ch == '\\' || ch == '/')
            {
                return path;
            }

            if (path.IndexOf('\\') >= 0)
            {
                return path + '\\';
            }
            if (path.IndexOf('/') >= 0)
            {
                return path + '/';
            }
            return path + System.IO.Path.DirectorySeparatorChar;
        }

        internal static string ExcludeTrailingPathDelimiter(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            char ch = path[path.Length - 1];
            if (ch == '\\' || ch == '/')
            {
                return path.Remove(path.Length - 1);
            }

            return path;
        }

        public static string FixPathDelimiter(string path)
        {
            return path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }

        #endregion

        #region shortcuts for tasks

        public static void MkDir(string path)
        {
            new MkDirTask() { Path = path }.Execute();
        }

        public static void Delete(string path)
        {
            new DeleteTask(path).Execute();
        }

        public static void CopyToDir(FileSet files, DirectoryPath toDir)
        {
            new CopyTask(files) { ToDir = toDir }.Execute();
        }

        public static void CopyToDir(FileSet files, DirectoryPath toDir, Overwrite overwrite)
        {
            new CopyTask(files) { ToDir = toDir, Overwrite = overwrite}.Execute();
        }

        public static void CopyToFile(FilePath file, FilePath toFile)
        {
            new CopyTask(file) { ToFile = toFile }.Execute();
        }

        public static void CopyToFile(FilePath file, FilePath toFile, Overwrite overwrite = Overwrite.IfNewer, bool failOnError = true)
        {
            new CopyTask(file) { ToFile = toFile, Overwrite = overwrite, FailOnError = failOnError }.Execute();
        }

        public static void Move(FileSet files, DirectoryPath toDir, Overwrite overwrite, bool flatten = false, bool failOnError = true)
        {
            new MoveTask(files) { ToDir = toDir, Overwrite = overwrite, Flatten = flatten, FailOnError = failOnError }.Execute();
        }

        public static void Move(FilePath file, DirectoryPath toDir, Overwrite overwrite, bool failOnError = true)
        {
            new MoveTask(file) { ToDir = toDir, Overwrite = overwrite, FailOnError = failOnError }.Execute();
        }

        public static void Move(string file, DirectoryPath toDir, Overwrite overwrite, bool failOnError = true)
        {
            Move((FilePath)file, toDir, overwrite, failOnError);
        }

        public static void Rename(FilePath file, FilePath toFile, Overwrite overwrite)
        {
            new MoveTask(file) { ToFile = toFile, Overwrite = overwrite }.Execute();
        }

        public static void Rename(FilePath file, FilePath toFile, Overwrite overwrite = Overwrite.IfNewer, bool failOnError = true)
        {
            new MoveTask(file) { ToFile = toFile, Overwrite = overwrite, FailOnError = failOnError }.Execute();
        }

        public static string NewTempDir(bool createDirectory = true, string parentDirectory = null)
        {
            if (string.IsNullOrEmpty(parentDirectory))
            {
                parentDirectory = System.IO.Path.GetTempPath();
            }
            string result;
            do
            {
                result = System.IO.Path.Combine(parentDirectory, System.IO.Path.GetRandomFileName());
            } while (System.IO.Directory.Exists(result));

            if (createDirectory)
            {
                System.IO.Directory.CreateDirectory(result);
            }
            return result;
        }

        #endregion
    }
}
