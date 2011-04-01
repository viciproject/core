#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2011 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

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
