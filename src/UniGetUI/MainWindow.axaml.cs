using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
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
        private int _currentSubtitlePxLength;
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
            ApplyProxyVariableToProcess();
            _applySubtitleToWindow();
            
            // Process command line arguments
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                ParametersToProcess.Enqueue(arg);
            }

            // Setup event handlers
            Opened += MainWindow_Opened;
            Closing += MainWindow_Closing;
            
            if (CoreData.IsDaemon)
            {
                DWMThreadHelper.ChangeState_DWM(true);
                DWMThreadHelper.ChangeState_XAML(true);
                CoreData.IsDaemon = false;
            }
            else
            {
                Show();
            }
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
            
            if (!Settings.Get(Settings.K.DisableSystemTray))
            {
                e.Cancel = true;
                DWMThreadHelper.ChangeState_DWM(true);
                DWMThreadHelper.ChangeState_XAML(true);
                Hide();
            }
        }

        private void _applySubtitleToWindow()
        {
            if (Settings.Get(Settings.K.ShowVersionNumberOnTitlebar))
            {
                AddToSubtitle(CoreTools.Translate("version {0}", CoreData.VersionName));
            }

            if (CoreTools.IsAdministrator())
            {
                AddToSubtitle(CoreTools.Translate("[RAN AS ADMINISTRATOR]"));
            }

            if (CoreData.IsPortable)
            {
                AddToSubtitle(CoreTools.Translate("Portable mode"));
            }

#if DEBUG
            AddToSubtitle(CoreTools.Translate("DEBUG BUILD"));
#endif
        }

        public static void ApplyProxyVariableToProcess()
        {
            try
            {
                var proxyUri = Settings.GetProxyUrl();
                if (proxyUri is null || !Settings.Get(Settings.K.EnableProxy))
                {
                    Environment.SetEnvironmentVariable("HTTP_PROXY", "", EnvironmentVariableTarget.Process);
                    return;
                }

                string content;
                if (Settings.Get(Settings.K.EnableProxyAuth) is false)
                {
                    content = proxyUri.ToString();
                }
                else
                {
                    var creds = Settings.GetProxyCredentials();
                    if (creds is null)
                    {
                        content = $"--proxy {proxyUri.ToString()}";
                    }
                    else
                    {
                        content = $"{proxyUri.Scheme}://{Uri.EscapeDataString(creds.UserName)}" +
                                  $":{Uri.EscapeDataString(creds.Password)}" +
                                  $"@{proxyUri.AbsoluteUri.Replace($"{proxyUri.Scheme}://", "")}";
                    }
                }

                Environment.SetEnvironmentVariable("HTTP_PROXY", content, EnvironmentVariableTarget.Process);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to apply proxy settings:");
                Logger.Error(ex);
            }
        }

        private void AddToSubtitle(string line)
        {
            if (_currentSubtitle.Length > 0)
                _currentSubtitle += " - ";
            _currentSubtitle += line;
            _currentSubtitlePxLength = _currentSubtitle.Length * 4;
            Title = "UniGetUI - " + _currentSubtitle;
        }

        private void ClearSubtitle()
        {
            _currentSubtitle = "";
            _currentSubtitlePxLength = 0;
            Title = "UniGetUI";
        }

        public void ApplyTheme()
        {
            Logger.Debug("Applying theme");
            // Avalonia theme application logic
            // Will be implemented based on settings
        }

        private void LoadGeometry()
        {
            if (HasLoadedLastGeometry)
                return;

            try
            {
                string geometry = Settings.GetValue(Settings.K.WindowGeometry);
                string[] items = geometry.Split(",");
                if (items.Length != 5)
                {
                    Logger.Warn($"The restored geometry did not have exactly 5 items (found length was {items.Length})");
                    return;
                }

                int X, Y, Width, Height, State;
                try
                {
                    X = int.Parse(items[0]);
                    Y = int.Parse(items[1]);
                    Width = int.Parse(items[2]);
                    Height = int.Parse(items[3]);
                    State = int.Parse(items[4]);
                }
                catch (Exception ex)
                {
                    Logger.Error("Could not parse window geometry integers");
                    Logger.Error(ex);
                    return;
                }

                if (State == 1)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    Position = new PixelPoint(X, Y);
                    Width = Width;
                    Height = Height;
                }

                Logger.Debug("Window geometry loaded successfully");
                HasLoadedLastGeometry = true;
            }
            catch (Exception ex)
            {
                // TODO: Avalonia - Logger.Error doesn't accept (string, Exception), use Exception overload
                Logger.Error(ex);
            }
        }

        private void SaveGeometry()
        {
            try
            {
                int windowState = WindowState == WindowState.Maximized ? 1 : 0;
                string geometry = $"{Position.X},{Position.Y},{(int)Width},{(int)Height},{windowState}";
                
                Logger.Debug($"Saving window geometry {geometry}");
                Settings.SetValue(Settings.K.WindowGeometry, geometry);
            }
            catch (Exception ex)
            {
                // TODO: Avalonia - Logger.Error doesn't accept (string, Exception), use Exception overload
                Logger.Error(ex);
            }
        }

        public void UpdateSystemTrayStatus()
        {
            // System tray icon updates will be implemented here
            Logger.Debug("Updating system tray status");
        }

        // TODO: Avalonia - Stub for WinGetWarningBanner property
        public WinGetWarningBannerStub WinGetWarningBanner { get; } = new();

        public void SwitchToInterface()
        {
            Dispatcher.UIThread.Post(() =>
            {
                Logger.Info("Switching to main interface");
                // Load main UI here
                // For now, just update the content area
            });
        }
    }

    // TODO: Avalonia - Stub class for WinGetWarningBanner
    public class WinGetWarningBannerStub
    {
        public bool IsOpen { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public Button? ActionButton { get; set; }
    }
}
