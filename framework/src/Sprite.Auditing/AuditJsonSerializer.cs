using System.Text.Json;

namespace Sprite.Auditing
{
    public class AuditJsonSerializer : IAuditSerializer
    {
        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public async Task<string> SerializeAsync(object obj)
        {
            await using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, obj);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}