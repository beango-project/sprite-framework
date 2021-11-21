using System.Collections.Generic;
using System.IO;
using ImTools;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Primitives;
using Sprite.VirtualFileSystem.Extensions;

namespace Sprite.VirtualFileSystem
{
    public class InMemoryFileProvider : IFileProvider
    {
        protected virtual ImTools.ImHashMapEntry<string, IFileInfo> Files { get; }

        protected virtual string NormalizePath(string path) => path;

        public virtual IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            var file = Files.GetValueOrDefault(NormalizePath(subpath));

            if (file == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            return file;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var directory = GetFileInfo(subpath);
            if (!directory.IsDirectory)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var fileList = new List<IFileInfo>();

            var directoryPath = subpath.EndsWith('/') ? subpath : subpath + '/';
            foreach (var fileEntry in Files.Enumerate())
            {
                var fullPath = fileEntry.Value.GetVirtualOrPhysicalPathOrNull();
                if (!fullPath.StartsWith(directoryPath))
                {
                    continue;
                }

                var relativePath = fullPath.Substring(directoryPath.Length);
                if (relativePath.Contains("/"))
                {
                    continue;
                }

                fileList.Add(fileEntry.Value);
            }

            return new EnumerableDirectoryContents(fileList);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
    }
}