using Microsoft.Data.ConnectionUI.Dialog.ViewModels;
using Microsoft.Data.DataConnectionConfiguration;
using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog;

internal partial class DataConnectionDialog : Window
{
    internal DataConnectionViewModel ViewModel { get; private set; }
    private DataConnectionDialog(DataConnectionViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    internal static string? ShowDialog(IDataConnectionConfiguration config)
    {
        var viewModel = new DataConnectionViewModel(config);
        var window = new DataConnectionDialog(viewModel);
        return window.ShowDialog();
    }

    private new string? ShowDialog()
    {
        base.ShowDialog();
        if (ViewModel.ConnectionStringInitialized)
        {
            return ViewModel.ConnectionString;
        }
        return null;
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Close();

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedDataProvider is not null && ViewModel.SelectedDataSource is not null)
        {
            Hide();
            var connectionControl = ViewModel.SelectedDataProvider.GetConnectionControl(ViewModel.SelectedDataSource);
            var window =
                ConnectionPropertiesDialog.GetDialog(connectionControl, ViewModel.SelectedDataSource, ViewModel.SelectedDataProvider);
            window.Owner = this;
            ViewModel.ConnectionString = window.ShowDialog();
        }
    }
}