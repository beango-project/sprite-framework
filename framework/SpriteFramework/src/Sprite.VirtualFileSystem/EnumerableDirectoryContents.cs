using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Sprite.VirtualFileSystem
{
    public class EnumerableDirectoryContents : IDirectoryContents
    {
        private readonly IEnumerable<IFileInfo> _files;

        public EnumerableDirectoryContents(IEnumerable<IFileInfo> files)
        {
            _files = files;
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        public bool Exists => true;
    }
}