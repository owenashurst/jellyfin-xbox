using System;

namespace Jellyfin.Logging
{
    public interface ILogManager
    {
        void LogInfo(string text);
        void LogWarn(string text);
        void LogDebug(string text);
        void LogError(string text);
        void LogError(Exception xc, string text);
    }
}
