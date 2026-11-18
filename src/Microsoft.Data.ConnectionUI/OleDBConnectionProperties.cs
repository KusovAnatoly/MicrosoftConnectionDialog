//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.Data.ConnectionUI.Resources;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;

namespace Microsoft.Data.ConnectionUI;

public class OleDBConnectionProperties : AdoDotNetConnectionProperties
{
	public OleDBConnectionProperties()
		: base("System.Data.OleDb")
	{
	}

	public bool DisableProviderSelection
	{
		get
		{
			return _disableProviderSelection;
		}
		set
		{
			_disableProviderSelection = value;
		}
	}

    public override bool IsComplete => ConnectionStringBuilder["Provider"] is string { Length: > 0 };

    protected override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
	{
		PropertyDescriptorCollection descriptors = base.GetProperties(attributes);
		if (_disableProviderSelection)
		{
			PropertyDescriptor? providerDescriptor = descriptors.Find("Provider", true);
			if (providerDescriptor is not null)
			{
				int index = descriptors.IndexOf(providerDescriptor);
				PropertyDescriptor[] descriptorArray = new PropertyDescriptor[descriptors.Count];
				descriptors.CopyTo(descriptorArray, 0);
				descriptorArray[index] = new DynamicPropertyDescriptor(providerDescriptor, ReadOnlyAttribute.Yes);
				((DynamicPropertyDescriptor)descriptorArray[index]).CanResetValueHandler = new CanResetValueHandler(CanResetProvider);
				descriptors = new PropertyDescriptorCollection(descriptorArray, true);
			}
		}
		return descriptors;
	}

    /// <summary>
    /// Gets the registered OLE DB providers as an array of ProgIDs.
    /// </summary>
    public static List<string> GetRegisteredProviders()
    {
        // Get the sources rowset for the root OLE DB enumerator
        Type rootEnumeratorType = Type.GetTypeFromCLSID(NativeMethods.CLSID_OLEDB_ENUMERATOR)
            ?? throw new InvalidOperationException("Could not retrieve type from CLSID.");

        // Read the CLSIDs of each data source (not binders or enumerators)
        HashSet<string> sourceClsids = new(StringComparer.OrdinalIgnoreCase);
        using (OleDbDataReader dataReader = OleDbEnumerator.GetEnumerator(rootEnumeratorType))
        {
            while (dataReader.Read())
            {
                if (dataReader["SOURCES_TYPE"] is int type
                    and (NativeMethods.DBSOURCETYPE_DATASOURCE_TDP or NativeMethods.DBSOURCETYPE_DATASOURCE_MDP)
                    && dataReader["SOURCES_CLSID"] is string clsid)
                {
                    sourceClsids.Add(clsid);
                }
            }
        }

        // Get the ProgID for each data source
        using Win32.RegistryKey clsidKey = Win32.Registry.ClassesRoot.OpenSubKey("CLSID")
            ?? throw new InvalidOperationException("Could not retrieve CLSID key.");

        var sourceProgIds = new List<string>(sourceClsids.Count);
        foreach (string clsid in sourceClsids)
        {
            using Win32.RegistryKey? subKey = clsidKey.OpenSubKey($@"{clsid}\ProgID");
            if (subKey?.GetValue(null) is string progId)
            {
                sourceProgIds.Add(progId);
            }
        }

        // Remove the unsupported OLE DB provider for ODBC, then sort
        sourceProgIds.RemoveAll(static id => string.Equals(id, "MSDASQL.1", StringComparison.OrdinalIgnoreCase));
        sourceProgIds.Sort(StringComparer.Ordinal);

        return sourceProgIds;
    }

    private bool CanResetProvider(object component) => false;

    private bool _disableProviderSelection;
}

public class OleDBSpecializedConnectionProperties : OleDBConnectionProperties
{
	public OleDBSpecializedConnectionProperties(string provider)
	{
		Debug.Assert(provider != null);
		_provider = provider;
		LocalReset();
	}

	public override void Reset()
	{
		base.Reset();
		LocalReset();
	}

	protected override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
	{
		bool disableProviderSelection = DisableProviderSelection;
		try
		{
			DisableProviderSelection = true;
			return base.GetProperties(attributes);
		}
		finally
		{
			DisableProviderSelection = disableProviderSelection;
		}
	}

	private void LocalReset()
	{
		// Initialize with the selected provider
		this["Provider"] = _provider;
	}

	private string _provider;
}

public class OleDBSqlConnectionProperties : OleDBSpecializedConnectionProperties
{

