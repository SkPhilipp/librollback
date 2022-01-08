using NUnit.Framework;
using Rollback.structures;

namespace Rollback.Tests.structures
{
    [TestFixture]
    public class EmbeddedObjectExampleTest
    {
        [Test]
        public void Test()
        {
            var rollbackClock = new RollbackClock(100);
            var embeddedObject = new EmbeddedObjectExample(rollbackClock);
            embeddedObject.X = 100;
            embeddedObject.Y = 200;
            Assert.AreEqual(100, embeddedObject.X);
            Assert.AreEqual(200, embeddedObject.Y);
            rollbackClock.MoveTo(0);
            embeddedObject.Rollback();
            Assert.AreEqual(0, embeddedObject.X);
            Assert.AreEqual(0, embeddedObject.Y);
        }
    }
}