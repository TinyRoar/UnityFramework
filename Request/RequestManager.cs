using System;
using System.Net;
using System.IO;
using System.Threading;
using UnityEngine;

/*
 * (c) 2015 - 2016 by Tiny Roar | Dario D. Müller
 * Don't visit us. But if you want to do: www.tinyroar-games.com
 * 
 * Usage: DoRequest(@"http://yourdoma.in/xyz", Action Callback(string content));
 * public void Callback(string content) { Debug.Log(content); }
 *
 * Known Issues:
 * - not yet ready for HTTPS url's
 *
 */

namespace TinyRoar.Framework
{
    public class RequestManager : Singleton<RequestManager>
    {

        // With Error-Results
        public void DoRequest(string url, Action<WebExceptionStatus, string> action = null)
        {
            Thread thread = new Thread(delegate () { WorkThreadFunction(url, action); });
            thread.Start();
        }

        public void WorkThreadFunction(string url, Action<WebExceptionStatus, string> action = null)
        {
            try
            {
                HttpWebRequest webRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                webRequest.Method = WebRequestMethods.Http.Get;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    // check response statusCode = 200
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var content = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(content))
                            {
                                string strContent = reader.ReadToEnd();
                                if (action != null)
                                    action(WebExceptionStatus.Success, strContent);
                            }
                        }
                    }
                    else
                    {
                        // Exception -> Wrong StatusCode for example 404
                        if (action != null)
                            action(WebExceptionStatus.ProtocolError, response.StatusCode.ToString());
                    }
                }
            }
            catch (WebException ex)
            {
                // Exception -> Web Request failed because [WebExceptionStatus]
                if (action != null)
                    action(ex.Status, ex.Message);
            }
            catch (Exception ex)
            {
                // Exception -> Cant talk to server, maybe you dont have network access
                if (action != null)
                    action(WebExceptionStatus.UnknownError, ex.Message);
            }

        }

    }
}