using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Sprite.VirtualFileSystem
{
    public class VirtualFileProvider: InMemoryFileProvider, IVirtualFileProvider
    {
        public IFileInfo GetFileInfo(string subpath)
        {
            throw new System.NotImplementedException();
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new System.NotImplementedException();
        }

        public IChangeToken Watch(string filter)
        {
            throw new System.NotImplementedException();
        }
    }
}