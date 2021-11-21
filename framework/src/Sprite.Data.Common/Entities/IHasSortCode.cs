namespace Sprite.Data.Entities
{
    /// <summary>
    /// 用于标准化实体排序码。
    /// 在框架中对已经实现对该属性的自动赋值（保存时检查为空时）。
    /// </summary>
    public interface IHasSortCode
    {
        /// <summary>
        /// 排序编码
        /// </summary>
        string SortCode { get; set; }
    }
}