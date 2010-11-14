﻿// Copyright 2010 Bastien Hofmann <kamo@cfagn.net>
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

namespace Blib
{
    public class BuildThread
    {
        internal BuildThread(Runner runner, string name)
        {
            _runner = runner;
            _name = name;
            _thread = new Thread(Execute);
            _thread.Name = "BuildThread" + ++_threadCount;
            _thread.IsBackground = true;
            _thread.Start();
        }

        #region private fields

        private static int _threadCount;

        private string _name;
        private Thread _thread;
        private Runner _runner;
        private Queue<ExecutableElement> _elements = new Queue<ExecutableElement>();
        private object _elementsSyncRoot = new object();
        private bool _success = true;
        private EventWaitHandle _wakeUpEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

        #endregion

        #region private methods

        private void Execute()
        {
            try
            {
                bool notified = false;
                try
                {
                    while (!_runner.Finished)
                    {
                        ExecutableElement element = null;
                        do
                        {
                            lock (_elementsSyncRoot)
                            {
                                element = _elements.Count > 0 ? _elements.Dequeue() : null;
                            }

                            if (element != null)
                            {
                                if (!notified)
                                {
                                    notified = true;
                                    _runner.BeginThread(this);
                                }
                                element.Execute();
                            }

                        } while (element != null);

                        _runner.Sleep(this, !notified);
                        _wakeUpEvent.WaitOne();
                        _wakeUpEvent.Reset();
                        _runner.WakeUp(this);
                    }
                }
                catch (FailedBuiledException)
                {
                    _success = false;
                }
                catch (Exception ex)
                {
                    _success = false;

                    _runner.Log(LogLevel.Error, ex.ToString());
                }
                finally
                {
                    _runner.Sleep(this, notified);
                }
            }
            catch
            {
                // make sure we don't crash the program
            }
        }

        #endregion

        #region internal members

        internal Thread Thread
        {
            get { return _thread; }
        }

        internal bool Success
        {
            get { return _success; }
        }

        #endregion

        #region public members

        public string Name
        {
            get { return _name; }
        }

        public void Enqueue(ExecutableElement element)
        {
            lock (_elementsSyncRoot)
            {
                _elements.Enqueue(element);
            }
            element.InternalThread = this;
            _wakeUpEvent.Set();
        }

        #endregion
    }
}
