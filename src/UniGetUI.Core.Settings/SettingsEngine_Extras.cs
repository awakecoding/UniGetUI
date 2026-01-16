using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using UniGetUI.Core.Logging;
#if WINDOWS
using Windows.Security.Credentials;
#endif

namespace UniGetUI.Core.SettingsEngine;

public partial class Settings
{
    /*
     *
     *
     */

    public static bool AreNotificationsDisabled()
        => Get(K.DisableSystemTray) || Get(K.DisableNotifications);

    public static bool AreUpdatesNotificationsDisabled()
        => AreNotificationsDisabled() || Get(K.DisableUpdatesNotifications);

    public static bool AreErrorNotificationsDisabled()
        => AreNotificationsDisabled() || Get(K.DisableErrorNotifications);

    public static bool AreSuccessNotificationsDisabled()
        => AreNotificationsDisabled() || Get(K.DisableSuccessNotifications);

    public static bool AreProgressNotificationsDisabled()
        => AreNotificationsDisabled() || Get(K.DisableProgressNotifications);

    public static Uri? GetProxyUrl()
    {
        if (!Get(K.EnableProxy)) return null;

        string plainUrl = GetValue(K.ProxyURL);
        Uri.TryCreate(plainUrl, UriKind.RelativeOrAbsolute, out Uri? var);
        if(Get(K.EnableProxy) && var is null) Logger.Warn($"Proxy setting {plainUrl} is not valid");
        return var;
    }

    private const string PROXY_RES_ID = "UniGetUI_proxy";

    public static NetworkCredential? GetProxyCredentials()
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            var credentials = vault.Retrieve(PROXY_RES_ID, GetValue(K.ProxyUsername));

            return new NetworkCredential()
            {
                UserName = credentials.UserName,
                Password = credentials.Password,
            };
        }
        catch (Exception ex)
        {
            Logger.Error("Could not retrieve Proxy credentials");
            Logger.Error(ex);
            return null;
        }
#else
        Logger.Warn("Proxy credentials storage not yet implemented for non-Windows platforms");
        return null;
#endif
    }

    public static void SetProxyCredentials(string username, string password)
    {
#if WINDOWS
        try
        {
            var vault = new PasswordVault();
            SetValue(K.ProxyUsername, username);
            vault.Add(new PasswordCredential(PROXY_RES_ID, username, password));
        }
        catch (Exception ex)
        {
            Logger.Error("Cannot save Proxy credentials");
            Logger.Error(ex);
        }
#else
        Logger.Warn("Proxy credentials storage not yet implemented for non-Windows platforms");
        SetValue(K.ProxyUsername, username);
#endif
    }

    public static JsonSerializerOptions SerializationOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        AllowTrailingCommas = true,
        WriteIndented = true,
    };
}
