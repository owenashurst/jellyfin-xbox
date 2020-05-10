namespace Jellyfin.Logging
{
    public class NLogManager : ILogManager
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public void LogInfo(string text)
        {
            Logger.Info(text);
        }

        public void LogWarn(string text)
        {
            Logger.Warn(text);
        }

        public void LogDebug(string text)
        {
            Logger.Debug(text);
        }

        public void LogError(string text)
        {
            Logger.Error(text);
        }
    }
}
