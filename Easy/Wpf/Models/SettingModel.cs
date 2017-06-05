using System.Linq;
using Easy.Data.Attributes;
namespace Easy.Models
{
    [TableMap("Setting")]
    public class SettingModel
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("Theme", typeof(string), "Theme", typeof(string))]
        public string Theme { get; set; } = "Light";
        [PropertyFieldMap("FontSize", typeof(string), "FontSize", typeof(string))]
        public string FontSize { get; set; } = "large";
        [PropertyFieldMap("Color", typeof(string), "Color", typeof(string))]
        public string Color { get; set; } = "red";

    }
    /*
    public class SettingModelDataMigration : DataMigrationImpl
    {
        public SettingModelDataMigration()
        { }
        public int Create()
        {
            SchemaBuilder.CreateTable("Setting", table => table
                                .Column<int>("Id", column => column.PrimaryKey().Identity())
                                .Column<string>("Theme")
                                .Column<string>("FontSize")
                                .Column<string>("Color"));
            return 1;
        }
    }*/
}