	public OleDBSqlConnectionProperties()
		: base("SQLOLEDB")
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
			if (!base.IsComplete)
			{
				return false;
			}
			if (ConnectionStringBuilder["Data Source"] is not string { Length: > 0 })
			{
				return false;
			}
			if (!string.Equals(ConnectionStringBuilder["Integrated Security"]?.ToString(), "SSPI", StringComparison.OrdinalIgnoreCase) &&
				ConnectionStringBuilder["User ID"] is not string { Length: > 0 })
            {
                return false;
            }
            return true;
		}
	}

	protected override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
	{
		PropertyDescriptorCollection descriptors = base.GetProperties(attributes);
		if (SqlNativeClientRegistered)
		{
            if (descriptors.Find("Provider", true) is DynamicPropertyDescriptor providerDescriptor)
            {
                if (!DisableProviderSelection)
                {
                    providerDescriptor.SetIsReadOnly(false);
                }
                providerDescriptor.SetConverterType(typeof(SqlProviderConverter));
            }
        }
		return descriptors;
	}

	private void LocalReset()
	{
		// We always start with integrated security turned on
		this["Integrated Security"] = "SSPI";
	}

	public static List<string> SqlNativeClientProviders
	{
		get
		{
			if (_sqlNativeClientProviders == null)
			{
				_sqlNativeClientProviders = new List<string>();

				List<string> providers = GetRegisteredProviders();
				Debug.Assert(providers != null, "provider list is null");
				foreach (string provider in providers)
				{
					if (provider.StartsWith("SQLNCLI"))
					{
						int idx = provider.IndexOf(".");
						if (idx > 0)
						{
							_sqlNativeClientProviders.Add(provider.Substring(0, idx).ToUpperInvariant());
						}
					}
				}

				_sqlNativeClientProviders.Sort();
			}

			Debug.Assert(_sqlNativeClientProviders != null, "Native Client list is null");
			return _sqlNativeClientProviders;
		}
	}

	private static bool SqlNativeClientRegistered
	{
		get
		{
			if (!_gotSqlNativeClientRegistered)
			{
				Win32.RegistryKey? key = null;
				try
				{
					_sqlNativeClientRegistered = SqlNativeClientProviders.Count > 0;
				}
				finally
				{
					key?.Close();
				}
				_gotSqlNativeClientRegistered = true;
			}
			return _sqlNativeClientRegistered;
		}
	}

	private static bool _sqlNativeClientRegistered;
	private static List<string>? _sqlNativeClientProviders = null;
	private static bool _gotSqlNativeClientRegistered;

	private class SqlProviderConverter : StringConverter
	{
		public SqlProviderConverter()
		{
		}

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
		{
			List<string> stdCollection = ["SQLOLEDB", .. SqlNativeClientProviders];
            return new StandardValuesCollection(stdCollection);
		}
	}
}

public class OleDBOracleConnectionProperties : OleDBSpecializedConnectionProperties
{
	public OleDBOracleConnectionProperties()
		: base("MSDAORA")
	{
	}

    public override bool IsComplete => base.IsComplete && ConnectionStringBuilder["Data Source"] is string { Length: > 0 } && ConnectionStringBuilder["User ID"] is string { Length: > 0 };
}

public class OleDBAccessConnectionProperties : OleDBSpecializedConnectionProperties
{
	public OleDBAccessConnectionProperties()
		: base("Microsoft.Jet.OLEDB.4.0")
	{
		_userChangedProvider = false;
	}

	public override void Reset()
	{
		base.Reset();
		_userChangedProvider = false;
	}

	public override object this[string propertyName]
	{
		set
		{
			base[propertyName] = value;
			if (string.Equals(propertyName, "Provider", StringComparison.OrdinalIgnoreCase))
			{
				if (value != null && value != DBNull.Value)
				{
					OnProviderChanged(ConnectionStringBuilder, EventArgs.Empty);
				}
				else
				{
					_userChangedProvider = false;
				}
			}
			if (string.Equals(propertyName, "Data Source", StringComparison.Ordinal))
			{
				OnDataSourceChanged(ConnectionStringBuilder, EventArgs.Empty);
			}
		}
	}

	public override void Remove(string propertyName)
	{
		base.Remove(propertyName);
		if (string.Equals(propertyName, "Provider", StringComparison.OrdinalIgnoreCase))
		{
			_userChangedProvider = false;
		}
		if (string.Equals(propertyName, "Data Source", StringComparison.Ordinal))
		{
			OnDataSourceChanged(ConnectionStringBuilder, EventArgs.Empty);
		}
	}

	public override void Reset(string propertyName)
	{
		base.Reset(propertyName);
		if (string.Equals(propertyName, "Provider", StringComparison.OrdinalIgnoreCase))
		{
			_userChangedProvider = false;
		}
		if (string.Equals(propertyName, "Data Source", StringComparison.Ordinal))
		{
			OnDataSourceChanged(ConnectionStringBuilder, EventArgs.Empty);
		}
	}

