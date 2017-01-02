using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MemoryModelTests
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class WriteRead
    {
        private volatile bool A;
        private volatile bool B;
        private volatile bool A_Won;
        private volatile bool B_Won;

        private void ThreadA()
        {
            A = true;
            // Thread.MemoryBarrier();
            if (!B) A_Won = true;
        }

        private void ThreadB()
        {
            B = true;
            // Thread.MemoryBarrier();
            if (!A) B_Won = true;
        }

        [Test]
        public void WriteReadTest()
        {
            for (var i = 0; i < 1 << 21; i++)
            {
                A = B = A_Won = B_Won = false;

                var t1 = Task.Run(() => ThreadA());
                var t2 = Task.Run(() => ThreadB());
                Task.WaitAll(t1, t2);

                if (A_Won && B_Won)
                    Assert.Fail("There can only be one");
            }
        }
    }
}