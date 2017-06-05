using System;
using Easy.QQRob.Models;
using Easy.QQRob.Data.SmartQQ;
namespace Easy.QQRob.Services
{

    public static  class Unitities
    {

        public static FriendInfo GetFriendInfo(FriendDetail detailData)
        {
            var info = new FriendInfo();
            if (detailData == null)
            {
                return info;
            }
            info.Face = detailData.Result.Face;
            info.Occupation = detailData.Result.Occupation;
            info.Phone = detailData.Result.Phone;
            info.College = detailData.Result.College;
            info.Blood = detailData.Result.Blood;
            info.Homepage = detailData.Result.Homepage;
            info.VipInfo = detailData.Result.VipInfo;
            info.Country = detailData.Result.Country;
            info.City = detailData.Result.City;
            info.Personal = detailData.Result.Personal;
            info.Nick = detailData.Result.Nick;
            info.ShengXiao = detailData.Result.ShengXiao;
            info.Email = detailData.Result.Email;
            info.Province = detailData.Result.Province;
            info.Gender = detailData.Result.Gender;
            if (detailData.Result.BirthDay.Year != 0 && detailData.Result.BirthDay.Month != 0 && detailData.Result.BirthDay.Day != 0)
                info.Birthday = new DateTime(detailData.Result.BirthDay.Year, detailData.Result.BirthDay.Month, detailData.Result.BirthDay.Day).ToShortDateString();
            if (!string.IsNullOrEmpty(info.Homepage) && info.Homepage.IndexOf("qzone.qq.com") > 0)
            {
                info.RealQQ =long.Parse( info.Homepage.Replace(".qzone.qq.com", "").Replace("http://", ""));
            }
            if (!string.IsNullOrEmpty(info.Email) && info.Email.IndexOf("qq.com") > 0)
            {
                long real = 0;
                long.TryParse( info.Email.Replace("@qq.com", ""),out real);
                info.RealQQ = real;
            }
            return info;
        }
        public static void MapFromFriendDetail(this FriendInfo info, FriendDetail detailData)
        {
            if (detailData == null)
                return;
            if (detailData.Result == null)
                return;
            info.Face = detailData.Result.Face;
            info.Occupation = detailData.Result.Occupation;
            info.Phone = detailData.Result.Phone;
            info.College = detailData.Result.College;
            info.Blood = detailData.Result.Blood;
            info.Homepage = detailData.Result.Homepage;
            info.VipInfo = detailData.Result.VipInfo;
            info.Country = detailData.Result.Country;
            info.City = detailData.Result.City;
            info.Personal = detailData.Result.Personal;
            info.Nick = detailData.Result.Nick;
            info.ShengXiao = detailData.Result.ShengXiao;
            info.Email = detailData.Result.Email;
            info.Province = detailData.Result.Province;
            info.Gender = detailData.Result.Gender;
            if (detailData.Result.BirthDay.Year != 0 && detailData.Result.BirthDay.Month != 0 && detailData.Result.BirthDay.Day != 0)
                info.Birthday = new DateTime(detailData.Result.BirthDay.Year, detailData.Result.BirthDay.Month, detailData.Result.BirthDay.Day).ToShortDateString();
            if (!string.IsNullOrEmpty(info.Homepage) && info.Homepage.IndexOf("qzone.qq.com") > 0)
            {
                info.RealQQ = long.Parse(info.Homepage.Replace(".qzone.qq.com", "").Replace("http://", ""));
            }
            if (!string.IsNullOrEmpty(info.Email) && info.Email.IndexOf("qq.com") > 0)
            {
                long real = 0;
                long.TryParse(info.Email.Replace("@qq.com", ""), out real);
                info.RealQQ = real;
            }
            //return info;
        }
        public static GroupInfo GetGroupInfoFromGroupName(GroupName groupData,long qq)
        {
            var group = new GroupInfo();
            group.Name = groupData.Name;
            group.Code = groupData.Code;
            group.GroupId = groupData.Gid;
            group.QQNum = qq;// WorkContext.GetState<long>(Constract.CurrentQQ);
            return group;
        }
        public static void MapFromGroupName(this GroupInfo group, GroupName groupData)
        {
   
            group.Name = groupData.Name;
            group.Code = groupData.Code;
        }
        public static GroupInfo GetGroupInfoFromGroupDetail(GroupDetailResult result)
        {
            var group = new GroupInfo();
            group.Name = result.GroupDetail.Name;
            group.CreateTime = result.GroupDetail.CreateTime;
            group.Face = result.GroupDetail.Face;
            group.Owner = result.GroupDetail.Owner;
            group.Memo = result.GroupDetail.Memo;
            group.MarkName = result.GroupDetail.MarkName;
            group.Level = result.GroupDetail.Level;
            return group;
        }
        public static void MapFromGroupDetail(this GroupInfo group, GroupDetailResult result)
        {

            group.Name = result.GroupDetail.Name;
            group.CreateTime = result.GroupDetail.CreateTime;
            group.Face = result.GroupDetail.Face;
            group.Owner = result.GroupDetail.Owner;
            group.Memo = result.GroupDetail.Memo;
            group.MarkName = result.GroupDetail.MarkName;
            group.Level = result.GroupDetail.Level;
        }
        public static MemberInfo GetMemberInfoFromGroupDetail(GroupMemberInfo result,long qq)
        {
            var member = new MemberInfo();

            member.City = result.City;
            member.Province = result.Province;
            member.Country = result.Country;
            member.Gender = result.Gender;
            member.Nick = result.Nick;
            member.QQNum = qq;//WorkContext.GetState<long>(Constract.CurrentQQ);
            member.Uin = result.Uin;
            return member;
        }
        public static void MapFromMemberInfoDetail(this MemberInfo member, GroupMemberInfo result)
        {

           member.City = result.City;
           member.Province = result.Province;
           member.Country = result.Country;
           member.Gender = result.Gender;
           member.Nick = result.Nick;
            
        }
    }
}
