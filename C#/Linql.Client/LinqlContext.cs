using Linql.Client.Internal;
using Linql.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.IO;

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
                };
            }
            else
            {
                this.JsonOptions = JsonOptions;
            }
        }


        protected virtual async Task<TResult> GetResult<TResult>(IQueryable Query, LinqlSearch Search)
        {
            if(this.HttpClient == null)
            {
                throw new Exception("No HttpClient was configured in this LinqlContext.  Please pass a BaseUrl string into the constructor or derive and override the HttpClient property.");
            }

            Type enumerableType = Query.GetType().GetEnumerableType();
            string url = this.GetEndpoint(enumerableType);
            return await this.SendLinqlRequest< TResult>(url, Search);
            
        }

        protected virtual async Task<TResult> SendLinqlRequest<TResult>(string Endpoint, LinqlSearch Search)
        {
            string search = JsonSerializer.Serialize(Search);
            StringContent requestContent = new StringContent(search, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.HttpClient.PostAsync(Endpoint, requestContent);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream, this.JsonOptions);
            return result;
        }

        public override async Task<TResult> SendRequestAsync<TResult>(IQueryable LinqlSearch)
        {
            LinqlSearch search = LinqlSearch.ToLinqlSearch();
            return await this.GetResult<TResult>(LinqlSearch, search);
        }

        public override TResult SendRequest<TResult>(IQueryable LinqlSearch)
        {
            LinqlSearch search = LinqlSearch.ToLinqlSearch();
            Task<TResult> task = this.GetResult<TResult>(LinqlSearch, search);
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
