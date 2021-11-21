using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;

namespace Sprite.VirtualFileSystem.Extensions
{
    public static class FileInfoExtensions
    {
        public static string GetVirtualOrPhysicalPathOrNull([NotNull] this IFileInfo fileInfo)
        {
            // Check.NotNull(fileInfo, nameof(fileInfo));

            if (fileInfo is EmbeddedResourceFileInfo embeddedFileInfo)
            {
                return embeddedFileInfo.PhysicalPath;
            }

            // if (fileInfo is InMemoryFileInfo inMemoryFileInfo)
            // {
            //     return inMemoryFileInfo.DynamicPath;
            // }

            return fileInfo.PhysicalPath;
        }
    }
}