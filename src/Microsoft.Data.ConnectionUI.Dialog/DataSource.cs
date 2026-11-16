//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Data.ConnectionUI.Dialog.Resources;

namespace Microsoft.Data.ConnectionUI;


using System.Collections.ObjectModel;

public sealed class DataSource
{
    public const string MicrosoftSqlServerFileName = "MicrosoftSqlServerFile";

    internal const string MicrosoftSqlServerName = "MicrosoftSqlServer";
    internal const string OracleName = "Oracle";
    internal const string MicrosoftAccessName = "MicrosoftAccess";
    internal const string OdbcDsnName = "OdbcDsn";

    private readonly List<DataProvider> _providers = [];
    private readonly ReadOnlyCollection<DataProvider> _readOnlyProviders;

    private DataProvider? _defaultProvider;

    private DataSource()
        : this(string.Empty, Strings.DataSource_UnspecifiedDisplayName)
    {
    }

    public DataSource(string name, string? displayName)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        Name = name;
        DisplayName = displayName ?? name;

        _readOnlyProviders = _providers.AsReadOnly();
    }

    public string Name { get; }

    public string DisplayName { get; }

    public IReadOnlyList<DataProvider> Providers => _readOnlyProviders;

    public DataProvider? DefaultProvider
    {
        get => _defaultProvider ?? _providers.FirstOrDefault();

        set
        {
            if (value is not null && !_providers.Contains(value))
            {
                throw new InvalidOperationException(
                    Strings.DataSource_DataProviderNotFound);
            }

            if (_providers.Count == 1 && _defaultProvider != value)
            {
                throw new InvalidOperationException(
                    Strings.DataSource_CannotChangeSingleDataProvider);
            }

            _defaultProvider = value;
        }
    }

    private static readonly Lazy<DataSource> SqlDataSourceInstance =
        new(CreateSqlDataSource);

    private static readonly Lazy<DataSource> SqlFileDataSourceInstance =
        new(CreateSqlFileDataSource);

    private static readonly Lazy<DataSource> OracleDataSourceInstance =
        new(CreateOracleDataSource);

    private static readonly Lazy<DataSource> AccessDataSourceInstance =
        new(CreateAccessDataSource);

    private static readonly Lazy<DataSource> OdbcDataSourceInstance =
        new(CreateOdbcDataSource);

    public static DataSource SqlDataSource =>
        SqlDataSourceInstance.Value;

    public static DataSource SqlFileDataSource =>
        SqlFileDataSourceInstance.Value;

    public static DataSource OracleDataSource =>
        OracleDataSourceInstance.Value;

    public static DataSource AccessDataSource =>
        AccessDataSourceInstance.Value;

    public static DataSource OdbcDataSource =>
        OdbcDataSourceInstance.Value;

    private static DataSource CreateSqlDataSource()
    {
        var source = new DataSource(
            MicrosoftSqlServerName,
            Strings.DataSource_MicrosoftSqlServer);

        source.AddProvider(DataProvider.SqlDataProvider);
        source.AddProvider(DataProvider.OleDBDataProvider);

        source._defaultProvider = DataProvider.SqlDataProvider;

        return source;
    }

    private static DataSource CreateSqlFileDataSource()
    {
        var source = new DataSource(
            MicrosoftSqlServerFileName,
            Strings.DataSource_MicrosoftSqlServerFile);

        source.AddProvider(DataProvider.SqlDataProvider);

        return source;
    }

    private static DataSource CreateOracleDataSource()
    {
        var source = new DataSource(
            OracleName,
            Strings.DataSource_Oracle);

        source.AddProvider(DataProvider.OracleDataProvider);
        source.AddProvider(DataProvider.OleDBDataProvider);

        source._defaultProvider = DataProvider.OracleDataProvider;

        return source;
    }

    private static DataSource CreateAccessDataSource()
    {
        var source = new DataSource(
            MicrosoftAccessName,
            Strings.DataSource_MicrosoftAccess);

        source.AddProvider(DataProvider.OleDBDataProvider);

        return source;
    }

    private static DataSource CreateOdbcDataSource()
    {
        var source = new DataSource(
            OdbcDsnName,
            Strings.DataSource_MicrosoftOdbcDsn);

        source.AddProvider(DataProvider.OdbcDataProvider);

        return source;
    }

    internal void AddProvider(DataProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (!_providers.Contains(provider))
        {
            _providers.Add(provider);
        }
    }

    internal bool RemoveProvider(DataProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var removed = _providers.Remove(provider);

        if (removed && _defaultProvider == provider)
        {
            _defaultProvider = null;
        }

        return removed;
    }

    internal void ClearProviders()
    {
        _providers.Clear();
        _defaultProvider = null;
    }

    internal static void AddProviders(DataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);

        dataSource.AddProvider(DataProvider.SqlDataProvider);
        dataSource.AddProvider(DataProvider.OracleDataProvider);
        dataSource.AddProvider(DataProvider.OleDBDataProvider);
        dataSource.AddProvider(DataProvider.OdbcDataProvider);
    }

    internal static DataSource CreateUnspecified() => new();
}
