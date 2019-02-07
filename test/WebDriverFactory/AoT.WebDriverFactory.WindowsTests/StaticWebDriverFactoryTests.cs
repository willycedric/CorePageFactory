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
        private IWebDriver LocalDriver { get; set; }
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
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(browser) ;
            LocalDriver.Url = "https://example.com/";
            LocalDriver.Title.Should().Be("Example Domain");
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
            LocalDriver = StaticWebDriverFactory.GetLocalWebDriver(browser, true);
            LocalDriver.Url = "https://example.com/";
            LocalDriver.Title.Should().Be("Example Domain");
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
                LocalDriver?.Quit();
            }
           if(RemoteDriver !=null)
            {
                RemoteDriver?.Quit();
            }
        }
    }
}
