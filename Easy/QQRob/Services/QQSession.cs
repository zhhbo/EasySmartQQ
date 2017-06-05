namespace Easy.QQRob.Services
{
    public class QQSession
    {
        public long ClientId { get; set; } = 53999199;
        public long MessageId { get; set; } = 43690001;
        public long QQNum;

        // 鉴权参数
        public string Ptwebqq { get; set; }
        public long Uin { get; set; }
        public string Vfwebqq { get; set; }

        public string QRsig { get; set; }
        public string Hash { get; set; }
        public string Psessionid { get; set; }

        public string PSkey { get; set; }

        public string Skey { get; set; }
        public string PUin { get; set; }

    }
}
