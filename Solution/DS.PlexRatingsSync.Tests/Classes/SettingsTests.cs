using DS.PlexRatingsSync;
using Moq;
using NUnit.Framework;

namespace DS.PlexRatingsSync.Tests
{
  [TestFixture]
  public class SettingsTests
  {
    [Test]
    public void TestGetPreferences()
    {
      Assert.Ignore("Needs message handler moq'd");
      Settings.GetPreferences();
    }

    [Test]
    public void TestSavePreferences()
    {
      Assert.Ignore("Needs message handler moq'd");
      Settings.SavePreferences();
    }

    /*
    [TestCase(null, ExpectedResult = 0)]
    [TestCase("1 - vbtrek", ExpectedResult = 1)]
    [TestCase("invalid account format", ExpectedResult = 0)]
    public int TestPlexAccountId(string plexAccount)
    {
        Settings.PlexAccount = plexAccount;
        return Settings.PlexAccountId;
    }
    */
  }
}