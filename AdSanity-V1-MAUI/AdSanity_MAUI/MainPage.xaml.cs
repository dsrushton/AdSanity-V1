namespace AdSanity_MAUI;

using Microsoft.Playwright;
using Stripe;

using System.Collections.ObjectModel;
public partial class MainPage : ContentPage
{
    // Constants
    private const string STRIPE_SECRET_KEY = "your_stripe_secret_key";
    private const string PRODUCT_PRICE_ID = "your_price_id";
    private const int FREE_TRIAL_RUNS = 3;
    private const int PAID_VERSION_RUNS = 100;

    // Lists to store search words
    private List<string>? words;
    private List<string>? words2;

    // Flag to track if operation is running
    private bool isRunning = false;
    private CancellationTokenSource? cancellationTokenSource;

    public MainPage()
    {
        InitializeComponent();
        SetupStripe();
        InitializeWordLists();
    }

    private void SetupStripe()
    {
        StripeConfiguration.ApiKey = STRIPE_SECRET_KEY;
    }

    private void InitializeWordLists()
    {
        words = new List<string>
        {
            "Vintage", "Night Vision", "Handheld", "Tesla Coil", "UV Light",
            // ... (rest of your words list)
        };

        words2 = new List<string>
        {
            "Compass", "Goggles", "Meteorite Fragment", "Geiger Counter", "Lighter",
            // ... (rest of your words2 list)
        };
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
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1000, // $10.00
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Here you would typically redirect to a payment page
            // For demo purposes, we'll assume payment is successful
            if (true) // Replace with actual payment verification
            {
                await RunSearches(PAID_VERSION_RUNS);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Payment processing error: {ex.Message}", "OK");
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
                await Task.Delay(3000); // 3 second delay between searches
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
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            string searchQuery = GenerateSearchQuery();
            await page.GotoAsync("https://www.google.com");
            await page.WaitForLoadStateAsync();

            await page.FillAsync("[name=q]", searchQuery);
            await page.PressAsync("[name=q]", "Enter");

            await page.WaitForSelectorAsync("h3");
            await page.ClickAsync("h3");
            
            await Task.Delay(5000);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error during search operation: {ex.Message}", "OK");
        }
    }

    private string GenerateSearchQuery()
    {
        Random rand = new Random();
        string word1 = words[rand.Next(words.Count)];
        string word2 = words2[rand.Next(words2.Count)];
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
}