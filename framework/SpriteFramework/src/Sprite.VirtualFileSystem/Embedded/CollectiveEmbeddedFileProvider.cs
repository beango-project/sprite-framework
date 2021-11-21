using System.Reflection;
using ImTools;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;

namespace Sprite.VirtualFileSystem.Embedded
{
    /// <summary>
    /// 收集式嵌入式文件提供者
    /// </summary>
    public class CollectiveEmbeddedFileProvider:InMemoryFileProvider
    {
        [NotNull]
        public Assembly Assembly { get; }

        [CanBeNull]
        public string BaseNamespace { get; }

        protected override ImHashMapEntry<string, IFileInfo> Files { get; }
    }
}