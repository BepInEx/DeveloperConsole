using BepInEx.Logging;

namespace BepInEx
{
    public partial class DeveloperConsole
    {
        private sealed class LogListener : ILogListener
        {
            public void Dispose() { }

            public void LogEvent(object sender, LogEventArgs eventArgs)
            {
                OnEntryLogged(eventArgs);
            }
        }
    }
}
