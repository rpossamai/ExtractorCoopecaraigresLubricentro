using System;

using log4net;
using log4net.Config;

using ExtractorFacturero.Core;

namespace ExtractorFacturero.Console
{
    public class Program
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            LOG.Info("Iniciando el extractor");

            try
            {
                SyncOperations ops = new SyncOperations();
                //ops.UpdateUnackedDocuments();
                //ops.UpdateStatuslessInquiries();
                ops.PostPendingDocuments();
                //ops.PostPendingNotes();
            }
            catch (Exception ex)
            {
                LOG.Error("Error del servicio", ex);
            }
        }
    }
}
