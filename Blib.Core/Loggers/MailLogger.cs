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
using System.Net.Mail;
using System.Threading;

namespace Blib.Loggers
{
    public class MailLogger : BaseHtmlLogger
    {
        public MailLogger()
        {
            SmtpPort = 25;
        }

        #region private members

        private List<StringBuilder> _builders = new List<StringBuilder>();
        private object _buildersSyncRoot = new object();

        private void AppendOutput(object data, string output)
        {
            ((StringBuilder)data).Append(output);
        }

        #endregion

        #region protected members

        protected override KeyValuePair<object, BaseHtmlLogger.AppendOutputDelegate> CreateOutput(BuildThread buildThread)
        {
            StringBuilder builder = new StringBuilder();
            KeyValuePair<object, BaseHtmlLogger.AppendOutputDelegate> result = new KeyValuePair<object, AppendOutputDelegate>(builder, AppendOutput);
            lock (_buildersSyncRoot)
            {
                _builders.Add(builder);
            }
            return result;
        }

        #endregion

        #region public members

        public string To { get; set; }
        public string From { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }

        public override void Finish(bool success)
        {
            base.Finish(success);

            StringBuilder output = new StringBuilder();

            lock (_buildersSyncRoot)
            {
                foreach (var builder in _builders)
                {
                    output.Append(builder);
                }
            }

            MailMessage message = new MailMessage(From, To);
            message.Subject = success ? "Build successful" : "Build failed!";
            message.IsBodyHtml = UseHtml;
            message.Body = output.ToString();
            SmtpClient smtpClient = new SmtpClient(SmtpHost, SmtpPort);
            smtpClient.Send(message);
        }

        #endregion
    }
}
