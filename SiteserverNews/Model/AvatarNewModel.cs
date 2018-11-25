using System;
using System.Collections.Generic;
using System.Text;

namespace SiteserverNews.Model
{
    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string uniquekey { get; set; }
        /// <summary>
        /// 车联网领域异军突起，5G法宝能否为华为拔得头筹
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 科技
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// e号研究院
        /// </summary>
        public string author_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string thumbnail_pic_s { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string thumbnail_pic_s02 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string thumbnail_pic_s03 { get; set; }
    }

    public class Result
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> data { get; set; }
    }

    public class AvatarNewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Result result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
    }
}
