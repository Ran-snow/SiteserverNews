using Quartz;
using RestSharp;
using SiteserverNews.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteserverNews.Job
{
    public class CreateNewsJob : IJob
    {
        static RestClient restClient = new RestClient();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"当前时间为{DateTime.Now.ToString()},开始执行任务");
                SiteserverContentAPI siteserverContentAPI = new SiteserverContentAPI(restClient);
                await siteserverContentAPI.Login();

                AvatarNewModel avatarNewModel = await new AvatarNewAPI(restClient).GetNews();

                List<Task<bool>> tasks = new List<Task<bool>>();

                foreach (var item in avatarNewModel.result.data)
                {
                    tasks.Add(siteserverContentAPI.SetContent(item));
                }

                await Task.WhenAll(tasks);

                Console.WriteLine($"当前时间为{DateTime.Now.ToString()},任务完成");
                return;
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CreateNewsHttpRequestJob->" + ex.Message);
                throw ex;
            }
            catch (NullReferenceException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CreateNewsNullReferenceJob->" + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CreateNewsExceptionJob->" + ex.Message);
                throw ex;
            }

        }
    }
}
