using System;
using System.Linq;

namespace Vici.Core
{
    public static class CoreExtensions
    {
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter) 
        {  
            if (array == null)  
                throw new ArgumentException();  
#if WINDOWS_PHONE || SILVERLIGHT
            return (from item in array select converter(item)).ToArray();  
#else
            return Array.ConvertAll(array,converter);
#endif
        } 
    }
}
