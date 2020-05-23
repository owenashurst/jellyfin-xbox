using System;
using NLog;

namespace Jellyfin.Logging
{
    public class LogItem
    {
        public LogLevel Level { get; set; }

        public string Text { get; set; }

        public Exception Exception { get; set; }
    }
}
