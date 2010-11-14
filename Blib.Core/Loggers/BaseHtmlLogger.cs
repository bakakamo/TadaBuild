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
using System.Threading;

namespace Blib.Loggers
{
    public abstract class BaseHtmlLogger : Logger
    {
        public BaseHtmlLogger()
        {
            UseHtml = true;
            _runnerThread = Thread.CurrentThread;
        }

        #region nested types

        private class ElementInfo
        {
            public ElementInfo(Element element, bool initialized)
            {
                Element = element;
                Initialized = initialized;
            }

            public Element Element;
            public bool Initialized;
        }

        #endregion

        #region private members

        private Thread _runnerThread;
        private Dictionary<Thread, KeyValuePair<object, AppendOutputDelegate>> _outputs = new Dictionary<Thread, KeyValuePair<object, AppendOutputDelegate>>();
        private Dictionary<Thread, Stack<ElementInfo>> _elements = new Dictionary<Thread, Stack<ElementInfo>>();
        private object _syncRoot = new object();

        private void GetOutputAndElements(out KeyValuePair<object, AppendOutputDelegate> output, out Stack<ElementInfo> elements)
        {
            Thread thread = Thread.CurrentThread;

            lock (_syncRoot)
            {
                if (_outputs.TryGetValue(thread, out output))
                {
                    _elements.TryGetValue(thread, out elements);
                }
                else
                {
                    CreateOutput(null, thread, out output, out elements);
                }
            }
        }

        private Stack<ElementInfo> CreateOutput(BuildThread buildThread, Thread thread, out KeyValuePair<object, AppendOutputDelegate> output, out Stack<ElementInfo> elements)
        {
            output = CreateOutput(buildThread);
            _outputs[thread] = output;
            elements = new Stack<ElementInfo>();
            _elements[thread] = elements;
            if (UseHtml)
            {
                output.Value(output.Key, "<style>tr { border: solid 1px blue; background-color: #BBE4EC; }</style>");
            }
            return elements;
        }

        private static string NewLinesToBR(string message)
        {
            return string.Join("<br />", message.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
        }

        #endregion

        #region protected members

        protected delegate void AppendOutputDelegate(object data, string output);

        protected abstract KeyValuePair<object, AppendOutputDelegate> CreateOutput(BuildThread thread);

        #endregion

        #region public members

        public bool UseHtml { get; set; }

        public override void Log(LogLevel level, string message)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            if (!UseHtml)
            {
                if (elements.Count > 0)
                {
                    ElementInfo element = elements.Peek();
                    if (element.Element is Task)
                    {
                        output.Value(output.Key, string.Format("[{0}] ", element.Element.Name));
                    }
                }
                output.Value(output.Key, message + Environment.NewLine);
            }
            else
            {
                if (elements.Count > 0)
                {
                    ElementInfo element = elements.Peek();
                    if (element.Element is Task && !element.Initialized)
                    {
                        output.Value(output.Key, string.Format("<table width='100%'><tr><td>{0}</td><td>{1}</td><td>" + Environment.NewLine, DateTime.Now.ToLongTimeString(), element.Element.Name));
                        element.Initialized = true;
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    switch (level)
                    {
                        case LogLevel.Error:
                            output.Value(output.Key, "<font color=\"red\">");
                            break;
                        case LogLevel.Warning:
                            output.Value(output.Key, "<font color=\"orange\">");
                            break;
                    }
                    output.Value(output.Key, NewLinesToBR(message.Replace("<", "&lt;")));
                    if (level == LogLevel.Error || level == LogLevel.Warning)
                    {
                        output.Value(output.Key, "</font>");
                    }
                    output.Value(output.Key, "<br />" + Environment.NewLine);
                }
            }
        }

        public override void BeginConfiguration(Configuration configuration)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            if (UseHtml)
            {
                output.Value(output.Key, string.Format("<div style=\"border:solid 1px;font-weight:bold;\">{1} Using configuration {0}</div>" + Environment.NewLine, configuration.Name, DateTime.Now.ToLongTimeString()));
            }
            else
            {
                output.Value(output.Key, "[Using configuration " + configuration.Name + "]" + Environment.NewLine);
            }
            elements.Push(new ElementInfo(configuration, true));
        }

        public override void EndConfiguration(Configuration configuration)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            elements.Pop();

            base.EndConfiguration(configuration);
        }

        public override void BeginTarget(Target target)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            if (UseHtml)
            {
                output.Value(output.Key, string.Format("<div style=\"border:solid 1px;\"><div style=\"font-weight:bold\">{1} &lt;{0}&gt;</div><div style=\"margin-left:10px\">" + Environment.NewLine, target.Name, DateTime.Now.ToLongTimeString()));
            }
            else
            {
                output.Value(output.Key, '<' + target.Name + '>' + Environment.NewLine);
            }
            elements.Push(new ElementInfo(target, true));
        }

        public override void EndTarget(Target target)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            elements.Pop();
            if (UseHtml)
            {
                output.Value(output.Key, "</div></div>" + Environment.NewLine);
            }
        }

        public override void BeginTask(Task task)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            elements.Push(new ElementInfo(task, false));
        }

        public override void EndTask(Task task)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;
            GetOutputAndElements(out output, out elements);

            ElementInfo info = elements.Pop();
            if (info.Initialized && UseHtml)
            {
                output.Value(output.Key, "</td></tr></table>" + Environment.NewLine);
            }
        }

        public override void BeginThread(BuildThread buildThread)
        {
            KeyValuePair<object, AppendOutputDelegate> output;
            Stack<ElementInfo> elements;

            Thread thread = Thread.CurrentThread;

            lock (_syncRoot)
            {
                CreateOutput(buildThread, thread, out output, out elements);
            }

            if (UseHtml)
            {
                output.Value(output.Key, string.Format("<div style=\"border:solid 1px;font-weight:bold;background:black;color:white;\">Starting thread \"{0}\"...</div>" + Environment.NewLine, buildThread.Name));
            }
            else
            {
                output.Value(output.Key, "<Starting thread \"" + buildThread.Name + "\">" + Environment.NewLine);
            }
        }

        public override void Finish(bool success)
        {
            lock (_syncRoot)
            {
                foreach (var thread in _outputs)
                {
                    if (UseHtml)
                    {
                        if (thread.Key != _runnerThread)
                        {
                            thread.Value.Value(thread.Value.Key, "</div>" + Environment.NewLine);
                        }
                        thread.Value.Value(thread.Value.Key, "<br/><br/>" + Environment.NewLine);
                    }
                }
            }
        }

        #endregion
    }
}