    public override bool IsComplete => base.IsComplete && ConnectionStringBuilder["Data Source"] is string { Length: > 0 };

    public override void Test()
	{
        if (ConnectionStringBuilder["Data Source"] is not string { Length: > 0})
        {
            throw new InvalidOperationException(Strings.OleDBAccessConnectionProperties_MustSpecifyDataSource);
        }
        base.Test();
	}

	public override string ToDisplayString()
	{
		string? savedPassword = null;
		if (ConnectionStringBuilder.ContainsKey("Jet OLEDB:Database Password") &&
			ConnectionStringBuilder.ShouldSerialize("Jet OLEDB:Database Password"))
		{
			savedPassword = ConnectionStringBuilder["Jet OLEDB:Database Password"] as string;
			ConnectionStringBuilder.Remove("Jet OLEDB:Database Password");
		}
		string displayString = base.ToDisplayString();
		if (!string.IsNullOrWhiteSpace(savedPassword))
		{
			ConnectionStringBuilder["Jet OLEDB:Database Password"] = savedPassword;
		}
		return displayString;
	}

	protected override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
	{
		PropertyDescriptorCollection descriptors = base.GetProperties(attributes);
		if (Access12ProviderRegistered)
		{
            if (descriptors.Find("Provider", true) is DynamicPropertyDescriptor providerDescriptor)
            {
                if (!DisableProviderSelection)
                {
                    providerDescriptor.SetIsReadOnly(false);
                }
                providerDescriptor.SetConverterType(typeof(JetProviderConverter));
                providerDescriptor.AddValueChanged(ConnectionStringBuilder, new EventHandler(OnProviderChanged));
            }
			if (descriptors.Find("DataSource", true) is PropertyDescriptor dataSourceDescriptor)
			{
				int index = descriptors.IndexOf(dataSourceDescriptor);
				PropertyDescriptor[] descriptorArray = new PropertyDescriptor[descriptors.Count];
				descriptors.CopyTo(descriptorArray, 0);
				descriptorArray[index] = new DynamicPropertyDescriptor(dataSourceDescriptor);
				descriptorArray[index].AddValueChanged(ConnectionStringBuilder, new EventHandler(OnDataSourceChanged));
				descriptors = new PropertyDescriptorCollection(descriptorArray, true);
			}
		}
		if (descriptors.Find("Jet OLEDB:Database Password", true) is PropertyDescriptor passwordDescriptor)
		{
			int index = descriptors.IndexOf(passwordDescriptor);
			PropertyDescriptor[] descriptorArray = new PropertyDescriptor[descriptors.Count];
			descriptors.CopyTo(descriptorArray, 0);
			descriptorArray[index] = new DynamicPropertyDescriptor(passwordDescriptor, PasswordPropertyTextAttribute.Yes);
			descriptors = new PropertyDescriptorCollection(descriptorArray, true);
		}
		return descriptors;
	}

	private static bool Access12ProviderRegistered
	{
		get
		{
			if (!_gotAccess12ProviderRegistered)
			{
				Win32.RegistryKey? key = null;
				try
				{
					key = Win32.Registry.ClassesRoot.OpenSubKey("Microsoft.ACE.OLEDB.12.0");
					_access12ProviderRegistered = (key is not null);
				}
				finally
				{
					key?.Close();
				}
				_gotAccess12ProviderRegistered = true;
			}
			return _access12ProviderRegistered;
		}
	}
	private static bool _access12ProviderRegistered;
	private static bool _gotAccess12ProviderRegistered;

	private class JetProviderConverter : StringConverter
	{
		public JetProviderConverter()
		{
		}

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context) => new StandardValuesCollection(_oleDBProvders);
        private static readonly string[] _oleDBProvders = ["Microsoft.Jet.OLEDB.4.0", "Microsoft.ACE.OLEDB.12.0"];
	}

	private void OnProviderChanged(object? sender, EventArgs e)
	{
		if (Access12ProviderRegistered)
		{
			_userChangedProvider = true;
		}
	}

	private void OnDataSourceChanged(object? sender, EventArgs e)
	{
		if (Access12ProviderRegistered && !_userChangedProvider)
		{
            if (this["Data Source"] is string dataSource)
            {
                dataSource = dataSource.Trim().ToUpperInvariant();
                if (dataSource.EndsWith(".ACCDB", StringComparison.Ordinal))
                {
                    base["Provider"] = "Microsoft.ACE.OLEDB.12.0";
                }
                else
                {
                    base["Provider"] = "Microsoft.Jet.OLEDB.4.0";
                }
            }
        }
	}

	private bool _userChangedProvider;
}
