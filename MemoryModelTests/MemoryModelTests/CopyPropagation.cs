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
            if (a.S.Length != 2 || a.S2.Length != 2)
                throw new Exception($"Copy was not propagated properly! S = {a.S}, S2 = {a.S2}");
        }

        public void DoEvil(Container a)
        {
            a.S = null;
        }

        [Test]
        public void CopyPropagationTest()
        {
            var c = new Container();

            for (var i = 0; i < 1 << 20; i++)
            {
                var t1 = Task.Run(() =>
                {
                    c.S = "OK";
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