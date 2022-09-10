using System;
using System.Collections.Generic;
using System.Text;
using BaiduHelper;
using System.Text.RegularExpressions;
namespace Tieba
{
    public class Lcid
    {
        public string Url;
        public List<string> lun = new List<string>();

         public List<string> llevel = new List<string>();

        public List<string> lcid = new List<string>();

        public List<string> ltime = new List<string>();

        public List<string> lcontent = new List<string>();

       // public List<string> lhimg = new List<string>();

        public List<string> luid = new List<string>();
        public List<string> lpor = new List<string>();

        public Lcid() { }

        public Lcid(string tid,string fid,int pn=1) 
        {

            Url = "https://tieba.baidu.com/p/totalComment?tid="+tid+"&fid="+fid+"&pn="+pn+"&see_lz=0";
            GetHtml();
        }
        //public static string GetTime(string timeStamp)
        //{
        //    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //    long lTime = long.Parse(timeStamp + "0000000");
        //    TimeSpan toNow = new TimeSpan(lTime);
        //    return string.Format("{0:g}", dtStart.Add(toNow));
        //}  
  
        public void GetHtml()
        {

            
            try
            {
                string html = HttpHelper.HttpGet(Url, Encoding.UTF8,null,false,true);
            //new Regex("<a.*?href=[^>]+?>([^<]+?)</a>")new Regex(@"<img.*?src=\\""?(.*?)\\""?[^>]+?>")
                html=html.Replace("\\/", "/");
               // html = Regex.Replace(html, @"\\u56de\\u590d <a.*?href=[^>]+?>[^<]+?</a>  :", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, "<a.*?href=[^>]+?>([^<]+?)</a>", "$1", RegexOptions.IgnoreCase);
                // html = Regex.Replace(html, @"<img.*?src=\\""?(.*?)\\""?[^>]+?>", "$1", RegexOptions.IgnoreCase);
             //   string htm2 = html.Substring(html.IndexOf("\"user_list\""));
                // 6945808321 Regex rg = new Regex(@"post_id"":""(\d{1,12})"",""comment_id"":""(\d{1,12})"",""username"":""([^""]*)"",""user_id"":([^,]+),""now_time"":[^,]+,""content"":""(.*?)"",");
                Regex rg =  new Regex(@"post_id"":(\d{1,12}),""content"":""(.*?)"",""username"":""([^""]*)"",""now_time"":([^,]+),""user_id"":([^,]+),.+?""comment_id"":(\d{1,12}),.+?show_nickname"":""([^""]+)");
                MatchCollection mcs1 = new Regex("\"user_id\":([^,]+),\"user_name.+?\"portrait\":\"([^\"]+)\",\"nickname\":\"[^\"]+\"").Matches(html);
                MatchCollection mcs=rg.Matches(html);
                Dictionary<string, string> uidpor = new Dictionary<string, string>();
                foreach (Match item in mcs1)
                {
                    if (!uidpor.ContainsKey(item.Groups[1].Value))
                    {
                        uidpor.Add(item.Groups[1].Value, item.Groups[2].Value);
                    }
                    
                }
              
                foreach (Match item in mcs)
                {
                    string un = item.Groups[3].Value == "" ? "昵称:" + item.Groups[7].Value : item.Groups[3].Value;
                    lun.Add(Regex.Unescape( un));
                    llevel.Add("0");
                    lcid.Add(item.Groups[6].Value);
                    luid.Add(item.Groups[5].Value);
                   // string par = "portrait\":\"([^\"]+)\",\"nickname\":\""+ item.Groups[7].Value.Replace("\\","\\\\");
                    lpor.Add(uidpor[item.Groups[5].Value]);
                    ltime.Add(item.Groups[4].Value);
                   // string ssss = item.Groups[5].Value;
                    lcontent.Add(Regex.Unescape(item.Groups[2].Value));

                    //ss++;
                }

                
            }
            catch (Exception ee)
            {
                throw new Exception("Lcid->"+ee.Message); 
                
            }
        
        }
    }
}
