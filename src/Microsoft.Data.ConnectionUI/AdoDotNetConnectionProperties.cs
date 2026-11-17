//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Data.ConnectionUI.Resources;
using System.ComponentModel;
using System.Data.Common;

namespace Microsoft.Data.ConnectionUI;

public class AdoDotNetConnectionProperties : IDataConnectionProperties, ICustomTypeDescriptor
{
    private readonly string _providerName;
    private readonly DbProviderFactory _providerFactory;
    private readonly DbConnectionStringBuilder _connectionStringBuilder;

    public AdoDotNetConnectionProperties(string providerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(providerName);

        _providerName = providerName;

        _providerFactory = DbProviderFactories.GetFactory(providerName);

        _connectionStringBuilder = _providerFactory.CreateConnectionStringBuilder()
            ?? throw new InvalidOperationException(
                $"The provider '{providerName}' does not support connection string builders.");

        _connectionStringBuilder.BrowsableConnectionString = false;
    }

    public virtual void Reset()
    {
        _connectionStringBuilder.Clear();
        OnPropertyChanged(EventArgs.Empty);
    }

    public virtual void Parse(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        _connectionStringBuilder.ConnectionString = connectionString;
        OnPropertyChanged(EventArgs.Empty);
    }

    public virtual bool IsExtensible => !_connectionStringBuilder.IsFixedSize;

    public virtual void Add(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        if (!_connectionStringBuilder.ContainsKey(propertyName))
        {
            _connectionStringBuilder.Add(propertyName, string.Empty);
            OnPropertyChanged(EventArgs.Empty);
        }
    }

    public virtual bool Contains(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        return _connectionStringBuilder.ContainsKey(propertyName);
    }

    public virtual object this[string propertyName]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName);

            if (!_connectionStringBuilder.TryGetValue(propertyName, out object? value))
            {
                throw new ArgumentOutOfRangeException(propertyName);
            }

            if (_connectionStringBuilder.ShouldSerialize(propertyName))
            {
                return value;
            }

            return value ?? DBNull.Value;
        }
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName);

            if (value == DBNull.Value)
            {
                Reset(propertyName);
                return;
            }

            _connectionStringBuilder[propertyName] = value!;

            // Remove values equal to their provider-defined defaults.
            if (!_connectionStringBuilder.ShouldSerialize(propertyName))
            {
                _connectionStringBuilder.Remove(propertyName);
            }

            OnPropertyChanged(EventArgs.Empty);
        }
    }

    public virtual void Remove(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        if (_connectionStringBuilder.Remove(propertyName))
        {
            OnPropertyChanged(EventArgs.Empty);
        }
    }

    public event EventHandler? PropertyChanged;

    public virtual void Reset(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        Remove(propertyName);
    }

    public virtual bool IsComplete => true;

    public virtual void Test()
    {
        var testString = ToTestString();

        if (string.IsNullOrEmpty(testString))
        {
            throw new InvalidOperationException(
                Strings.AdoDotNetConnectionProperties_NoProperties);
        }

        using var connection = _providerFactory.CreateConnection()
            ?? throw new InvalidOperationException(
                $"The provider '{_providerName}' could not create a connection.");

        connection.ConnectionString = testString;
        connection.Open();

        Inspect(connection);
    }

    public override string ToString() => ToFullString();

    public virtual string ToFullString()
    {
        return _connectionStringBuilder.ConnectionString;
    }

    public virtual string ToDisplayString()
    {
        // Work on a copy so displaying the connection string never
        // temporarily modifies the actual connection properties.
        var displayBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = _connectionStringBuilder.ConnectionString
        };

        var sensitiveProperties = GetProperties(
            [PasswordPropertyTextAttribute.Yes]);

        foreach (PropertyDescriptor property in sensitiveProperties)
        {
            displayBuilder.Remove(property.Name);
        }

        return displayBuilder.ConnectionString;
    }

    public DbConnectionStringBuilder ConnectionStringBuilder =>
        _connectionStringBuilder;

    protected virtual PropertyDescriptor? DefaultProperty =>
        TypeDescriptor.GetDefaultProperty(_connectionStringBuilder, true);

    protected virtual PropertyDescriptorCollection GetProperties(
        Attribute[]? attributes)
    {
        return TypeDescriptor.GetProperties(
            _connectionStringBuilder,
            attributes,
            true);
    }

    protected virtual void OnPropertyChanged(EventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    protected virtual string ToTestString()
    {
        return _connectionStringBuilder.ConnectionString;
    }

    protected virtual void Inspect(DbConnection connection)
    {
    }

    #region ICustomTypeDescriptor

    string? ICustomTypeDescriptor.GetClassName()
    {
        return TypeDescriptor.GetClassName(_connectionStringBuilder, true);
    }

    string? ICustomTypeDescriptor.GetComponentName()
    {
        return TypeDescriptor.GetComponentName(_connectionStringBuilder, true);
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
        return TypeDescriptor.GetAttributes(_connectionStringBuilder, true);
    }

    object? ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
        ArgumentNullException.ThrowIfNull(editorBaseType);

        return TypeDescriptor.GetEditor(
            _connectionStringBuilder,
            editorBaseType,
            true);
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
        return TypeDescriptor.GetConverter(_connectionStringBuilder, true);
    }

    PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
    {
        return DefaultProperty;
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
        return GetProperties([]);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(
        Attribute[]? attributes)
    {
        return GetProperties(attributes);
    }

    EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent()
    {
        return TypeDescriptor.GetDefaultEvent(_connectionStringBuilder, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
        return TypeDescriptor.GetEvents(_connectionStringBuilder, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(
        Attribute[]? attributes)
    {
        return TypeDescriptor.GetEvents(
            _connectionStringBuilder,
            attributes,
            true);
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? propertyDescriptor)
    {
        return _connectionStringBuilder;
    }

    #endregion
}
