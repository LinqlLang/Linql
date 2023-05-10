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
    /// <summary>
    /// The default implementation of a LinqlContext.  This context uses System.Text.Json for serialization/deserialization, and uses the built in HttpClient to make requests.
    /// </summary>
    public class LinqlContext : ALinqlContext
    {
        /// <summary>
        /// .Net Http Client used to make requests.
        /// </summary>
        protected HttpClient HttpClient { get; set; }

        /// <summary>
        /// System.Text.Json serializer options. 
        /// </summary>
        protected JsonSerializerOptions JsonOptions { get; set; }

        /// <summary>
        /// Returns the base url of the Linql Server.  This method will get/set the url directly off the HttpClient.BaseAddress.AbsoluteUri
        /// </summary>
        public virtual string BaseUrl
        {
            get
            {
                if (this.HttpClient != null)
                {
                    return this.HttpClient.BaseAddress.AbsoluteUri;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    this.HttpClient = null;
                }
                else
                {
                    if (this.HttpClient == null)
                    {
                        this.HttpClient = new HttpClient();
                    }
                    this.HttpClient.BaseAddress = new Uri(value);
                }
            }
        }

        /// <summary>
        /// Creates a new LinqlContext with the optional BaseUrl or JsonOptions.
        /// </summary>
        /// <param name="BaseUrl">The baseurl of the Linql Server</param>
        /// <param name="JsonOptions">JsonSerializerOptions if desired. By Default: 
        /// DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        /// PropertyNameCaseInsensitive = true
        /// </param>
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

        /// <summary>
        /// Executes the LinqlSearch.  This method checks to make sure HttpClient is not null, extracts the endpoint, then calls SendHttpRequest.
        /// </summary>
        /// <typeparam name="TResult">The result of the LinqlSearch</typeparam>
        /// <param name="Search">The LinqlSearch</param>
        /// <returns>Returns a result of type TResult</returns>
        /// <exception cref="System.Exception">Throws if HttpClient is not configured</exception>
        protected virtual async Task<TResult> GetResult<TResult>(LinqlSearch Search)
        {
            if (this.HttpClient == null)
            {
                throw new System.Exception("No HttpClient was configured in this LinqlContext.  Please pass a BaseUrl string into the constructor or derive and override the HttpClient property.");
            }

            string url = this.GetEndpoint(Search);
            return await this.SendHttpRequest<TResult>(url, Search);

        }
        /// <summary>
        /// Default implementation of sending a Linql HttpRequest.  By default, the Search is serialized, and then the request sent, received, and deserialized.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="Endpoint">The Linql Server endpoint</param>
        /// <param name="Search">The LinqlSearch</param>
        /// <returns>A task with a result of type TResult</returns>
        protected virtual async Task<TResult> SendHttpRequest<TResult>(string Endpoint, LinqlSearch Search)
        {
            string search = this.ToJson(Search);
            StringContent requestContent = new StringContent(search, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.HttpClient.PostAsync(Endpoint, requestContent);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<TResult>(contentStream, this.JsonOptions);
            return result;
        }
        /// <summary>
        /// Allows you to force sending a LinqlSearch without having it materialize itself.  Used in extension methods that materialize the search.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="LinqlSearch">The LinqlSearch to execute</param>
        /// <returns>A Task of type TResult</returns>
        public override async Task<TResult> SendRequestAsync<TResult>(LinqlSearch LinqlSearch)
        {
            return await this.GetResult<TResult>(LinqlSearch);
        }

        /// <summary>
        /// Allows you to force sending a LinqlSearch without having it materialize itself.  Used in extension methods that materialize the search.
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="LinqlSearch">The LinqlSearch to execute</param>
        /// <returns>A TResult</returns>
        public override TResult SendRequest<TResult>(IQueryable LinqlSearch)
        {
            LinqlSearch search = LinqlSearch.ToLinqlSearch();
            Task<TResult> task = this.GetResult<TResult>(search);
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
