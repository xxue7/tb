﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net;
using BaiduHelper;

namespace Tieba
{
    public  delegate void TxtCallback(string str, Color color);
   
    public delegate void ListCallback(Log log);
    class Task
    {

        public static bool detilinfo=false;
      
        private Thread th;

       
        private TxtCallback txtCallback;

        private ListCallback listCallback;

        private ManualResetEvent manua;

        private Mode mode;   

        //fuza
        private object object1;

        private object object2;

        private object object3;

        //外部初始化

        

        //fuza
       private int itaskCount;

       private int iflagCount;

        //内部
       private List<string> listpid = new List<string>();

       private List<string> ltids = new List<string>();

      // private string tidComplex;

      // private string lasthtmlcahe;

       private volatile bool boolstop;

       private List<string> cachetid = new List<string>();

       private List<string> cachereplycount = new List<string>();

       private List<string> cachetiddefult = new List<string>();

       private List<string> cacheuid = new List<string>();

       public Task(ListCallback lc, TxtCallback tc,Mode mode, int threadCount=0)
       {
           ServicePointManager.DefaultConnectionLimit = 128;

           ServicePointManager.Expect100Continue = false;

           
           
           this.listCallback = lc;

           this.txtCallback = tc;

           this.manua  = new ManualResetEvent(true);

           this.mode = mode;

           if (threadCount!=0)
           {
               this.itaskCount = threadCount;
               object1 = new object();
               object2 = new object();
               object3 = new object();
               boolstop = false;
           }

        }

       public Task()
       {
          
       
       }
        
        //private void ThreadEasey()
        //{
            
        //        th = new Thread(new ThreadStart(delegate
        //        {
                   
        //            while (true)
        //            {
        //                EaseMode();
        //                txtCallback("等待" + mode.sctime + "s", Color.Red);
        //                Thread.Sleep(mode.sctime * 1000);


        //            }
        //        }));

        //        th.IsBackground = true;

        //        th.Start();
            
            
        //}

       
        private void ThreadFuZa()
        {
            th = new Thread(new ThreadStart(delegate
            {
                while (!boolstop)
                {
                    try
                    {
                        if (boolstop)
                        {
                            break;
                        }
                      
                        tidreplycount();
                        txtCallback("开始->" + "总任务数->" + ltids.Count, Color.Green);
                        if (ltids.Count!=0)
                        {
                            Thread[] ths = new Thread[this.itaskCount];
                            iflagCount = itaskCount;
                            for (int i = 0; i < this.itaskCount; i++)
                            {
                                ths[i] = new Thread(FuZaMode);

                                ths[i].Name = (i + 1).ToString();

                                ths[i].IsBackground = true;
                            }

                            for (int i = 0; i < this.itaskCount; i++)
                            {
                                ths[i].Start(i);
                            }

                            for (int i = 0; i < this.itaskCount; i++)
                            {
                                ths[i].Join();
                            }
                            //  MessageBox.Show("");
                            
                            
                        }
                        if (iflagCount == 0)
                        {
                            txtCallback("等待" + mode.sctime + "s", Color.Red);
                            Thread.Sleep(mode.sctime * 1000);


                        }
                    
                    }
                    catch (Exception cx)
                    {

                        txtCallback(cx.Message, Color.Red);
                    }
                   
                }
                

            }));

            th.IsBackground = true;

            th.Start();

            
            
        }

        public void startThread()
        {
            //if (mode.mode == "简单模式")
            //{
            //    ThreadEasey();
            //}
            //else if (mode.mode == "复杂模式")
            //{
                ThreadFuZa();
            //}
        
        }

        public void stopThread()
        {
          
            boolstop = true;
            th.Abort();
            while (iflagCount!=0)
            {
                Thread.Sleep(200);
             }

           
        }

