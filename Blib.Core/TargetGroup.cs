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
using System.Reflection;

namespace Blib
{
    public class TargetGroup : Target
    {
        public TargetGroup(params Target[] targets)
        {
            string name = GetType().Name;
            if (name.EndsWith("TargetGroup"))
            {
                name = name.Remove(name.Length - "TargetGroup".Length);
                Name = name;
            }
            Add(targets);
        }

        public TargetGroup(string name, params Target[] targets)
        {
            Name = name;
            Add(targets);
        }

        #region private members

        private void AddTarget(Target target)
        {
            _targets.Add(target);
            target.Parent = this;
            target.Initialize();
        }

        #endregion

        #region protected members

        protected List<Target> _targets = new List<Target>();

        protected void Add(params Target[] targets)
        {
            foreach (Target target in _targets)
            {
                AddTarget(target);
            }
        }

        protected T Add<T>(T target)
            where T : Target
        {
            AddTarget(target);
            return target;
        }

        protected T Add<T>(ref T target)
            where T : Target, new()
        {
            if (typeof(T) == typeof(Target) || typeof(T) == typeof(TargetGroup))
            {
                throw new BuildException("Only use this overload of Add with a field of the correct target type, not Target or TargetGroup!");
            }
            target = new T();
            AddTarget(target);
            return target;
        }

        protected override void DoExecute()
        {
            foreach (Target target in _targets)
            {
                if (target.Thread != null)
                {
                    target.Thread.Enqueue(target);
                }
                else
                {
                    target.ExecuteOnce();
                }
            }
        }

        #endregion

        #region public members

        public IEnumerable<Target> Targets
        {
            get { return _targets; }
        }

        #endregion
    }
}
