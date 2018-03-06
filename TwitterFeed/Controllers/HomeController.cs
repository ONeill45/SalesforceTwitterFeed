using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TwitterFeed.Models;

namespace TwitterFeed.Controllers
{
    public class HomeController : Controller
    {
        #region properties
        public static List<Tweet> Tweets;
        private static string ConsumerKey = "waJfw5AvQjAzj5xODVM9mAoiz";
        private static string ConsumerSecret = "MJ0WkSnCVxlP9lZpaMWvy6UM5X1Ib0wJ2SYmRhK26rVb6UxAMI";
        private static string Token = "966452011-0mA9FaiVFq0CjPiD2O06TEgI6mGial7yrHcBt6YX";
        private static string TokenSecret = "H5Na5fdGGCywjJwL6oWjWsrv3CBLqn4yaaIkq4k50JU59";
        #endregion

        public async Task<List<Tweet>> GetTweetsByOauth()
        {
            string consumerKey = ConsumerKey;
            string consumerSecret = ConsumerSecret;
            string token = Token;
            string tokenSecret = TokenSecret;
            string baseUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";
            Dictionary<string, string> httpParams = new Dictionary<string, string>();
            httpParams.Add("count", "10");
            httpParams.Add("screen_name", "salesforce");

            OAuthApi oauth = new OAuthApi("GET", consumerKey, consumerSecret, token, tokenSecret, baseUrl, httpParams);
            return await oauth.GetTweets();
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            Tweets = await GetTweetsByOauth();
            return View("~/Views/Home/TwitterFeed.cshtml", Tweets);
        }
        [HttpPost]
        public ActionResult FilterTweetsOnPage(string search)
        {
            List<Tweet> filterTweets = FilterTweets(search);
            return PartialView("~/Views/Home/Tweets.cshtml", filterTweets);
        }
        public List<Tweet> FilterTweets(string search)
        {
            return Tweets.Where(t => t.Content.Contains(search)).ToList();
        }
        
        [HttpPost]
        public ActionResult UnfilterTweets()
        {
            return PartialView("~/Views/Home/Tweets.cshtml", Tweets);
        }
        [HttpGet]
        public async Task<ActionResult> RefreshTweets()
        {
            Tweets = await GetTweetsByOauth();
            return PartialView("~/Views/Home/Tweets.cshtml", Tweets);
        }
    }
}
