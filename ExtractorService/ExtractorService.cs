using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using Quartz;
using Quartz.Impl;

namespace ExtractorFacturero.Service
{
    public partial class ExtractorService : ServiceBase
    {
        private string eventSourceName = "Facturero.cr";
        private string logName = "Application";

        public ExtractorService()
        {
            InitializeComponent();

            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }
        }

        protected override void OnStart(string[] args)
        {
            new EventLog
            {
                Source = eventSourceName,
                Log = logName
            }.WriteEntry("Servicio Iniciado");

            IJobDetail job = JobBuilder.Create<ExtractorOperations>()
                    .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(2)
                    .RepeatForever())
                .Build();

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

        protected override void OnStop()
        {
            StdSchedulerFactory.GetDefaultScheduler().Shutdown();
        }
    }
}
