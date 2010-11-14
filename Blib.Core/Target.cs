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

namespace Blib
{
    public class Target : ExecutableElement
    {
        public Target()
        {
            Enabled = true;
            string name = GetType().Name;
            if (name.EndsWith("Target"))
            {
                name = name.Remove(name.Length - "Target".Length);
            }
            Name = name;
        }

        #region private members

        private Queue<ExecutableElement> _dependencies = new Queue<ExecutableElement>();
        private object _dependenciesSyncRoot = new object();

        #endregion

        #region protected members

        protected override bool ShouldExecute
        {
            get
            {
                return base.ShouldExecute
                    && Enabled;
            }
        }

        protected override void BeforeExecute()
        {
            base.BeforeExecute();

            ExecutableElement dependency = null;

            do
            {
                lock (_dependenciesSyncRoot)
                {
                    dependency = _dependencies.Count > 0 ? _dependencies.Dequeue() : null;
                }

                if (dependency != null)
                {
                    if (!dependency.Executed)
                    {
                        if (!(dependency is Target) || ((Target)dependency).Enabled)
                        {
                            if (dependency.InternalThread == null || dependency.InternalThread == InternalThread)
                            {
                                dependency.ExecuteOnce();
                            }
                            else
                            {
                                Log(LogLevel.Info, "Waiting for \"{0}\"...", dependency.Name);
                                dependency.Wait();
                                if (!dependency.Success)
                                {
                                    throw new BuildDependencyException(string.Format("Required dependency \"{0\" was not executed successfully!", dependency.Name));
                                }
                            }
                        }
                    }
                }
            } while (dependency != null);
        }

        #endregion

        #region protected internal members

        protected internal virtual void Initialize()
        {
            // create children targets here
        }

        #endregion

        #region public members

        public bool Enabled { get; set; }

        public string Basedir
        {
            get { return base.InternalBasedir; }
            set { base.InternalBasedir = value; }
        }

        public BuildThread Thread
        {
            get { return InternalThread; }
            set { InternalThread = value; }
        }

        public void AddDependency(ExecutableElement dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException("dependency");
            }
            lock (_dependenciesSyncRoot)
            {
                _dependencies.Enqueue(dependency);
            }
        }

        #endregion
    }

    [Serializable]
    public class BuildDependencyException : BuildException
    {
        public BuildDependencyException() { }
        public BuildDependencyException(string message) : base(message) { }
        public BuildDependencyException(string message, Exception inner) : base(message, inner) { }
        protected BuildDependencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
