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
using System.Text.RegularExpressions;
using Blib.Tasks.IO;

namespace Blib.Types
{
    public class FileSet : BaseFileSet, IEnumerable<string>
    {
        #region private members

        private static string RegexDirSeparator = Regex.Escape(System.IO.Path.DirectorySeparatorChar.ToString());

        private Dictionary<string, byte> _files;
        private List<KeyValuePair<string, bool>> _items = new List<KeyValuePair<string, bool>>();

        private static readonly Regex PatternRegex = new Regex(@"\*\*" + RegexDirSeparator + @"|\*|[^\*]+");

        private void FindFiles()
        {
            if (_files != null)
            {
                return;
            }

            _files = new Dictionary<string, byte>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var item in _items)
            {
                if (item.Value)
                {
                    IncludeFiles(item.Key);
                }
                else
                {
                    ExcludeFiles(item.Key);
                }
            }
        }

        private void IncludeFiles(string pattern)
        {
            pattern = pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            string fullPattern = GetFullPath(pattern);
            int wildcardIndex = fullPattern.IndexOf('*');
            bool foundSomething = false;
            if (wildcardIndex < 0)
            {
                if (System.IO.File.Exists(fullPattern))
                {
                    foundSomething = true;
                    _files[fullPattern] = 1;
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

                string name = System.IO.Path.GetFileName(pattern);
                Regex regex = GetRegex(fullPattern);
                if (System.IO.Directory.Exists(rootPath))
                {
                    foreach (string path in System.IO.Directory.GetFiles(rootPath, name, System.IO.SearchOption.AllDirectories))
                    {
                        if (regex.IsMatch(path))
                        {
                            foundSomething = true;
                            _files[path] = 1;
                        }
                    }
                }
            }

            if (!foundSomething && !IgnoreEmptyPatterns)
            {
                throw new BuildException(string.Format("Did not find anything to include with pattern \"{0}\"!", pattern));
            }
        }

        private void ExcludeFiles(string pattern)
        {
            pattern = pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            Regex regex = GetRegex(GetFullPath(pattern));
            List<string> remove = new List<string>();
            foreach (var path in _files.Keys)
            {
                if (regex.IsMatch(path))
                {
                    remove.Add(path);
                }
            }
            foreach (var path in remove)
            {
                _files.Remove(path);
            }
        }

        private static Regex GetRegex(string pattern)
        {
            StringBuilder regexPattern = new StringBuilder();
            if (System.IO.Path.IsPathRooted(pattern))
            {
                regexPattern.Append("^");
            }
            else
            {
                regexPattern.Append(RegexDirSeparator);
            }
            MatchCollection matches = PatternRegex.Matches(pattern);
            foreach (Match match in matches)
            {
                switch (match.Value)
                {
                    case "**\\":
                    case "**/":
                        regexPattern.Append("(.*" + RegexDirSeparator + ")?");
                        break;
                    case "*":
                        regexPattern.Append("[^" + RegexDirSeparator + "]*");
                        break;
                    default:
                        regexPattern.Append(Regex.Escape(match.Value));
                        break;
                }
            }
            regexPattern.Append("($|").Append(RegexDirSeparator).Append(')');

            Regex regex = new Regex(regexPattern.ToString(), RegexOptions.IgnoreCase);
            return regex;
        }

        #endregion

        #region public members

        public void Include(string pattern)
        {
            _items.Add(new KeyValuePair<string, bool>(pattern, true));
        }

        public void Exclude(string pattern)
        {
            _items.Add(new KeyValuePair<string, bool>(pattern, false));
        }

        public bool IgnoreEmptyPatterns
        {
            get;
            set;
        }

        public bool IsEmpty
        {
            get
            {
                FindFiles();
                return _files.Count == 0;
            }
        }

        public override bool HasSingleItem
        {
            get
            {
                return _items.Count == 1 && _items[0].Value && !_items[0].Key.Contains("*");
            }
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

        public override IEnumerator<string> GetEnumerator()
        {
            FindFiles();
            return _files.Keys.GetEnumerator();
        }

        #endregion
    }
}
