using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Data.ConnectionUI.Dialog.ViewModels;

internal partial class DataConnectionViewModel : ObservableObject
{
    public ObservableCollection<DataSource> DataSources { get; set; }

    [ObservableProperty]
    public partial DataSource? SelectedDataSource { get; set; }

    [ObservableProperty]
    public partial DataProvider? SelectedDataProvider { get; set; }

    [MemberNotNullWhen(true, nameof(ConnectionString))]
    public bool ConnectionStringInitialized => !string.IsNullOrWhiteSpace(ConnectionString);

    public string? ConnectionString { get; private set; }

    public DataConnectionViewModel(DataConnectionConfiguration.IDataConnectionConfiguration config)
    {
        DataSources = new ObservableCollection<DataSource>(config.GetDataSources());
        SelectedDataSource = DataSources.FirstOrDefault();
    }

    partial void OnSelectedDataSourceChanged(DataSource? value)
    {
        SelectedDataProvider = value?.Providers.FirstOrDefault();
    }

    private bool CanOpenConnectionProperties() => SelectedDataProvider is not null;

    [RelayCommand(CanExecute = nameof(CanOpenConnectionProperties))]
    private void OpenConnectionProperties()
    {
        if (SelectedDataProvider is not null)
        {
            var connectionControl = SelectedDataProvider.GetConnectionControl(SelectedDataSource);
            ConnectionString = ConnectionPropertiesDialog.ShowDialog(connectionControl);
        }
    }

    [RelayCommand]
    private void About()
    {
        RTLAwareMessageBox.Show("About", "Modified version by Karen Payne. Updated to WPF .NET 11 by Anatoly Kusov.", System.Windows.MessageBoxImage.None);
    }
}
