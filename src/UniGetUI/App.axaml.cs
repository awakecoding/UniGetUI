using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using UniGetUI.Core.Data;
using UniGetUI.Core.IconEngine;
using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.SettingsEngine.SecureSettings;
using UniGetUI.Core.Tools;
using UniGetUI.Interface;
using UniGetUI.Interface.Telemetry;
using UniGetUI.PackageEngine;
using UniGetUI.PackageEngine.Classes.Manager.Classes;
using UniGetUI.PackageEngine.Interfaces;

namespace UniGetUI
{
    public partial class MainApp : Application
    {
        public static Dispatcher Dispatcher { get; private set; } = null!;
        public static MainApp Instance { get; private set; } = null!;
        public MainWindow MainWindow { get; private set; } = null!;
        public bool RaiseExceptionAsFatal = true;

        private readonly BackgroundApiRunner BackgroundApi = new();

        public static class Tooltip
        {
            private static int _errors_occurred;
            public static int ErrorsOccurred
            {
                get => _errors_occurred;
                set { _errors_occurred = value; Instance?.MainWindow?.UpdateSystemTrayStatus(); }
            }

            private static bool _restart_required;
            public static bool RestartRequired
            {
                get => _restart_required;
                set { _restart_required = value; Instance?.MainWindow?.UpdateSystemTrayStatus(); }
            }

            private static int _available_updates;
            public static int AvailableUpdates
            {
                get => _available_updates;
                set { _available_updates = value; Instance?.MainWindow?.UpdateSystemTrayStatus(); }
            }
        }

        public override void Initialize()
        {
            try
            {
                Instance = this;
                AvaloniaXamlLoader.Load(this);
                ApplyThemeToApp();
            }
            catch (Exception e)
            {
                CrashHandler.ReportFatalException(e);
            }
        }

        private void ApplyThemeToApp()
        {
            // Avalonia theme application logic
            // Will be implemented based on settings
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher = Avalonia.Threading.Dispatcher.UIThread;
                
                RegisterErrorHandling();
                
                // Start async component loading
                _ = LoadComponentsAsync();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void RegisterErrorHandling()
        {
            try
            {
                TaskScheduler.UnobservedTaskException += (sender, args) =>
                {
                    Logger.Error($"An unhandled exception occurred in a Task (sender: {sender?.GetType().ToString() ?? "null"})");
                    Exception? e = args.Exception.InnerException;
                    Logger.Error(args.Exception);
                    while (e is not null)
                    {
                        Logger.Error("------------------------------");
                        Logger.Error(e);
                        e = e.InnerException;
                    }

                    if (Debugger.IsAttached) Debugger.Break();

                    Dispatcher.Post(() =>
                    {
                        if (MainWindow is null)
                            return;
                        try
                        {
                            Logger.Warn("Showing error notification to user");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    });

                    args.SetObserved();
                };
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                Logger.Error(ex);
            }
        }

        private async Task LoadComponentsAsync()
        {
            try
            {
                // MainWindow depends on this
                await Task.Run(PEInterface.LoadLoaders);

                // Create MainWindow
                InitializeMainWindow();

                var iniTasks = new[]
                {
                    Task.Run(PEInterface.LoadManagers),
                    Task.Run(async () => await IconDatabase.Instance.LoadFromCacheAsync()),
                    Task.Run(LoadGSudo),
                    Task.Run(InitializeBackgroundAPI),
                };

                // Load essential components
                await Task.WhenAll(iniTasks);

                // Load non-essential components
                _ = TelemetryHandler.InitializeAsync();
                _ = IconDatabase.Instance.LoadIconAndScreenshotsDatabaseAsync();

                // Load interface
                Logger.Info("LoadComponentsAsync finished executing. All managers loaded. Proceeding to interface.");
                MainWindow.SwitchToInterface();

                RaiseExceptionAsFatal = false;
            }
            catch (Exception e)
            {
                CrashHandler.ReportFatalException(e);
            }
        }

        private static async Task LoadGSudo()
        {
            try
            {
                if (Settings.Get(Settings.K.ProhibitElevation))
                {
                    Logger.Warn("UniGetUI Elevator has been disabled since elevation is prohibited!");
                }

                if (SecureSettings.Get(SecureSettings.K.ForceUserGSudo))
                {
                    var res = await CoreTools.WhichAsync("gsudo.exe");
                    if (res.Item1)
                    {
                        CoreData.ElevatorPath = res.Item2;
                        Logger.Warn($"Using user GSudo (forced by user) at {CoreData.ElevatorPath}");
                        return;
                    }
                }

#if DEBUG
                Logger.Warn($"Using bundled GSudo at {CoreData.ElevatorPath} since UniGetUI Elevator is not available!");
                CoreData.ElevatorPath = (await CoreTools.WhichAsync("gsudo.exe")).Item2;
#else
                string elevatorKind = Settings.Get(Settings.K.UseLegacyElevator)
                    ? "UniGetUI Elevator (Legacy).exe"
                    : "UniGetUI Elevator.exe";
                CoreData.ElevatorPath = System.IO.Path.Join(CoreData.UniGetUIExecutableDirectory, "Assets", "Utilities", elevatorKind);
                Logger.Debug($"Using built-in UniGetUI Elevator at {CoreData.ElevatorPath}");
#endif
            }
            catch (Exception ex)
            {
                Logger.Error("Elevator/GSudo failed to be loaded!");
                Logger.Error(ex);
            }
        }

        private void InitializeMainWindow()
        {
            MainWindow = new MainWindow
            {
                BlockLoading = true
            };

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = MainWindow;
            }
        }

        private async Task InitializeBackgroundAPI()
        {
            try
            {
                if (Settings.Get(Settings.K.DisableApi))
                    return;

                BackgroundApi.OnOpenWindow += (_, _) =>
                    Dispatcher.Post(() => MainWindow.Activate());

                await BackgroundApi.Start();
            }
            catch (Exception ex)
            {
                Logger.Error("Could not initialize Background API:");
                Logger.Error(ex);
            }
        }

        public void DisposeAndQuit(int outputCode = 0)
        {
            Logger.Warn("Quitting UniGetUI");
            DWMThreadHelper.ChangeState_DWM(false);
            DWMThreadHelper.ChangeState_XAML(false);
            MainWindow?.Close();
            BackgroundApi?.Stop();
            Environment.Exit(outputCode);
        }

        public void KillAndRestart()
        {
            Process.Start(CoreData.UniGetUIExecutableFile);
            Instance.MainWindow?.Close();
            Environment.Exit(0);
        }
    }
}
