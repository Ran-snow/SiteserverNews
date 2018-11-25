using Quartz;
using Quartz.Impl;
using SiteserverNews.Job;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteserverNews
{
    class Program
    {

        static void Main(string[] args)
        {
            RunProgram().GetAwaiter().GetResult();

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        private async static Task RunProgram()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(200).RepeatForever())//每两秒执行一次
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
