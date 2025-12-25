using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;

namespace IMSWEB.Infrastructure.Diagnostics
{
    public class RequestTimingDbInterceptor : DbCommandInterceptor
    {
        private static readonly string StopwatchKey = "IMSWEB.DbStopwatch";

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            StartTiming(interceptionContext);
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            StopTiming(interceptionContext);
            base.ReaderExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            StartTiming(interceptionContext);
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            StopTiming(interceptionContext);
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            StartTiming(interceptionContext);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            StopTiming(interceptionContext);
            base.ScalarExecuted(command, interceptionContext);
        }

        private static void StartTiming<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            interceptionContext.UserState[StopwatchKey] = Stopwatch.StartNew();
        }

        private static void StopTiming<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            if (interceptionContext.UserState[StopwatchKey] is Stopwatch stopwatch)
            {
                stopwatch.Stop();
                RequestDiagnostics.AddDbDuration(stopwatch.Elapsed);
            }
        }
    }
}
