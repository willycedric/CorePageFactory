using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AoT.WebDriverFactory.LinuxTests
{
    [TestFixture]
    public class WebDriverFactoryInstanceTests
    {
        private IWebDriver Driver { get; set; }
        private readonly PlatformType thisPlatformType = PlatformType.Linux;
        private IWebDriverFactory WebDriverFactory { get; set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            Assume.That(() => Platform.CurrentPlatform.IsPlatformType(thisPlatformType));
            this.WebDriverFactory = new DefaultWebDriverFactory();
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        public void LocalWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            Driver = this.WebDriverFactory.GetLocalWebDriver(browser);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedWebDriverThrowsInformativeException(Browser browser)
        {
            Action act = () => this.WebDriverFactory.GetLocalWebDriver(browser);
            act.Should()
                .Throw<PlatformNotSupportedException>($"because {browser} is not supported on {thisPlatformType}.")
                .WithMessage("*is only available on*");
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        public void HeadlessBrowsersCanBeLaunched(Browser browser)
        {
            Driver = this.WebDriverFactory.GetLocalWebDriver(browser, true);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedHeadlessBrowserThrowsInformativeException(Browser browser)
        {
            Action act = () => this.WebDriverFactory.GetLocalWebDriver(browser, true);
            act.Should()
                .ThrowExactly<ArgumentException>($"because headless mode is not supported on {browser}.")
                .WithMessage($"Headless mode is not currently supported for {browser}.");
        }

        [Test]
        public void HdBrowserIsOfRequestedSize()
        {
            Driver = this.WebDriverFactory.GetLocalWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(true), WindowSize.Hd);

            Assert.Multiple(() =>
            {
                Size size = Driver.Manage().Window.Size;
                size.Width.Should().Be(1366);
                size.Height.Should().Be(768);
            });
        }

        [Test]
        public void FhdBrowserIsOfRequestedSize()
        {
            Driver = this.WebDriverFactory.GetLocalWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(true), WindowSize.Fhd);

            Assert.Multiple(() =>
            {
                Size size = Driver.Manage().Window.Size;
                size.Height.Should().Be(1080);
                size.Width.Should().Be(1920);
            });
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Chrome)]
        public void RemoteWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            Driver = this.WebDriverFactory.GetRemoteWebDriver(browser, null, PlatformType.Windows);
            Driver.Url = "https://example.com/";
            Driver.Title.Should().Be("Example Domain");
        }


        [TearDown]
        public void Teardown()
        {
            Driver?.Quit();
        }
    }
}
