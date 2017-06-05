namespace Easy.Data.Common.Interface
{
    /// <summary>
    /// 数据包
    /// </summary>
    public interface IDataPackage<T>
        where T : IMessage
    {
        T Depackage(IDataPackageWrapper<T> dataPackageWrapper);

        byte[] Enpackage(T message, IDataPackageWrapper<T> dataPackageWrapper);
    }
}
