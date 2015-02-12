using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vici.Core.Json;

namespace Vici.Core.DataStore
{
    public class JsonDataStore<T> : DataStore<T,object> where T : IJsonDataStoreObject<T>, new()
    {
        private readonly string _fileName;

        public JsonDataStore(string fileName)
        {
            _fileName = fileName;
        }

        protected override IEnumerable<object> ReadRecords()
        {
            if (!FileIO.Delegates.FileExists(_fileName))
                return new Object[0];

            JsonObject json = JsonParser.Parse(FileIO.Delegates.ReadAllText(_fileName));

            return json.AsArray();
        }

        protected override void SaveRecords(IEnumerable<object> records)
        {
            FileIO.Delegates.WriteAllText(_fileName, JsonSerializer.ToJson(records.ToArray()));
        }
    }
}