using Quartz;
using RestSharp;
using SiteserverNews.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiteserverNews.Job
{
    public class CreateNewsJob : IJob
    {
        static RestClient restClient = new RestClient();

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                try
                {
                    AvatarNewAPI avatarNewAPI = new AvatarNewAPI(restClient);

                    AvatarNewModel avatarNewModel = avatarNewAPI.GetNews();

                    SiteserverContentAPI siteserverContentAPI = new SiteserverContentAPI(restClient);

                    foreach (var item in avatarNewModel.result.data)
                    {
                        siteserverContentAPI.SetContent(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CreateNewsJob->" + ex.Message);
                    throw ex;
                }

            });
        }
    }
}
