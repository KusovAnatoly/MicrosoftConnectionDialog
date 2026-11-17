using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Data.ConnectionUI.Dialog;
/// <summary>
/// Interaction logic for AddPropertyDialog.xaml
/// </summary>
public partial class AddPropertyDialog : Window
{
    public AddPropertyDialog()
    {
        InitializeComponent();
    }

    public string PropertyName => PropertyTextBox.Text;
        
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
