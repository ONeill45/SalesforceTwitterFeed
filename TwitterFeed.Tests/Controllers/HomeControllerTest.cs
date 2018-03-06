using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterFeed;
using TwitterFeed.Controllers;
using System.Threading.Tasks;
using System.Linq;

namespace TwitterFeed.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public async Task Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, HomeController.Tweets.Count);
        }


        [TestMethod]
        public async Task RefreshTweets()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = await controller.RefreshTweets() as ViewResult;

            // Assert
            Assert.AreEqual(10, HomeController.Tweets.Count);
        }

        [TestMethod]
        public async Task FilterTweets(string text) 
        {
            // Arrange
            HomeController controller = new HomeController();
            await controller.Index();

            // Act
            var filteredTweets = controller.FilterTweets(text);

            // Assert
            var testFilter = HomeController.Tweets
                .Where(t => t.Content.Contains(text))
                .ToList();
            Assert.AreEqual(filteredTweets, testFilter);
        }
    }
}
