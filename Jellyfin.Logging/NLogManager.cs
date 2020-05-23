using System;
using NLog;

namespace Jellyfin.Logging
{
    public class NLogManager : ILogManager
    {
        public void LogInfo(string text)
        {
            TaskLogQueue.Instance.LogItemQueue.EnqueueTask(new LogItem
            {
                Level = LogLevel.Info,
                Text = text
            });
        }

        public void LogWarn(string text)
        {
            TaskLogQueue.Instance.LogItemQueue.EnqueueTask(new LogItem
            {
                Level = LogLevel.Warn,
                Text = text
            });
        }

        public void LogDebug(string text)
        {
            TaskLogQueue.Instance.LogItemQueue.EnqueueTask(new LogItem
            {
                Level = LogLevel.Debug,
                Text = text
            });

        }

        public void LogError(string text)
        {
            TaskLogQueue.Instance.LogItemQueue.EnqueueTask(new LogItem
            {
                Level = LogLevel.Error,
                Text = text
            });
        }

        public void LogError(Exception xc, string text)
        {
            TaskLogQueue.Instance.LogItemQueue.EnqueueTask(new LogItem
            {
                Level = LogLevel.Error,
                Exception = xc,
                Text = text
            });
        }
    }
}