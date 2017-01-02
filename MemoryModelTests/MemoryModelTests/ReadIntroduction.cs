using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MemoryModelTests
{
    [TestFixture]
    public class ReadIntroduction
    {
        private object someObject;

        private void PrintObj()
        {
            var localReference = someObject;
            if (localReference != null)
                Console.WriteLine(localReference.ToString());
        }

        private void Uninitialize()
        {
            someObject = null;
        }

        [Test]
        public void ReadIntroductionTest()
        {
            for (var i = 0; i < 1 << 21; i++)
            {
                someObject = new object();

                var t1 = Task.Run(() => PrintObj());
                var t2 = Task.Run(() => Uninitialize());

                Task.WaitAll(t1, t2);
            }
        }
    }
}