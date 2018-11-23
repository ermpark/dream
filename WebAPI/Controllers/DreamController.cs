using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Text;
using WebAPI.Models;
using AngleSharp.Parser.Html;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DreamController : ControllerBase
    {
        static int? PageEnd = null;
        static string PageEndUrl = null;
        private MainDBContext _context;
        public DreamController(MainDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetMengXinfo()
        {
            GetData("http://www.xzw.com/jiemeng/lib/qita/", 1);
            return Ok(PageEnd);
        }
        [Route("detail")]
        [HttpGet]
        public async Task<IActionResult> GetDreamInfo()
        {
            var domain = "http://www.xzw.com";
            //查询出其他分类的梦数据来解析详细内容
            var dreamList = _context.Dream.Where(l => l.CateName == "其他").ToList();
            foreach (var dream in dreamList)
            {
                GetDetailData(dream.Id, domain + dream.Url);
            }
            return Ok(PageEnd);
        }
        /// <summary>
        /// 获取详情页的页面解析
        /// </summary>
        /// <param name="dreamId"></param>
        /// <param name="url"></param>
        public void GetDetailData(int dreamId, string url)
        {
            var html = GetHtml(url);
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            //#wraper > div.main-wrap > div.pleft.fl > div.viewbox.box > div.sbody
            var sbody = document.QuerySelector("#wraper > div.main-wrap > div.pleft.fl > div.viewbox.box > div.sbody");
            var dllist = sbody.QuerySelectorAll("dl");
            var title = sbody.QuerySelector("h2").TextContent;//标题
            if (dllist.Length > 0)
            {
                foreach (var detail in dllist)
                {
                    //#wraper > div.main-wrap > div.pleft.fl > div.viewbox.box > div.sbody > dl:nth-child(4) > dt > strong
                    var info = new DreamInfo();
                    info.FkDreamId = dreamId;
                    info.DreamName = title;
                    info.Name = detail.QuerySelector("dt > strong").TextContent;
                    info.Content = detail.QuerySelector("dd").TextContent; ;
                    info.CreateTime = DateTime.Now;
                    _context.DreamInfo.Add(info);
                    _context.SaveChanges();
                }

            }
        }
        /// <summary>
        /// 定义一个获取列表数据的方法,需要传递一个列表页面的url地址
        /// </summary>
        /// <param name="rul"></param>
        public void GetData(string url, int pageIndex)
        {
            var thisUrl = url + pageIndex + ".html";
            var html = GetHtml(thisUrl);
            //创建一个（可重用）解析器前端
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            var mengList = document.QuerySelectorAll("#list > div.main > div.l-item > ul > li");
            //#list > div.main > div.pagelist > a.end
            //获取最末页数据
            if (!PageEnd.HasValue)
            {
                var pageEnd = document.QuerySelector("#list > div.main > div.pagelist > a.end")?.TextContent;
                PageEnd = Convert.ToInt32(pageEnd);
            }
            var list = new List<Dream>();
            for (var i = 0; i < mengList.Length; i++)
            {
                var meng = new Dream();
                meng.CateName = "其他";
                //名称
                meng.Name = mengList[i].QuerySelector("h3 > a").TextContent;
                //简介
                meng.Summary = mengList[i].QuerySelector("p").TextContent;
                //连接
                meng.Url = mengList[i].QuerySelector("h3 > a").GetAttribute("href");
                meng.CreateTime = DateTime.Now;
                _context.Dream.Add(meng);
                _context.SaveChanges();
            }
            //翻页记录页码
            pageIndex++;
            if (pageIndex <= Convert.ToInt32(PageEnd))
            {
                Console.WriteLine(pageIndex + "/" + PageEnd);
                //翻页完之前一直抓取
                GetData(url, pageIndex);
            }
        }
        /// <summary>
        /// 定义一个获取html页面内容的方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string GetHtml(string url)
        {
            HttpWebRequest myReq =
            (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            return readStream.ReadToEnd();
        }

    }
}