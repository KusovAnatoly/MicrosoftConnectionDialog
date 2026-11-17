using Microsoft.Data.ConnectionUI.Dialog.ViewModels;
using System.Windows;

namespace Microsoft.Data.ConnectionUI.Dialog;

public partial class DataConnectionDialog : Window
{
    public DataConnectionViewModel ViewModel { get; private set; }
    private DataConnectionDialog(DataConnectionViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    public static string ShowDialog(DataConnectionConfiguration.IDataConnectionConfiguration config)
    {
        var viewModel = new DataConnectionViewModel(config);
        var window = new DataConnectionDialog(viewModel);
        bool? result = window.ShowDialog();
        return result == true ? "" : "";
    }
}