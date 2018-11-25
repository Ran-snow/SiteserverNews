using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using SimpleJson;
using System.Net;
using SiteserverNews.Model;

namespace SiteserverNews.Job
{
    /// <summary>
    /// 内容Api接口。
    /// </summary>
    public class SiteserverContentAPI
    {
        private readonly RestClient siteserverClient;
        public SiteserverContentAPI(RestClient restClient)
        {
            siteserverClient = restClient;
        }

        public bool SetContent(DataItem avatarNew)
        {
            var client = new RestClient(avatarNew.url);
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(@"ERROR-爬网页->" + ((int)response.StatusCode).ToString() + response.Content);
            }

            string json = SimpleJson.SimpleJson.SerializeObject(new
            {
                Title = avatarNew.title,
                Content = response.Content,
                IsChecked = true,
                AddDate = avatarNew.date,
                Author = avatarNew.author_name
            });

            client = new RestClient("https://www.ipacs.vip/api/v1/contents/1/5");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-SS-API-KEY", "xxxxxxxxxxxxxx");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);

            response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(@"ERROR-网络->" + ((int)response.StatusCode).ToString() + response.Content);
            }

            return true;
        }
    }
}
