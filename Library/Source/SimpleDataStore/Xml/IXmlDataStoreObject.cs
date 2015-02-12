using System.Xml.Linq;

namespace Vici.Core.DataStore
{
    public interface IXmlDataStoreObject<T> : IDataStoreObject<T,XElement>
    {
    }
}