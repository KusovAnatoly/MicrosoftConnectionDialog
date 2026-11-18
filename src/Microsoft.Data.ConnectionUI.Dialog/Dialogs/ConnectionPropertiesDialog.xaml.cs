using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;

/// <summary>
/// Interaction logic for ConnectionPropertiesDialog.xaml
/// </summary>
internal partial class ConnectionPropertiesDialog : Window
{
    private ConnectionPropertiesDialog(UserControl userControl)
    {
        InitializeComponent();
        Grid.SetRow(userControl, 1);
        RootGrid.Children.Add(userControl);
    }

    internal static string ShowDialog(ControlType type)
    {
        UserControl userControl = HelpUtils.GetUserControl(type);
        var dialog = new ConnectionPropertiesDialog(userControl);
        dialog.ShowDialog();
        return ((IDataConnectionControl)dialog.Content).GetConnectionString();
    }

    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OkButon_Click(object sender, RoutedEventArgs e)
    {

    }

    private void TestButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
