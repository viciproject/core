#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2012 Philippe Leybaert
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

namespace Vici.Core
{
    public static class FileIO
    {
        public delegate string ReadAllText(string path);
        public delegate void WriteAllText(string path, string s);
        public delegate bool FileExists(string path);
        public delegate Stream OpenReadStream(string path, bool exclusive);
        public delegate Stream OpenWriteStream(string path, bool exclusive, bool create);

        public class FileIODelegates
        {
            public ReadAllText ReadAllText;
            public WriteAllText WriteAllText;
            public Func<string, string> BuildFullPath;
            public FileExists FileExists;
            public OpenReadStream OpenReadStream;
            public OpenWriteStream OpenWriteStream;
        }

        //public static ReadAllText _ReadAllText = path => null;

        public static FileIODelegates Delegates = new FileIODelegates()
#if PCL
        {
            ReadAllText = path => null,
            WriteAllText = (path, s) => { },
            BuildFullPath = path => path,
            FileExists = path => false,
            OpenReadStream = (path,exclusive) => Stream.Null,
            OpenWriteStream = (path,exclusive,create) => Stream.Null
        };
#else
        {
            ReadAllText = path => File.ReadAllText(path),
            WriteAllText = (path,s) => File.WriteAllText(path,s),
            BuildFullPath = path => Path.GetFullPath(path),
            FileExists = path => File.Exists(path),
            OpenReadStream = (path, exclusive) => File.Open(path,FileMode.Open,FileAccess.Read,exclusive ? FileShare.None : FileShare.Read),
            OpenWriteStream = (path, exclusive, create) => File.Open(path, create ? FileMode.Create : FileMode.Open, FileAccess.ReadWrite, exclusive ? FileShare.None : FileShare.Read),

        };
#endif

    }
}
