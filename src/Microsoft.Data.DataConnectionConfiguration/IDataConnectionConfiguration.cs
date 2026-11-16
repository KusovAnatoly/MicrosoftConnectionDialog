using System.Data.Common;

namespace Microsoft.Data.DataConnectionConfiguration;

public interface IDataConnectionConfiguration
{
    string GetSelectedSource();
    void SaveSelectedSource(string provider);

    string GetSelectedProvider();
    void SaveSelectedProvider(string provider);
}
