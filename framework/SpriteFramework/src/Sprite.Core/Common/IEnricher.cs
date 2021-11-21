namespace Sprite.Common
{
    public interface IEnricher<TContext>
    {
        void Enrich(TContext context);
    }
}