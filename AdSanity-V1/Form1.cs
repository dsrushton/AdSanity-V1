using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Stripe;
using System.Collections.Generic;
using System.Threading;
using System.IO;
// Use the full namespace when referencing Stripe
using StripeConfiguration = Stripe.StripeConfiguration;
using StripeService = Stripe.PaymentIntentService;
using StripeOptions = Stripe.PaymentIntentCreateOptions;
// WebDriverManager -- Posssible Login Workaround
//using WebDriverManager;
//using WebDriverManager.DriverConfigs.Impl;


namespace AdSanity_V1
{
    public partial class Form1: Form
    {
        // Constants
        private const string STRIPE_SECRET_KEY = "your_stripe_secret_key";
        private const string PRODUCT_PRICE_ID = "your_price_id";
        private const int FREE_TRIAL_RUNS = 3;
        private const int PAID_VERSION_RUNS = 100;

        // Lists to store search words (you'll add these manually)
        private List<string> words = new List<string>();
        private List<string> words2 = new List<string>();

        // Flag to track if operation is running
        private bool isRunning = false;
        private CancellationTokenSource cancellationTokenSource;

        public Form1()
        {
            InitializeializeComponents();
            SetupStripe();

            // Initialize word lists
            words = new List<string> {
                "Vintage", "Night Vision", "Handheld", "Tesla Coil", "UV Light",
                "Portable", "Magnetic Levitating", "Solar Powered", "Wind-Up", "Laser",
                "Infrared", "Magnet", "Parabolic", "Imaging", "Stirling Engine",
                "DIY Bioluminescent", "Raspberry Pi", "Electric Plasma Arc", "Digital",
                "Antique-style", "Kinetic", "Personal Ozone", "Neodymium", "3D Hologram",
                "Sonic", "Weather Balloon", "Solar", "Morse Code", "Ham Radio",
                "NFC", "UV Reactive", "Hydroponic", "Faraday Cage", "Compass Card",
                "DIY", "Electric", "Mini", "Bluetooth", "Aromatherapy",
                "Silk", "Aromatherapy Oil", "Compact", "Rose Quartz", "Leather",
                "Essential Oil", "Makeup Brush", "Nail Art", "Luxury", "Crossbody",
                "Velvet", "Mini Perfume", "Reusable", "Facial", "Handheld Mini",
                "Lip Balm", "Crystal", "LED Vanity", "Floral", "Rose Gold",
                "Eco-Friendly", "Fashionable Phone", "Adjustable", "Reed", "Facial Mist",
                "Hair Claw", "LED", "Vegan Leather", "Decorative", "Silicone",
                "Colorful", "Tea Sampler", "Massage", "Wireless Charging", "USB",
                "Fitness", "Wireless", "Coffee", "Camping", "Phone",
                "Desk", "Scented", "Key", "Pressure", "Reusable Coffee",
                "Yoga", "Tool", "Power", "LED Desk", "Hair",
                "WiFi Range", "Pet Water", "Electric", "Noise-Canceling", "Smart",
                   "Waterproof", "Foldable", "Laptop", "Phone"
             };

            words2 = new List<string> {
                "Compass", "Goggles", "Meteorite Fragment", "Geiger Counter", "Lighter",
                "Sterilizer", "Microscope", "Globe", "Radio", "Flashlight",
                "Fog Machine", "Engraver", "Thermometer", "Fishing Kit", "Microphone",
                "Gyroscope Kit", "Camera", "Model", "Algae Kit", "Kit",
                "Pen", "Anemometer", "Seismograph Kit", "Telescope", "Sand Kit",
                "Generator", "Magnet Set", "Wind Turbine", "Projector", "Screwdriver Replica",
                "Oven", "Key", "Ring", "Paint Set", "Garden",
                "Observatory Tent", "Bag", "Sundial", "Cloud Chamber", "Pottery Wheel",
                "Drone with Camera", "Spectrum Analyzer", "Diffuser", "Scarf", "Mirror",
                "Roller", "Journal", "Bath Bombs", "Jewelry Organizer", "Set",
                "Hand Cream", "Manicure Kit", "Purse", "Scrunchies", "Atomizer",
                "Makeup Remover Pads", "Steamer", "Fan", "Bracelet", "Eye Mask",
                "Sleep Mask", "Tea Infuser", "Pillowcase", "Desk Organizer", "Makeup Case",
                "Tote Bag", "Ring Stand", "Ring Set", "Eyebrow Trimmer", "Sprayer",
                "Clips", "Jewelry Box", "Nail Lamp", "Wallet", "Fabric Shaver",
                "Trinket Dish", "Humidifier", "Bathrobe", "Garment Steamer", "Face Mask Brush",
                "Stationery Set", "Roller", "Pad", "Espresso Maker", "Water Bottle",
                "Keychain", "Spinner", "Umbrella", "Welder", "Speaker",
                "Air Fryer", "Bulb", "Toothbrush", "Charger", "Flash Drive",
                "Tracker", "Earbuds", "Grinder", "Lantern", "Stand",
                "Organizer", "Kettle", "Candle", "Notebook", "Gun",
                "Finder", "Blender", "Cooker", "Cup", "Mat",
                "Strip", "Lamp", "Dryer", "Extender", "Fountain",
                "Vacuum", "Opener", "Headphones", "Plug", "Backpack",
                "Shaver", "Scooter", "Cooler", "Compressor", "Chair",
                "Tripod", "Blanket", "Thermostat"
            };
        }

