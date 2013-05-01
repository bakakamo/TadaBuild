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
using System.Security.Cryptography;

namespace Tada.Tasks.Cryptography
{
    using Tada.Types;

    public class ChecksumTask : Task
    {
        public ChecksumTask()
        {
            Algorithm = "MD5";
        }

        protected override void DoExecute()
        {
            if (string.IsNullOrEmpty(FileExt))
            {
                FileExt = Algorithm;
            }

            HashAlgorithm hashAlgorithm;
            switch (Algorithm.ToLowerInvariant())
            {
                case "md5":
                    hashAlgorithm = MD5.Create();
                    break;
                case "sha1":
                    hashAlgorithm = SHA1.Create();
                    break;
                default:
                    throw new ArgumentException("Unknown algorithm!", "Algorithm");
            }

            foreach (string filename in Files)
            {
                byte[] hash = hashAlgorithm.ComputeHash(System.IO.File.OpenRead(filename));
                string targetFilename = filename + '.' + FileExt;
                System.IO.File.WriteAllText(targetFilename, HashToString(hash));
            }
        }

        public string Algorithm { get; set; }
        public string FileExt { get; set; }
        public FileSet Files { get; set; }

        public static string HashToString(byte[] hash)
        {
            StringBuilder result = new StringBuilder(hash.Length * 2);
            foreach (byte a in hash)
            {
                if (a < 16)
                    result.Append('0').Append(a.ToString("X"));
                else
                    result.Append(a.ToString("X"));
            }
            return result.ToString();
        }
    }
}
