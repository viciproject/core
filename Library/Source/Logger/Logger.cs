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
using System.Linq;

namespace Vici.Core.Logging
{
    public class Logger : IDisposable
    {
        private readonly List<LoggingProvider> _loggingProviders = new List<LoggingProvider>();

        private readonly object _instanceLock = new object();

        public static Logger Default { get; private set; }

        static Logger()
        {
            Default = new Logger();
        }

        public void Dispose()
        {
            foreach (LoggingProvider provider in _loggingProviders)
                provider.Dispose();
        }

        public void AddProvider(LoggingProvider provider)
        {
            lock (_instanceLock)
                _loggingProviders.Add(provider);
        }

        public void Log(string formatString, params object[] p)
        {
            Log(LogLevel.Information, formatString, p);
        }

        public void LogException(Exception e)
        {
            LogException(LogLevel.Information, e);
        }

        public void LogException(LogLevel level, Exception e)
        {
            DateTime now = DateTime.Now;

            lock (_instanceLock)
            {
                foreach (var provider in _loggingProviders.Where(provider => (level & provider.LogLevelMask) != 0 && level >= provider.MinimumLogLevel))
                {
                    provider.LogException(now, level, e);
                }
            }
        }

        public void Log(LogLevel level, string formatString, params object[] p)
        {
            string fmt = String.Format(formatString, p);

            DateTime now = DateTime.Now;

            lock (_instanceLock)
            {
                foreach (var provider in _loggingProviders.Where(provider => (level & provider.LogLevelMask) != 0 && level >= provider.MinimumLogLevel))
                {
                    provider.LogText(now, level, fmt);
                }
            }
        }
    }
}