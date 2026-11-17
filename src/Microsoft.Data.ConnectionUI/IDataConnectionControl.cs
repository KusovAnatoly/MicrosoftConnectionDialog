namespace Microsoft.Data.ConnectionUI;

public interface IDataConnectionControl
{
    void Initialize(IDataConnectionProperties connectionProperties);
    void LoadProperties();
}