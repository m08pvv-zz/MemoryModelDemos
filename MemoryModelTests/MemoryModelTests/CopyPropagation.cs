using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MemoryModelTests
{
    public class CopyPropagation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckStrings(Container a)
        {
            if (a.S.Length == 0 || a.S2.Length == 0)
                throw new InvalidOperationException();
        }

        public void DoEvil(Container a)
        {
            a.S = null;
        }

        [Test]
        public void CopyPropagationTest()
        {
            var c = new Container();
            var date = DateTime.UtcNow.ToString("O");
            for (var i = 0; i < 1 << 20; i++)
            {
                var t1 = Task.Run(() =>
                {
                    c.S = date;
                    c.S2 = c.S;
                    CheckStrings(c);
                });
                var t2 = Task.Run(() => DoEvil(c));

                Task.WaitAll(t1, t2);
            }
        }

        public class Container
        {
            public string S { get; set; }
            public string S2 { get; set; }
        }
    }
}