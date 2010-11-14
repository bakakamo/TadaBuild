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
using System.Xml;
using Blib.Types;

namespace Blib.Tasks
{
    public class XmlPokeTask : Task
    {
        public XmlPokeTask()
        {
            Namespaces = new Dictionary<string, string>();
        }

        protected override void DoExecute()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(File.FullName);
            XmlNamespaceManager namespaces = new XmlNamespaceManager(xml.NameTable);
            foreach (var ns in Namespaces)
            {
                namespaces.AddNamespace(ns.Key, ns.Value);
            }
            XmlNode node = xml.SelectSingleNode(XPath, namespaces);
            node.InnerXml = Value;
            xml.Save(File.FullName);
        }

        [Required]
        public string XPath { get; set; }
        [Required]
        public FilePath File { get; set; }
        public string Value { get; set; }

        public Dictionary<string, string> Namespaces { get; private set; }
    }
}
