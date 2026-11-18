//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Data.ConnectionUI.Resources;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;

namespace Microsoft.Data.ConnectionUI;

public class SqlConnectionProperties : AdoDotNetConnectionProperties
{
    public SqlConnectionProperties()
        : base("Microsoft.Data.SqlClient")
    {
        LocalReset();
    }

    public override void Reset()
    {
        base.Reset();
        LocalReset();
    }

    public override bool IsComplete
    {
        get
        {
            if (ConnectionStringBuilder["Data Source"] is not string { Length: > 0 })
            {
                return false;
            }

            if (ConnectionStringBuilder["Integrated Security"] is not true &&
                ConnectionStringBuilder["User ID"] is not string { Length: > 0 })
            {
                return false;
            }

            return true;
        }
    }

    public override void Test()
    {
        if (ConnectionStringBuilder["Data Source"] is not string { Length: > 0 })
        {
            throw new InvalidOperationException(Strings.SqlConnectionProperties_MustSpecifyDataSource);
        }

        var database = ConnectionStringBuilder["Initial Catalog"] as string;

        try
        {
            base.Test();
        }
        catch (SqlException e) when (e.Number == SqlError_CannotOpenDatabase && database is { Length: > 0 })
        {
            throw new InvalidOperationException(Strings.SqlConnectionProperties_CannotTestNonExistentDatabase, e);
        }
    }

    protected override PropertyDescriptor? DefaultProperty => GetProperties(Array.Empty<Attribute>())["DataSource"];

    protected override string ToTestString()
    {
        var savedPooling = ConnectionStringBuilder["Pooling"] is true;
        var wasDefault = !ConnectionStringBuilder.ShouldSerialize("Pooling");

        try
        {
            ConnectionStringBuilder["Pooling"] = false;
            return ConnectionStringBuilder.ConnectionString;
        }
        finally
        {
            if (wasDefault)
            {
                ConnectionStringBuilder.Remove("Pooling");
            }
            else
            {
                ConnectionStringBuilder["Pooling"] = savedPooling;
            }
        }
    }

    protected override void Inspect(DbConnection connection)
    {
        if (connection.ServerVersion.StartsWith("07", StringComparison.Ordinal) ||
            connection.ServerVersion.StartsWith("08", StringComparison.Ordinal))
        {
            throw new NotSupportedException(Strings.SqlConnectionProperties_UnsupportedSqlVersion);
        }
    }

    private void LocalReset()
    {
        // We always start with integrated security turned on
        this["Integrated Security"] = true;
    }

    private const int SqlError_CannotOpenDatabase = 4060;
}

public class SqlFileConnectionProperties : SqlConnectionProperties
{
    public SqlFileConnectionProperties()
        : this(null)
    {
    }

    public SqlFileConnectionProperties(string? defaultInstanceName)
    {
        _defaultDataSource = ".";
        if (defaultInstanceName is { Length: > 0 })
        {
            _defaultDataSource += "\\" + defaultInstanceName;
        }
        else
        {
            var conv = new DataSourceConverter();
            TypeConverter.StandardValuesCollection coll = conv.GetStandardValues(null);
            if (coll.Count > 0)
            {
                _defaultDataSource = coll[0] as string ?? throw new Exception("Failed to set Default Data Source");
            }
        }
        LocalReset();
    }

    public override void Reset()
    {
        base.Reset();
        LocalReset();
    }

    public override bool IsComplete => base.IsComplete && ConnectionStringBuilder["AttachDbFilename"] is string { Length: > 0 };

