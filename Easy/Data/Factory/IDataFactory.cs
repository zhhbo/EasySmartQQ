using Easy.Data.Database.Core;
using Easy.Data.Attributes;
namespace Easy.Data.Factory
{
    interface IDataFactory
    {
        TableMapAttribute TableAttr { get; }

        IDataFactory DataFactory { get; }

        IDatabase Database { get; set; }

        object GetResult();
    }
}
