using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using BL;
using DataModel;
using log4net;

namespace TGBookingAPI
{
    public class HeaderAuth
    {
        private static readonly ILog Log =
                       LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool Authorize(HttpRequestHeaders header, string webmode, IAPIConfigServices apiConfigServices)
        {
            bool isLogin = true;
            Log.Debug("Authorize");
            if (header.Contains("Authorization"))
            {
                Log.Debug("contain Authorize");
                string authorization = header.GetValues("Authorization").First();
                if (authorization.ToLower().IndexOf("bearer ") != 0)
                {
                    Log.Debug("not found bearer");
                    isLogin = false;
                }
                else
                {
                    Log.Debug("found bearer");
                    APIUser user = apiConfigServices.GetByToken(authorization.Substring(7));
                    if (user == null)
                    {
                        isLogin = false;
                    }
                    else
                    {
                        int mode = webmode == "DEMO" ? 0 : 1;
                        if (user.WebMode.GetValueOrDefault() != mode)
                        {
                            isLogin = false;
                        }
                    }
                }
            }
            else
            {
                Log.Debug("not contain Authorize");
                isLogin = false;
            }

            //temp
            if (isLogin == false)// && webmode == "DEMO")
            {
                isLogin = true;
                if (header.Contains("user"))
                {
                    string user = header.GetValues("user").First();
                    if (user != "demo")
                    {
                        isLogin = false;
                    }
                }
                else
                {
                    isLogin = false;
                }
                if (header.Contains("password"))
                {
                    string password = header.GetValues("password").First();
                    if (password != "1234")
                    {
                        isLogin = false;
                    }
                }
                else
                {
                    isLogin = false;
                }
            }

            return isLogin;
        }

        public static APIUser GetUser(HttpRequestHeaders header, string webmode, IAPIConfigServices apiConfigServices)
        {
            APIUser user = null;
            if (header.Contains("Authorization"))
            {
                string authorization = header.GetValues("Authorization").First();
                if (authorization.ToLower().IndexOf("bearer ") != 0)
                {
                    return null;
                }
                else
                {
                    user = apiConfigServices.GetByToken(authorization.Substring(7));
                    if (user == null)
                    {
                        return null;
                    }
                    else
                    {
                        int mode = webmode == "DEMO" ? 0 : 1;
                        if (user.WebMode.GetValueOrDefault() != mode)
                        {
                            return null;
                        }
                    }
                }
            }
            

            return user;
        }
    }
}