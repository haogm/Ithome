using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using System.IO;

namespace Ithome爬虫
{
    class Program
    {       
        static Regex ex = new Regex(@"unescape\('(?<data>[\s\S]+)'\)");
        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args)
        {
            Console.WriteLine("请输入起始页编号：");
            int start = int.Parse(Console.ReadLine()); //获取第一个评论页面编号
            Console.WriteLine("请输入结束页编号：");
            int end = int.Parse(Console.ReadLine());  //获取最后一个评论页面编号
            string url = "http://www.ithome.com/ithome/CommentCount.aspx?newsid=";
            WebClient wc = null;
            string html = string.Empty;
            for (int i = start; i <= end; i++)
            {
                Console.WriteLine("读取第" + i + "个页面");
                wc = new WebClient();
                wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");
                html = wc.DownloadString(url + i.ToString());
                ResolveComment(html);
            }
            File.WriteAllText("1.txt",sb.ToString());
        }
        /// <summary>
        /// 解析评论
        /// </summary>
        static void ResolveComment(string html)
        {
            Console.WriteLine("读取到一个页面");
            var m = ex.Match(html);
            if (m.Length==0) //如果匹配不到，则返回继续下一个循环
            {
                return;
            }
            Console.WriteLine("获取到评论列表");
            var data = m.Groups["data"].Value;
            var result = HttpUtility.UrlDecode(data);
            var doc = new HtmlDocument();
            doc.LoadHtml(result);
            var nodes = doc.DocumentNode.SelectNodes("/ul[@class='list']/li");
            if (nodes==null||nodes.Count==0) //如果抓取不到任何节点，则返回执行下一个循环
            {
                return;   
            }
            foreach (var li in nodes)
            {
                var p = li.SelectSingleNode("./p");
                if (p.InnerHtml.Trim().Length >= 50) //如果评论字符数大于50，则拼接成字符串
                {
                    sb.Append(p.InnerHtml.Trim() + "\r\n");
                        Console.WriteLine("成功写入一条评论");
                }
            }
        }    
    }
}
