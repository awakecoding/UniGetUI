using System;
using System.Diagnostics;
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
        public static MainApp? Instance { get; private set; }
        public MainWindow? MainWindow { get; private set; }

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

            private static int _operations_in_progress;
            public static int OperationsInProgress
            {
                get => _operations_in_progress;
                set { _operations_in_progress = value; Instance?.MainWindow?.UpdateSystemTrayStatus(); }
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Instance = this;
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Dispatcher = Avalonia.Threading.Dispatcher.UIThread;
                
                // Initialize application
                InitializeApp();
                
                // Create and show main window
                MainWindow = new MainWindow();
                desktop.MainWindow = MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void InitializeApp()
        {
            try
            {
                Logger.Info("═══════════════════════════════════════════════════════════════════════════════════════════════════");
                Logger.Info($"Starting UniGetUI version {CoreData.VersionName}");
                Logger.Info("═══════════════════════════════════════════════════════════════════════════════════════════════════");

                // Initialize core components
                Logger.Info("Initializing core components...");
                
                // Initialize settings
                Settings.Initialize();
                
                // Initialize icon engine
                IconDatabase.LoadFromPath();
                
                // Initialize package managers
                Logger.Info("Initializing package managers...");
                PEInterface.Initialize();
                
                Logger.Info("Application initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to initialize application", ex);
                throw;
            }
        }

        public static void DispatcherBeginInvoke(Action action)
        {
            Dispatcher.Post(action);
        }

        public static void DispatcherInvoke(Action action)
        {
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.InvokeAsync(action).Wait();
            }
        }
    }
}
