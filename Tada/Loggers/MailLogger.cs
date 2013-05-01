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
using System.Net.Mail;
using System.Threading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Tada.Loggers
{
    public class MailLogger : BaseHtmlLogger
    {
        public MailLogger()
        {
            SmtpPort = 25;
        }

        #region nested types

        private class MailThreadLogger : BaseHtmlThreadLogger
        {
            public MailThreadLogger(MailLogger owner, Thread thread)
                : base(owner, thread)
            {
            }

            private StringBuilder _builder = new StringBuilder();

            protected override void AppendOutput(string output)
            {
                _builder.Append(output);
            }

            public StringBuilder Builder
            {
                get { return _builder; }
            }
        }

        #endregion

        #region protected members

        protected override ThreadLogger CreateLogger(Thread thread)
        {
            return new MailThreadLogger(this, thread);
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

            StringBuilder mainOutput = new StringBuilder();

            MemoryStream contentStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(contentStream);
            zipStream.SetLevel(9);

            lock (SyncRoot)
            {
                foreach (MailThreadLogger logger in GetLoggers())
                {
                    StringBuilder text = ((MailThreadLogger)logger).Builder;
                    if (logger.Thread == RunnerThread)
                    {
                        mainOutput.Append(text);
                    }
                    else
                    {
                        ZipEntry zipEntry = new ZipEntry(logger.Thread.Name + (UseHtml ? ".html" : ".log"));
                        zipStream.PutNextEntry(zipEntry);

                        byte[] buffer = Encoding.UTF8.GetBytes(text.ToString());
                        zipStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            MailMessage message = new MailMessage(From, To);
            message.Subject = success ? "Build successful" : "Build failed!";
            message.IsBodyHtml = UseHtml;
            message.Body = mainOutput.ToString();

            // log files
            zipStream.Finish();
            contentStream.Position = 0;
            Attachment attachment = new Attachment(contentStream, "logs.zip");
            message.Attachments.Add(attachment);

            SmtpClient smtpClient = new SmtpClient(SmtpHost, SmtpPort);
            smtpClient.Send(message);

            zipStream.Close();
        }

        #endregion
    }
}
