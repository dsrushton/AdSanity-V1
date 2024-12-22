using AdSanity_MAUI.Models;
using AdSanity_MAUI.StaticHelpers;

public class WordGenService
{
    private readonly Random _random = new Random();
    private List<string> _firstWords = new List<string>();
    private List<string> _secondWords = new List<string>();

    public WordGenService()
    {
        
    }

    public string GetWord()
    {
        return $"{GetRandomWord(_firstWords)} {GetRandomWord(_secondWords)}";
    }

    public void UpdateWords(SearchTopic searchTopic)
    {
        SetWords(searchTopic);
    }

    public void SetWords(SearchTopic searchTopic)
    {
        switch (searchTopic)
        {
            case SearchTopic.OutdoorAndAdventure:
                _firstWords = WordLists.OutdoorAndAdventureFirstWords;
                _secondWords = WordLists.OutdoorAndAdventureSecondWords;
                break;
            case SearchTopic.Technology:
                _firstWords = WordLists.TechnologyFirstWords;
                _secondWords = WordLists.TechnologySecondWords;
                break;
            case SearchTopic.HomeAndLiving:
                _firstWords = WordLists.HomeAndLivingFirstWords;
                _secondWords = WordLists.HomeAndLivingSecondWords;
                break;
            case SearchTopic.SportsAndFitness:
                _firstWords = WordLists.SportsAndFitnessFirstWords;
                _secondWords = WordLists.SportsAndFitnessSecondWords;
                break;
            case SearchTopic.ArtsAndCrafts:
                _firstWords = WordLists.ArtsAndCraftsFirstWords;
                _secondWords = WordLists.ArtsAndCraftsSecondWords;
                break;
            case SearchTopic.BooksAndLearning:
                _firstWords = WordLists.BooksAndLearningFirstWords;
                _secondWords = WordLists.BooksAndLearningSecondWords;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetRandomWord(List<string> words)
    {
        if (words == null || words.Count == 0)
            return string.Empty;
        return words[_random.Next(words.Count)];
    }
}
