using Linql.Core;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Linql.Client
{
    public class LinqlContext : ALinqlContext
    {
        protected  HttpClient HttpClient { get; set; }
        protected JsonSerializerOptions JsonOptions { get; set; }

        public virtual string BaseUrl
        {
            get
            {
                if(this.HttpClient != null)
                {
                    return this.HttpClient.BaseAddress.AbsoluteUri;
                }
                return null;
            }
             set
            {
                if(value == null)
                {
                    this.HttpClient = null;
                }
                else
                {
                    if(this.HttpClient == null)
                    {
                        this.HttpClient = new HttpClient();
                    }
                    this.HttpClient.BaseAddress = new Uri(value);
                }
            }
        }

        public LinqlContext(string BaseUrl = null, JsonSerializerOptions JsonOptions = null)
        {
            this.BaseUrl = BaseUrl;

            if (JsonOptions == null)
            {
                this.JsonOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true
                };
            }
            else
            {
                this.JsonOptions = JsonOptions;
            }
        }


        protected virtual async Task<TResult> GetResult<TResult>(Type Type, LinqlSearch Search)
        {
            if(this.HttpClient == null)
            {
                throw new System.Exception("No HttpClient was configured in this LinqlContext.  Please pass a BaseUrl string into the constructor or derive and override the HttpClient property.");
            }

            string url = this.GetEndpoint(Type, Search);
            return await this.SendHttpRequest< TResult>(url, Search);
            
        }

        protected virtual async Task<TResult> SendHttpRequest<TResult>(string Endpoint, LinqlSearch Search)
        {
            string search = JsonSerializer.Serialize(Search, this.JsonOptions);
            StringContent requestContent = new StringContent(search, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.HttpClient.PostAsync(Endpoint, requestContent);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream, this.JsonOptions);
            return result;
        }

        public override async Task<TResult> SendRequestAsync<TResult>(Type Type, LinqlSearch LinqlSearch)
        {
            return await this.GetResult<TResult>(Type, LinqlSearch);
        }

        //public override TResult SendRequest<TResult>(Type Type, LinqlSearch LinqlSearch)
        //{
        //    Task <TResult> task = this.GetResult<TResult>(Type, LinqlSearch);
        //    task.Wait();
        //    return task.Result;
        //}


        //public override async Task<TResult> SendRequestAsync<TResult>(IQueryable LinqlSearch)
        //{
        //    LinqlSearch search = LinqlSearch.ToLinqlSearch();
        //    Type type = LinqlSearch.GetType().GetEnumerableType();
        //    return await this.GetResult<TResult>(type, search);
        //}

        public override TResult SendRequest<TResult>(IQueryable LinqlSearch)
        {
            LinqlSearch search = LinqlSearch.ToLinqlSearch();
            Type type = LinqlSearch.GetType().GetEnumerableType();
            Task<TResult> task = this.GetResult<TResult>(type, search);
            task.Wait();
            return task.Result;
        }

        public override string ToJson(LinqlSearch Search)
        {
            return JsonSerializer.Serialize(Search, this.JsonOptions);
        }

        public override async Task<string> ToJsonAsync(LinqlSearch Search)
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, Search, typeof(LinqlSearch), this.JsonOptions);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