        public void reSet()
        {
            manua.Reset();
        
        }
        public void set()
        {
            manua.Set();
        }
        /*
        private void EaseMode()
        {

            List<string> templisttid = new List<string>();
            try
            {
                TbInfo info = new TbInfo(mode.mangertb);

                // lasthtmlcahe = info.html;
                Log log = new Log();
                for (int i = 0, len = info.Authors.Count; i < len; i++)
                {
                    manua.WaitOne();
                    string tid = "", pikey;
                    bool blackFlag = false, valuesFlag = false;
                    try
                    {
                        if (listpid.Contains(info.Tids[i]))
                        {
                            txtCallback("跳过扫描:" + (i + 1).ToString() + "." + info.Titles[i] + "--->" + info.Authors[i], Color.Green);
                            continue;
                        }

                        if (whiteblackMethod(info.Authors[i].Replace("昵称:", ""), 'w', out pikey)) { txtCallback("跳过-->白名单-" + info.Authors[i], Color.Blue); templisttid.Add(info.Tids[i]); continue; }

                        txtCallback((i + 1).ToString() + "." + info.Titles[i] + "--->" + info.Authors[i], Color.Black);
                        //if (mode.isblack)
                        //{
                        blackFlag = whiteblackMethod(info.Authors[i].Replace("昵称:", ""), 'b', out pikey);

                        if (blackFlag)
                        {
                            tid = info.Tids[i];
                            log.type = "黑名单：" + info.Authors[i];
                        }

                        //}
                        if (!blackFlag)
                        {
                            if (whiteblackMethod(info.Titles[i].Trim(), 'c', out pikey) || whiteblackMethod(info.Replay[i], 'c', out pikey)) { txtCallback("跳过-->信任内容-" + pikey, Color.Blue); templisttid.Add(info.Tids[i]); continue; }

                            valuesFlag = whiteblackMethod(info.Titles[i].Trim(), 't', out pikey) || whiteblackMethod(info.Replay[i], 'r', out pikey);

                            if (valuesFlag)
                            {
                                tid = info.Tids[i];
                                log.type = "匹配到关键词：" + pikey;
                            }
                        }


                        if (tid != "")
                        {

                            txtCallback(log.type, Color.Red);
                            log.uid = info.Uids[i];
                            string ncikname = info.Authors[i].StartsWith("昵称:")?"": info.Authors[i];
                            //if (blackFlag)
                            //{

                            //    log.result = Common.Delete(tid) + "-->" + Common.Block(ncikname,info.Uids[i], 10, mode.reason);

                            //}
                            //else
                            //{
                                
                            //}
                            
                            if (ageNumMethod(info.Uids[i]))
                            {
                                templisttid.Add(info.Tids[i]);
                                txtCallback(log.type + "--跳过,吧龄或发贴数不符合", Color.Red);
                                continue;
                            }

                            bool zxbool = false;

                            if (pikey.StartsWith("[k:1]"))
                            {
                                templisttid.Add(info.Tids[i]);
                                log.result = "需要手动确认";
                                zxbool = true;
                            }
                            else if (pikey.StartsWith("[k:0]"))
                            {
                                zxbool = false;
                            }
                            //}

                            if (mode.isdel && !zxbool)
                            {

                                log.result = Common.Delete(tid);

                            }

                            if (mode.isblackname && !zxbool)
                            {

                                log.result = log.result + "-->" + Common.Black(info.Authors[i]);

                            }

                            if (mode.isblock && !zxbool)
                            {
                                log.result = log.result + "-->" + Common.Block(ncikname, info.Uids[i], mode.blockday, mode.reason);

                            }



                        }
                        else
                        {
                            templisttid.Add(info.Tids[i]);
                        }

                    }
                    catch (Exception er)
                    {

                        log.result = er.Message;
                    }
                    if (tid != "")
                    {
                        log.author = info.Authors[i];
                        log.title = info.Titles[i] + "-->" + info.Replay[i];
                        log.tid = tid;
                        log.tbname = Common.Kw;
                        log.fid = Common.Fid;
                        listCallback(log);
                        txtCallback(log.result, Color.Red);
                    }

                }



            }
            catch (Exception ee)
            {
                txtCallback("简单模式:" + ee.Message, Color.Red);

            }
            if (templisttid.Count != 0)
            {
                lock (object1)
                {
                    if (listpid.Count > 20000)
                    {
                        listpid.Clear();
                    }
                    listpid.AddRange(templisttid);
                }
            }

        }*/


