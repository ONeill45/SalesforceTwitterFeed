using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace TwitterFeed.Models
{
    public class OAuthApi
    {
        #region enum
        //enum for http types
        enum HttpType
        {
            GET,
            POST
        };
        //enum for params
        enum Parameter
        {
            screen_name,
            count
        }
        #endregion
        #region properties
        public string BaseUrl { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string Signature { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Type { get; set; }
        public string AuthorizationHeader { get; set; }
        public string FullUrl { get; set; }
        
        //sorted dictionary to build signature from(sorted since values must be alphabetical)
        private SortedDictionary<string, string> SignatureValues { get; set; }
        private string Nonce { get; set; }
        private string Timestamp { get; set; }

        private static string SignatureMethod { get; set; }
        private static string Version { get; set; }
        #endregion
        #region constructors
        public OAuthApi() { }
        //constructor to build oauth header for http client
        public OAuthApi(string type, string key, string secret, string token, string tokenSecret, string url)
        {
            this.Type = type;
            this.ConsumerKey = key;
            this.ConsumerSecret = secret;
            this.Token = token;
            this.TokenSecret = tokenSecret;
            this.BaseUrl = url;
            this.Nonce = GetNonce();
            this.Timestamp = GetTimestamp();
            SignatureMethod = "HMAC-SHA1";
            Version = "1.0";
            SignatureValues = new SortedDictionary<string, string>();
            SignatureValues.Add("oauth_consumer_key", this.ConsumerKey);
            SignatureValues.Add("oauth_nonce", this.Nonce);
            SignatureValues.Add("oauth_timestamp", this.Timestamp);
            SignatureValues.Add("oauth_signature_method", SignatureMethod);
            SignatureValues.Add("oauth_token", this.Token);
            SignatureValues.Add("oauth_version", Version);
            this.FullUrl = GetUrl();
            this.BuildSignature();
            this.BuildHeader();

        }
        //constructor to build outh header for http client with params
        public OAuthApi(string type, string key, string secret, string token, string tokenSecret, string url, Dictionary<string, string> httpParams)
        {
            this.Type = type;
            this.ConsumerKey = key;
            this.ConsumerSecret = secret;
            this.Token = token;
            this.TokenSecret = tokenSecret;
            this.BaseUrl = url;
            this.Nonce = GetNonce();
            this.Timestamp = GetTimestamp();
            SignatureMethod = "HMAC-SHA1";
            Version = "1.0";
            SignatureValues = new SortedDictionary<string, string>();
            SignatureValues.Add("oauth_consumer_key", this.ConsumerKey);
            SignatureValues.Add("oauth_nonce", this.Nonce);
            SignatureValues.Add("oauth_timestamp", this.Timestamp);
            SignatureValues.Add("oauth_signature_method", SignatureMethod);
            SignatureValues.Add("oauth_token", this.Token);
            SignatureValues.Add("oauth_version", Version);

            this.Parameters = new Dictionary<string, string>();
            //check if param is in enum before adding to property
            foreach(KeyValuePair<string, string> kv in httpParams)
            {
                if (Enum.IsDefined(typeof(Parameter), kv.Key))
                {
                    this.Parameters.Add(kv.Key, kv.Value);
                    SignatureValues.Add(kv.Key, kv.Value);
                }
            }
            this.FullUrl = GetUrl();
            this.BuildSignature();
            this.BuildHeader();
            
        }
        #endregion
        #region methods
        //method to build signature
        public void BuildSignature() 
        {
            string parameterString = String.Empty;
            foreach (KeyValuePair<string, string> kv in SignatureValues)
            {
                parameterString += string.Format("{0}={1}&", Uri.EscapeDataString(kv.Key), Uri.EscapeDataString(kv.Value));
            }
            parameterString = parameterString.Substring(0, parameterString.Length - 1);
            parameterString = string.Concat(this.Type, "&", Uri.EscapeDataString(this.BaseUrl), "&", Uri.EscapeDataString(parameterString));
           
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}&{1}", this.ConsumerSecret, this.TokenSecret))))
            {
                this.Signature = Convert.ToBase64String(hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(parameterString)));
            }
        }
        //method to build total header
        public void BuildHeader()
        {
            string headerTemplate = "OAuth oauth_consumer_key=\"{0}\",oauth_token=\"{1}\"," +
                "oauth_signature_method=\"{2}\",oauth_timestamp=\"{3}\"," +
                "oauth_nonce=\"{4}\",oauth_version=\"{5}\"," +
                "oauth_signature=\"{6}\"";
            string authorizationHeader = string.Format(headerTemplate,
                Uri.EscapeDataString(this.ConsumerKey),
                Uri.EscapeDataString(this.Token),
                Uri.EscapeDataString(SignatureMethod),
                Uri.EscapeDataString(this.Timestamp),
                Uri.EscapeDataString(this.Nonce),
                Uri.EscapeDataString(Version),
                Uri.EscapeDataString(this.Signature)
                );

            this.AuthorizationHeader = authorizationHeader;
        }
        //method to set timestamp
        private static string GetTimestamp()
        {
            //timestamp is number of ticks since Unix epoch time
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
        }
        //method to set nonce
        private static string GetNonce()
        {
            return Guid.NewGuid().ToString("N");
        }
        //generate full url with params if exist
        private string GetUrl()
        {
            string fullUrl = BaseUrl;
            if (this.Parameters != null)
            {
                fullUrl += "?";
                foreach (KeyValuePair<string, string> kv in this.Parameters)
                {
                    fullUrl += kv.Key + "=" + kv.Value + "&";
                }
                fullUrl = fullUrl.Substring(0, fullUrl.Length - 1);
            }
            return fullUrl;
        }

        public async Task<List<Tweet>> GetTweets()
        {
            List<Tweet> tweets = new List<Tweet>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", this.AuthorizationHeader);
                HttpResponseMessage response = await client.GetAsync(this.FullUrl);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string tweetResponse = await response.Content.ReadAsStringAsync();
                    tweets.AddRange(JsonConvert.DeserializeObject<List<Tweet>>(tweetResponse));
                }
            }

            return tweets;
        }
        #endregion
    }
}