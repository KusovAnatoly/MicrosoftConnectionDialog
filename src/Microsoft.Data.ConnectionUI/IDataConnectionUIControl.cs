namespace Microsoft.Data.ConnectionUI;

public interface IDataConnectionUIControl
{
    void Initialize(IDataConnectionProperties connectionProperties);
    void LoadProperties();
}