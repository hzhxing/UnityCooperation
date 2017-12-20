using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Common.Version
{
    public struct ResVersion
    {
        public string version;
        public string url;
        public string md5;
        public long size;
    }

    public class VersionContent
    {

        public static VersionContent current = null;

        public string version;
        public string resUrl;
        public List<ResVersion> resVersions = new List<ResVersion>();

        public VersionContent()
        {
        }

        public int resVersionCount { get { return this.resVersions.Count; } }

        public ResVersion GetResVersion(int index)
        {
            return this.resVersions[index];
        }

        public void AddResVersion(ResVersion version)
        {
            this.resVersions.Add(version);
        }

        public void Parse(string text)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(text);
                XmlNode versionNode = doc.GetElementsByTagName("version")[0];
                XmlNode resUrlNode = doc.GetElementsByTagName("resUrl")[0];
                this.version = versionNode.InnerText;
                this.resUrl = resUrlNode.InnerText;

                XmlNode resVersionNode = doc.GetElementsByTagName("resVersion")[0];
                List<ResVersion> resVersions = new List<ResVersion>();
                for (int i = 0; i < resVersionNode.ChildNodes.Count; i++)
                {
                    XmlElement element = resVersionNode.ChildNodes[i] as XmlElement;
                    if (element.Name == "res")
                    {
                        ResVersion version;
                        version.version = element.GetAttribute("version");
                        version.url = element.GetAttribute("url");
                        version.md5 = element.GetAttribute("md5");
                        if (element.HasAttribute("size"))
                        {
                            version.size = long.Parse(element.GetAttribute("size"));
                        }
                        else
                        {
                            version.size = 0;
                        }

                        resVersions.Add(version);
                    }
                }
                this.resVersions = resVersions;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw new Exception("Version Parse Error.text=" + text);
            }
            
        }

        public void Save(string file)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("root");

            XmlElement version = doc.CreateElement("version");
            version.InnerText = this.version;

            XmlElement resUrl = doc.CreateElement("resUrl");
            resUrl.InnerText = this.resUrl;

            XmlElement resVersion = doc.CreateElement("resVersion");
            if (this.resVersions != null)
            {
                for (int i = 0; i < this.resVersions.Count; i++)
                {
                    ResVersion curResVersion = this.resVersions[i];
                    XmlElement resItem = doc.CreateElement("res");
                    resItem.SetAttribute("version", curResVersion.version);
                    resItem.SetAttribute("url", curResVersion.url);
                    resItem.SetAttribute("md5", "");
                    resItem.SetAttribute("size", curResVersion.size.ToString());
                    resVersion.AppendChild(resItem);
                }
            }

            root.AppendChild(version);
            root.AppendChild(resUrl);
            root.AppendChild(resVersion);

            doc.AppendChild(root);

            doc.Save(file);
        }
    }
}