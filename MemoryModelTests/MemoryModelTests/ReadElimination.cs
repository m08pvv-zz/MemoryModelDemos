using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MemoryModelTests
{
    /// <summary>
    ///     This sample shows that read may be eliminated unless it's volatile read
    ///     TODO: Some say that through .NET Framework 4 volatile doesn't help (CLR contains a bug)
    /// </summary>
    [TestFixture]
    public class ReadElimination
    {
        // TODO: (step 2) uncomment the line below to fix the test
        private /*volatile*/ int a;
        private int b;

        [Test]
        public void ReadEliminationTest()
        {
            // We must have enough iterations to hit conditions
            for (var i = 0; i < 1 << 21; i++)
            {
                a = 0;
                b = 0;


                var t1 = Task.Run(() =>
                {
                    b = -5;
                    // Ensure that 'b' is already assigned when 'a' is assigned
                    Thread.MemoryBarrier();
                    a = 42;
                });

                var t2 = Task.Run(() =>
                    {
                        // TODO: (step 1) uncomment the line below to introduce extra read of field b
                        // if (b == -1) throw new Exception();

                        var first = a;
                        var second = b;
                        if (second != -5 && first == 42)
                            Assert.Fail("Read was eliminated");
                    }
                );

                Task.WaitAll(t1, t2);
            }
        }
    }
}