namespace Easy.QQRob.Models
{
    using Easy.Data.Attributes;
    /// <summary>
    /// 好友资料类
    /// </summary>
    [TableMap("FriendInfomation")]
    public class FriendInfo
    {
        [PrimaryKey("Id", typeof(int), "Id", typeof(int))]
        public int Id { get; set; }
        [PropertyFieldMap("QQNum", typeof(long), "QQNum", typeof(long))]
        public long QQNum { get; set; }
        [PropertyFieldMap("Uin", typeof(long), "Uin", typeof(long))]
        public long Uin { get; set; }
        [PropertyFieldMap("MarkName", typeof(string), "MarkName", typeof(string))]
        public string MarkName { get; set; }
        [PropertyFieldMap("Nick", typeof(string), "Nick", typeof(string))]
        public string Nick { get; set; }
        [PropertyFieldMap("Gender", typeof(string), "Gender", typeof(string))]
        public string Gender { get; set; }
        [PropertyFieldMap("Face", typeof(int), "Face", typeof(int))]
        public int Face { get; set; }
        [PropertyFieldMap("ClientType", typeof(int), "ClientType", typeof(int))]
        public int ClientType { get; set; }
        [PropertyFieldMap("Categories", typeof(string), "Categories", typeof(string))]
        public string Categories { get; set; }
        [PropertyFieldMap("Status", typeof(string), "Status", typeof(string))]
        public string Status { get; set; }
        [PropertyFieldMap("Occupation", typeof(string), "Occupation", typeof(string))]
        public string Occupation { get; set; }   //职业          
        [PropertyFieldMap("College", typeof(string), "College", typeof(string))]
        public string College { get; set; }
        [PropertyFieldMap("Country", typeof(string), "Country", typeof(string))]
        public string Country { get; set; }
        [PropertyFieldMap("Province", typeof(string), "Province", typeof(string))]
        public string Province { get; set; }
        [PropertyFieldMap("City", typeof(string), "City", typeof(string))]
        public string City { get; set; }
        [PropertyFieldMap("Personal", typeof(string), "Personal", typeof(string))]
        public string Personal { get; set; }     //简介
        [PropertyFieldMap("Homepage", typeof(string), "Homepage", typeof(string))]
        public string Homepage { get; set; }
        [PropertyFieldMap("Email", typeof(string), "Email", typeof(string))]
        public string Email { get; set; }
        [PropertyFieldMap("Mobile", typeof(string), "Mobile", typeof(string))]
        public string Mobile { get; set; }
        [PropertyFieldMap("Phone", typeof(string), "Phone", typeof(string))]
        public string Phone { get; set; }
        [PropertyFieldMap("Birthday", typeof(string), "Birthday", typeof(string))]
        public string Birthday { get; set; }
        [PropertyFieldMap("Blood", typeof(int), "Blood", typeof(int))]
        public int Blood { get; set; }
        [PropertyFieldMap("ShengXiao", typeof(int), "ShengXiao", typeof(int))]
        public int ShengXiao { get; set; }
        [PropertyFieldMap("VipInfo", typeof(int), "VipInfo", typeof(int))]
        public int VipInfo { get; set; }
        [PropertyFieldMap("RealQQ", typeof(long), "RealQQ", typeof(long))]
        public long RealQQ { get; set; }
        [PropertyFieldMap("Alias", typeof(string), "Alias", typeof(string))]
        public string Alias { get; set; }//= Categories + "：" +this.MarkName ??this. Nick;
        // [PropertyFieldMap("Categories", typeof(int), "Categories", typeof(int))]
        // public int Categories { get; set; }


    }
}
