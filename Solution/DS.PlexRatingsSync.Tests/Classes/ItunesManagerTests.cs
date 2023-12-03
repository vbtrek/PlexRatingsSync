using DS.PlexRatingsSync;
using Moq;
using NUnit.Framework;

namespace DS.PlexRatingsSync.Tests
{
  [TestFixture]
  public class ItunesManagerTests
  {
    [SetUp]
    public void TestInitialize()
    {
      // We need to moq the MessageHandler
      //var mockMessageHandler = new Mock<IMessageHandler>();
      //mockMessageHandler.Setup(x => x.ExceptionWrite(It.IsAny<object>(), It.IsAny<Exception>()));
      //MessageManager.Instance.SetTestingInstance(mockMessageHandler);
    }

    [Test]
    public void TestGetItunesPlayLists()
    {
      Assert.Ignore("Need some moq's");
      //ItunesManager manager = CreateManager();
      //manager.GetItunesPlayLists();
    }

    private ItunesManager CreateManager()
    {
      return null;
      //return new ItunesManager();
    }
  }
}