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
using System.Threading;

namespace Tada.Loggers
{
    public abstract class BaseHtmlLogger : MultithreadLogger
    {
        public BaseHtmlLogger()
        {
            UseHtml = true;
        }

        #region public members

        public bool UseHtml { get; set; }

        #endregion
    }

    public abstract class BaseHtmlThreadLogger : ThreadLogger
    {
        public BaseHtmlThreadLogger(BaseHtmlLogger owner, Thread thread)
            : base(thread)
        {
            _owner = owner;
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

        #region private fields

        private BaseHtmlLogger _owner;
        private Stack<ElementInfo> _elements = new Stack<ElementInfo>();

        #endregion

        #region private members

        private static string NewLinesToBR(string message)
        {
            return string.Join("<br />", message.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
        }

        #endregion

        #region protected members

        protected abstract void AppendOutput(string output);

        protected internal override void Start()
        {
            if (_owner.UseHtml)
            {
                AppendOutput("<style>tr { border: solid 1px blue; background-color: #BBE4EC; }</style>");
                AppendOutput(string.Format("<div style=\"border:solid 1px;font-weight:bold;background:black;color:white;\">Starting thread \"{0}\"...</div>" + Environment.NewLine, Thread.Name));
            }
            else
            {
                AppendOutput("<Starting thread \"" + Thread.Name + "\">" + Environment.NewLine);
            }
        }
        
        #endregion

        public override void Log(LogLevel level, string message)
        {
            if (!_owner.UseHtml)
            {
                if (_elements.Count > 0)
                {
                    ElementInfo element = _elements.Peek();
                    if (element.Element is Task)
                    {
                        AppendOutput(string.Format("[{0}] ", element.Element.Name));
                    }
                }
                AppendOutput(message + Environment.NewLine);
            }
            else
            {
                if (_elements.Count > 0)
                {
                    ElementInfo element = _elements.Peek();
                    if (element.Element is Task && !element.Initialized)
                    {
                        AppendOutput(string.Format("<table width='100%'><tr><td>{0}</td><td>{1}</td><td>" + Environment.NewLine, DateTime.Now.ToLongTimeString(), element.Element.Name));
                        element.Initialized = true;
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    switch (level)
                    {
                        case LogLevel.Error:
                            AppendOutput("<font color=\"red\">");
                            break;
                        case LogLevel.Warning:
                            AppendOutput("<font color=\"orange\">");
                            break;
                    }
                    AppendOutput(NewLinesToBR(message.Replace("<", "&lt;")));
                    if (level == LogLevel.Error || level == LogLevel.Warning)
                    {
                        AppendOutput("</font>");
                    }
                    AppendOutput("<br />" + Environment.NewLine);
                }
            }
        }

        public override void BeginConfiguration(Configuration configuration)
        {
            if (_owner.UseHtml)
            {
                AppendOutput(string.Format("<div style=\"border:solid 1px;font-weight:bold;\">{1} Using configuration {0}</div>" + Environment.NewLine, configuration.Name, DateTime.Now.ToLongTimeString()));
            }
            else
            {
                AppendOutput("[Using configuration " + configuration.Name + "]" + Environment.NewLine);
            }
            _elements.Push(new ElementInfo(configuration, true));
        }

        public override void EndConfiguration(Configuration configuration)
        {
            _elements.Pop();

            base.EndConfiguration(configuration);
        }

        public override void BeginTarget(Target target)
        {
            if (_owner.UseHtml)
            {
                AppendOutput(string.Format("<div style=\"border:solid 1px;\"><div style=\"font-weight:bold\">{1} &lt;{0}&gt;</div><div style=\"margin-left:10px\">" + Environment.NewLine, target.Name, DateTime.Now.ToLongTimeString()));
            }
            else
            {
                AppendOutput('<' + target.Name + '>' + Environment.NewLine);
            }
            _elements.Push(new ElementInfo(target, true));
        }

        public override void EndTarget(Target target)
        {
            _elements.Pop();
            if (_owner.UseHtml)
            {
                AppendOutput("</div></div>" + Environment.NewLine);
            }
        }

        public override void BeginTask(Task task)
        {
            _elements.Push(new ElementInfo(task, false));
        }

        public override void EndTask(Task task)
        {
            ElementInfo info = _elements.Pop();
            if (info.Initialized && _owner.UseHtml)
            {
                AppendOutput("</td></tr></table>" + Environment.NewLine);
            }
        }

        public override void Finish(bool success)
        {
            if (_owner.UseHtml)
            {
                if (Thread != _owner.RunnerThread)
                {
                    AppendOutput("</div>" + Environment.NewLine);
                }
                AppendOutput("<br/><br/>" + Environment.NewLine);
            }
        }
    }
}
