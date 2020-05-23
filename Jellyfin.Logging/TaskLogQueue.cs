using Jellyfin.Core;
using NLog;

namespace Jellyfin.Logging
{
    public class TaskLogQueue
    {
        #region Singleton

        private static TaskLogQueue _instance;

        public static TaskLogQueue Instance => _instance ?? (_instance = new TaskLogQueue());

        private TaskLogQueue()
        {
            LogItemQueue = new TaskQueue<LogItem>(1, ProcessLogItem);
        }

        #endregion

        #region Properties

        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public TaskQueue<LogItem> LogItemQueue { get; set; }

        #endregion

        #region Additional methods

        private void ProcessLogItem(LogItem i)
        {
            if (i.Level == LogLevel.Error)
            {
                Logger.Error(i.Exception, i.Text);
            }
            else
            {
                Logger.Log(i.Level, i.Text);
            }
            
        }

        #endregion
    }
}
