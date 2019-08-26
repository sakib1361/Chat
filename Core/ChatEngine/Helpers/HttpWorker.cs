using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using ChatClient.Services;

namespace ChatClient.Helpers
{
    public class HttpWorker
    {
        static readonly HttpClient HttpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(120),
        };

        internal static async Task<T> RunWorker<T>(HttpRequest httpRequest)
        {
            httpRequest.AddParameter("app_id", AppConstants.AppName);
            if (!string.IsNullOrWhiteSpace(AppService.Token))
                httpRequest.AddParameter("access_token", AppService.Token);
            var httpPost = httpRequest.GenrateRequest();
           
            var worker = WorkerService.Instance;
            worker.ErrorCode = 0;
            var s = SettingService.Instance;
            var urlString = string.Format("https://{0}:{1}/api/{2}",
                                   s.ServerName,
                                   s.Port,
                                   httpRequest.UrlPath);
            Console.WriteLine(urlString);
            
            try
            {
                using (var strResponse = await HttpClient.PostAsync(urlString, httpPost))
                {
                    var strContent = await strResponse.Content.ReadAsStringAsync();

                    httpPost.Dispose();

                    if (strResponse.IsSuccessStatusCode)
                    {
                        try
                        {
                            Debug.WriteLine(strContent);
                            if(typeof(T) == typeof(string))
                            {
                                return (T)(object)strContent;
                            }
                            else if(typeof(T)== typeof(bool))
                            {
                                return (T)(object)true;
                            }
                            else
                            {
                                var data = JsonConvert.DeserializeObject<T>(strContent);
                                return data;
                            }
                        }
                        catch
                        {
                            worker.ErrorCode = 1;
                            worker.ErrorMessage = strContent.Trim();
                        }
                    }
                    else
                    {

                        worker.ErrorCode = 1;
                        worker.ErrorMessage = string.IsNullOrWhiteSpace(strContent) ? strResponse.ReasonPhrase : strContent;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                worker.ErrorCode = 1;
                worker.ErrorMessage = ex.Message;
            }

            return default;
        }
    }
}
