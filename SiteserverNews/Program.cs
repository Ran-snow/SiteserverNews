using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using SiteserverNews.Job;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteserverNews
{
    class Program
    {
        public static IConfiguration Config;

        static void Main(string[] args)
        {
            RunProgram().GetAwaiter().GetResult();

            Console.WriteLine("开始抓取新闻");
            Console.ReadKey();
        }

        private async static Task RunProgram()
        {
            try
            {
                Config = new ConfigurationBuilder()
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
                Console.WriteLine("开始执行计划");

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
                await Console.Error.WriteLineAsync(se.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
