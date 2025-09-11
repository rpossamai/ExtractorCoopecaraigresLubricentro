using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Quartz;

using ExtractorFacturero.Core;

namespace ExtractorFacturero.Service
{
    class ExtractorOperations : IJob
    {
        private static string eventSourceName = "Facturero.cr";
        private static string logName = "Application";

        private static void WriteEventLogEntry(string fmt, params object[] args)
        {
            new EventLog
            {
                Source = eventSourceName,
                Log = logName
            }.WriteEntry(string.Format(fmt, args));
        }

        public void Execute(IJobExecutionContext ctx)
        {
            try
            {
                SyncOperations ops = new SyncOperations();
                ops.UpdateUnackedDocuments();
                ops.UpdateStatuslessInquiries();
                ops.PostPendingDocuments();
                //ops.PostPendingNotes();
            }
            catch (Exception ex)
            {
                WriteEventLogEntry(ex.ToString());
            }
        }
    }
}
