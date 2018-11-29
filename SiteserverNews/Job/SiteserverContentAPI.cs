using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using SimpleJson;
using System.Net;
using SiteserverNews.Model;
using AngleSharp.Parser.Html;
using System.Threading.Tasks;
using System.Security.Cryptography;
using RestSharp.Extensions;

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

        public async Task<bool> SetContent(DataItem avatarNew)
        {
            var client = new RestClient(avatarNew.url);
            IRestResponse response = client.Execute(new RestRequest(Method.GET));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(@"ERROR-爬网页->" + ((int)response.StatusCode).ToString() + response.Content);
            }

            var parser = new HtmlParser();
            var document = await parser.ParseAsync(response.Content);
            var news = document.QuerySelector("#J_article").OuterHtml;
            var title = document.QuerySelector(".article-title").OuterHtml;
            news = news.Replace(title, string.Empty);

            string json = SimpleJson.SimpleJson.SerializeObject(new
            {
                Title = avatarNew.title,
                Content = news,
                IsChecked = true,
                CheckedLevel = 10,
                AddDate = avatarNew.date,
                Author = avatarNew.author_name,
                ImageUrl = avatarNew.thumbnail_pic_s
            });

            // POST /api/v1/contents/{siteId}/{channelId} HTTP/1.1
            // siteId       path  整数 是 站点Id
            // channelId    path  整数 是 栏目Id
            client = new RestClient("https://www.ipacs.vip/api/v1/contents/1/6");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-SS-API-KEY", Program.config["siteserver:key"]);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", json, ParameterType.RequestBody);

            Program.log.Info("插入新闻->" + json.Replace(Environment.NewLine, string.Empty));
            response = await Task.Run(() => client.Execute(request));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(@"ERROR-网络->" + ((int)response.StatusCode).ToString() + response.Content);
            }

            return true;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public async Task<string> Login()
        {
            string json = SimpleJson.SimpleJson.SerializeObject(new
            {
                account = Program.config["siteserver:username"],
                password = Md5ByString(Program.config["siteserver:userpwd"])
            });

            var client = new RestClient("https://www.ipacs.vip/api/v1/administrators/actions/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("X-SS-API-KEY", Program.config["siteserver:key"]);
            request.AddParameter("undefined", json, ParameterType.RequestBody);

            IRestResponse response = await Task.Run(() => client.Execute(request));

            return response.Content;
        }

        private static string Md5ByString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
