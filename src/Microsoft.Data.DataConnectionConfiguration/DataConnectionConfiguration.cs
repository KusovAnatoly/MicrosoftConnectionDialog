using Microsoft.Data.ConnectionUI;
using System.Xml.Linq;

namespace Microsoft.Data.DataConnectionConfiguration;

public class DataConnectionConfiguration : IDataConnectionConfiguration
{
    private const string ConfigFileName = @"DataConnection.xml";
    private readonly string _fullFilePath;
    private readonly XDocument _xDoc;

    public XElement RootElement { get; set; }

    private DataSource? _unspecifiedDataSource;

    // Available data sources:
    private IDictionary<string, DataSource> _dataSources = new Dictionary<string, DataSource>();

    // Available data providers: 
    private IDictionary<string, DataProvider> _dataProviders = new Dictionary<string, DataProvider>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="path">Configuration file path.</param>
    public DataConnectionConfiguration(string? path = null)
    {
        if (!string.IsNullOrEmpty(path))
        {
            _fullFilePath = Path.GetFullPath(Path.Combine(path, ConfigFileName));
        }
        else
        {
            _fullFilePath = Path.Combine(Environment.CurrentDirectory, ConfigFileName);
        }

        if (!string.IsNullOrEmpty(_fullFilePath) && File.Exists(_fullFilePath))
        {
            _xDoc = XDocument.Load(_fullFilePath);
        }
        else
        {
            _xDoc = new XDocument();
            _xDoc.Add(new XElement("ConnectionDialog", new XElement("DataSourceSelection")));
        }

        if (_xDoc.Root is not XElement root)
        {
            throw new InvalidOperationException("Document has no root element.");
        }

        RootElement = root;
    }

    public DataSource GetUnspecified()
    {
        _unspecifiedDataSource = DataSource.CreateUnspecified();
        _unspecifiedDataSource.AddProvider(DataProvider.SqlDataProvider);
        _unspecifiedDataSource.AddProvider(DataProvider.OracleDataProvider);
        _unspecifiedDataSource.AddProvider(DataProvider.OleDBDataProvider);
        _unspecifiedDataSource.AddProvider(DataProvider.OdbcDataProvider);
        _dataSources.Add(_unspecifiedDataSource.DisplayName, _unspecifiedDataSource);
        return _unspecifiedDataSource;
    }

    public ICollection<DataSource> GetDataSources()
    {
        var dataSource = GetUnspecified();
        var dataSources = new List<DataSource>()
        {
            DataSource.SqlDataSource,DataSource.SqlFileDataSource,
            DataSource.OracleDataSource,
            DataSource.AccessDataSource,
            DataSource.OdbcDataSource,
            dataSource
        };

        _dataSources.Add(DataSource.SqlDataSource.Name, DataSource.SqlDataSource);
        _dataSources.Add(DataSource.SqlFileDataSource.Name, DataSource.SqlFileDataSource);
        _dataSources.Add(DataSource.OracleDataSource.Name, DataSource.OracleDataSource);
        _dataSources.Add(DataSource.AccessDataSource.Name, DataSource.AccessDataSource);
        _dataSources.Add(DataSource.OdbcDataSource.Name, DataSource.OdbcDataSource);

        _dataProviders.Add(DataProvider.SqlDataProvider.Name, DataProvider.SqlDataProvider);
        _dataProviders.Add(DataProvider.OracleDataProvider.Name, DataProvider.OracleDataProvider);
        _dataProviders.Add(DataProvider.OleDBDataProvider.Name, DataProvider.OleDBDataProvider);
        _dataProviders.Add(DataProvider.OdbcDataProvider.Name, DataProvider.OdbcDataProvider);

        return dataSources;
    }

    public ICollection<DataProvider> GetDataProviders()
    {
        var dataProviders = new List<DataProvider>()
        {
            DataProvider.SqlDataProvider,
            DataProvider.OracleDataProvider,
            DataProvider.OleDBDataProvider,
            DataProvider.OdbcDataProvider
        };

        _dataProviders.Add(DataProvider.SqlDataProvider.Name, DataProvider.SqlDataProvider);
        _dataProviders.Add(DataProvider.OracleDataProvider.Name, DataProvider.OracleDataProvider);
        _dataProviders.Add(DataProvider.OleDBDataProvider.Name, DataProvider.OleDBDataProvider);
        _dataProviders.Add(DataProvider.OdbcDataProvider.Name, DataProvider.OdbcDataProvider);

        return dataProviders;
    }

    public void SaveConfiguration(DataSource dataSource, DataProvider dataProvider)
    {
        if (dataSource is not null)
        {
            if (dataSource == _unspecifiedDataSource)
            {
                SaveSelectedSource(dataSource.DisplayName);
            }
            else
            {
                SaveSelectedSource(dataSource.Name);
            }
        }
        if (dataProvider is not null)
        {
            SaveSelectedProvider(dataProvider.Name);
        }

        _xDoc.Save(_fullFilePath);
    }

    public string? GetSelectedSource()
    {
        XElement xElem = RootElement.Element("DataSourceSelection") ??
                throw new InvalidOperationException("RootElement has no DataSourceSelection element.");

        XElement? sourceElem = xElem.Element("SelectedSource");

        if (sourceElem is null)
        {
            return null;
        }

        return sourceElem.Value;
    }

    public string? GetSelectedProvider()
    {
        XElement xElem = RootElement.Element("DataSourceSelection") ??
                throw new InvalidOperationException("RootElement has no DataSourceSelection element.");

        XElement? providerElem = xElem.Element("SelectedProvider");

        if (providerElem is null)
        {
            return null;
        }

        return providerElem.Value;
    }

    public void SaveSelectedSource(string source)
    {
        if (!string.IsNullOrEmpty(source))
        {
            XElement xElem = RootElement.Element("DataSourceSelection") ??
                throw new InvalidOperationException("RootElement has no DataSourceSelection element.");

            XElement sourceElem = xElem.Element("SelectedSource") ??
                throw new InvalidOperationException("DataSourceSelection has no SelectedSource element.");

            if (sourceElem is not null)
            {
                sourceElem.Value = source;
            }
            else
            {
                xElem.Add(new XElement("SelectedSource", source));
            }
        }
    }

    public void SaveSelectedProvider(string provider)
    {
        if (!string.IsNullOrEmpty(provider))
        {
            XElement xElem = RootElement.Element("DataSourceSelection") ??
                    throw new InvalidOperationException("RootElement has no DataSourceSelection element.");

            XElement providerElem = xElem.Element("SelectedSource") ??
                throw new InvalidOperationException("DataSourceSelection has no SelectedSource element.");

            if (providerElem != null)
            {
                providerElem.Value = provider;
            }
            else
            {
                xElem.Add(new XElement("SelectedProvider", provider));
            }
        }
    }
}