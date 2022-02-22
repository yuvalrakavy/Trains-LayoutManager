using MethodDispatcher;
using System.Diagnostics;

namespace MethodDispatcherTest {
    public class SomeClass {

    }

    class DervivedClass : SomeClass {
    }

    public static class Extensions {
#if notdef
        static void CallMe(this Dispatcher d) {

        }

        static void AndMe(this Dispatcher d) {
        }

        [DispatchSource]
        static async Task<SomeClass?> GetSomeClassOptional(this Dispatcher d) {
            return null;
        }

#endif

        [DispatchSource]
        public static SomeClass GetSomeClass(this Dispatcher d, int i) {
            return d[nameof(GetSomeClass)].Call<SomeClass>(i);
        }

        static DispatchSource? dCollectNames;

        [DispatchSource]
        public static IEnumerable<string> CollectNames(this Dispatcher d) {
            return (dCollectNames ??= d[nameof(CollectNames)]).CallCollect<string>();
        }
    }

    class TestingClass {
        [DispatchTarget]
        static SomeClass GetSomeClass([DispatchFilter] int i = 4) {
            Trace.WriteLine($"GetSomeClass was called!!! {i}");
            return new SomeClass();
        }

        [DispatchTarget]
        static string First_CollectNames() {
            return "Yuval";
        }

        [DispatchTarget]
        static string Second_CollectNames() {
            return "Rakavy";
        }
    }

    internal static class Program {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            try {
                Dispatch.InitializeDispatcher();
            }
            catch (DispatcherErrorsException ex) {
                ex.Save();
                MessageBox.Show("Errors while initializing dispatcher (see dispatcher_errors.txt)");
                Application.Exit();
            }

            Dispatch.Call.GetSomeClass(4);

            foreach (var s in Dispatch.Call.CollectNames()) {
                Trace.WriteLine($"Collected {s}");
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}