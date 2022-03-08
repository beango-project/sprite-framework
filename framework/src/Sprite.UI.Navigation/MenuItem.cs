namespace Sprite.UI.Navigation;

public class MenuItem
{
    
    public int Order { get; set; }
    
    /// <summary>
    /// The HTML Id of the menu item.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Menu name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// The optional url the menu item should link to.
    /// </summary>
    public string Url { get; set; }
    
    /// <summary>
    /// black
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// The child menu items.
    /// </summary>
    public List<MenuItem> Items { get; }
    
    
    /// <summary>
    /// The css classes to render with the menu item.
    /// </summary>
    public List<string> Classes { get; }

    public MenuItem()
    {
        Items = new List<MenuItem>();
    }

    public MenuItem AddItem(MenuItem menuItem)
    {
        Items.Add(menuItem);
        return this;
    }
}