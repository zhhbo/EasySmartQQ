using System;
using System.Security.Cryptography;
using System.Text;

namespace Easy.Data.Common.Utility
{
    public sealed class UniqueIdentityGenerator
    {
        private static readonly object LockObject = new object();

        private static UniqueIdentityGenerator _singletonInstance = null;

        public static UniqueIdentityGenerator SingletonInstance
        {
            get
            {
                lock (LockObject)
                {
                    if (_singletonInstance == null)
                    {
                        _singletonInstance = new UniqueIdentityGenerator();
                    }
                }
                return _singletonInstance;
            }
        }

        private UniqueIdentityGenerator()
        {
            
        }

        public ulong GetUniqueUlong()
        {
            string uniqueStr = GetUniqueString();
            byte[] byteArr = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(uniqueStr));
            ulong uniqueUlong = 0;
            for (int i = 0; i < 8; i++)
            {
                uniqueUlong += (ulong) byteArr[i] << 8*i;
            }
            return uniqueUlong;
        }

        public string GetUniqueString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
