using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace IMSWEB.Infrastructure.Diagnostics
{
    public static class RequestDiagnostics
    {
        private const string ContextKey = "IMSWEB.RequestDiagnostics";
        public const int SlowRequestThresholdMs = 800;
        private const int MaxSqlLogLength = 20000;

        public static void StartRequest()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            if (context.Items[ContextKey] == null)
            {
                context.Items[ContextKey] = new RequestDiagnosticsContext();
            }

            var diagnostics = (RequestDiagnosticsContext)context.Items[ContextKey];
            diagnostics.Stopwatch.Restart();
        }

        public static RequestDiagnosticsSummary StopRequest()
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return RequestDiagnosticsSummary.Empty;
            }

            var diagnostics = context.Items[ContextKey] as RequestDiagnosticsContext;
            if (diagnostics == null)
            {
                return RequestDiagnosticsSummary.Empty;
            }

            diagnostics.Stopwatch.Stop();

            var summary = new RequestDiagnosticsSummary(
                diagnostics.Stopwatch.ElapsedMilliseconds,
                diagnostics.DbDurationMs,
                diagnostics.SqlLog.ToString());

            diagnostics.Reset();
            return summary;
        }

        public static void AddDbDuration(TimeSpan duration)
        {
            var diagnostics = GetContext();
            if (diagnostics == null)
            {
                return;
            }

            diagnostics.DbDurationMs += (long)duration.TotalMilliseconds;
        }

        public static void AppendEfLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var diagnostics = GetContext();
            if (diagnostics == null)
            {
                return;
            }

            var sanitized = SanitizeEfLog(message);
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return;
            }

            if (diagnostics.SqlLog.Length >= MaxSqlLogLength)
            {
                return;
            }

            if (diagnostics.SqlLog.Length + sanitized.Length > MaxSqlLogLength)
            {
                sanitized = sanitized.Substring(0, MaxSqlLogLength - diagnostics.SqlLog.Length);
            }

            diagnostics.SqlLog.Append(sanitized);
        }

        private static RequestDiagnosticsContext GetContext()
        {
            var context = HttpContext.Current;
            return context?.Items[ContextKey] as RequestDiagnosticsContext;
        }

        private static string SanitizeEfLog(string message)
        {
            var sanitized = Regex.Replace(message, @"Parameters=\[(.*?)\]", "Parameters=[REDACTED]", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, @"@__.+?=.+?(,|\])", "@__REDACTED=$1", RegexOptions.IgnoreCase);
            return sanitized;
        }

        private sealed class RequestDiagnosticsContext
        {
            public Stopwatch Stopwatch { get; } = new Stopwatch();
            public StringBuilder SqlLog { get; } = new StringBuilder();
            public long DbDurationMs { get; set; }

            public void Reset()
            {
                SqlLog.Clear();
                DbDurationMs = 0;
                Stopwatch.Reset();
            }
        }
    }

    public sealed class RequestDiagnosticsSummary
    {
        public static readonly RequestDiagnosticsSummary Empty = new RequestDiagnosticsSummary(0, 0, string.Empty);

        public RequestDiagnosticsSummary(long elapsedMs, long dbDurationMs, string sqlLog)
        {
            ElapsedMs = elapsedMs;
            DbDurationMs = dbDurationMs;
            SqlLog = sqlLog ?? string.Empty;
        }

        public long ElapsedMs { get; }
        public long DbDurationMs { get; }
        public string SqlLog { get; }
        public bool HasSqlLog => !string.IsNullOrWhiteSpace(SqlLog);
    }
}
