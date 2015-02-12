using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Vici.Core.DataStore
{
    public class XmlDataStore<T> : DataStore<T,XElement> where T : IXmlDataStoreObject<T>, new()
    {
        private readonly string _fileName;

        public XmlDataStore(string fileName)
        {
            _fileName = fileName;
        }

        protected override IEnumerable<XElement> ReadRecords()
        {
            if (!FileIO.Delegates.FileExists(_fileName))
                return new XElement[0];

            XDocument xDoc;

            using (var stream = FileIO.Delegates.OpenReadStream(_fileName, true))
                xDoc = XDocument.Load(stream);

            return xDoc.Root.Elements();
        }

        protected override void SaveRecords(IEnumerable<XElement> records)
        {
            var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"), new XElement("records", records));

            using (var stream = FileIO.Delegates.OpenWriteStream(_fileName,true,true))
                xDoc.Save(stream);
        }
    }
}