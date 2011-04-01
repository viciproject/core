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

namespace Vici.Core.Logging
{
    public abstract class LoggingProvider : IDisposable
    {
        private LogLevel _logLevelMask = LogLevel.All;
        private LogLevel _minimumLogLevel = LogLevel.Information;

        public LoggingProvider()
        {
            TimeFormatString = "yyyy.MM.dd HH:mm:ss.ff";
        }

        public virtual LogLevel LogLevelMask
        {
            get { return _logLevelMask; }
            set { _logLevelMask = value; }
        }

        public virtual LogLevel MinimumLogLevel
        {
            get { return _minimumLogLevel; }
            set { _minimumLogLevel = value; }
        }

        public string TimeFormatString { get; set; }

        public abstract void Dispose();

        public abstract void LogText(DateTime timeStamp, LogLevel logLevel, string s);

        public virtual void LogException(DateTime timeStamp, LogLevel logLevel, Exception e)
        {
            string text = "";

            Exception innerException = e;
            
            while (innerException != null)
            {
                text += "*** Exception: " + innerException.Message + "\r\n";
                text += "*** Stacktrace: " + innerException.StackTrace + "\r\n";
            
                innerException = innerException.InnerException;
            }
            
            LogText(timeStamp, logLevel, text);
        }

        public virtual string FormatTime(DateTime time)
        {
            return time.ToString(TimeFormatString);
        }
    }
}