        private void FuZaMode(object start)
        {
            int index = (int)start;

           
                for (int i = index; i < this.ltids.Count; i += itaskCount)
                {
                    try
                    {
                        if (boolstop)
                        {
                            break;
                        }
                         Complex(ltids[i]);
                    }
                    catch (Exception ee)
                    {
                        if (!ee.Message.StartsWith("-1"))
                        {
                            adddefulttid(ltids[i]);
                        }
                        
                        txtCallback(Thread.CurrentThread.Name + "--ThreadComplex("+ltids[i]+"):" + ee.StackTrace+"\r\n"+ee.Message, Color.Red);
                    }


                }

                Interlocked.Decrement(ref iflagCount);
            
        
        }


        private bool hashmethod(string content,out int dd,out string imgname)
        {
            dd = 100;

            imgname = "";
            string urlhash = "";
            Regex rg = new Regex(@"http://tiebapic\.baidu\.com/forum/.*?\.jpg");
            if (rg.IsMatch(content))
            {
                string urlimg = rg.Match(content).Value;

                imghash ihash=new imghash(urlimg,mode.isimgdct);
               
                urlhash = ihash.GetHash(); 
            }
            //if (mode.ishimg)
            //{
              // hhash= new imghash(himg, mode.isimgdct).GetHash();
            //}

            for (int h = 0; h < mode.localimghash.Length; h++)
            {
                string[] result = mode.localimghash[h].Split(':');

                if (urlhash!="")
                {
                    dd = imghash.CalcSimilarDegree(urlhash, result[1]);

                    if (dd <= mode.hashdistance)
                    {
                        imgname = result[0];
                        return true;

                    }
                }

               /* if (mode.ishimg)
                {
                    dd = imghash.CalcSimilarDegree(hhash, result[1]);

                    if (dd <= mode.hashdistance)
                    {
                        imgname = result[0];
                        return true;
                    
                    }
                }*/
            }






            return false;
        }

        private bool ageNumMethod(string unq)
        {
            ID id = null;
            if (mode.ispostnum)
            {
                id = new ID(unq,true);

                if (id.error != "")
                {
                    txtCallback("跳过-->id获取错误-" + id.error, Color.Black);
                    return true;
                }
                else
                {
                    double postnumq = 0;
                    //if (id.postNum.Contains("万"))
                    //{
                    //    id.postNum = id.postNum.Replace("万", "");

                    //    postnumq = double.Parse(id.postNum) * 10000;

                    //}
                    //else
                    //{
                        postnumq = double.Parse(id.postNum);
                  //  }

                    if (mode.postnum <= postnumq)
                    {
                        txtCallback("跳过-->大于设定发帖量-" + id.postNum, Color.Black);
                        return true;
                    }
                }
            }


            //if (mode.istbage)
            //{
            //    if (id == null)
            //    {

            //        id = new ID(unq,true);
            //    }


            //    if (id.error != "")
            //    {
            //        txtCallback("跳过-->id获取错误-" + id.error, Color.Black);
            //        return true;
            //    }
            //    else
            //    {
            //        if (mode.tbage <= double.Parse(id.age))
            //        {
            //            txtCallback("跳过-->大于设定吧龄-" + id.age, Color.Black);
            //            return true;
            //        }
            //    }
            //}

            return false;
        }

