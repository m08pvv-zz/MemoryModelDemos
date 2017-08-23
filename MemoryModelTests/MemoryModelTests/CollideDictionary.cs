using NUnit.Framework;

namespace MemoryModelTests
{
    public struct NonAtomic
    {
        public int x;
        public int y;
        public int z;
        public int r;
        public int g;
        public int b;
        public int a;
    }


    public class WeirdKey
    {
        public override int GetHashCode()
        {
            return 42;
        }
    }

    [TestFixture]
    public class CollideDictionary
    {
        [Test]
        public void Collide()

        {
            var dict = new BuggyConcurrentDictionary<WeirdKey, NonAtomic>();

            var key42 = new WeirdKey();
            var key43 = new WeirdKey();
            var key44 = new WeirdKey();

            dict[key42] = new NonAtomic {a = 42};
            dict[key43] = new NonAtomic {a = 43};
            dict[key44] = new NonAtomic {a = 44};

            dict.AddOrUpdate(key44, _ => new NonAtomic {a = 30}, (_, v) =>
            {
                v.a++;
                return v;
            });

            NonAtomic x;
            dict.TryGetValue(key42, out x);
        }
    }
}