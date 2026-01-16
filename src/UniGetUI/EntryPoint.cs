using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using UniGetUI.Core.Data;
using UniGetUI.Core.Logging;

namespace UniGetUI
{
    public static class EntryPoint
    {
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                if (args.Contains(CLIHandler.HELP))
                {
                    CLIHandler.Help();
                    Environment.Exit(0);
                }
                else if (args.Contains(CLIHandler.MIGRATE_WINGETUI_TO_UNIGETUI))
                {
                    int ret = CLIHandler.WingetUIToUniGetUIMigrator();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.UNINSTALL_UNIGETUI) || args.Contains(CLIHandler.UNINSTALL_WINGETUI))
                {
                    int ret = CLIHandler.UninstallUniGetUI();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.IMPORT_SETTINGS))
                {
                    int ret = CLIHandler.ImportSettings();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.EXPORT_SETTINGS))
                {
                    int ret = CLIHandler.ExportSettings();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.ENABLE_SETTING))
                {
                    int ret = CLIHandler.EnableSetting();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.DISABLE_SETTING))
                {
                    int ret = CLIHandler.DisableSetting();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.SET_SETTING_VAL))
                {
                    int ret = CLIHandler.SetSettingsValue();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.ENABLE_SECURE_SETTING))
                {
                    int ret = CLIHandler.EnableSecureSetting();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.DISABLE_SECURE_SETTING))
                {
                    int ret = CLIHandler.DisableSecureSetting();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.ENABLE_SECURE_SETTING_FOR_USER))
                {
                    int ret = CLIHandler.EnableSecureSettingForUser();
                    Environment.Exit(ret);
                }
                else if (args.Contains(CLIHandler.DISABLE_SECURE_SETTING_FOR_USER))
                {
                    int ret = CLIHandler.DisableSecureSettingForUser();
                    Environment.Exit(ret);
                }
                else
                {
                    CoreData.WasDaemon = CoreData.IsDaemon = args.Contains(CLIHandler.DAEMON);
                    StartAvaloniaApp(args);
                }
            }
            catch (Exception e)
            {
                CrashHandler.ReportFatalException(e);
            }
        }

        /// <summary>
        /// UniGetUI Avalonia app entry point
        /// </summary>
        private static void StartAvaloniaApp(string[] args)
        {
            try
            {
                string textart = $"""
                     __  __      _ ______     __  __  ______
                    / / / /___  (_) ____/__  / /_/ / / /  _/
                   / / / / __ \/ / / __/ _ \/ __/ / / // /
                  / /_/ / / / / / /_/ /  __/ /_/ /_/ // /
                  \____/_/ /_/_/\____/\___/\__/\____/___/
                      Welcome to UniGetUI Version {CoreData.VersionName}
                  """;

                Logger.ImportantInfo(textart);
                Logger.ImportantInfo("  ");
                Logger.ImportantInfo($"Build {CoreData.BuildNumber}");
                Logger.ImportantInfo($"Data directory {CoreData.UniGetUIDataDirectory}");
                Logger.ImportantInfo($"Encoding Code Page set to {CoreData.CODE_PAGE}");

                // Build and run Avalonia app
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                CrashHandler.ReportFatalException(e);
            }
        }

        /// <summary>
        /// Configure and build the Avalonia application
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<MainApp>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
