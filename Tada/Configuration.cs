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

namespace Tada
{
    public class Configuration : TargetGroup
    {
        public Configuration(params Target[] targets)
            : base(targets)
        {
            string name = GetType().Name;
            if (name.EndsWith("Configuration"))
            {
                name = name.Remove(name.Length - "Configuration".Length);
                Name = name;
            }
        }
    }
}
