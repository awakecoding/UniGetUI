using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using UniGetUI.Core.Data;
using UniGetUI.Core.Logging;
using UniGetUI.Core.SettingsEngine;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine;
using UniGetUI.Core.Classes;

namespace UniGetUI.Interface
{
    public partial class MainWindow : Window
    {
        public static readonly ObservableQueue<string> ParametersToProcess = new();
        
        private bool HasLoadedLastGeometry;
        private string _currentSubtitle = "";
        public bool BlockLoading;
        public int LoadingDialogCount;

        public MainWindow()
        {
            InitializeComponent();
            
            // Set window icon
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var iconPath = System.IO.Path.Join(CoreData.UniGetUIExecutableDirectory, "Assets", "Images", "icon.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    Icon = new Avalonia.Controls.WindowIcon(iconPath);
                }
            }

            // Initialize application
            ApplyTheme();
            
            // Process command line arguments
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                ParametersToProcess.Enqueue(arg);
            }

            // Setup event handlers
            Opened += MainWindow_Opened;
            Closing += MainWindow_Closing;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void MainWindow_Opened(object? sender, EventArgs e)
        {
            Logger.Info("MainWindow opened");
            LoadGeometry();
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            Logger.Info("MainWindow closing");
            SaveGeometry();
        }

        public void ApplyTheme()
        {
            Logger.Debug("Applying theme");
            // Theme application logic will be implemented here
            // Avalonia uses RequestedThemeVariant instead of WinUI's ElementTheme
        }

        private void LoadGeometry()
        {
            if (HasLoadedLastGeometry)
                return;

            try
            {
                // Load saved window position and size
                // This will need to be implemented with Avalonia's position/size properties
                Logger.Debug("Loading window geometry");
                HasLoadedLastGeometry = true;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load window geometry", ex);
            }
        }

        private void SaveGeometry()
        {
            try
            {
                // Save window position and size
                Logger.Debug("Saving window geometry");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save window geometry", ex);
            }
        }

        public void UpdateSystemTrayStatus()
        {
            // System tray icon updates will be implemented here
            Logger.Debug("Updating system tray status");
        }
    }
}
