//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.Data.ConnectionUI;

internal sealed partial class NativeMethods
{
    private NativeMethods()
    {
    }

    #region Macros

    internal static bool SQL_SUCCEEDED(short rc)
    {
        return (((rc) & (~1)) == 0);
    }

    internal static short LOWORD(int dwValue)
    {
        return (short)(dwValue & 0xffff);
    }

    internal static short HIWORD(int dwValue)
    {
        return (short)((dwValue >> 16) & 0xffff);
    }

    #endregion

    #region Interfaces

    [ComImport]
    [Guid("2206CCB1-19C1-11D1-89E0-00C04FD7A829")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDataInitialize
    {
        void GetDataSource(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            [In, MarshalAs(UnmanagedType.U4)] int dwClsCtx,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszInitializationString,
            [In] ref Guid riid,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppDataSource);

        void GetInitializationString(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pDataSource,
            [In, MarshalAs(UnmanagedType.I1)] bool fIncludePassword,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppwszInitString);

        void Unused_CreateDBInstance();
        void Unused_CreateDBInstanceEx();
        void Unused_LoadStringFromStorage();
        void Unused_WriteStringToStorage();
    }

    [ComImport]
    [Guid("2206CCB0-19C1-11D1-89E0-00C04FD7A829")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDBPromptInitialize
    {
        void PromptDataSource(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            [In] IntPtr hwndParent,
            [In, MarshalAs(UnmanagedType.U4)] int dwPromptOptions,
            [In, MarshalAs(UnmanagedType.U4)] int cSourceTypeFilter,
            [In] IntPtr rgSourceTypeFilter,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszszzProviderFilter,
            [In] ref Guid riid,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppDataSource);

        void Unused_PromptFileName();
    }

    #endregion

    #region Structures

    [StructLayout(LayoutKind.Sequential)]
    internal class HELPINFO
    {
        public int cbSize = Marshal.SizeOf<HELPINFO>();
        public int iContextType;
        public int iCtrlId;
        public IntPtr hItemHandle;
        public int dwContextId;
        public POINT MousePos;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    #endregion

    #region Functions

    [LibraryImport("odbc32.dll")]
    internal static partial short SQLAllocEnv(out IntPtr EnvironmentHandle);

    [LibraryImport("odbc32.dll")]
    internal static partial short SQLAllocConnect(IntPtr EnvironmentHandle, out IntPtr ConnectionHandle);

    [LibraryImport("odbc32.dll", EntryPoint = "SQLDriverConnectW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial short SQLDriverConnect(IntPtr hdbc, IntPtr hwnd, string szConnStrIn, short cbConnStrIn, [Out] char[] szConnStrOut, short cbConnStrOutMax, out short pcbConnStrOut, ushort fDriverCompletion);

    [LibraryImport("odbc32.dll")]
    internal static partial short SQLDisconnect(IntPtr ConnectionHandle);

    [LibraryImport("odbc32.dll")]
    internal static partial short SQLFreeConnect(IntPtr ConnectionHandle);

    [LibraryImport("odbc32.dll")]
    internal static partial short SQLFreeEnv(IntPtr EnvironmentHandle);

    [LibraryImport("odbccp32.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SQLGetInstalledDrivers([Out] char[] lpszBuf, int cbBufMax, ref int pcbBufOut);

    [LibraryImport("odbccp32.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int SQLGetPrivateProfileString(string lpszSection, string lpszEntry, string lpszDefault, [Out] char[] RetBuffer, int cbRetBuffer, string lpszFilename);

    // Used to check if OS is 64 bits
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool pIsWow64);

    [LibraryImport("advapi32.dll", EntryPoint = "RegOpenKeyExW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int RegOpenKeyEx(UIntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out UIntPtr phkResult);

    [LibraryImport("advapi32.dll", EntryPoint = "RegQueryValueExW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int RegQueryValueEx(UIntPtr hKey, string lpValueName, uint lpReserved, ref uint lpType, IntPtr lpData, ref int lpchData);

    [LibraryImport("advapi32.dll", EntryPoint = "RegQueryInfoKeyW")]
    internal static partial int RegQueryInfoKey(UIntPtr hkey, byte[]? lpClass, IntPtr lpcbClass, IntPtr lpReserved, out uint lpcSubKeys, IntPtr lpcbMaxSubKeyLen, IntPtr lpcbMaxClassLen, out uint lpcValues, IntPtr lpcbMaxValueNameLen, IntPtr lpcbMaxValueLen, IntPtr lpcbSecurityDescriptor, IntPtr lpftLastWriteTime);

    [LibraryImport("advapi32.dll", EntryPoint = "RegEnumValueW", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int RegEnumValue(UIntPtr hkey, uint index, [Out] char[] lpValueName, ref uint lpcbValueName, IntPtr reserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

    [LibraryImport("advapi32.dll")]
    internal static partial uint RegCloseKey(UIntPtr hKey);

    internal static readonly UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(((uint)0x80000002));
    internal const int KEY_WOW64_64KEY = 0x0100;
    internal const int KEY_WOW64_32KEY = 0x0200;
    internal const int KEY_QUERY_VALUE = 0x1;

    #endregion

    #region Guids

    internal static Guid IID_IUnknown = new Guid("00000000-0000-0000-c000-000000000046");
    internal static Guid CLSID_DataLinks = new Guid("2206CDB2-19C1-11d1-89E0-00C04FD7A829");
    internal static Guid CLSID_OLEDB_ENUMERATOR = new Guid("C8B522D0-5CF3-11ce-ADE5-00AA0044773D");
    internal static Guid CLSID_MSDASQL_ENUMERATOR = new Guid("C8B522CD-5CF3-11ce-ADE5-00AA0044773D");

    #endregion

    #region Constants

    // HRESULT codes
    internal const int
        DB_E_CANCELED = unchecked((int)0x80040E4E);

    // COM class contexts
    internal const int
        CLSCTX_INPROC_SERVER = 1;

    // Window messages
    internal const int
        WM_SETFOCUS = 0x0007,
        WM_HELP = 0x0053,
        WM_CONTEXTMENU = 0x007B,
        WM_SYSCOMMAND = 0x0112;

    // Window system commands
    internal const int
        SC_CONTEXTHELP = 0xF180;

    // HELPINFO constants
    internal const int
        HELPINFO_WINDOW = 0x0001;

    // OLE DB database source types
    internal const int
        DBSOURCETYPE_DATASOURCE_TDP = 1,
        DBSOURCETYPE_DATASOURCE_MDP = 3;

    // OLE DB Data Links dialog prompt options
    internal const int
        DBPROMPTOPTIONS_PROPERTYSHEET = 0x02,
        DBPROMPTOPTIONS_DISABLE_PROVIDER_SELECTION = 0x10;

    // ODBC Driver prompt options
    internal const ushort
        SQL_DRIVER_PROMPT = 2;

    // ODBC return values
    internal const short
        SQL_NO_DATA = 100;

    #endregion
}