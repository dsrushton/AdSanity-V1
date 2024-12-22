namespace AdSanity_MAUI.Models;

public class Category(string categoryName, int categoryIndex)
{
    public string CategoryName { get; private set; } = categoryName;
    public int CategoryIndex { get; private set; } = categoryIndex;
}