using DS.PlexRatingsSync;
using NUnit.Framework;
using System;

namespace DS.PlexRatingsSync.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [TestCase("test", ExpectedResult = "test")]
        [TestCase("C:\\A really long\\path name\\that should be\\truncated by\\EllipsisString", ExpectedResult = "C:\\...\\EllipsisString")]
        public string TestEllipsisString(string input)
        {
            return input.EllipsisString();
        }

        [TestCase(null)]
        public void TestEllipsisStringThrows(string input)
        {
            Assert.That(() => input.EllipsisString(), Throws.TypeOf<NullReferenceException>());
        }
    }
}