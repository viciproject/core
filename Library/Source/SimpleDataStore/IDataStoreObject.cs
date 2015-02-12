using System;
using System.Xml.Linq;

namespace Vici.Core.DataStore
{
    public interface IDataStoreObject<T,TSource> : IEquatable<T>
    {
        void Deserialize(TSource source);
        TSource Serialize();
    }
}