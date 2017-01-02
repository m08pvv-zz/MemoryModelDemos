using System.Threading.Tasks;
using NUnit.Framework;

namespace MemoryModelTests
{
    [TestFixture]
    internal class LoopReadHoisting
    {
        private bool flag = true;

        [Test]
        public void LoopReadHoistingTest()
        {
            Task.Run(() => { flag = false; });

            while (flag)
            {
            }
        }
    }
}