using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;

namespace XamarinDemo
{
    [Activity(Label = "XamarinDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly object buttonLockObject = new object();
        private readonly object personLockObject = new object();
        private Person person;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += StartDemo;
        }

        private async void StartDemo(object sender, EventArgs eventArgs)
        {
            var okCount = 0;
            var failCount = 0;

            var button = FindViewById<Button>(Resource.Id.MyButton);
            var editText = FindViewById<EditText>(Resource.Id.outputEditText);

            lock (buttonLockObject)
            {
                if (button.Enabled == false)
                    return;

                button.Enabled = false;
            }

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
                                failCount++;
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

        // We can force inlining
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Person()
        {
            Name = JohnSmithName;
            Age = int.MaxValue;
        }

        public string Name { get; }
        public int Age { get; }

        public bool IsInitialized()
        {
            return (Name == JohnSmithName) && (Age == int.MaxValue);
        }
    }
}