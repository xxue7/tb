﻿using System;
using System.Collections.Generic;
using BaiduHelper;
using System.Text.RegularExpressions;

namespace Tieba
{
    class ClientTit
    {
        // private string Url { set; get; }
        //  public string CidUrl { set; get; }
        public string title { set; get; }

        public List<string> Content = new List<string>();

        public List<string> UnContent = new List<string>();

        public List<string> Times = new List<string>();

        public List<string> Authors = new List<string>();

        public List<string> Pids = new List<string>();

        public List<string> Level = new List<string>();

        //public List<string> Himgs = new List<string>();

        public List<string> Uids = new List<string>();

        public int maxPn { set; get; }

        //private int pn;

        // private string tid;



        // private Lcid lcid;
        // public long t2;
        public ClientTit(string tid, int pn = 1)
        {

            string data = "_client_id=wappc_1568832181855_350&_client_type=2&_client_version=12.21.1.0&_phone_imei=fbce298035372371ff25decc0951c03&back=0&kz=" + tid + "&net_type=3&pn=" + pn + "&q_type=2&rn=30&with_floor=1";
            data = data + "&sign=" + HttpHelper.GetMD5HashFromFile(data.Replace("&", "") + "tiebaclient!!!");
            string res = HttpHelper.HttpPost(Conf.APP_URL + "/c/f/pb/page", data, null, null);
            //Pids.AddRange(HttpHelper.Jq(res, "\"pids\":\"", ",\"").Split(','));5803678320
            title = HttpHelper.Jq(res, "collect_mark_pid\":\"0\",\"title\":\"", "\"");
            if (title == null)
            {
                throw new Exception("获取标题错误，请检查帖子是否存在");
            }
            title = Regex.Unescape(title);
            maxPn = int.Parse(HttpHelper.Jq(res, "\"total_page\":\"", "\""));
          //  int indexsplit = res.IndexOf(",\"user_list\":[");
            MatchCollection mcs = new Regex(@"""id"":""([^""]+)"",""title"".+?""time"":""(\d+)"",""content"":\[(.*?)\],""lbs_info"".+?(\]|\}),""author_id"":""([^""""]+)""").Matches(HttpHelper.Jq(res, "\"post_list\":[", "],\"thread\":{"));
            MatchCollection mc1 = new Regex(@"""id"":""([^""]+)"",""portrait"":"".+?"",""name"":""([^""]*)"",""name_show"":""([^""]+)"",.+?""level_id"":""([^""]*)""").Matches(HttpHelper.Jq(res, "\"user_list\":[", "partial_visible_toast"));
            Dictionary<string, string[]> uidname = new Dictionary<string, string[]>();
            for (int i = 0; i < mc1.Count; i++)
            {
                if (mc1[i].Groups[4].Value == "")
                {
                    continue;
                }
                uidname.Add(mc1[i].Groups[1].Value, new string[] { mc1[i].Groups[2].Value, mc1[i].Groups[3].Value, mc1[i].Groups[4].Value });
            }

            for (int i = 0; i < mcs.Count; i++)
            {
                if (!uidname.ContainsKey(mcs[i].Groups[5].Value))
                {
                    continue;
                }
                Pids.Add(mcs[i].Groups[1].Value);
                Times.Add(mcs[i].Groups[2].Value);
                UnContent.Add(mcs[i].Groups[3].Value);
                Content.Add(Regex.Unescape(mcs[i].Groups[3].Value));
                Uids.Add(mcs[i].Groups[5].Value);
                string[] namelev = uidname[mcs[i].Groups[5].Value];
                Authors.Add(Regex.Unescape(namelev[0] == "" ? "昵称:" + namelev[1] : namelev[0]));
                // Himgs.Add("https://gss0.bdstatic.com/6LZ1dD3d1sgCo2Kml5_Y_D3/sys/portrait/item/" + mcs[i].Groups[5].Value);
                Level.Add(namelev[2]);

            }



            if (new Regex(@"sub_post_number"":""[1-9][^""]*""").IsMatch(res))
            {
                Lcid lcid = new Lcid();
                try
                {
                    // Common.Fid = "190294";
                    if (Common.Fid == null)
                    {
                        lcid = new Lcid(tid, HttpHelper.Jq(res, "forum\":{\"id\":\"", "\""), pn);
                    }
                    else
                    {
                        lcid = new Lcid(tid, Common.Fid, pn);
                    }


                }
                catch (Exception)
                {


                }

                if (lcid.lun.Count > 0)
                {
                    Content.AddRange(lcid.lcontent);
                    Authors.AddRange(lcid.lun);
                    Pids.AddRange(lcid.lcid);
                    Times.AddRange(lcid.ltime);
                    Level.AddRange(lcid.llevel);
                    //Himgs.AddRange(lcid.lhimg);
                    Uids.AddRange(lcid.luid);
                }
            }
            //  string res1 = "";
            /*
            Authors.AddRange(HttpHelper.P_jq(res, "\",\"name\":\"","\"name_show"));

            Uids.AddRange(HttpHelper.P_jq(res, "\"author\":{\"id\":\"", "\", "));

            string[] hist = HttpHelper.P_jq(res, "\"portrait\":\"", "\", \"type");

            foreach (string item in hist)
            {
                Himgs.Add("https://gss0.bdstatic.com/6LZ1dD3d1sgCo2Kml5_Y_D3/sys/portrait/item/" + item);
            }
            Level.AddRange(HttpHelper.P_jq(res, "\"level_id\":","\""));
            //https://gss0.bdstatic.com/6LZ1dD3d1sgCo2Kml5_Y_D3/sys/portrait/item/
          */
            // GetHtml();

        }

    }
}
