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

namespace Blib.Types
{
    public class DirSet : Element, IEnumerable<string>
    {
        #region private members

        private static string RegexDirSeparator = Regex.Escape(System.IO.Path.DirectorySeparatorChar.ToString());

        private Dictionary<string, byte> _dirs;
        private List<KeyValuePair<string, bool>> _items = new List<KeyValuePair<string, bool>>();

        private static readonly Regex PatternRegex = new Regex(@"\*\*" + RegexDirSeparator + @"|\*|[^\*]+");

        private void FindDirs()
        {
            if (_dirs != null)
            {
                return;
            }

            _dirs = new Dictionary<string, byte>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var item in _items)
            {
                if (item.Value)
                {
                    IncludeDirs(item.Key);
                }
                else
                {
                    ExcludeDirs(item.Key);
                }
            }
        }

        private void IncludeDirs(string pattern)
        {
            pattern = pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            string fullPattern = GetFullPath(pattern);
            int wildcardIndex = fullPattern.IndexOf('*');
            if (wildcardIndex < 0)
            {
                _dirs[fullPattern] = 1;
            }
            else
            {
                string rootPath = fullPattern.Remove(wildcardIndex);
                if (!rootPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    rootPath = System.IO.Path.GetDirectoryName(rootPath);
                }

                string name = pattern;
                if (name.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    name = name.Remove(name.Length - 1);
                }
                name = System.IO.Path.GetFileName(name);
                Regex regex = GetRegex(fullPattern);
                foreach (string path in System.IO.Directory.GetDirectories(rootPath, name, System.IO.SearchOption.AllDirectories))
                {
                    if (regex.IsMatch(path))
                    {
                        _dirs[path] = 1;
                    }
                }
            }
        }

        private void ExcludeDirs(string pattern)
        {
            pattern = pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            Regex regex = GetRegex(GetFullPath(pattern));
            List<string> remove = new List<string>();
            foreach (var path in _dirs.Keys)
            {
                if (regex.IsMatch(path))
                {
                    remove.Add(path);
                }
            }
            foreach (var path in remove)
            {
                _dirs.Remove(path);
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

        public bool IsEmpty
        {
            get
            {
                FindDirs();
                return _dirs.Count == 0;
            }
        }

        public static implicit operator DirSet(string path)
        {
            DirSet result = new DirSet();
            result.Include(path);
            return result;
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            FindDirs();
            return _dirs.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
