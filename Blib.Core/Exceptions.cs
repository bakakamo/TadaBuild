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

namespace Blib
{
    [Serializable]
    public class BuildException : Exception
    {
        public BuildException() { }
        public BuildException(string message) : base(message) { }
        public BuildException(string message, Exception inner) : base(message, inner) { }
        protected BuildException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class BuildArgumentException : BuildException
    {
        public BuildArgumentException() { }
        public BuildArgumentException(string argumentName) : base(string.Format("Argument {0} is required!", argumentName)) { }
        public BuildArgumentException(string message, Exception inner) : base(message, inner) { }
        protected BuildArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class BuildArgumentMissingException : BuildArgumentException
    {
        public BuildArgumentMissingException() { }
        public BuildArgumentMissingException(string argumentName) : base(string.Format("Argument {0} is required!", argumentName)) { }
        public BuildArgumentMissingException(string message, Exception inner) : base(message, inner) { }
        protected BuildArgumentMissingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