        private bool whiteblackMethod(ref List<ContentType> listtemp,string unq,char m,out string pikey,string uid="null")
        {
           // List<ContentType> listtemp = new List<ContentType>();
            pikey = "";
            //switch (m)
            //{
            //    case 'w': listtemp = mode.cttxtwhites; break;
            //    case 'b': listtemp = mode.cttxtblacks; ; break;
            //    case 'r': listtemp = mode.ctrkeys; break;
            //    case 't': listtemp = mode.cttkeys; break;
            //    case 'c': listtemp = mode.cttxtconwhites; break;
            //    default:
            //        throw new Exception("没有该项目");
            //}

            foreach (ContentType item in listtemp)
            {
                bool sucbool = false;
                if (item.iszz)
                {

                    //if ((tr == 't' && item.type == "标题关键词") || (tr == 'r' && item.type == "回复关键词") || tr == ' ')
                    //{
                        if (new Regex(item.content).IsMatch(unq))
                        {
                            sucbool = true;

                        }
                        else if (mode.isintro && uid != "null" && !cacheuid.Contains(uid))
                        {
                            if (new Regex(item.content).IsMatch(Common.uidtointro(uid)))
                            {
                                sucbool = true;
                            }
                            else
                            {
                                lock (object3)
                                {
                                    //if (!cacheuid.Contains(uid))
                                    //{
                                    if (cacheuid.Count > 20000)
                                    {
                                        cacheuid.Clear();
                                    }
                                    cacheuid.Add(uid);
                                    //}
                                }
                            }

                        }

                    //}
                    
                   
                }
                else
                {
                    if (m == 'r' || m == 't' || m == 'c')
                    {
                        //if ((tr == 't' && item.type == "标题关键词") || (tr == 'r' && item.type == "回复关键词")||tr==' ')
                        //{
                            if (unq.Contains(item.content))
                            {
                                sucbool = true;
                            }
                            else if (mode.isintro && uid != "null" && !cacheuid.Contains(uid))
                            {
                                if (Common.uidtointro(uid).Contains(item.content))
                                {
                                    sucbool = true;
                                }
                                else
                                {
                                    lock (object3)
                                    {
                                        //if (!cacheuid.Contains(uid))
                                        //{
                                        if (cacheuid.Count > 20000)
                                        {
                                            cacheuid.Clear();
                                        }
                                        cacheuid.Add(uid);
                                        //}
                                    }

                                }

                            }
                        
                        //}

                       
                    }
                    else
                    {
                        if (item.content == unq)
                        {
                            sucbool = true;

                        }
                    }
                    
                }
                if (sucbool)
                {
                    pikey = item.content;

                    if (m == 'r' || m == 't')
                    {
                        string shou = item.isshou == true ? "k:1" : "k:0";
                        pikey = "["+shou+"]" + pikey;
                    }

                   // txtCallback("跳过-->白名单-" + unq, Color.Black);
                    return true;
                }
                
            }
            return false;
        }

        private void adddefulttid(string tidComplex)
        {
            lock (object2)
            {
                if (!cachetiddefult.Contains(tidComplex))
                {
                    cachetiddefult.Add(tidComplex);
                }
            }
        }

