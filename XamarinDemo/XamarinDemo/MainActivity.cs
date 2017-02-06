using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace XamarinDemo
{
    [Activity(Label = "XamarinDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly object personLockObject = new object();
        private Person person;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            StartDemo();
        }

        private async void StartDemo()
        {
            var okCount = 0;
            var failCount = 0;

            var editText = FindViewById<EditText>(Resource.Id.outputEditText);

            while (true)
                await Task.Run(() =>
                {
                    person = null;
                    Parallel.Invoke(
                        () =>
                        {
                            // Double-checked lock
                            if (person != null)
                                return;

                            lock (personLockObject)
                            {
                                if (person != null)
                                    return;

                                person = new Person();
                            }
                        },
                        () =>
                        {
                            while (person == null)
                                Thread.MemoryBarrier();

                            if (!person.IsInitialized())
                            {
                                failCount++;
                                Log.Error("m08pvv", $"Fails: {failCount}, ok: {okCount}");
                            }
                            else
                                okCount++;

                            RunOnUiThread(() => editText.Text = $"Fails: {failCount}, ok: {okCount}");
                        });
                });
        }
    }

    public class Person
    {
        private const string JohnSmithName = "John Smith";
        public string Name { get; }
        public int Age { get; }
        public int OneMoreInt { get; }

        // We can force inlining
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Person()
        {
            Name = JohnSmithName;
            Age = int.MaxValue;
            OneMoreInt = int.MaxValue;
        }

        public bool IsInitialized()
        {
            return (Name == JohnSmithName)
                   && (Age == int.MaxValue)
                   && (OneMoreInt == int.MaxValue);
        }
    }
}