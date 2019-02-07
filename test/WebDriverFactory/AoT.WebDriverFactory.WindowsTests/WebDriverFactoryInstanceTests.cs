using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace AoT.WebDriverFactory.WindowsTests
{
    [TestFixture]
    public class WebDriverFactoryInstanceTests
    {
        private CustomLocalWebDriver<IWebDriver> LocalDriver { get; set; }
        private CustomRemoteWebDriver RemoteDriver { get; set; }
        private readonly PlatformType thisPlatformType = PlatformType.Windows;
        private IWebDriverFactory WebDriverFactory { get; set; }      
        private delegate void LocalDriverTest <TDriver>(Browser browser, bool headless =false);
        private delegate void RemoteDriverTest<TDriver>(Browser browser, PlatformType platform= PlatformType.Any, bool headless = false);


        [OneTimeSetUp]
        public void SetUp()
        {
            Assume.That(() => Platform.CurrentPlatform.IsPlatformType(thisPlatformType));
            this.WebDriverFactory = new DefaultWebDriverFactory();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDriver"></typeparam>
        /// <param name="lbrowser"></param>
        private void CheckBrowserLaunchAndLoadExampleDotCom<TDriver>(Browser lbrowser, bool headless=false)
        {
            var driver = this.WebDriverFactory.GetLocalWebDriver(lbrowser,headless) as CustomLocalWebDriver<TDriver>;
            driver.GetLocalDriver().Url = "https://example.com/";
            driver.GetLocalDriver().Title.Should().Be("Example Domain");
            driver.GetLocalDriver().Quit();
        }
        private void CheckBrowserLaunchRemoteAndLoadExampleDotCom<TDriver>(Browser browser, PlatformType platform = PlatformType.Any,  bool headless = false)
        {
            RemoteDriver = this.WebDriverFactory.GetRemoteWebDriver(browser, null, PlatformType.Any, true) as CustomRemoteWebDriver;
            RemoteDriver.Url = "https://example.com/";
            RemoteDriver.Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Chrome)]
        public void LocalWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            switch (browser)
            {
                case Browser.Firefox:
                    {
                        LocalDriverTest<FirefoxDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<FirefoxDriver>;
                        f(browser);                        
                        break;
                    }
                case Browser.Chrome:
                    {
                        LocalDriverTest <ChromeDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<ChromeDriver>;
                        f(browser);
                        break;
                    }
                case Browser.InternetExplorer:
                    {
                        LocalDriverTest<InternetExplorerDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<InternetExplorerDriver>;
                        f(browser);
                        break;
                    }
                case Browser.Edge:
                    {
                        LocalDriverTest<EdgeDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<EdgeDriver>;
                        f(browser);
                        break;
                    }
                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }

        }

        [Test]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedWebDriverThrowsInformativeException(Browser browser)
        {
            Action act = () => this.WebDriverFactory.GetLocalWebDriver(browser);
            act.Should()
                .Throw<PlatformNotSupportedException>($"because {browser} is not supported on {Platform.CurrentPlatform}.")
                .WithMessage("*is only available on*");
        }

        [Test]
        [TestCase(Browser.Firefox )]
        [TestCase(Browser.Chrome)]
        public void HeadlessBrowsersCanBeLaunched(Browser browser)
        {
            switch (browser)
            {
                case Browser.Firefox:
                    {
                        LocalDriverTest<FirefoxDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<FirefoxDriver>;
                        f(browser, true);
                        break;
                    }
                case Browser.Chrome:
                    {
                        LocalDriverTest<ChromeDriver> f = CheckBrowserLaunchAndLoadExampleDotCom<ChromeDriver>;
                        f(browser, true);
                        break;
                    }
            }
        }
        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        public void HeadlessRemoteBrowsersCanBeLaunched(Browser browser)
        {

            RemoteDriverTest<CustomRemoteWebDriver> f = CheckBrowserLaunchRemoteAndLoadExampleDotCom<CustomRemoteWebDriver>;
            f(browser);

        }

        [Test]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedHeadlessBrowserThrowsInformativeException(Browser browser)
        {
            Action act = () => this.WebDriverFactory.GetLocalWebDriver(browser,true);
            act.Should()
                .ThrowExactly<ArgumentException>($"because headless mode is not supported on {browser}.")
                .WithMessage($"Headless mode is not currently supported for {browser}.");
        }

        [Test]
        [TestCase(Browser.Firefox)]
        public void HdBrowserIsOfRequestedSize(Browser browser)
        {
            var local = this.WebDriverFactory.GetLocalWebDriver(browser, false, WindowSize.Hd) as CustomLocalWebDriver<FirefoxDriver>;

            Assert.Multiple(() =>
            {
                Size size = local.GetLocalDriver().Manage().Window.Size;
                size.Width.Should().Be(1366);
                size.Height.Should().Be(768);
            });
            local.GetLocalDriver().Quit();
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Chrome)]
        public void RemoteWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            RemoteDriver = this.WebDriverFactory.GetRemoteWebDriver(browser) as CustomRemoteWebDriver;
            RemoteDriver.Url = "https://example.com/";
            RemoteDriver.Title.Should().Be("Example Domain");
        }


        [TearDown]
        public void Teardown()
        {
            if (LocalDriver != null)
                LocalDriver.GetLocalDriver()?.Quit();
            if (RemoteDriver != null)
                RemoteDriver?.Quit();
        }
    }
}
