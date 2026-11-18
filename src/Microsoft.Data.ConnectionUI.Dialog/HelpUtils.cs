//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Data.ConnectionUI.Dialog.UserControls;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.Data.ConnectionUI;

internal static class HelpUtils
{

    public static bool IsContextHelpMessage(
        int message,
        IntPtr wParam)
    {
        return message == NativeMethods.WM_SYSCOMMAND
            && ((int)wParam & 0xFFF0) == NativeMethods.SC_CONTEXTHELP;
    }

    /// <summary>
    /// Registers a WPF contentControl hook to translate context-help messages
    /// into WM_HELP messages.
    /// </summary>
    public static void AttachContextHelp(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.SourceInitialized += (_, _) =>
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            if (HwndSource.FromHwnd(hwnd) is not { } source)
                return;

            source.AddHook((hwnd, message, wParam, lParam, ref handled) =>
            {
                if (!IsContextHelpMessage(message, wParam))
                    return IntPtr.Zero;

                var activeElement = GetActiveElement(window);

                if (activeElement is null)
                    return IntPtr.Zero;

                var targetHwnd = GetHwnd(activeElement);

                if (targetHwnd == IntPtr.Zero)
                    targetHwnd = hwnd;

                var helpInfo = new NativeMethods.HELPINFO
                {
                    iContextType = NativeMethods.HELPINFO_WINDOW,
                    iCtrlId = 0,
                    hItemHandle = targetHwnd,
                    dwContextId = 0
                };

                helpInfo.MousePos.x =
                    (int)NativeMethods.LOWORD(lParam.ToInt32());

                helpInfo.MousePos.y =
                    (int)NativeMethods.HIWORD(lParam.ToInt32());

                var memory = Marshal.AllocHGlobal(
                    Marshal.SizeOf<NativeMethods.HELPINFO>());

                try
                {
                    Marshal.StructureToPtr(helpInfo, memory, false);

                    var helpMessage = new MSG
                    {
                        hwnd = targetHwnd,
                        message = NativeMethods.WM_HELP,
                        wParam = IntPtr.Zero,
                        lParam = memory
                    };

                    handled = true;

                    return IntPtr.Zero;
                }
                finally
                {
                    Marshal.FreeHGlobal(memory);
                }
            });
        };
    }

    /// <summary>
    /// Gets the currently focused WPF element.
    /// </summary>
    public static IInputElement? GetActiveElement(ContentControl contentControl)
    {
        ArgumentNullException.ThrowIfNull(contentControl);

        return Keyboard.FocusedElement
            ?? FocusManager.GetFocusedElement(contentControl);
    }

    private static IntPtr GetHwnd(IInputElement element)
    {
        if (element is not Visual visual)
            return IntPtr.Zero;

        var source = PresentationSource.FromVisual(visual) as HwndSource;

        return source?.Handle ?? IntPtr.Zero;
    }

    public static UserControl GetUserControl(ControlType type)
    {
        return type switch
        {
            ControlType.Access => new AccessConnectionUserControl(),
            ControlType.OleDB => new OleDBConnectionUserControl(),
            ControlType.Odbc => new OdbcConnectionUserControl(),
            ControlType.Oracle => new OracleConnectionUserControl(),
            ControlType.SqlServer => new SqlServerConnectionUserControl(),
            ControlType.SqlServerFile => new SqlServerFileConnectionUserControl(),
            _ => new OdbcConnectionUserControl()
        };
    }
}