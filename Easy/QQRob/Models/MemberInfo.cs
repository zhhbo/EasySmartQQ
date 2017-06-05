namespace Easy.QQRob.Models
{
    using Easy.Data.Attributes;
    [TableMap("MemberInfomation")]
    public class MemberInfo
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("Uin", typeof(long), "Uin", typeof(long))]
        public long Uin { get; set; }
        [PropertyFieldMap("RealQQ", typeof(long), "RealQQ", typeof(long))]
        public long RealQQ { get; set; }
        [PropertyFieldMap("GroupId", typeof(long), "GroupId", typeof(long))]
        public long GroupId { get; set; }
        [PropertyFieldMap("Nick", typeof(string), "Nick", typeof(string))]
        public string Nick { get; set; }
        [PropertyFieldMap("Country", typeof(string), "Country", typeof(string))]
        public string Country { get; set; }
        [PropertyFieldMap("Province", typeof(string), "Province", typeof(string))]
        public string Province { get; set; }
        [PropertyFieldMap("City", typeof(string), "City", typeof(string))]
        public string City { get; set; }
        [PropertyFieldMap("Gender", typeof(string), "Gender", typeof(string))]
        public string Gender { get; set; }
        [PropertyFieldMap("Card", typeof(string), "Card", typeof(string))]
        public string Card { get; set; }
        [PropertyFieldMap("IsManager", typeof(bool), "IsManager", typeof(bool))]
        public bool IsManager { get; set; }

        [PropertyFieldMap("Status", typeof(string), "Status", typeof(string))]
        public string Status { get; set; }
        [PropertyFieldMap("ClientType", typeof(int), "ClientType", typeof(int))]
        public int ClientType { get; set; }

        [PropertyFieldMap("IsVip", typeof(bool), "IsVip", typeof(bool))]
        public bool IsVip { get; set; }
        [PropertyFieldMap("VipLevel", typeof(int), "VipLevel", typeof(int))]
        public int VipLevel { get; set; }

        [PropertyFieldMap("Alias", typeof(string), "Alias", typeof(string))]
        public string Alias { get; set; }// { return Card + "：" + Nick; }
    }
}
