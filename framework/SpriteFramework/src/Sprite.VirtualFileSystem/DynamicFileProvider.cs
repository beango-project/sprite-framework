using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Sprite.VirtualFileSystem
{
    public class DynamicFileProvider : InMemoryFileProvider, IDynamicFileProvider
    {
        private IFileProvider GetFileProvider()
        {
            var fileProviders = new List<IFileProvider>();

            return new CompositeFileProvider(fileProviders);
        }

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
            //TODO:构建混合文件提供者
            //TODO:构建内存文件提供者，使用者可以将文件放入内存中进行读取
            //TODO:设计可以跨程序集读取嵌入式资源的嵌入式文件提供者（程序集信息收集者）
            //TODO:针对物理文件提供者，有必要进行扩展或者重写监视，以提供持续的多文件监控
            throw new System.NotImplementedException();
        }
    }
}