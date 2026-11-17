using Microsoft.Data.ConnectionUI;

namespace Microsoft.Data.DataConnectionConfiguration;

public interface IDataConnectionConfiguration
{
    string? GetSelectedSource();
    void SaveSelectedSource(string provider);

    string? GetSelectedProvider();
    void SaveSelectedProvider(string provider);

    DataSource GetUnspecified();
    ICollection<DataSource> GetDataSources();
    ICollection<DataProvider> GetDataProviders();

    void SaveConfiguration(DataSource dataSource, DataProvider dataProvider);
}
