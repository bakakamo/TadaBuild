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

namespace Blib.Types
{
    public class FilePath : BaseFileSet
    {
        public FilePath(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        public override string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
            protected set { base.Name = value; }
        }

        public string FullName
        {
            get { return GetFullPath(Path); }
        }

        public bool Exists
        {
            get { return System.IO.File.Exists(FullName); }
        }

        public override bool HasSingleItem
        {
            get { return true; }
        }

        public override IEnumerator<string> GetEnumerator()
        {
            yield return FullName;
        }

        public override string ToString()
        {
            return FullName;
        }

        public static implicit operator string(FilePath path)
        {
            if (path == null)
            {
                return string.Empty;
            }
            return path.FullName;
        }

        public static implicit operator FilePath(string path)
        {
            return new FilePath(path);
        }
    }
}
