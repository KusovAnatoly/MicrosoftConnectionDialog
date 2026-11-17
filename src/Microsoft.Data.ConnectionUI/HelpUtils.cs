//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics;

namespace Microsoft.Data.ConnectionUI;

internal static class HelpUtils
{
    private const int KeyValueNameLength = 1024;

    public static bool IsContextHelpMessage(
        int message,
        IntPtr wParam)
    {
        return message == NativeMethods.WM_SYSCOMMAND
            && ((int)wParam & 0xFFF0) == NativeMethods.SC_CONTEXTHELP;
    }

    /// <summary>
    /// Checks whether the current process is running under WOW64.
    /// </summary>
    public static bool IsWow64()
    {
        if (!Environment.Is64BitOperatingSystem ||
            Environment.OSVersion.Version.Major < 5)
        {
            return false;
        }

        try
        {
            using var process = Process.GetCurrentProcess();

            return NativeMethods.IsWow64Process(
                process.Handle,
                out var isWow64) && isWow64;
        }
        catch (Exception exception)
        {
            Debug.Fail($"Failed in calling IsWow64Process: {exception.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets registry value names from the 64-bit or 32-bit registry view.
    /// </summary>
    public static string[] GetValueNamesWow64(
        string registryKey,
        int ulOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registryKey);

        var result = NativeMethods.RegOpenKeyEx(
            NativeMethods.HKEY_LOCAL_MACHINE,
            registryKey,
            0,
            ulOptions,
            out var keyHandle);

        if (result != 0 || keyHandle == UIntPtr.Zero)
            return [];

        try
        {
            result = NativeMethods.RegQueryInfoKey(
                keyHandle,
                null!,
                IntPtr.Zero,
                IntPtr.Zero,
                out _,
                IntPtr.Zero,
                IntPtr.Zero,
                out var numValues,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (result != 0 || numValues == 0)
                return [];

            var valueNames = new List<string>((int)numValues);

            for (uint index = 0; index < numValues; index++)
            {
                var name = new char[KeyValueNameLength];
                uint size = KeyValueNameLength;

                result = NativeMethods.RegEnumValue(
                    keyHandle,
                    index,
                    name,
                    ref size,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    IntPtr.Zero);

                if (result == 0)
                    valueNames.Add(new string(name, 0, name.Length));
            }

            return [.. valueNames];
        }
        finally
        {
            NativeMethods.RegCloseKey(keyHandle);
        }
    }
}