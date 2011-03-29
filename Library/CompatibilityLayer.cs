using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vici.Core.CompatibilityLayer
{
#if WINDOWS_PHONE || SILVERLIGHT
    public static class SilverlightExtensions
    {
        public static void RemoveAll<T>(this List<T> list, Func<T, bool> filter)
        {
            for (int i = list.Count-1; i >=0 ; i--)
            {
                if (filter(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }

        public delegate bool TypeFilter(
            Type m,
            Object filterCriteria
            );

        public static Type[] FindInterfaces(this Type type, TypeFilter typeFilter, object filterCriteria )
        {
            return type.GetInterfaces().Where(i => typeFilter(i, filterCriteria)).ToArray();
        }
    }
#endif

    public static class File
    {
        public static string ReadAllText(string filename)
        {
#if WINDOWS_PHONE || SILVERLIGHT
            using (var stream = System.IO.File.OpenText(filename))
            {
                return stream.ReadToEnd();
            }
#else
            return System.IO.File.ReadAllText(filename);
#endif
        }

        public static void AppendAllText(string filename, string text)
        {
#if WINDOWS_PHONE || SILVERLIGHT
            using (var stream = System.IO.File.OpenWrite(filename))
            {
                stream.Seek(0, SeekOrigin.End);

                using (var textStream = new StreamWriter(stream))
                {
                    textStream.Write(text);
                }
            }
#else
            System.IO.File.AppendAllText(filename,text);
#endif            
        }
        
    }
}
