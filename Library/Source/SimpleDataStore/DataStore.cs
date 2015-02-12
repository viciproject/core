using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vici.Core.Json;

namespace Vici.Core.DataStore
{
    public abstract class DataStore<T,TSerialized> : IDataStore<T> where T : IDataStoreObject<T,TSerialized>, new()
    {
        private readonly object _lock = new object();

        protected abstract IEnumerable<TSerialized> ReadRecords();
        protected abstract void SaveRecords(IEnumerable<TSerialized> records);

        public IEnumerable<T> Read(Predicate<T> condition = null)
        {
            lock (_lock)
            {
                foreach (var entry in ReadRecords())
                {
                    T obj = new T();

                    obj.Deserialize(entry);

                    if (condition == null || condition(obj))
                        yield return obj;
                }
            }
        }

        private void SaveOrDelete(T newObject, bool delete)
        {
            lock (_lock)
            {
                bool replaced = false;

                var newList = new List<TSerialized>();

                foreach (var record in ReadRecords())
                {
                    var obj = new T();

                    obj.Deserialize(record);

                    if (obj.Equals(newObject))
                    {
                        if (!delete)
                            newList.Add(newObject.Serialize());

                        replaced = true;
                    }
                    else
                    {
                        newList.Add(record);
                    }
                }

                if (!replaced && !delete)
                    newList.Add(newObject.Serialize());

                SaveRecords(newList);
            }
        }

        public void Save(T newObject)
        {
            SaveOrDelete(newObject,false);
        }

        public T ReadOne(Predicate<T> condition)
        {
            return Read(condition).FirstOrDefault();
        }

        public void Delete(T obj)
        {
            SaveOrDelete(obj, true);
        }

        public void Delete(Predicate<T> condition)
        {
            lock (_lock)
            {
                var newList = new List<TSerialized>();

                foreach (var record in ReadRecords())
                {
                    var obj = new T();

                    obj.Deserialize(record);

                    if (!condition(obj))
                        newList.Add(record);
                }

                SaveRecords(newList);
            }
        }
    }

}