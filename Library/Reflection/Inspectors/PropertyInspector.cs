using System.Reflection;

namespace Vici.Core
{
    public class PropertyInspector : MemberInspector
    {
        private PropertyInfo _propertyInfo;

        public PropertyInspector(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }

        public bool CanWrite
        {
            get { return _propertyInfo.CanWrite; }
        }

    }
}