using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Dialogs;

namespace BarcodeGenerator
{
    class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            var builder = BuildAvaloniaApp();

            if (args.Contains("--drm"))
            {
                SilenceConsole();
                return builder.StartLinuxDrm(args, scaling: 1);
            }
            
            
            return builder.StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .LogToDebug()
                .UseReactiveUI();

        static void SilenceConsole()
        {
            new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
        }
    }
}
