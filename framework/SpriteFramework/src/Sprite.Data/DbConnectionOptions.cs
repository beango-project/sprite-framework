namespace Sprite.Data
{
    public class DbConnectionOptions
    {
        public DbConnectionOptions()
        {
            ConnectionStrings = new ConnectionStrings();
        }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}