        private  void tidreplycount()
        {
            ltids.Clear();

            if (cachetid.Count>800)
            {
                cachetid.Clear();

                cachereplycount.Clear();
            }
            #region MyRegion
            //string html = HttpHelper.HttpGet("http://tieba.baidu.com/f?ie=utf-8&kw=" + mode.mangertb, Encoding.UTF8, null, false, true);

            //string[] tds = HttpHelper.P_jq(html, "'{&quot;id&quot;:", ",", false);

            //string[] replaycounts = HttpHelper.P_jq(html, ",&quot;reply_num&quot;:", ",", false);

            //string replycountzhi = HttpHelper.Jq(html, "id=\"interviewReply\" title=\"", "个");

            //string tidzhi = HttpHelper.Jq(html, "tId         : '", "'");

            //if (string.IsNullOrEmpty(tidzhi) && html.IndexOf("id=\"pic_theme_list\"") > -1)
            //{
            //    tidzhi = HttpHelper.Jq(html, "id=\"pic_theme_list\"", "\">").Replace("tid=\"", "").Trim();
            //    try
            //    {
            //        replycountzhi = HttpHelper.Jq(HttpHelper.HttpPost("http://tieba.baidu.com/photo/bw/picture/toplist", "kw=" + Common.Kw + "&tid=" + tidzhi + "&ie=utf-8&pn=0&rn=10", null, null), "reply_amount\":", ",");
            //    }
            //    catch (Exception)
            //    {


            //    }
            //}

            #endregion
            // string html1 = HttpHelper.HttpGet("http://wapp.baidu.com/f?ie=utf-8&kw="+mode.mangertb , Encoding.UTF8,"USER_RS="+Common.user.uid+"_50_30_1; "+Common.user.cookie, false, true);
           string html1= Common.scantidcount(mode.mangertb);
            string[] tids = HttpHelper.P_jq(html1, "tieba.baidu.com/p/", "\"");
            string[] rtimes= HttpHelper.P_jq(html1, "last_time_int\":", ",");
            // MatchCollection mcs = new Regex(@"thread_id"":""([^""]+)"",""original_tid"":""0"".+?last_time_int"":""([^""]+)""").Matches(html1);
            //string[] tds =HttpHelper.P_jq( html,"m?kz=","&amp;");
            //new Regex(@"kz=(\d{1,10}).+?(\d{1,2}[:-]\d{1,2})</p>", RegexOptions.Singleline)
            //string[] replytimes = HttpHelper.P_jq(html, "&#160;", "</p>");
            if (tids.Length==rtimes.Length)
            {
                for (int i = 0; i < tids.Length; i++)
                {
                    string tidte = tids[i], replycountte =rtimes[i];
                    #region MyRegion
                    //if (i == tds.Length)
                    //{
                    //    if (!string.IsNullOrEmpty(tidzhi))
                    //    {
                    //        tidte = tidzhi;
                    //        replycountte = replycountzhi;
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }

                    //}
                    //else
                    //{
                    //    tidte = tds[i];
                    //    replycountte = replaycounts[i];
                    //}
                    //int index = cachetid.IndexOf(tidte);

                    //if ((index >= 0 && cachereplycount[index] == replycountte)&& !cachetiddefult.Contains(tidte))
                    //{
                    //    continue;

                    //}

                    #endregion

                    int index = cachetid.IndexOf(tidte);

                    if ((index >= 0 && cachereplycount[index] == replycountte) && !cachetiddefult.Contains(tidte))
                    {
                        continue;

                    }


                    ltids.Add(tidte);

                    if (cachetiddefult.Contains(tidte))
                    {
                        cachetiddefult.Remove(tidte);
                    }

                    if (index >= 0)
                    {
                        cachereplycount[index] = replycountte;
                    }
                    else
                    {
                        cachetid.Add(tidte);
                        cachereplycount.Add(replycountte);
                    }


                }
            }


            
        }


