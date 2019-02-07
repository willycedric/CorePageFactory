using System;
using System.Drawing;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AoT.WebDriverFactory.WindowsTests
{
    [TestFixture]
    public class StaticWebDriverFactoryTests
    {
        private CustomLocalWebDriver LocalDriver { get; set; }
        private CustomRemoteWebDriver RemoteDriver { get; set; }
        private readonly PlatformType thisPlatformType = PlatformType.Windows;

        [OneTimeSetUp]
        public void CheckForValidPlatform()
        {
            Assume.That(() => Platform.CurrentPlatform.IsPlatformType(thisPlatformType));
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.Chrome)]
        public void LocalWebDriverCanBeLaunchedAndLoadExampleDotCom(Browser browser)
        {
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(browser) as CustomLocalWebDriver;
            LocalDriver.GetLocalDriver().Url = "https://example.com/";
            LocalDriver.GetLocalDriver().Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedWebDriverThrowsInformativeException(Browser browser)
        {
            Action act = () => StaticWebDriverFactory.GetLocalWebDriver(browser);
            act.Should()
                .Throw<PlatformNotSupportedException>($"because {browser} is not supported on {thisPlatformType}.")
                .WithMessage("*is only available on*");
        }

        [Test]
        [TestCase(Browser.Firefox)]
        [TestCase(Browser.Chrome)]
        public void HeadlessBrowsersCanBeLaunched(Browser browser)
        {
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(browser, true) as CustomLocalWebDriver;
            LocalDriver.GetLocalDriver().Url = "https://example.com/";
            LocalDriver.GetLocalDriver().Title.Should().Be("Example Domain");
        }

        [Test]
        [TestCase(Browser.Edge)]
        [TestCase(Browser.InternetExplorer)]
        [TestCase(Browser.Safari)]
        public void RequestingUnsupportedHeadlessBrowserThrowsInformativeException(Browser browser)
        {
            Action act = () => StaticWebDriverFactory.GetLocalWebDriver(browser, true);
            act.Should()
                .ThrowExactly<ArgumentException>($"because headless mode is not supported on {browser}.")
                .WithMessage($"Headless mode is not currently supported for {browser}.");
        }

        [Test]
        public void HdBrowserIsOfRequestedSize()
        {
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(true), WindowSize.Hd) as CustomLocalWebDriver;

            Assert.Multiple(() =>
            {
                Size size = LocalDriver.GetLocalDriver().Manage().Window.Size;
                size.Width.Should().Be(1366);
                size.Height.Should().Be(768);
            });
        }

        [Test]
        public void FhdBrowserIsOfRequestedSize()
        {
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(true), WindowSize.Fhd) as CustomLocalWebDriver;

            Assert.Multiple(() =>
            {
                Size size = LocalDriver.GetLocalDriver().Manage().Window.Size;
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
            RemoteDriver = StaticWebDriverFactory.GetRemoteWebDriver(browser, new Uri("http://localhost:4444/wd/hub"), PlatformType.Any) as CustomRemoteWebDriver;
            RemoteDriver.Url = "https://example.com/";
            RemoteDriver.Title.Should().Be("Example Domain");
        }


        [TearDown]
        public void Teardown()
        {
           if(LocalDriver!=null)
            {
                LocalDriver.GetLocalDriver()?.Quit();
            }
           if(RemoteDriver !=null)
            {
                RemoteDriver?.Quit();
            }
        }
    }
}
