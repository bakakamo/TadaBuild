﻿// Copyright 2010-2013 Bastien Hofmann <bastien@tadabuild.net>
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
using Tada.Tasks.IO;

namespace Tada.Types
{
    using Tada.Tasks.IO;

    public class DirectoryPath : BaseDirSet
    {
        public DirectoryPath(string path)
        {
            Path = path;
        }

        #region protected members

        protected override void FindPaths()
        {
            // nothing here
        }

        #endregion

        public string Path { get; set; }

        public override string Name
        {
            get { return System.IO.Path.GetFileName(IO.ExcludeTrailingPathDelimiter(Path)); }
            protected set { base.Name = value; }
        }

        public string FullName
        {
            get { return GetFullPath(Path); }
        }

        public bool Exists
        {
            get { return System.IO.Directory.Exists(FullName); }
        }

        public override bool IsEmpty
        {
            get { return !Exists; }
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

        public static implicit operator string(DirectoryPath path)
        {
            if (path == null)
            {
                return string.Empty;
            }
            return path.FullName;
        }

        public static implicit operator DirectoryPath(string path)
        {
            return new DirectoryPath(path);
        }
    }
}
