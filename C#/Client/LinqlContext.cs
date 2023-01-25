using System;
using System.Net.Http;

namespace Linql.Client
{
    public class LinqlContext
    {
        protected HttpClient HttpClient { get; set; }

        public LinqlContext(string BaseUrl = null)
        {
            if (BaseUrl != null)
            {
                this.HttpClient = new HttpClient();
                this.HttpClient.BaseAddress = new Uri(BaseUrl);
            }
        }

        public LinqlSearch<T> Set<T>()
        {
            return new LinqlSearch<T>();
        }


    }
}
