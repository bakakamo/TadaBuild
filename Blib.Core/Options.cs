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
using System.IO;
using Blib.Tasks.IO;

namespace Blib
{
    public class Options
    {
        #region private members

        private static string Unquote(string str)
        {
            str = str.Trim();
            if (str.Length > 2 && str[0] == '"' && str[str.Length - 1] == '"')
            {
                return str.Substring(1, str.Length - 2).Trim();
            }
            return str;
        }

        private string GetAttributeValue(XmlNode node, string attributeName, string defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attributeName];
            return attribute != null ? attribute.Value : defaultValue;
        }

        #endregion

        public string ExecuteBefore;

        public string BuildFile;
        public string DllFilename;
        public string ProjectFilename;
        public string ConfigurationName;

        public Dictionary<string, string> ProjectProperties = new Dictionary<string, string>();
        public Dictionary<string, string> ConfigurationProperties = new Dictionary<string, string>();

        public Dictionary<string, string> Loggers = new Dictionary<string, string>();

        public List<string> DisabledTargets = new List<string>();

        public void Parse(string[] args)
        {
            string configurationFile = null;

            bool startParsing = true;
            while (startParsing)
            {
                startParsing = false;

                foreach (string arg in args)
                {
                    if (startParsing)
                    {
                        break;
                    }

                    if (arg.StartsWith("-"))
                    {
                        int colonIndex = arg.IndexOf(':');
                        if (colonIndex > -1)
                        {
                            string optionName = arg.Remove(colonIndex);
                            string value = arg.Substring(colonIndex + 1);
                            switch (optionName)
                            {
                                case "-d":
                                case "--disable":
                                    DisabledTargets.Add(Unquote(value).ToLower());
                                    break;
                                case "-b":
                                case "--before":
                                    ExecuteBefore = Unquote(value).ToLower();
                                    break;
                                case "-p":
                                case "--project-property":
                                    string property = value;
                                    int equalsIndex = property.IndexOf('=');
                                    if (equalsIndex < 0)
                                    {
                                        throw new ArgumentException("Missing value for option " + arg);
                                    }
                                    ProjectProperties[Unquote(property.Remove(equalsIndex))] = Unquote(property.Substring(equalsIndex + 1));
                                    break;
                                case "-c":
                                case "--configuration-property":
                                    property = value;
                                    equalsIndex = property.IndexOf('=');
                                    if (equalsIndex < 0)
                                    {
                                        throw new ArgumentException("Missing value for option " + arg);
                                    }
                                    ConfigurationProperties[Unquote(property.Remove(equalsIndex))] = Unquote(property.Substring(equalsIndex + 1));
                                    break;
                                case "-l":
                                case "--logger":
                                    property = value;
                                    equalsIndex = property.IndexOf('=');
                                    if (equalsIndex < 0)
                                    {
                                        Loggers[Unquote(property)] = string.Empty;
                                    }
                                    else
                                    {
                                        Loggers[Unquote(property.Remove(equalsIndex))] = Unquote(property.Substring(equalsIndex + 1));
                                    }
                                    break;
                                case "--assembly":
                                    DllFilename = Unquote(value);
                                    break;
                                case "--project-file":
                                    ProjectFilename = Unquote(value);
                                    break;
                                case "--buildfile":
                                    if (configurationFile == null)
                                    {
                                        configurationFile = Unquote(value);
                                        LoadFromFile(configurationFile);
                                        startParsing = true;
                                    }
                                    break;
                                default:
                                    throw new ArgumentException("Unrecognized option: " + arg);
                            }
                        }
                    }
                    else
                        if (arg.EndsWith(".blib", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (configurationFile == null)
                            {
                                configurationFile = arg;
                                LoadFromFile(configurationFile);
                                startParsing = true;
                            }
                        }
                        else
                            if (arg.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                            {
                                DllFilename = arg;
                            }
                            else
                                if ((arg.EndsWith(".sln", StringComparison.InvariantCultureIgnoreCase) || arg.EndsWith(".csproj", StringComparison.InvariantCultureIgnoreCase)) && System.IO.File.Exists(arg))
                                {
                                    ProjectFilename = arg;
                                }
                }
            }
        }

        public void LoadFromFile(string filename)
        {
            BuildFile = filename;

            string rootPath = Path.GetDirectoryName(filename);

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);
            foreach (XmlNode rootNode in xml.DocumentElement.ChildNodes)
            {
                if (rootNode.NodeType == XmlNodeType.Element)
                {
                    switch (rootNode.LocalName)
                    {
                        case "Target":
                            DisabledTargets.Add(rootNode.InnerText);
                            break;
                        case "ExecuteBefore":
                            ExecuteBefore = rootNode.InnerText;
                            break;
                        case "Project":
                            foreach (XmlNode childNode in rootNode.ChildNodes)
                            {
                                if (childNode.NodeType == XmlNodeType.Element)
                                {
                                    switch (childNode.LocalName)
                                    {
                                        case "Property":
                                            ProjectProperties[childNode.Attributes["key"].Value] = GetAttributeValue(childNode, "value", childNode.InnerText);
                                            break;
                                        default:
                                            throw new ArgumentException("Unrecognized buildfile param: " + childNode.LocalName);
                                    }
                                }
                            }
                            break;
                        case "Configuration":
                            foreach (XmlNode childNode in rootNode.ChildNodes)
                            {
                                if (childNode.NodeType == XmlNodeType.Element)
                                {
                                    switch (childNode.LocalName)
                                    {
                                        case "Property":
                                            ConfigurationProperties[childNode.Attributes["key"].Value] = GetAttributeValue(childNode, "value", childNode.InnerText);
                                            break;
                                        default:
                                            throw new ArgumentException("Unrecognized buildfile param: " + childNode.LocalName);
                                    }
                                }
                            }
                            break;
                        case "Loggers":
                            foreach (XmlNode childNode in rootNode.ChildNodes)
                            {
                                if (childNode.NodeType == XmlNodeType.Element)
                                {
                                    switch (childNode.LocalName)
                                    {
                                        case "Logger":
                                            Loggers[childNode.Attributes["class"].Value] = GetAttributeValue(childNode, "settings", childNode.InnerText);
                                            break;
                                    }
                                }
                            }
                            break;
                        case "Assembly":
                            DllFilename = Path.Combine(rootPath, rootNode.InnerText);
                            break;
                        case "ProjectFile":
                            ProjectFilename = Path.Combine(rootPath, rootNode.InnerText);
                            break;
                        default:
                            throw new ArgumentException("Unrecognized buildfile param: " + rootNode.LocalName);
                    }
                }
            }
        }
    }
}
