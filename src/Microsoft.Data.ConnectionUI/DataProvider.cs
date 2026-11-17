//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Data.ConnectionUI;

using Microsoft.Data.ConnectionUI.Resources;
using System.Collections.ObjectModel;

public sealed class DataProvider
{
    private readonly IReadOnlyDictionary<string, string> _dataSourceDescriptions;
    private readonly IReadOnlyDictionary<string, Type> _connectionControlTypes;
    private readonly IReadOnlyDictionary<string, Type> _connectionPropertiesTypes;

    public DataProvider(
        string name,
        string displayName,
        string shortDisplayName,
        string? description = null,
        Type? targetConnectionType = null,
        IDictionary<string, string>? dataSourceDescriptions = null,
        IDictionary<string, Type>? connectionControlTypes = null,
        IDictionary<string, Type>? connectionPropertiesTypes = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(shortDisplayName);

        Name = name;
        DisplayName = displayName;
        ShortDisplayName = shortDisplayName;
        Description = description;
        TargetConnectionType = targetConnectionType;

        _dataSourceDescriptions = Copy(dataSourceDescriptions);
        _connectionControlTypes = Copy(connectionControlTypes);
        _connectionPropertiesTypes = Copy(connectionPropertiesTypes);
    }

    public string Name { get; }

    public string DisplayName { get; }

    public string ShortDisplayName { get; }

    public string? Description { get; }

    public Type? TargetConnectionType { get; }

    private static IReadOnlyDictionary<string, TValue> Copy<TValue>(
        IDictionary<string, TValue>? source)
    {
        var copy = source is null
            ? new Dictionary<string, TValue>()
            : new Dictionary<string, TValue>(source);

        return new ReadOnlyDictionary<string, TValue>(copy);
    }

    public string? GetDescription(DataSource? dataSource = null)
    {
        if (dataSource is not null &&
            _dataSourceDescriptions.TryGetValue(
                dataSource.Name,
                out var description))
        {
            return description;
        }

        return Description;
    }

    public IDataConnectionControl? CreateConnectionControl(
        DataSource? dataSource = null)
    {
        if (!TryGetType(
            _connectionControlTypes,
            dataSource,
            out var controlType))
        {
            return null;
        }

        return Activator.CreateInstance(controlType)
            as IDataConnectionControl;
    }

    public IDataConnectionProperties? CreateConnectionProperties(
        DataSource? dataSource = null)
    {
        if (!TryGetType(
            _connectionPropertiesTypes,
            dataSource,
            out var propertiesType))
        {
            return null;
        }

        return Activator.CreateInstance(propertiesType)
            as IDataConnectionProperties;
    }

    private static bool TryGetType(
        IReadOnlyDictionary<string, Type> types,
        DataSource? dataSource,
        out Type type)
    {
        if (dataSource is not null &&
            types.TryGetValue(dataSource.Name, out type!))
        {
            return true;
        }

        return types.TryGetValue(string.Empty, out type!);
    }

    private static readonly Lazy<DataProvider> SqlDataProviderInstance =
        new(CreateSqlDataProvider);

    private static readonly Lazy<DataProvider> OracleDataProviderInstance =
        new(CreateOracleDataProvider);

    private static readonly Lazy<DataProvider> OleDBDataProviderInstance =
        new(CreateOleDBDataProvider);

    private static readonly Lazy<DataProvider> OdbcDataProviderInstance =
        new(CreateOdbcDataProvider);

    public static DataProvider SqlDataProvider =>
        SqlDataProviderInstance.Value;

    public static DataProvider OracleDataProvider =>
        OracleDataProviderInstance.Value;

    public static DataProvider OleDBDataProvider =>
        OleDBDataProviderInstance.Value;

    public static DataProvider OdbcDataProvider =>
        OdbcDataProviderInstance.Value;

