using System;
using Avalonia.Controls;

namespace UniGetUI.Extensions;

public static class WindowExtensions
{
    /// <summary>
    /// Gets the native window handle for the Avalonia window.
    /// This is used for interop with Windows-specific APIs.
    /// </summary>
    public static IntPtr GetWindowHandle(this Window window)
    {
        if (window.TryGetPlatformHandle()?.Handle is IntPtr handle)
        {
            return handle;
        }
        
        throw new InvalidOperationException("Unable to get platform window handle");
    }
}
