using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;
/// <summary>
/// Interaction logic for AddPropertyDialog.xaml
/// </summary>
internal partial class AddPropertyDialog : Window
{
    internal AddPropertyDialog()
    {
        InitializeComponent();
    }

    internal string PropertyName => PropertyTextBox.Text;
        
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void PropertyTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        OkButton.IsEnabled = (PropertyName.Trim().Length > 0);
    }
}
