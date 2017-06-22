﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using WebQQ2.Extends;

namespace WebQQ2.WebQQ2
{
    public abstract class QQ_Base
    {
        protected Random _random;
        protected CookieContainer _cookiecontainer;
        protected HttpHelper _helper;
        protected QQUser _user;
        protected string _baseRefer = "http://qun.qq.com/member.html";
        public QQUser User { get { return _user; } }


        //private static readonly string qq_zone_friend = "http://r.qzone.qq.com/cgi-bin/tfriend/friend_ship_manager.cgi?uin={0}&do=1&rd={2}&fupdate=1&clean=1&g_tk={1}";
        //private static readonly string qq_qun_group = "http://qun.qzone.qq.com/cgi-bin/get_group_list?groupcount=4&count=4&uin={0}&g_tk={1}&r={2}";
        //private static readonly string qq_qun_member = "http://qun.qzone.qq.com/cgi-bin/get_group_member?uin={0}&groupid={1}&neednum=1&g_tk={2}&r={3}";

        private static readonly string qq_qun_myinfo = "http://qun.qq.com/cgi-bin/qunwelcome/myinfo?callback=?&bkn={0}";
        private static readonly string qq_qun_friend = "http://qun.qq.com/cgi-bin/qun_mgr/get_friend_list";
        private static readonly string qq_qun_group = "http://qun.qq.com/cgi-bin/qun_mgr/get_group_list";
        private static readonly string qq_qun_member = "http://qun.qq.com/cgi-bin/qun_mgr/search_group_members";



        public static readonly string qq_qun_sign = "http://qiandao.qun.qq.com/cgi-bin/sign";

        protected class qq_qun_friend_post
        {
            public string bkn { get; set; }
        };

        protected class qq_qun_group_post
        {
            public string bkn { get; set; }
        };

        protected class qq_qun_member_post
        {
            public long gc { get; set; }
            public int st { get; set; }
            public int end { get; set; }
            public int sort { get; set; }
            public string bkn { get; set; }
        };

        protected virtual void OnInit()
        {
            _random = new Random();
            _cookiecontainer = new CookieContainer();
            _helper = new HttpHelper(_cookiecontainer);
        }

        public QQ_Base()
        {
            this._user = new QQUser();
            this.OnInit();
        }
        public bool IsPreLoged
        {
            get
            {
                return this._user != null && this._user.IsPreLoged;
            }
        }
        public bool IsLoged
        {
            get
            {
                return this.IsPreLoged && this._user != null && this._user.IsLoged;
            }
        }



        #region QuickOperation

        //public Dictionary<string, object> GetFriendInfoFromZone()
        //{
        //    var furl = string.Format(qq_zone_friend, _user.QQNum, _user.GTK, _random.NextDouble());
        //    var fresult = _helper.GetUrlText(furl, _baseRefer);
        //    int fstart = fresult.IndexOf('(');
        //    string fsub = fresult.Substring(fstart + 1, fresult.LastIndexOf(')') - fstart - 1);
        //    return QQHelper.FromJson<Dictionary<string, object>>(fsub);
        //}
        //public Dictionary<string, object> GetGroupInfoFromQun()
        //{
        //    var furl = string.Format(qq_qun_group, _user.QQNum, _user.GTK, _random.NextDouble());
        //    var fresult = _helper.GetUrlText(furl, _baseRefer);
        //    int fstart = fresult.IndexOf('(');
        //    string fsub = fresult.Substring(fstart + 1, fresult.LastIndexOf(')') - fstart - 1);
        //    return QQHelper.FromJson<Dictionary<string, object>>(fsub);
        //}
        //public Dictionary<string, object> GetMemberInfoFromQun(string gid)
        //{
        //    var furl = string.Format(qq_qun_member, _user.QQNum, gid, _user.GTK, _random.NextDouble());
        //    var fresult = _helper.GetUrlText(furl, _baseRefer);
        //    int fstart = fresult.IndexOf('(');
        //    string fsub = fresult.Substring(fstart + 1, fresult.LastIndexOf(')') - fstart - 1);
        //    return QQHelper.FromJson<Dictionary<string, object>>(fsub);
        //}        

        public void VisitUrl(string url,string refer = null, int timeout = 60000)
        {
            _helper.GetNoRedirectResponse(url, refer??_baseRefer, timeout);
        }

        public Dictionary<string, object> GetMyInfo()
        {
            var furl = string.Format(qq_qun_myinfo, _user.GTK);
            string fresult = _helper.GetUrlText(furl, null,_baseRefer);
            var dict = QQHelper.FromJson<Dictionary<string, object>>(fresult);
            if((int)dict["retcode"] == 0)
            {
                var data = dict["data"] as Dictionary<string, object>;
                if(data != null)
                {
                    _user.Uin = data["uin"].ToString();
                    _user.QQNum = data["uin"].ToString();
                    _user.QQName = (string)data["nickName"];
                }
            }
            return dict;
        }

