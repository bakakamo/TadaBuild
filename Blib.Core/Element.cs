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
using Blib.Tasks.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace Blib
{
    public class Element
    {
        public Element()
        {
            _id = ++_count;
            Parent = Runner.GetCurrentParent();
        }

        #region private fields

        private static int _count;

        private int _id;
        private Runner _runner;
        private string _name = string.Empty;
        private string _basedir;
        private LogLevel? _logLevel;

        #endregion

        #region protected members

        protected Element _parent;
        protected List<Element> _children = new List<Element>();

        protected void BeginElement(Element element)
        {
            Runner.BeginElement(element);
        }

        protected void EndElement()
        {
            Runner.EndElement();
        }

        #endregion

        #region protected internal members

        protected internal string InternalBasedir
        {
            get { return _basedir ?? (_parent == null ? string.Empty : _parent.InternalBasedir); }
            set { _basedir = IO.FixPathDelimiter(value); }
        }

        #endregion

        #region internal members

        internal int Id
        {
            get { return _id; }
        }

        internal Element Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    if (_parent != null)
                    {
                        _parent._children.Remove(this);
                    }
                    _parent = value;
                    if (_parent != null)
                    {
                        _parent._children.Add(this);
                    }
                }
            }
        }

        #endregion

        #region public members

        public Runner Runner
        {
            get { return _runner ?? Runner.Current; }
            set { _runner = value; }
        }

        public virtual Project ParentProject
        {
            get { return _parent != null ? Parent.ParentProject : null; }
        }

        public LogLevel LogLevel
        {
            get { return _logLevel ?? (_parent == null ? LogLevel.Info : _parent.LogLevel); }
            set { _logLevel = value; }
        }

        public void Log(LogLevel level, string message)
        {
            if (level <= LogLevel)
            {
                Runner.Log(level, message);
            }
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            Log(level, string.Format(message, args));
        }

        public virtual string Name
        {
            get { return _name; }
            protected set { _name = value ?? string.Empty; }
        }

        public string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            if (System.IO.Path.IsPathRooted(path))
            {
                return IO.FixPathDelimiter(path);
            }
            return System.IO.Path.Combine(InternalBasedir, IO.FixPathDelimiter(path));
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    public class ExecutableElement : Element
    {
        public ExecutableElement()
        {
            FailOnError = true;
        }

        #region private fields

        private bool _success;
        private bool _executed;
        private EventWaitHandle _finishedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private BuildThread _thread;

        #endregion

        #region protected members

        protected virtual void DoExecute()
        {
            // nothing here
        }

        protected virtual bool ShouldExecute
        {
            get { return true; }
        }

        protected virtual void BeforeExecute()
        {
            // nothing here
        }

        protected virtual string GetWorkingDir()
        {
            return InternalBasedir;
        }

        protected virtual void Validate()
        {
            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                object[] attributes = property.GetCustomAttributes(typeof(ValidatorAttribute), true);

                if (attributes.Length > 0)
                {
                    object value = property.GetValue(this, null);

                    foreach (ValidatorAttribute validator in attributes)
                    {
                        validator.Validate(this, property, value);
                    }
                }
            }
        }

        protected void EnsureExecuted()
        {
            if (!_executed)
            {
                throw new BuildException("Trying to get output before executing the element!");
            }
        }

        protected string GetCaller()
        {
            StackFrame sf = new StackFrame(2, true);
            MethodBase method = sf.GetMethod();
            return string.Format("Called from {0}.{1}() in {2}: line {3}", method.DeclaringType.FullName, method.Name, sf.GetFileName(), sf.GetFileLineNumber());
        }

        #endregion

        #region internal members

        internal void Wait()
        {
            _finishedEvent.WaitOne();
        }

        internal void Cancel()
        {
            _success = false;
            _executed = true;
            _finishedEvent.Set();
        }

        internal BuildThread InternalThread
        {
            get { return _thread ?? (_parent is ExecutableElement ? null : ((ExecutableElement)_parent).InternalThread); }
            set { _thread = value; }
        }

        #endregion

        #region public members

        public void Execute()
        {
            _finishedEvent.Reset();
            try
            {
                if (ShouldExecute)
                {
                    try
                    {
                        BeforeExecute();
                        Validate();
                    }
                    catch (FailedBuiledException)
                    {
                        throw;
                    }
                    catch (BuildException ex)
                    {
                        Log(LogLevel.Error, ex.ToString());
                        if (FailOnError)
                        {
                            throw new FailedBuiledException();
                        }
                        else
                        {
                            _success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(LogLevel.Error, ex.ToString());
                        throw new FailedBuiledException();
                    }

                    string directory = GetWorkingDir();
                    string previousDirectory = Environment.CurrentDirectory;
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Environment.CurrentDirectory = directory;
                    }

                    BeginElement(this);
                    try
                    {
                        Runner.BeginExecute(this);
                        try
                        {
                            try
                            {
                                DoExecute();
                                _success = true;
                            }
                            catch (FailedBuiledException)
                            {
                                throw;
                            }
                            catch (BuildException ex)
                            {
                                Log(LogLevel.Error, ex.ToString() + Environment.NewLine + GetCaller());
                                if (FailOnError)
                                {
                                    throw new FailedBuiledException();
                                }
                                else
                                {
                                    _success = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(LogLevel.Error, ex.ToString() + Environment.NewLine + GetCaller());
                                throw new FailedBuiledException();
                            }

                            foreach (Element child in _children)
                            {
                                if (child is Task && !((Task)child)._executed && ((Task)child).InternalThread == InternalThread)
                                {
                                    throw new BuildException(string.Format("Child element {0} was created but not executed!", child.Name));
                                }
                            }

                            _children.Clear();
                        }
                        finally
                        {
                            Runner.EndExecute(this);
                        }
                    }
                    finally
                    {
                        EndElement();

                        Environment.CurrentDirectory = previousDirectory;
                    }
                }
            }
            finally
            {
                _executed = true;
                _finishedEvent.Set();
            }
        }

        public void ExecuteOnce()
        {
            if (!_executed)
            {
                Execute();
            }
        }

        public bool FailOnError { get; set; }

        public bool Success
        {
            get { return _success; }
        }

        public bool Executed
        {
            get { return _executed; }
        }

        #endregion
    }

    [Serializable]
    internal class FailedBuiledException : Exception
    {
        public FailedBuiledException() { }
        public FailedBuiledException(string message) : base(message) { }
        public FailedBuiledException(string message, Exception inner) : base(message, inner) { }
        protected FailedBuiledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    public enum LogLevel
    {
        None,
        Error,
        Warning,
        Info,
        Verbose,
        Debug
    }
}