    public override void Test()
    {
        if (ConnectionStringBuilder["AttachDbFilename"] is not string attachDbFilename || string.IsNullOrWhiteSpace(attachDbFilename))
        {
            throw new InvalidOperationException(Strings.SqlFileConnectionProperties_NoFileSpecified);
        }

        try
        {
            string fullPath = Path.GetFullPath(attachDbFilename);
            ConnectionStringBuilder["AttachDbFilename"] = fullPath;

            if (!File.Exists(fullPath))
            {
                throw new InvalidOperationException(Strings.SqlFileConnectionProperties_CannotTestNonExistentMdf);
            }

            base.Test();
        }
        catch (SqlException e) when (e.Number == -2) // Timeout
        {
            string message = $"{e.Errors[0].Message}{Environment.NewLine}{Strings.SqlFileConnectionProperties_TimeoutReasons}";
            throw new TimeoutException(message, e);
        }
        finally
        {
            ConnectionStringBuilder["AttachDbFilename"] = attachDbFilename;
        }
    }

    protected override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
    {
        PropertyDescriptorCollection descriptors = base.GetProperties(attributes);

        if (descriptors.Find("DataSource", ignoreCase: true) is not { } dataSourceDescriptor)
        {
            return descriptors;
        }

        int index = descriptors.IndexOf(dataSourceDescriptor);
        var descriptorArray = new PropertyDescriptor[descriptors.Count];
        descriptors.CopyTo(descriptorArray, 0);

        descriptorArray[index] = new DynamicPropertyDescriptor(
            dataSourceDescriptor,
            new TypeConverterAttribute(typeof(DataSourceConverter)))
        {
            CanResetValueHandler = CanResetDataSource,
            ResetValueHandler = ResetDataSource
        };

        return new PropertyDescriptorCollection(descriptorArray, readOnly: true);
    }

    private void LocalReset()
    {
        this["Data Source"] = _defaultDataSource;
        this["User Instance"] = true;
        this["Connection Timeout"] = 30;
    }

    private bool CanResetDataSource(object component) =>
        this["Data Source"] is not string dataSource || !string.Equals(dataSource, _defaultDataSource, StringComparison.OrdinalIgnoreCase);

    private void ResetDataSource(object component)
    {
        this["Data Source"] = _defaultDataSource;
    }

    private class DataSourceConverter : StringConverter
    {
        public DataSourceConverter()
        {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            if (_standardValues is not null)
            {
                return _standardValues;
            }

            string[]? dataSources = null;

            if (HelpUtils.IsWow64())
            {
                var dataSourceList = new List<string>();
                // Read 64 registry key of SQL Server Instances Names. 
                dataSourceList.AddRange(
                    HelpUtils.GetValueNamesWow64(
                        "SOFTWARE\\Microsoft\\Microsoft SQL Server\\Instance Names\\SQL",
                        NativeMethods.KEY_WOW64_64KEY | NativeMethods.KEY_QUERY_VALUE));
                // Read 32 registry key of SQL Server Instances Names. 
                dataSourceList.AddRange(
                    HelpUtils.GetValueNamesWow64(
                        "SOFTWARE\\Microsoft\\Microsoft SQL Server\\Instance Names\\SQL",
                        NativeMethods.KEY_WOW64_32KEY | NativeMethods.KEY_QUERY_VALUE));
                dataSources = dataSourceList.ToArray();
            }
            else
            {
                // Look in the registry for all local SQL Server instances
                Win32.RegistryKey? key = Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft SQL Server\\Instance Names\\SQL");
                if (key is not null)
                {
                    using (key)
                    {
                        dataSources = key.GetValueNames();
                    }
                }
            }

            if (dataSources is not null)
            {
                for (int i = 0; i < dataSources.Length; i++)
                {
                    if (string.Equals(dataSources[i], "MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
                    {
                        dataSources[i] = ".";
                    }
                    else
                    {
                        dataSources[i] = ".\\" + dataSources[i];
                    }
                }
                _standardValues = new StandardValuesCollection(dataSources);
            }
            else
            {
                _standardValues = new StandardValuesCollection(new string[0]);
            }

            return _standardValues;
        }

        private StandardValuesCollection? _standardValues;
    }

    private string _defaultDataSource;
}