        public Dictionary<string, object> GetFriendInfoFromQun()
        {
            var furl = qq_qun_friend;
            string para = "bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            return QQHelper.FromJson<Dictionary<string, object>>(fresult);
        }
        public Dictionary<string, object> GetGroupInfoFromQun()
        {
            var furl = qq_qun_group;
            string para = "bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            return QQHelper.FromJson<Dictionary<string, object>>(fresult);
        }
        public Dictionary<string, object> GetMemberInfoFromQun(long gcode, int st, int end)
        {
            var furl = qq_qun_member;
            string para = "gc=" + gcode + "&st=" + st + "&end=" + end + "&sort=0&bkn=" + _user.GTK;
            string fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            var dict = QQHelper.FromJson<Dictionary<string, object>>(fresult);
            return dict;
        }
        #endregion


        public void UpdateCookie(Uri uri, string cookie)
        {
            _cookiecontainer.SetCookies(uri, cookie);
        }

        public Dictionary<string, object> QunSign(long groupCode, bool doSign = false)
        {
            var furl = string.Format(qq_qun_sign, _user.QQNum, _user.GTK, _random.NextDouble());
            string para = string.Format("&gc={0}&is_sign={1}&bkn={2}", groupCode, doSign ? 0 : 1, _user.GTK);
            var fresult = _helper.GetUrlText(furl, Encoding.UTF8.GetBytes(para), _baseRefer);
            return QQHelper.FromJson<Dictionary<string, object>>(fresult);
        }


        public void AnylizeCookie(string cookie)
        {
            var cks = cookie.Split(new[] { ';' }, StringSplitOptions.None);
            foreach (var ck in cks)
            {
                var kv = ck.Trim().Split(new[] { '=' }, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    if (kv[0] == "skey")
                    {
                        _user.skey = kv[1];
                        _user.GTK = QQHelper.getGTK(_user.skey);
                    }
                    else if (kv[0] == "uin")
                    {
                        var uin = kv[1].TrimStart(new[] { 'o', '0' });
                        _user.QQNum = uin;
                        _user.Uin = uin;
                    }
                    else if (kv[0] == "ptnick_" + _user.QQNum)
                    {
                        var utf8name = kv[1];
                        if (!string.IsNullOrWhiteSpace(utf8name))
                        {
                            if (utf8name.Length % 2 == 0)
                            {
                                var bytes = new byte[utf8name.Length / 2];
                                for (int i = 0; i < utf8name.Length; i += 2)
                                {
                                    bytes[i / 2] = byte.Parse(utf8name.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                                }
                                _user.QQName = Encoding.UTF8.GetString(bytes);
                            }
                        }
                    }
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "qq.com"));
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "qun.qq.com"));
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "ptlogin2.qq.com"));
                }
            }
        }
        public void AnylizeCookie()
        {
            var cookie = GetCookieString("http://www.qq.com/");
            var cks = cookie.Split(new[] { ';' }, StringSplitOptions.None);
            foreach (var ck in cks)
            {
                var kv = ck.Trim().Split(new[] { '=' }, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    if (kv[0] == "skey")
                    {
                        _user.skey = kv[1];
                        _user.GTK = QQHelper.getGTK(_user.skey);
                    }
                    else if (kv[0] == "ptui_loginuin")
                    {
                        _user.QQNum = kv[1];
                    }
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "qq.com"));
                }
            }
            cookie = GetCookieString("http://ptlogin2.qq.com/");
            cks = cookie.Split(new[] { ';' }, StringSplitOptions.None);
            foreach (var ck in cks)
            {
                var kv = ck.Trim().Split(new[] { '=' }, StringSplitOptions.None);
                if (kv.Length == 2)
                {
                    if (kv[0] == "skey")
                    {
                        _user.skey = kv[1];
                        _user.GTK = QQHelper.getGTK(_user.skey);
                    }
                    else if (kv[0] == "ptui_loginuin")
                    {
                        _user.QQNum = kv[1];
                    }
                    _cookiecontainer.Add(new Cookie(kv[0], kv[1], "/", "ptlogin2.qq.com"));
                }
            }
        }

        #region DLL Imports
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("wininet.dll", EntryPoint = "InternetGetCookieExW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern bool InternetGetCookieEx([In] string Url, [In] string cookieName, [Out] StringBuilder cookieData, [In, Out] ref uint pchCookieData, uint flags, IntPtr reserved);
        #endregion
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref int pcchCookieData, int dwFlags, object lpReserved);
        private static string GetCookieString(string url)
        {
            // Determine the size of the cookie      
            uint datasize = 256;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, (uint)0x00002000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie      
                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, (uint)0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }
    }
}
