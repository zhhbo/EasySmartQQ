namespace Easy.Data.Common.Interface
{
    /// <summary>
    /// 数据包打包器
    /// </summary>
    public interface IDataPackageWrapper<T>
        where T : IMessage
    {
        byte[] GetBytes(T message);

        T GetMessage(byte[] objBytes);
    }
}
