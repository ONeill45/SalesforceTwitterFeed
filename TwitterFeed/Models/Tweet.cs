using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterFeed.Models
{
    public class Tweet
    {
        public User User { get; set; }
        [JsonProperty(PropertyName="text")]
        public string Content { get; set; }
        [JsonProperty(PropertyName="retweet_count")]
        public string RetweetCount { get; set; }
        [JsonProperty(PropertyName="created_at")]
        public string TweetPosted { get; set; }
        [JsonProperty(PropertyName = "entities")]
        public Entity Entities { get; set; }
    }
    public class User
    {
        [JsonProperty(PropertyName = "name")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "screen_name")]
        public string UserScreenName { get; set; }
        [JsonProperty(PropertyName = "profile_image_url")]
        public string ProfilePicture { get; set; }
    }

    public class Entity
    {
        [JsonProperty(PropertyName = "hashtags")]
        public List<Hashtag> Hashtags { get; set; }
        //[JsonProperty(PropertyName="media")]
        //public List<Media> Media { get; set; }
        [JsonProperty(PropertyName="urls")]
        public List<Url> Urls { get; set; }
        //public List<UserMention> UserMentions { get; set; }
        //public List<Symbol> Symbols { get; set; }
        //public List<Poll> Polls { get; set; }

    }
    public class Hashtag
    {

    }
    public class Media
    {
        [JsonProperty(PropertyName="media_url_https")]
        public string MediaUrl { get; set; }
    }
    public class Url
    {
        [JsonProperty(PropertyName="url")]
        public string ShortUrl { get; set; }
        [JsonProperty(PropertyName = "expanded_url")]
        public string ExpandedUrl { get; set; }
        [JsonProperty(PropertyName = "display_url")]
        public string DisplayUrl { get; set; }
        [JsonProperty(PropertyName = "indices")]
        public int[] Indices { get; set; }
    }
    public class UserMention
    {

    }
    public class Symbol
    {

    }
    public class Poll
    {

    }
}