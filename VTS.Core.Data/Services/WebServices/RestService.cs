using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VTS.Core.Data.Hundlers;

namespace VTS.Core.Data.WebServices
{
    public class RestService
    {
        private string _url;
        private int _timeout = 5000;      

        public RestService(string server, string model)
        {          
            _url = server + @"/api/" + model;

            Task task = Task.Run(async () => await TestConnect(server));
            task.Wait();
        }

        public int Timeout { get { return _timeout;  }  set { _timeout = value; } }

        private async Task TestConnect(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(_timeout);
                    var response = await httpClient.GetAsync(url);
                }
            }
            catch (TaskCanceledException ex)
            {
                var err = ex.Message;
                throw new ExceptionHandler();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public async Task<T> Get<T>(string id) where T : class
        {
            string url = _url;
            url += "/get";
            url += "?id=" + id;

            try
            {
                using (var httpClient = new HttpClient())
                {                   
                    var response = await httpClient.GetAsync(url);
                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<HttpResponseMessage> Get(int id)
        {
            string url = _url;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url + "?id=" + id);
                    return await httpClient.SendAsync(request);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }

        public async Task<List<T>> Get<T>() where T : class
        {
            string url = _url;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    var content = response.Content.ReadAsStringAsync().Result;
                    if (content == null)
                    {
                        return null;
                    }
                    return JsonConvert.DeserializeObject<List<T>>(content);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<T> Get<T>(int id) where T : class
        {
            string url = _url + "?id=" + id;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    var content = response.Content.ReadAsStringAsync().Result;

                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<HttpResponseMessage> Post<T>(T data)
        {
            string url = _url;           
            try
            {
                using (var httpClient = new HttpClient())
                {                 
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    return await httpClient.PostAsync(url, content);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            string url = _url;       
            url += "?id=" + id;
            try
            {
                using (var httpClient = new HttpClient())
                {                 
                    return await httpClient.DeleteAsync(url);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }

        public async Task<T> GetDataFromJsonFile<T>() where T : class
        {
            string url = _url;
            try
            {
                using (var httpClient = new HttpClient())
                {                   
                    var response = await httpClient.GetStringAsync(url);
                    return JsonConvert.DeserializeObject<T>(response);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return null;
            }
        }

        public async Task<HttpResponseMessage> Post(string name, string password)
        {
            string url = _url;

            try
            {
                using (var httpClient = new HttpClient())
                {                 
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url + "?name=" + name + "&password=" + password);
                    return await httpClient.SendAsync(request);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }

        public async Task<HttpResponseMessage> Post(int id)
        {
            string url = _url;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url + "?id=" + id);
                    return await httpClient.SendAsync(request);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }

        public async Task<HttpResponseMessage> Post()
        {
            string url = _url;

            try
            {
                using (var httpClient = new HttpClient())
                {                  
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                    return await httpClient.SendAsync(request);
                }
            }
            catch (TaskCanceledException)
            {
                throw new ExceptionHandler();
            }
        }
    }
}

