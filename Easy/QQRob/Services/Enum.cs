using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.QQRob.Services
{
    public enum TargetType
    {
        /// <summary>
        ///     好友。
        /// </summary>
        Friend,

        /// <summary>
        ///     群。
        /// </summary>
        Group,

        /// <summary>
        ///     讨论组。
        /// </summary>
        Discussion
    }
    public enum ClientStatus
    {
        /// <summary>
        ///     客户端并没有连接到SmartQQ。
        /// </summary>
        Idle,

        /// <summary>
        ///     客户端正在登录。
        /// </summary>
        LoggingIn,

        /// <summary>
        ///     客户端已登录到SmartQQ。
        /// </summary>
        Active
    }

    /// <summary>
    ///     登录结果。
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        ///     登录成功。
        /// </summary>
        Succeeded,

        /// <summary>
        ///     二维码失效。登录失败。
        /// </summary>
        QrCodeExpired,

        /// <summary>
        ///     cookie失效。登录失败。
        /// </summary>
        CookieExpired,

        /// <summary>
        ///     发生了二维码失效和cookie失效以外的错误。
        /// </summary>
        Failed
    }
}
