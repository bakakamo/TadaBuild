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
using System.Reflection;
using Tada.Loggers;
using Tada.Tasks.DotNet;
using Tada.Tasks;
using System.Threading;
using System.IO;

namespace Tada
{
    public class Runner
    {
        public Runner(Options options)
        {
            _options = options;
            Current = this;
        }

        #region private fields

        private Options _options;
        private List<Logger> _loggers = new List<Logger>();
        private Dictionary<Thread, Stack<Element>> _parents = new Dictionary<Thread, Stack<Element>>();
        private object _parentsSyncRoot = new object();
        private List<BuildThread> _threads = new List<BuildThread>();
        private object _threadsSyncRoot = new object();
        private int _sleepingThreadsCount;
        private bool _isRunning;
        private bool _finished;
        private bool _initialized;
        private DateTime _start = DateTime.Now;
        private Type _projectType;
        private Project _project;
        private BuildThread _mainThread;

        #endregion

        #region private methods

        private Stack<Element> Parents
        {
            get
            {
                Thread thread = Thread.CurrentThread;
                lock (_parentsSyncRoot)
                {
                    Stack<Element> threadParents;

                    if (!_parents.TryGetValue(thread, out threadParents))
                    {
                        threadParents = new Stack<Element>();
                        _parents[thread] = threadParents;
                    }

                    return threadParents;
                }
            }
        }

        private void DisableTargets(Target target, List<string> disable)
        {
            if (disable.Contains(target.Name.ToLower()))
            {
                target.Enabled = false;
                DisableTargets(target);
            }
            else
            {
                target.Enabled = true;
                if (target is TargetGroup)
                {
                    foreach (Target child in ((TargetGroup)target).Targets)
                    {
                        DisableTargets(child, disable);
                    }
                }
            }
        }

        private void DisableTargets(Target target)
        {
            if (target is TargetGroup)
            {
                foreach (Target child in ((TargetGroup)target).Targets)
                {
                    child.Enabled = false;
                    DisableTargets(child);
                }
            }
        }

        private void Finish(bool success)
        {
            _finished = true;
            foreach (Logger logger in _loggers)
            {
                logger.Finish(success);
            }
        }

        private void SetProperty(object obj, string property, string value)
        {
            MemberInfo[] members = obj.GetType().GetMember(property, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            foreach (MemberInfo member in members)
            {
                PropertyInfo propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, ConvertValueToType(value, propertyInfo.PropertyType), null);
                    break;
                }
                else
                {
                    FieldInfo fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(obj, ConvertValueToType(value, fieldInfo.FieldType));
                    }
                }
            }
        }

        private object ConvertValueToType(string value, Type type)
        {
            return Convert.ChangeType(value, type);
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            ThreadFailed = false;

            // start the the loggers, to make sure we get some feedback
            foreach (var loggerInfo in _options.Loggers)
            {
                Type loggerType = Type.GetType(loggerInfo.Key);
                if (loggerType == null)
                {
                    loggerType = Type.GetType("Tada.Loggers." + loggerInfo.Key);
                }
                Logger logger = (Logger)Activator.CreateInstance(loggerType);
                foreach (string loggerOption in loggerInfo.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int equalsIndex = loggerOption.IndexOf('=');
                    if (equalsIndex < 0)
                    {
                        throw new ArgumentException("Missing value for option " + loggerOption);
                    }
                    SetProperty(logger, loggerOption.Remove(equalsIndex), loggerOption.Substring(equalsIndex + 1));
                }
                AddLogger(logger);
            }

#if DEBUG
            Log(LogLevel.Warning, "Running Tada in debug mode!");
#endif

            if (!string.IsNullOrEmpty(_options.ExecuteBefore))
            {
                Log(LogLevel.Info, "Executing the \"before\" command...");

                int spaceIndex = _options.ExecuteBefore.IndexOf(' ');
                // there's a command to execute before starting the script
                string dir = !string.IsNullOrEmpty(_options.BuildFile) ? Path.GetDirectoryName(_options.BuildFile) : Path.GetDirectoryName(_options.DllFilename);
                ExecTask execTask = new ExecTask(Path.Combine(dir, spaceIndex < 0 ? _options.ExecuteBefore : _options.ExecuteBefore.Remove(spaceIndex)));
                execTask.WorkingDir = dir;
                if (spaceIndex >= 0)
                {
                    execTask.Args = _options.ExecuteBefore.Substring(spaceIndex);
                }
                execTask.Execute();
            }

#if !DEBUG
            if (!string.IsNullOrEmpty(_options.ProjectFilename))
            {
                Log(LogLevel.Info, "Compiling the project...");

                try
                {
                    Project tempProject = new Project();
                    tempProject.TargetFramework = Framework.DotNet_4_0;
                    MSBuildTask msbuild = new MSBuildTask()
                    {
                        ProjectFile = _options.ProjectFilename,
                        Configuration = "Release",
                        Platform = "AnyCPU",
                        Target = "build",
                        Verbosity = MSBuildVerbosity.Quiet
                    };
                    //msbuild.Properties["MSBuildToolsPath"] = tempProject.TargetFrameworkToolsPath;
                    msbuild.Execute();
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, ex.ToString());
                    throw;
                }
            }
