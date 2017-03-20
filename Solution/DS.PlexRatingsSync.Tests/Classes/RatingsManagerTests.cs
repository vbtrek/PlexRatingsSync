using DS.Library.MessageHandling;
using DS.PlexRatingsSync;
using Moq;
using NUnit.Framework;
using System;

namespace DS.PlexRatingsSync.Tests
{
    [TestFixture]
    public class RatingsManagerTests
    {
        [SetUp]
        public void TestInitialize()
        {
            // We need to moq the MessageHandler
            //var mockMessageHandler = new Mock<IMessageHandler>();
            //mockMessageHandler.Setup(x => x.ExceptionWrite(It.IsAny<object>(), It.IsAny<Exception>()));
            //MessageManager.Instance.SetTestingInstance(mockMessageHandler);
        }

        [TestCase("C:\\Invalid File.mp3", ExpectedResult = null)]
        public uint? TestFileRating(string file)
        {
            return RatingsManager.FileRating(file);
        }

        [TestCase("C:\\Invalid File.mp3", ExpectedResult = null)]
        public int? TestPlexRatingFromFile(string file)
        {
            return RatingsManager.PlexRatingFromFile(file);
        }

        [TestCase(null, ExpectedResult = null)]
        [TestCase(1, ExpectedResult = null)]
        [TestCase(2, ExpectedResult = 1)]
        [TestCase(3, ExpectedResult = null)]
        [TestCase(4, ExpectedResult = 25)]
        [TestCase(5, ExpectedResult = null)]
        [TestCase(6, ExpectedResult = 50)]
        [TestCase(7, ExpectedResult = null)]
        [TestCase(8, ExpectedResult = 75)]
        [TestCase(9, ExpectedResult = null)]
        [TestCase(10, ExpectedResult = 99)]
        [TestCase(15, ExpectedResult = null)]
        public int? TestFileRatingFromPlexRating(int? plexRating)
        {
            return RatingsManager.FileRatingFromPlexRating(plexRating);
        }
    }
}