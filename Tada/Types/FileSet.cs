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
using System.Text.RegularExpressions;
using Tada.Tasks.IO;

namespace Tada.Types
{
    using Tada.Tasks.IO;

    public class FileSet : BaseFileSet
    {
        #region protected members

        protected override void IncludePaths(PathSetPattern pattern)
        {
            string patternStr = pattern.Pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            string fullPattern = GetFullPath(patternStr);
            int wildcardIndex = fullPattern.IndexOf('*');
            bool foundSomething = false;
            if (wildcardIndex < 0)
            {
                if (System.IO.File.Exists(fullPattern))
                {
                    foundSomething = true;
                    _paths[fullPattern] = 1;
                }
            }
            else
            {
                string rootPath = fullPattern.Remove(wildcardIndex);
                if (!rootPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    rootPath = System.IO.Path.GetDirectoryName(rootPath);
                }

                // check for mistakes, for example by doing Include("/*")
                if (IO.IsFileSystemRoot(rootPath))
                {
                    throw new BuildException("Including files in a filset using the root of the filesystem as root!");
                }

                string name = System.IO.Path.GetFileName(patternStr);
                Regex regex = GetRegex(fullPattern);
                if (System.IO.Directory.Exists(rootPath))
                {
                    foreach (string path in System.IO.Directory.GetFiles(rootPath, name, System.IO.SearchOption.AllDirectories))
                    {
                        if (regex.IsMatch(path))
                        {
                            foundSomething = true;
                            _paths[path] = 1;
                        }
                    }
                }
            }

            if (!foundSomething && !IgnoreEmptyPatterns && !pattern.IgnoreEmpty)
            {
                throw new BuildException(string.Format("Did not find anything to include with pattern \"{0}\"!", patternStr));
            }
        }

        #endregion

        #region public members

        public void Include(string pattern, bool ignoreEmpty = false)
        {
            _items.Add(new PathSetPattern(pattern, true, ignoreEmpty));
        }

        public void Exclude(string pattern, bool ignoreEmpty = false)
        {
            _items.Add(new PathSetPattern(pattern, false, ignoreEmpty));
        }

        public bool IgnoreEmptyPatterns
        {
            get;
            set;
        }

        public static implicit operator FileSet(string filename)
        {
            FileSet result = new FileSet();
            result.Include(filename);
            return result;
        }

        public void Delete(LogLevel? logLevel = null)
        {
            IgnoreEmptyPatterns = true;
            DeleteTask delete = new DeleteTask(this);
            if (logLevel.HasValue)
            {
                delete.LogLevel = logLevel.Value;
            }
            delete.Execute();
        }

        #endregion
    }
}
