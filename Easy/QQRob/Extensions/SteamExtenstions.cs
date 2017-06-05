using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Easy.QQRob.Extensions
{
  public static  class SteamExtenstions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                stream.Close();
                return ms.ToArray();
            }
        }
    }
}
