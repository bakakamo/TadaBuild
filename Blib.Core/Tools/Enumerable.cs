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
using System.Collections;

namespace Blib.Tools
{
    public static class Enumerable
    {
        public static C GetFirst<C>(IEnumerable<C> enumerable)
        {
            foreach (C item in enumerable)
            {
                return item;
            }

            return default(C);
        }

        public static int Count(IEnumerable enumerable)
        {
            ICollection collection = enumerable as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            int count = 0;
            foreach (object item in enumerable)
            {
                count++;
            }
            return count;
        }
    }
}