    private static DataProvider CreateSqlDataProvider()
    {
        var descriptions = new Dictionary<string, string>
        {
            [DataSource.MicrosoftSqlServerName] =
                Strings.DataProvider_Sql_DataSource_Description,

            [DataSource.MicrosoftSqlServerFileName] =
                Strings.DataProvider_Sql_FileDataSource_Description
        };

        var controls = new Dictionary<string, Type>
        {
            [DataSource.MicrosoftSqlServerName] =
                typeof(ISqlServerConnectionControl),

            [DataSource.MicrosoftSqlServerFileName] =
                typeof(ISqlServerFileConnectionControl),

            [string.Empty] =
                typeof(ISqlServerConnectionControl)
        };

        var properties = new Dictionary<string, Type>
        {
            [DataSource.MicrosoftSqlServerFileName] =
                typeof(SqlFileConnectionProperties),

            [string.Empty] =
                typeof(SqlConnectionProperties)
        };

        return new DataProvider(
            "Microsoft.Data.SqlClient",
            Strings.DataProvider_Sql,
            Strings.DataProvider_Sql_Short,
            Strings.DataProvider_Sql_Description,
            typeof(Microsoft.Data.SqlClient.SqlConnection),
            descriptions,
            controls,
            properties);
    }

    private static DataProvider CreateOracleDataProvider()
    {
        var descriptions = new Dictionary<string, string>
        {
            [DataSource.OracleName] =
                Strings.DataProvider_Oracle_DataSource_Description
        };

        var controls = new Dictionary<string, Type>
        {
            [string.Empty] =
                typeof(IOracleConnectionControl)
        };

        var properties = new Dictionary<string, Type>
        {
            [string.Empty] =
                typeof(OracleConnectionProperties)
        };

        return new DataProvider(
            "Oracle.ManagedDataAccess.Client",
            Strings.DataProvider_Oracle,
            Strings.DataProvider_Oracle_Short,
            Strings.DataProvider_Oracle_Description,
            typeof(Oracle.ManagedDataAccess.Client.OracleConnection),
            descriptions,
            controls,
            properties);
    }

    private static DataProvider CreateOleDBDataProvider()
    {
        var descriptions = new Dictionary<string, string>
        {
            [DataSource.MicrosoftSqlServerName] =
                Strings.DataProvider_OleDB_SqlDataSource_Description,

            [DataSource.OracleName] =
                Strings.DataProvider_OleDB_OracleDataSource_Description,

            [DataSource.MicrosoftAccessName] =
                Strings.DataProvider_OleDB_AccessDataSource_Description
        };

        var controls = new Dictionary<string, Type>
        {
            [DataSource.MicrosoftSqlServerName] =
                typeof(ISqlServerConnectionControl),

            [DataSource.OracleName] =
                typeof(IOracleConnectionControl),

            [DataSource.MicrosoftAccessName] =
                typeof(IAccessConnectionControl),

            [string.Empty] =
                typeof(IOleDBConnectionControl)
        };

        var properties = new Dictionary<string, Type>
        {
            [DataSource.MicrosoftSqlServerName] =
                typeof(OleDBSqlConnectionProperties),

            [DataSource.OracleName] =
                typeof(OleDBOracleConnectionProperties),

            [DataSource.MicrosoftAccessName] =
                typeof(OleDBAccessConnectionProperties),

            [string.Empty] =
                typeof(OleDBConnectionProperties)
        };

        return new DataProvider(
            "System.Data.OleDb",
            Strings.DataProvider_OleDB,
            Strings.DataProvider_OleDB_Short,
            Strings.DataProvider_OleDB_Description,
            typeof(System.Data.OleDb.OleDbConnection),
            descriptions,
            controls,
            properties);
    }

    private static DataProvider CreateOdbcDataProvider()
    {
        var descriptions = new Dictionary<string, string>
        {
            [DataSource.OdbcDsnName] =
                Strings.DataProvider_Odbc_DataSource_Description
        };

        var controls = new Dictionary<string, Type>
        {
            [string.Empty] =
                typeof(IOdbcConnectionControl)
        };

        var properties = new Dictionary<string, Type>
        {
            [string.Empty] =
                typeof(OdbcConnectionProperties)
        };

        return new DataProvider(
            "System.Data.Odbc",
            Strings.DataProvider_Odbc,
            Strings.DataProvider_Odbc_Short,
            Strings.DataProvider_Odbc_Description,
            typeof(System.Data.Odbc.OdbcConnection),
            descriptions,
            controls,
            properties);
    }
}
