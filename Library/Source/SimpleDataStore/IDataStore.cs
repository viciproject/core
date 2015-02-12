using System;
using System.Collections.Generic;

namespace Vici.Core.DataStore
{
    public interface IDataStore<T>
    {
        IEnumerable<T> Read(Predicate<T> condition = null);
        void Save(T newObject);
        T ReadOne(Predicate<T> condition);
        void Delete(T obj);
        void Delete(Predicate<T> condition);
    }
}