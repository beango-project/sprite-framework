using System.Text.Json;

namespace Sprite.Auditing
{
    /// <summary>
    /// 审计序列化器
    /// </summary>
    public interface IAuditSerializer
    {
        string Serialize(object obj);

        Task<string> SerializeAsync(object obj);
    }
}