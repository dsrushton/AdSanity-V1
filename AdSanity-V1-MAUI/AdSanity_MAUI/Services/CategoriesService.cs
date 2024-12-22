using AdSanity_MAUI.Models;

public class CategoriesService
{
    public List<Category> Categories { get; } = new List<Category>();
    private Category _currentCategory;

    public CategoriesService()
    {
        Categories = new List<Category>
        {
            new Category("Random", 0),
            new Category("BooksAndLearning", 1),
            new Category("OutdoorAndAdventure", 2),
            new Category("Technology", 3),
            new Category("HomeAndLiving", 4),
            new Category("SportsAndFitness", 5),
            new Category("ArtsAndCrafts", 6)
        };
        _currentCategory = Categories[0];
    }

    public Category GetCurrentCategory()
    {
        return _currentCategory;
    }

    public void SetCurrentCategory(SearchTopic searchTopic)
    {
        _currentCategory = Categories[(int)searchTopic];
    }
}