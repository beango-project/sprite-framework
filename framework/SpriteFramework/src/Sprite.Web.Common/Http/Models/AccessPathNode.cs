using JetBrains.Annotations;

namespace Sprite.Web.Http.Models
{
    /// <summary>
    /// 访问路径节点
    /// </summary>
    public abstract class AccessPathNode
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 一下路径节点
        /// </summary>
        [CanBeNull]
        public virtual AccessPathNode NextPathNode { get; set; }
    }
}