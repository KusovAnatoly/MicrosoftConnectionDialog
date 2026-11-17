using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;
/// <summary>
/// Interaction logic for ConnectionPropertiesDialog.xaml
/// </summary>
internal partial class ConnectionPropertiesDialog : Window
{
    private ConnectionPropertiesDialog()
    {
        InitializeComponent();
    }

    internal static string ShowDialog(IDataConnectionControl control)
    {
        UserControl userControl = HelpUtils.GetUserControl(control);
        var dialog = new ConnectionPropertiesDialog()
        {
            Content = control,
        };
        return ((IDataConnectionControl)dialog.Content).GetConnection();
    }
}
