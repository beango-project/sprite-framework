namespace Sprite.Data.Entities
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}