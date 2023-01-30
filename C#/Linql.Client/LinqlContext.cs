using Linql.Client.Internal;
using Linql.Core;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Client
{
    public class LinqlContext
    {
        protected HttpClient HttpClient { get; set; }

        private LinqlProvider mProvider { get; set; }

        public LinqlProvider Provider
        {
            get
            {
                return this.mProvider;
            }
            set
            {
                this.mProvider = value;
                this.mProvider.Context = this;
            }
        }

        public string BaseUrl
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

        public LinqlContext(string BaseUrl = null, LinqlProvider Provider = null)
        {
            this.BaseUrl = BaseUrl;

            if(Provider != null)
            {
                this.Provider = Provider;
            }
            else
            {
                this.Provider = new LinqlProvider();
            }
        }

        public virtual async Task<TResult> GetResult<TResult>(IQueryable Query, LinqlSearch Search)
        {
            if(this.HttpClient == null)
            {
                throw new Exception("No HttpClient was configured in this LinqlContext.  Please pass a BaseUrl string into the constructor or derive and override the HttpClient property.");
            }

            Type enumerableType = Query.GetType().GetEnumerableType();
            string url = this.GetEndpoint(enumerableType);
            return await this.MakeLinqlRequest< TResult>(url, Search);
            
        }

        protected virtual async Task<TResult> MakeLinqlRequest<TResult>(string Endpoint, LinqlSearch Search)
        {
            string search = JsonSerializer.Serialize(Search);
            StringContent requestContent = new StringContent(search, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.HttpClient.PostAsync(Endpoint, requestContent);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream, this.Provider.JsonOptions);
            return result;
        }

        protected virtual string GetEndpoint(Type QueryableType)
        {
            return $"linql/{QueryableType.Name}";
        }
       
        public virtual LinqlSearch<T> Set<T>()
        {
            return new LinqlSearch<T>(this.Provider);
        }


    }
}
