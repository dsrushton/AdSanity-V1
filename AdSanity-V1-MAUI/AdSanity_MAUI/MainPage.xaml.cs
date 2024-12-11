using AdSanity_MAUI.StaticHelpers;

namespace AdSanity_MAUI;

using Microsoft.Playwright;
using Stripe;

using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Stripe;

using System.Collections.ObjectModel;
public partial class MainPage : ContentPage
{
    private const string STRIPE_SECRET_KEY = "your_stripe_secret_key";
    private const string PRODUCT_PRICE_ID = "your_price_id";
    private const int FREE_TRIAL_RUNS = 3;
    private const int PAID_VERSION_RUNS = 100;
    
    private bool isRunning = false;
    private CancellationTokenSource? cancellationTokenSource;
    private readonly Random _random = new();

    public MainPage()
    {
        InitializeComponent();
        SetupStripe();
    }

    private void SetupStripe()
    {
        StripeConfiguration.ApiKey = STRIPE_SECRET_KEY;
    }

    private async void OnTryButtonClicked(object sender, EventArgs e)
    {
        await RunSearches(FREE_TRIAL_RUNS);
    }

    private async void OnPaidButtonClicked(object sender, EventArgs e)
    {
        await HandlePaidVersion();
    }

    private void OnStopButtonClicked(object sender, EventArgs e)
    {
        StopOperation();
    }

    private async Task HandlePaidVersion()
    {
        try
        {
            // var options = new PaymentIntentCreateOptions
            // {
            //     Amount = 1000, // $10.00
            //     Currency = "usd",
            //     PaymentMethodTypes = new List<string> { "card" }
            // };
            //
            // var service = new PaymentIntentService();
            // var paymentIntent = await service.CreateAsync(options);

            // Here you would typically redirect to a payment page
            // For demo purposes, we'll assume payment is successful
            UpdateProgress("Loading...");
            if (true) // Replace with actual payment verification
            {
                SetSearchButtonEnabledStatus(false);
                await RunSearches(40);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Payment processing error: {ex.Message}", "OK");
        }
        finally
        {
            SetSearchButtonEnabledStatus(true);
        }
    }

    private async Task RunSearches(int numberOfSearches)
    {
        if (isRunning)
        {
            await DisplayAlert("Warning", "Operation already in progress!", "OK");
            return;
        }

        isRunning = true;
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            for (int i = 0; i < numberOfSearches && !cancellationTokenSource.Token.IsCancellationRequested; i++)
            {
                await PerformSingleSearch();
                UpdateProgress($"Completed search {i + 1} of {numberOfSearches}");
                await Task.Delay(_random.Next(3000, 6000));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error during search operation: {ex.Message}", "OK");
        }
        finally
        {
            isRunning = false;
            UpdateProgress("Operation completed");
        }
    }

    private async Task PerformSingleSearch()
    {
        IWebDriver driver = null;
        try
        {
            var options = new ChromeOptions();

            // Login Issue Here
            options.AddArgument($@"--user-data-dir=C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data");
            options.AddArgument("--profile-directory=Default");
            options.AddArgument("--new-window");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            // options.AddArgument("--remote-debugging-port=0");

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            driver = new ChromeDriver(service, options);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string searchQuery = GenerateSearchQuery();
            driver.Navigate().GoToUrl("https://www.google.com");
            await Task.Delay(500);

            var searchBox = wait.Until(e => e.FindElement(By.Name("q")));
            searchBox.SendKeys(searchQuery + OpenQA.Selenium.Keys.Return);

            // Wait for search results
            await Task.Delay(500);

            // Find all search result links
            var searchResults = wait.Until(e => e.FindElements(By.CssSelector("div.g")));

            // Try up to 10 results before giving up
            for (int i = 0; i < Math.Min(10, searchResults.Count); i++)
            {
                try
                {
                    var result = searchResults[i];
                    var link = result.FindElement(By.CssSelector("a"));
                    var href = link.GetAttribute("href");

                    // Skip if it's a common link - smaller companies should make a difference
                    var href_lower = href.ToLower();
                    if (href_lower.Contains("amazon") ||
                        href_lower.Contains("bestbuy") ||
                        href_lower.Contains("walmart") ||
                        href_lower.Contains("etsy") ||
                        href_lower.Contains("aliexpress") ||
                        href_lower.Contains("reddit"))
                    {
                        continue;
                    }

                    // Found a non-Amazon link, click it
                    link.Click();
                    await Task.Delay(2000);
                    break;
                }
                catch (Exception ex)
                {
                    // If there's an error with this result, try the next one
                    continue;
                }
            }
        }

    private string GenerateSearchQuery()
    {
        string word1 = WordLists.DefaultWords1[_random.Next(WordLists.DefaultWords1.Count)];
        string word2 = WordLists.DefaultWords2[_random.Next(WordLists.DefaultWords2.Count)];
        return $"{word1} {word2}";
    }

    private void StopOperation()
    {
        cancellationTokenSource?.Cancel();
        isRunning = false;
        UpdateProgress("Operation stopped by user");
    }

    private void UpdateProgress(string message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ProgressLabel.Text = message;
        });
    }

    private void SetSearchButtonEnabledStatus(bool value)
    {
        TryButton.IsEnabled = value;
        PaidButton.IsEnabled = value;
    }
}