        private void Complex(string tidComplex)
        {
            //Title restit;
            ClientTit restit;
            int maxpn = mode.pn;
           // tidComplex = "5626300517";
            List<string> templistpid = new List<string>();
            for (int j = 1; j <=maxpn; j++)
            {
                restit = new ClientTit(tidComplex, j);
                //if (tidComplex == "4564696916")
                //{
                //    int s = 0;
                //}
                //if (tidComplex=="5044882021")
                //{
                //    int ss = 0;
                //}
                Log log = new Log();
                txtCallback(Thread.CurrentThread.Name + "--" + iflagCount + "-->正在扫描:" + restit.title + "-->tid:" + tidComplex + "?pn=" + j, Color.Black);
               //Thread.Sleep(10000);
                if (restit.maxPn == 0)
                {
                    throw new Exception("获取错误tid:" + tidComplex);
                }
                //if (restit.maxPn<=maxpn)
                //{
                //    maxpn = restit.maxPn;
                //}
                //else
                //{
                //    if (j==maxpn-1)
                //    {
                //        maxpn = restit.maxPn;
                //        j = maxpn - 1;
                //    }
                //}
                if (j==1)
                {
                    if (1+maxpn<=restit.maxPn)
                    {


                        j = restit.maxPn - mode.pn;
                    }
                    maxpn = restit.maxPn;
                }
                //进入帖子
              
                for (int i = 0; i < restit.Authors.Count; i++)
                {
                   
                    string pid = "";
                    string outpikey;
                    bool flagB = false, imgFlag=false;
                    //bool isbool = false;
                    try
                    {
                        manua.WaitOne();

                       
                        string tconten = restit.Content[i];
                      
                        if (listpid.Contains(restit.Pids[i]))
                        {
                            if (Task.detilinfo)
                            {
                                txtCallback(Thread.CurrentThread.Name + "--" + iflagCount + "-->跳过扫描pid:" + restit.Pids[i] + "-->" + "作者:" + restit.Authors[i] + "-->内容:" + (tconten.Length > 27 ? tconten.Substring(0, 27) + "......" : tconten), Color.Green);

                            }
                            continue;
                        }



                       if(Task.detilinfo) txtCallback(Thread.CurrentThread.Name + "--" + iflagCount + "-->正在扫描pid:" + restit.Pids[i] + "-->" + "作者:" + restit.Authors[i] + "-->内容:" + (tconten.Length > 27 ? tconten.Substring(0, 27) + "......" : tconten), Color.Black);


                        if (whiteblackMethod(ref mode.ctwhitenames,restit.Authors[i].Replace("昵称:", ""), 'w' ,out outpikey))
                        {
                            if (Task.detilinfo) txtCallback("跳过-->白名单-" + restit.Authors[i], Color.Blue); templistpid.Add(restit.Pids[i]);
                            continue;
                        }
                              
            

                        //if (mode.isblack)
                        //{
                        flagB = whiteblackMethod(ref mode.ctblacknames,restit.Authors[i].Replace("昵称:",""), 'b',out outpikey);

                            if (flagB)
                            {
                                pid = restit.Pids[i];
                                log.type = "匹配到复杂模式黑名单:" + restit.Authors[i];
                            }

                            
                        //}
                       // if (tconten == "") continue;
                        if (!flagB)
                        {
                            bool ctconwitebool = false;
                            if (i==0)
                            {
                                ctconwitebool = whiteblackMethod(ref mode.ctwhitecontents,restit.title.Trim(), 'c' ,out outpikey) || whiteblackMethod(ref mode.ctwhitecontents,tconten, 'c',out outpikey);
                               
                            }
                            else
                            {
                                ctconwitebool = whiteblackMethod(ref mode.ctwhitecontents,tconten, 'c',out outpikey);
                            }

                            if (ctconwitebool) { if (Task.detilinfo) txtCallback("跳过-->信任内容-" + outpikey, Color.Blue); templistpid.Add(restit.Pids[i]); continue; }

                           

                            if (mode.istime && Common.ConTime(restit.Times[i], mode.dt) < 0) 
                            {
                                templistpid.Add(restit.Pids[i]);
                                if (Task.detilinfo) txtCallback("跳过-->该回复时间为" + Common.UnixTimeToStr(long.Parse(restit.Times[i])) + ",小于设定时间" + mode.dt.ToString("yyyy-MM-dd HH:mm:ss"),Color.Blue);
                                continue;
                            }


                            string flev = restit.Level[i] == "" ? "0" : restit.Level[i];

                            if (mode.islevel && int.Parse(flev) > mode.level)
                            {
                                templistpid.Add(restit.Pids[i]);
                                if (Task.detilinfo) txtCallback("跳过-->该用户等级为" + restit.Level[i] + ",大于设定等级" + mode.level, Color.Blue);
                                continue;
                            }

                            if (mode.isimghash)
                            {
                                int dd;
                                string imgname;
                                //string himgtt = restit.Himgs[i];
                                if (hashmethod( mode.isimghash? tconten:"", out dd, out imgname))
                                {
                                    imgFlag = true;
                                    pid = restit.Pids[i];

                                    log.type = "匹配到图片：" + imgname + "/" + dd;

                                }
                            }
                            if (!imgFlag)
                            {
                                bool valuesFlag = false;
                                if (i == 0)
                                {
                                    valuesFlag = whiteblackMethod(ref mode.cttitlekeys,restit.title.Trim(), 't' ,out outpikey, restit.Uids[i]) || whiteblackMethod(ref mode.ctreplaykeys,tconten, 'r', out outpikey);
                                }
                                else
                                {
                                    valuesFlag = whiteblackMethod(ref mode.ctreplaykeys,tconten, 'r' ,out outpikey, restit.Uids[i]);
                                }

                                if (valuesFlag)
                                {
                                    log.type = "匹配到复杂模式:" + outpikey;
                                    pid = restit.Pids[i];

                                }
                                
                            }
                            
                           
                           
                        }

                       

                        if (pid != "")
                        {
                            txtCallback(log.type, Color.Red);
                            log.author = restit.Authors[i];
                            log.fid = Common.Fid;
                            log.tbname = Common.Kw;
                            log.title = restit.title + "--->" + tconten;
                            log.tid = tidComplex + "&" + pid;
                            log.uid = restit.Uids[i];
                            log.por = restit.Por[i];
                            string nickname = restit.Authors[i].StartsWith("昵称:") ? "" : restit.Authors[i];
                            /*if (flagB)
                            {

                                log.result = Common.Del(tidComplex, pid) +"-->" + Common.Block(nickname, restit.Uids[i],10, mode.reason);

                            }
                            else
                            {
                               
                            }*/
                            // flagB = true;
                            if (!flagB&&ageNumMethod(restit.Uids[i]))
                            {
                                templistpid.Add(restit.Pids[i]);
                                txtCallback(log.type+"--跳过,吧龄或发贴数不符合", Color.Red);
                                continue;

                            }
                            log.result = "未执行删除封禁";
                            bool zxbool = false;
                            if (log.type.Contains("复杂模式"))
                            {
                                if (outpikey.StartsWith("[k:1]"))
                                {
                                    templistpid.Add(restit.Pids[i]);
                                    log.result = "需要手动确认";
                                    zxbool = true;
                                }
                                else if (outpikey.StartsWith("[k:0]"))
                                {
                                    zxbool = false;
                                }
                            }
                            if (mode.isblock && !zxbool)
                            {
                                log.result = Common.Block(nickname, restit.Por[i], mode.blockday, mode.reason);
                            }
                            if (mode.isblackname && !zxbool)
                            {

                                log.result = log.result + "-->" + Common.Black(restit.Authors[i]);

                            }
                            if (mode.isdel && !zxbool)
                            {
                                log.result = log.result.Replace("未执行删除封禁", "");
                                if ((mode.islz && restit.Authors[i] == restit.Authors[0]) || i == 0)
                                {

                                    log.result += Common.Delete(tidComplex);

                                }
                                else
                                {
                                    log.result += Common.Del(tidComplex, pid);
                                }

                            }
                            listCallback(log);
                            txtCallback(log.result, Color.Red);


                            if ((i == 0 || (restit.Authors[i] == restit.Authors[0]&&mode.islz)) && log.result.Contains("删除成功")) break;//当删除内容为一楼时，直接跳出


                        }
                        else
                        {
                            templistpid.Add(restit.Pids[i]);
                        }


                    }
                    catch (Exception ee)
                    {
                        adddefulttid(tidComplex);
                        if (pid != "") listCallback(log);
                        txtCallback("Complex()" + ee.Message, Color.Red);

                    }



                }
            }

            if (templistpid.Count!=0)
            {
                lock (object1)
                {
                    if (listpid.Count>20000)
                    {
                        listpid.Clear();
                    }
                    listpid.AddRange(templistpid);
                }
            }

            
            ////////////////////////////////////////
            //if (restit.maxPn>1&&pn!=restit.maxPn)
            //{
            //    Complex(restit.maxPn);
            //}
        }
    
    }
}
