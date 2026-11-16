using System.Globalization;
using System.Windows;

namespace Microsoft.Data.ConnectionUI;

internal sealed class RTLAwareMessageBox
{
    private RTLAwareMessageBox()
    {
    }

    public static MessageBoxResult Show(string caption, string text, MessageBoxImage icon)
    {
        MessageBoxOptions options = 0;
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
        {
            options = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
        }
        return MessageBox.Show(text, caption, MessageBoxButton.OK, icon, MessageBoxResult.OK, options);
    }
}