#endif

            Assembly assembly = Assembly.LoadFrom(_options.DllFilename);
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(Project).IsAssignableFrom(type))
                {
                    _projectType = type;
                    break;
                }
            }
            if (_projectType == null)
            {
                throw new ProjectNotFoundException(string.Format("Could not find a class inheriting from {0} in the given assembly.", typeof(Project).FullName));
            }

            _mainThread = NewThread("Main");

            _project = (Project)Activator.CreateInstance(_projectType);

            _initialized = true;
        }

        #endregion

        #region protected members

        protected virtual void Sleep()
        {
            Thread.Sleep(1000);
        }

        #endregion

        #region internal members

        internal bool Finished
        {
            get { return _finished; }
        }

        internal bool ThreadFailed { get; set; }

        internal void CheckThreadFailed()
        {
            if (ThreadFailed)
            {
                throw new FailedBuiledException("Stopping because another thread has failed!");
            }
        }

        internal void Sleep(BuildThread buildThread, bool firstSleep)
        {
            if (!firstSleep)
            {
                Log(LogLevel.Info, string.Format("{0} Thread {1} waiting for a task or target to execute...", DateTime.Now.ToLongTimeString(), buildThread.Name));
            }
            Interlocked.Increment(ref _sleepingThreadsCount);
        }

        internal void WakeUp(BuildThread buildThread)
        {
            Interlocked.Decrement(ref _sleepingThreadsCount);
        }

        internal void BeginExecute(ExecutableElement element)
        {
            Configuration configuration = element as Configuration;
            if (configuration != null)
            {
                foreach (Logger logger in _loggers)
                {
                    logger.BeginConfiguration(configuration);
                }
            }
            else
            {
                Target target = element as Target;
                if (target != null)
                {
                    foreach (Logger logger in _loggers)
                    {
                        logger.BeginTarget(target);
                    }
                }
                else
                {
                    Task task = element as Task;
                    if (task != null)
                    {
                        foreach (Logger logger in _loggers)
                        {
                            logger.BeginTask(task);
                        }
                    }
                }
            }
        }

        internal void EndExecute(ExecutableElement element)
        {
            Configuration configuration = element as Configuration;
            if (configuration != null)
            {
                foreach (Logger logger in _loggers)
                {
                    logger.EndConfiguration(configuration);
                }
            }
            else
            {
                Target target = element as Target;
                if (target != null)
                {
                    foreach (Logger logger in _loggers)
                    {
                        logger.EndTarget(target);
                    }
                }
                else
                {
                    Task task = element as Task;
                    if (task != null)
                    {
                        foreach (Logger logger in _loggers)
                        {
                            logger.EndTask(task);
                        }
                    }
                }
            }
        }

        internal void BeginThread(BuildThread buildThread)
        {
            foreach (Logger logger in _loggers)
            {
                logger.BeginThread(buildThread);
            }
        }

        internal Element GetCurrentParent()
        {
            Stack<Element> threadParents = Parents;
            if (threadParents.Count > 0)
            {
                return threadParents.Peek();
            }
            return null;
        }

        internal void BeginElement(Element element)
        {
            Parents.Push(element);
        }

        internal void EndElement()
        {
            Parents.Pop();
        }

        #endregion

        #region public members

        public static Runner Current;

        public Project Project
        {
            get
            {
                Initialize();
                return _project;
            }
        }

        public IEnumerable<Configuration> Configurations
        {
            get
            {
                Initialize();
                return _project.Configurations;
            }
        }

        public void Log(LogLevel level, string message)
        {
            foreach (Logger logger in _loggers)
            {
                logger.Log(level, message);
            }
        }

        public int Run(Configuration configuration)
        {
            bool success = true;

            _isRunning = true;

            Log(LogLevel.Info, string.Format("Tada {0}", Assembly.GetExecutingAssembly().GetName().Version));
            Log(LogLevel.Info, string.Format("Starting the build at {0}...", _start));

            try
            {
                Initialize();

                if (configuration == null)
                {
                    foreach (Configuration projectConfiguration in _project.Configurations)
                    {
                        if (string.IsNullOrEmpty(_options.ConfigurationName) || configuration.Name.Equals(_options.ConfigurationName))
                        {
                            configuration = projectConfiguration;
                            break;
                        }
                    }
                }
                if (configuration == null)
                {
                    return 4;
                }

                // options
                _project.Initialize();

                foreach (var property in _options.ProjectProperties)
                {
                    SetProperty(_project, property.Key, property.Value);
                }
                foreach (var property in _options.ConfigurationProperties)
                {
                    SetProperty(configuration, property.Key, property.Value);
                }

                if (_options.DisabledTargets != null)
                {
                    DisableTargets(configuration, _options.DisabledTargets);
                }

                _project.Execute();

                if (_loggers.Count == 0)
                {
                    AddLogger(new ConsoleLogger());
                }

                _mainThread.Enqueue(configuration);

                while (!_finished)
                {
                    Sleep();

                    lock (_threadsSyncRoot)
                    {
                        _finished = _sleepingThreadsCount == _threads.Count;
                    }
                }

                lock (_threadsSyncRoot)
                {
                    foreach (BuildThread thread in _threads)
                    {
                        success = success && thread.Success;
                    }
                }

                if (!success)
                {
                    return 1;
                }
            }
            finally
            {
                DateTime end = DateTime.Now;

                Log(LogLevel.Info, string.Format("Finishing the build at {0}, total time: {1}...", end, end - _start));

                Finish(success);

                _isRunning = false;
            }

            return 0;
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public void AddLogger(Logger logger)
        {
            _loggers.Add(logger);
        }

        public BuildThread NewThread(string name)
        {
            BuildThread thread = new BuildThread(this, name);
            lock (_threadsSyncRoot)
            {
                _threads.Add(thread);
            }
            return thread;
        }

        #endregion
    }

    [Serializable]
    public class RunnerException : Exception
    {
        public RunnerException() { }
        public RunnerException(string message) : base(message) { }
        public RunnerException(string message, Exception inner) : base(message, inner) { }
        protected RunnerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ProjectNotFoundException : RunnerException
    {
        public ProjectNotFoundException() { }
        public ProjectNotFoundException(string message) : base(message) { }
        public ProjectNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ProjectNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
