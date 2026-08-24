using Serilog.Core;
using Serilog.Events;

namespace Apex_Website_API.Logging
{
    public class DailyLogFileSink : ILogEventSink
    {
        private readonly string _rootPath;
        private readonly object _lock = new();

        private string? _currentFilePath;
        private StreamWriter? _writer;

        public DailyLogFileSink(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_lock)
            {
                var date = logEvent.Timestamp.LocalDateTime;

                var year = date.ToString("yyyy");
                var month = date.ToString("MM-MMMM");
                var day = date.ToString("dd");

                var directoryPath = Path.Combine(_rootPath,year,month);

                Directory.CreateDirectory(directoryPath);

                var filePath = Path.Combine(directoryPath,$"{day}.log");

                // Date changed
                if (_currentFilePath != filePath)
                {
                    _writer?.Dispose();

                    _writer = new StreamWriter(filePath,append: true);
                    _writer.AutoFlush = true;
                    _currentFilePath = filePath;
                }

                var message = $"[{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] " +
                              $"[{logEvent.Level.ToString().ToUpperInvariant()}] " +
                              $"[TraceId:{GetTraceId(logEvent)}] " +
                              $"{logEvent.RenderMessage()}";

                if (logEvent.Exception != null)
                {
                    message += Environment.NewLine + logEvent.Exception;
                }

                _writer.WriteLine(message);
            }
        }

        private static string GetTraceId(LogEvent logEvent)
        {
            if (logEvent.Properties.TryGetValue("TraceId",out var traceId))
            {
                return traceId.ToString().Trim('"');
            }
            return "-";
        }
    }
}