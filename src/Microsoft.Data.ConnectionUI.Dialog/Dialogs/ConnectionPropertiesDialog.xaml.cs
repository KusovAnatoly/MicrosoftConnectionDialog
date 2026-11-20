using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;

/// <summary>
/// Interaction logic for ConnectionPropertiesDialog.xaml
/// </summary>
internal partial class ConnectionPropertiesDialog : Window
{
    private ConnectionPropertiesDialog(UserControl userControl, DataSource dataSource, DataProvider dataProvider)
    {
        InitializeComponent();
        Grid.SetRow(userControl, 1);
        RootGrid.Children.Add(userControl);
        DataSourceTextBox.Text = $"{dataSource.DisplayName} ({dataProvider.ShortDisplayName})";
    }

    internal static ConnectionPropertiesDialog GetDialog(ControlType type, DataSource dataSource, DataProvider dataProvider)
    {
        UserControl userControl = HelpUtils.GetUserControl(type);
        return new ConnectionPropertiesDialog(userControl, dataSource, dataProvider);
    }

    public new string ShowDialog()
    {
        base.ShowDialog();
        return string.Empty;
    }

    private void ChangeButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
        Owner.ShowDialog();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
        Owner.Close();
    }

    private void OkButon_Click(object sender, RoutedEventArgs e)
    {
        Owner.Close();
    }

    private void TestButton_Click(object sender, RoutedEventArgs e)
    {

    }
}
