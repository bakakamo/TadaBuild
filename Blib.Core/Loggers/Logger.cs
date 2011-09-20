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
using System.Threading;

namespace Blib.Loggers
{
    public abstract class Logger
    {
        public abstract void Log(LogLevel level, string message);

        public abstract void BeginConfiguration(Configuration configuration);

        public virtual void EndConfiguration(Configuration configuration)
        {
            // nothing here
        }

        public abstract void BeginTarget(Target target);

        public virtual void EndTarget(Target target)
        {
            // nothing here
        }

        public abstract void BeginTask(Task task);

        public virtual void EndTask(Task task)
        {
            // nothing here
        }

        public abstract void Finish(bool success);

        public virtual void BeginThread(BuildThread thread)
        {
            // nothing here
        }
    }

    public abstract class MultithreadLogger : Logger
    {
        public MultithreadLogger()
        {
            _runnerThread = Thread.CurrentThread;
        }

        #region private fields

        private Thread _runnerThread;
        private Dictionary<Thread, ThreadLogger> _loggers = new Dictionary<Thread, ThreadLogger>();
        private object _syncRoot = new object();

        #endregion

        #region protected members

        protected object SyncRoot
        {
            get { return _syncRoot; }
        }

        protected abstract ThreadLogger CreateLogger(Thread thread);

        protected Logger GetThreadLogger()
        {
            Thread thread = Thread.CurrentThread;

            lock (_syncRoot)
            {
                ThreadLogger logger;
                if (!_loggers.TryGetValue(thread, out logger))
                {
                    logger = CreateLogger(thread);
                    logger.Start();
                    _loggers[thread] = logger;
                }
                return logger;
            }
        }

        protected Dictionary<Thread, ThreadLogger> Loggers
        {
            get { return _loggers; }
        }

        protected List<Logger> GetLoggers()
        {
            List<Logger> loggers;

            lock (SyncRoot)
            {
                loggers = new List<Logger>(_loggers.Values);
            }

            return loggers;
        }

        #endregion

        #region public members

        public Thread RunnerThread
        {
            get { return _runnerThread; }
        }

        public override void Log(LogLevel level, string message)
        {
            GetThreadLogger().Log(level, message);
        }

        public override void BeginConfiguration(Configuration configuration)
        {
            GetThreadLogger().BeginConfiguration(configuration);
        }

        public override void EndConfiguration(Configuration configuration)
        {
            GetThreadLogger().EndConfiguration(configuration);
        }

        public override void BeginTarget(Target target)
        {
            GetThreadLogger().BeginTarget(target);
        }

        public override void EndTarget(Target target)
        {
            GetThreadLogger().EndTarget(target);
        }

        public override void BeginTask(Task task)
        {
            GetThreadLogger().BeginTask(task);
        }

        public override void EndTask(Task task)
        {
            GetThreadLogger().EndTask(task);
        }

        public override void BeginThread(BuildThread buildThread)
        {
            GetThreadLogger().BeginThread(buildThread);
        }

        public override void Finish(bool success)
        {
            foreach (Logger logger in GetLoggers())
            {
                logger.Finish(success);
            }
        }

        #endregion
    }

    public abstract class ThreadLogger : Logger
    {
        public ThreadLogger(Thread thread)
        {
            _thread = thread;
        }

        private Thread _thread;

        protected internal virtual void Start()
        {
            // nothing here
        }

        public Thread Thread
        {
            get { return _thread; }
        }
    }
}
