﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using QQChat;

namespace QQChat
{
    public class QQHelper
    {
        public static long GetTime()
        {
            DateTime startDate = new DateTime(1970, 1, 1);
            DateTime endDate = DateTime.Now.ToUniversalTime();
            TimeSpan span = endDate - startDate;
            return (long)(span.TotalMilliseconds + 0.5);
        }

        public static DateTime ToTime(long time)
        {
            DateTime dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            dtZone = dtZone.AddSeconds(time);
            return dtZone.ToLocalTime();
        }

        public static string GetPassword(string uin, string password, string verficode)
        {
            var saltuin = uin2hex(uin);
            string rpass = HEXMD5.hexchar2bin(HEXMD5.Md5(password));
            rpass = HEXMD5.Md5(rpass + saltuin);
            rpass = HEXMD5.Md5(rpass + verficode.ToUpper());
            return rpass;
        }

        public static string uin2hex(string uin)
        {
            var str = Int64.Parse(uin);
            var str2 = new char[8];
            for (int i = 0; i < 8; i++)
            {
                str2[i] = (char)((str >> ((7 - i) * 8)) & 0xff);
            }
            var result = new string(str2);
            return result;
        }

        public static string GetPassword(string password, string verifycode)
        {
            return Md5(Md5_3(password) + verifycode.ToUpper());
        }

        public static string Md5_3(string input)
        {
            MD5 md = MD5.Create();
            byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(input));
            buffer = md.ComputeHash(buffer);
            buffer = md.ComputeHash(buffer);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static string Md5(string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public static TEntry FromJson<TEntry>(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<TEntry>(input);
        }

        public static object FromJson(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize(input, typeof(object));
        }

        public static string ToJson(object input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(input);
        }

        public static string ToPostData(object input)
        {
            string str = ToJson(input);
            return "r=" + HttpUtility.UrlEncode(str)
                        .Replace("%7b", "%7B")
                        .Replace("%7d", "%7D")
                        .Replace("%2c", "%2C")
                        .Replace("%3a", "%3A");
        }

        public static string GetToken(QQUser user)
        {
            //http://web.qstatic.com/webqqpic/pubapps/0/50/eqq.all.js
            //P = function(b, j) {
            return GetToken3(user.QQNum, user.PtWebQQ);
        }

        public struct u
        {
            public int s;
            public int e;
            public u(int w, int G)
            {
                this.s = w;
                this.e = G;
            }
        }

        public static string GetToken4(string b, string j)
        {
            var a = j + "password error";
            var i = "";
            var E = new EIArray<int>();
            for (;;)
            {
                if (i.Length <= a.Length)
                {
                    i += b;
                    if (i.Length == a.Length)
                    {
                        break;
                    }
                }
                else
                {
                    i = i.Substring(0, a.Length);
                    break;
                }
            }
            var c = 0;
            for (; c < i.Length; c++)
            {
                E[c] = i[c] ^ a[c];
            }
            a = "0123456789ABCDEF";
            i = "";
            c = 0;
            for (; c < E.Length; c++)
            {
                i += a[E[c] >> 4 & 15];
                i += a[E[c] & 15];
            }
            return i;
        }


        public static string GetToken3(string b, string i)
        {
            int bi = (int)Convert.ToInt64(b);
            var a0 = new int[4];
            for (var s = 0; s < i.Length; s++) a0[s % 4] ^= i[s];
            var j0 = new string[] { "EC", "OK" };
            var d0 = new int[4];
            d0[0] = bi >> 24 & 255 ^ j0[0][0];
            d0[1] = bi >> 16 & 255 ^ j0[0][1];
            d0[2] = bi >> 8 & 255 ^ j0[1][0];
            d0[3] = bi & 255 ^ j0[1][1];
            var j = new int[8];
            for (var s = 0; s < 8; s++) j[s] = s % 2 == 0 ? a0[s >> 1] : d0[s >> 1];
            var a = "0123456789ABCDEF";
            var d = "";
            for (var s = 0; s < j.Length; s++)
            {
                d += a[j[s] >> 4 & 15];
                d += a[j[s] & 15];
            }
            return d;
        }

        public static string GetToken2(string w, string G)
        {
            int wi = (int)Convert.ToInt64(w);
            int[] I = new int[4];
            I[0] = wi >> 24 & 255;
            I[1] = wi >> 16 & 255;
            I[2] = wi >> 8 & 255;
            I[3] = wi & 255;
            var T = new List<int>();
            for (int i = 0; i < G.Length; ++i)
            {
                T.Add(G[i]);
            }
            var V = new Stack<u>();
            V.Push(new u(0, T.Count - 1));
            for (; V.Count > 0;)
            {
                var P = V.Pop();
                if (!(P.s >= P.e || P.s < 0 || P.e >= T.Count)) if (P.s + 1 == P.e)
                    {
                        if (T[P.s] > T[P.e])
                        {
                            var Z = T[P.s];
                            T[P.s] = T[P.e];
                            T[P.e] = Z;
                        }
                    }
                    else
                    {
                        int Z = P.s;
                        int U = P.e;
                        int X = T[P.s];
                        for (; P.s < P.e;)
                        {
                            for (; P.s < P.e && T[P.e] >= X;)
                            {
                                P.e--;
                                I[0] = I[0] + 3 & 255;
                            }
                            if (P.s < P.e)
                            {
                                T[P.s] = T[P.e];
                                P.s++;
                                I[1] = I[1] * 13 + 43 & 255;
                            }
                            for (; P.s < P.e && T[P.s] <= X;)
                            {
                                P.s++;
                                I[2] = I[2] - 3 & 255;
                            }
                            if (P.s < P.e)
                            {
                                T[P.e] = T[P.s];
                                P.e--;
                                I[3] = (I[0] ^ I[1] ^ I[2] ^ I[3] + 1) & 255;
                            }
                        }
                        T[P.s] = X;
                        V.Push(new u(Z, P.s - 1));
                        V.Push(new u(P.s + 1, U));
                    }
            }
            string TE = "0123456789ABCDEF";
            string VE = "";
            for (int i = 0; i < I.Length; i++)
            {
                VE += TE[I[i] >> 4 & 15];
                VE += TE[I[i] & 15];
            }
            return VE;
        }



        public static string GetToken(string b, string i)
        {
            var a = i + "password error";
            var s = "";
            var j = "";
            for (;;)
            {
                if (s.Length <= a.Length)
                {
                    s += b;
                    if (s.Length == a.Length)
                        break;
                }
                else
                {
                    s = s.Substring(0, a.Length);
                    break;
                }
            }
            for (var d = 0; d < s.Length; d++)
                j += (char)(s[d] ^ a[d]);
            var a2 = "0123456789ABCDEF";
            s = "";
            for (var d = 0; d < j.Length; d++)
            {
                s += a2[j[d] >> 4 & 15];
                s += a2[j[d] & 15];
            }
            return s;
        }

        public static string getGTK(string str)
        {
            UInt32 hash = 5381;
            for (int i = 0, len = str.Length; i < len; ++i)
            {
                hash += (hash << 5) + str[i];
            }
            return (hash & 0x7fffffff).ToString();
        }

        public static string getPtQrToken(string qrsig)
        {
            var e = 0;
            var n = qrsig.Length;
            for (var i = 0; n > i; ++i)
                e += (e << 5) + qrsig[i];
            return (2147483647 & e).ToString();
        }
    }
}