        private void InitializeializeComponents()
        {
            // Form setup
            this.Text = "AdSanity";
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 2,
                               Screen.PrimaryScreen.Bounds.Height / 2);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Set background image
            try
            {
                this.BackgroundImage = Image.FromFile(Path.Combine(System.Windows.Forms.Application.StartupPath, "773426.jpg"));
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Background image not found. Error: {ex.Message}");
            }

            // Calculate center positions
            int formWidth = this.ClientSize.Width;
            int buttonWidth = 200;
            int startX = (formWidth - buttonWidth) / 2;
            int startY = 100; // Starting Y position

            // Create buttons with centered positions
            Button tryButton = new Button
            {
                Text = "Try It Out (3 Searches)",
                Size = new Size(buttonWidth, 50),
                Location = new Point(startX, startY),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            tryButton.Click += async (s, e) => await RunSearches(FREE_TRIAL_RUNS);

            Button paidButton = new Button
            {
                Text = "Run Full Version (100 Searches)",
                Size = new Size(buttonWidth, 50),
                Location = new Point(startX, startY + 70), // 70 pixels gap between buttons
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            paidButton.Click += async (s, e) => await HandlePaidVersion();

            Button stopButton = new Button
            {
                Text = "Stop",
                Size = new Size(buttonWidth, 50),
                Location = new Point(startX, startY + 140), // Another 70 pixels gap
                BackColor = Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            stopButton.Click += (s, e) => StopOperation();

            // Progress display - centered under buttons
            Label progressLabel = new Label
            {
                Location = new Point(startX, startY + 350),
                Size = new Size(buttonWidth, 20),
                ForeColor = Color.Black,
                BackColor = Color.Transparent,
                Name = "progressLabel",
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] { tryButton, paidButton, stopButton, progressLabel });
        }

        private void SetupStripe()
        {
            StripeConfiguration.ApiKey = STRIPE_SECRET_KEY;
        }

        private async Task HandlePaidVersion()
        {
            try
            {
                // Create Stripe payment session
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
                MessageBox.Show($"Payment processing error: {ex.Message}");
            }
        }

        private async Task RunSearches(int numberOfSearches)
        {
            if (isRunning)
            {
                MessageBox.Show("Operation already in progress!");
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
                MessageBox.Show($"Error during search operation: {ex.Message}");
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
                //Login issue here
                //options.AddArgument($@"--user-data-dir=C:\Users\{Environment.UserName}\AppData\Local\Google\Chrome\User Data");
                options.AddArgument("--profile-directory=Default");
                options.AddArgument("--new-window");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                //options.AddArgument("--remote-debugging-port=0");

                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                driver = new ChromeDriver(service, options);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                string searchQuery = GenerateSearchQuery();
                driver.Navigate().GoToUrl("https://www.google.com");
                await Task.Delay(2000);

                var searchBox = wait.Until(e => e.FindElement(By.Name("q")));
                searchBox.SendKeys(searchQuery + OpenQA.Selenium.Keys.Return);

                var firstResult = wait.Until(e => {
                    var element = e.FindElement(By.CssSelector("h3"));
                    return element.Enabled && element.Displayed ? element : null;
                });

                firstResult.Click();
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during search operation: {ex.Message}");
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
        }

        private string GenerateSearchQuery()
        {
            // Implement your random word selection logic here
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
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateProgress(message)));
                return;
            }

            var label = this.Controls.Find("progressLabel", true)[0] as Label;
            if (label != null)
            {
                label.Text = message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}