using Linql.Client.Internal;
using System;
using System.Net.Http;

namespace Linql.Client
{
    public class LinqlContext
    {
        protected HttpClient HttpClient { get; set; }

        public LinqlProvider Provider { get; set; }

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



        public virtual LinqlSearch<T> Set<T>()
        {
            return new LinqlSearch<T>(this.Provider);
        }


    }
}
