using RestSharp;
using SiteserverNews.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SiteserverNews.Job
{
    public class AvatarNewAPI
    {
        private readonly RestClient avatarClient;
        public AvatarNewAPI(RestClient restClient)
        {
            avatarClient = restClient;
        }

        public AvatarNewModel GetNews()
        {
            #region Get news
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("key", "xxxxxxxxxxxxxxxxxxxxxxx");
            request.AddQueryParameter("type", "keji");
            avatarClient.BaseUrl = new Uri("http://api.avatardata.cn/TouTiao/Query");
            IRestResponse<AvatarNewModel> response = avatarClient.Execute<AvatarNewModel>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(@"ERROR-网络->" + ((int)response.StatusCode).ToString() + response.Content);
            }

            if (response.Data.error_code != 0)
            {
                throw new Exception(@"ERROR-数据->" + response.Data.reason);
            }


            return response.Data;

            #endregion
        }
    }
}
