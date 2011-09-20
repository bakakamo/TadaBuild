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
using System.Text.RegularExpressions;

namespace Blib.Types
{
    public class PathSetPattern
    {
        public PathSetPattern(string pattern, bool include, bool ignoreEmpty)
        {
            Pattern = pattern;
            Include = include;
            IgnoreEmpty = ignoreEmpty;
        }

        public string Pattern;
        public bool Include;
        public bool IgnoreEmpty;
    }

    public abstract class BasePathSet : Element, IEnumerable<string>
    {
        #region protected members

        protected List<PathSetPattern> _items = new List<PathSetPattern>();
        protected Dictionary<string, byte> _paths;

        protected virtual void FindPaths()
        {
            if (_paths != null)
            {
                return;
            }

            _paths = new Dictionary<string, byte>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var item in _items)
            {
                if (item.Include)
                {
                    IncludePaths(item);
                }
                else
                {
                    ExcludePaths(item);
                }
            }
        }

        protected static string RegexDirSeparator = Regex.Escape(System.IO.Path.DirectorySeparatorChar.ToString());

        protected static readonly Regex PatternRegex = new Regex(@"\*\*" + RegexDirSeparator + @"|\*|[^\*]+");

        protected static Regex GetRegex(string pattern)
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

        protected virtual void IncludePaths(PathSetPattern pattern)
        {
            // nothing here
        }

        protected virtual void ExcludePaths(PathSetPattern pattern)
        {
            string patternStr = pattern.Pattern.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            Regex regex = GetRegex(GetFullPath(patternStr));
            List<string> remove = new List<string>();
            foreach (var path in _paths.Keys)
            {
                if (regex.IsMatch(path))
                {
                    remove.Add(path);
                }
            }
            foreach (var path in remove)
            {
                _paths.Remove(path);
            }
        }

        #endregion

        #region public members

        public DirectoryPath Basedir
        {
            get { return base.InternalBasedir; }
            set { base.InternalBasedir = value; }
        }

        public virtual bool HasSingleItem
        {
            get
            {
                return _items.Count == 1 && _items[0].Include && !_items[0].Pattern.Contains("*");
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                FindPaths();
                return _paths.Count == 0;
            }
        }

        #endregion

        #region IEnumerable<string> Members

        public virtual IEnumerator<string> GetEnumerator()
        {
            FindPaths();
            return _paths.Keys.GetEnumerator();
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
