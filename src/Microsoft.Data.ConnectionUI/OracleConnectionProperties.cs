//------------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Data.ConnectionUI;

public class OracleConnectionProperties : AdoDotNetConnectionProperties
{
	public OracleConnectionProperties()
		: base("System.Data.OracleClient")
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

    private void LocalReset()
	{
		// We always start with unicode turned on
		this["Unicode"] = true;
	}

}
