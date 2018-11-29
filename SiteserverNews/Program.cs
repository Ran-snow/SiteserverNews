using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using SiteserverNews.Job;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;

namespace SiteserverNews
{
    class Program
    {
        public static IConfiguration config;
        public static ILog log;

        static void Main(string[] args)
        {
            ConfigLog();

            RunProgram().GetAwaiter().GetResult();

            log.Info("开始抓取新闻");

            Task.Delay(-1).GetAwaiter().GetResult();
        }

        private static void ConfigLog()
        {
            ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            BasicConfigurator.Configure(repository);
            log = LogManager.GetLogger(repository.Name, "SiteserverNews");
        }

        private async static Task RunProgram()
        {
            try
            {
                config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, reloadOnChange: true).Build();

                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();
                log.Info("开始执行计划");

                var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
                .Build();

                var jobDetail = JobBuilder.Create<CreateNewsJob>()
                            .WithIdentity("job", "group")
                            .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (SchedulerException se)
            {
                log.Error(se.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
