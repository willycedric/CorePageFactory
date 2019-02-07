using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace AoT.WebDriverFactory
{
    public interface IWebDriverFactory
    {
        string DriverPath
        {
            get;
            set;
        }

        Uri GridUri
        {
            get;
            set;
        }

        /// <summary>
        /// Return a local webdriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="headless"></param>
        /// <returns></returns>
        ICustomWebDriver GetLocalWebDriver(Browser browser,  bool headless = false, WindowSize size = WindowSize.Maximise);

        /// <summary>
        /// Return a RemoteWebDriver of the given browser type with default settings.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="gridUrl"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        ICustomWebDriver GetRemoteWebDriver(DriverOptions options,
            Uri gridUrl = null,
            WindowSize windowSize = WindowSize.Hd);

        /// <summary>
        /// Return a configured RemoteWebDriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="gridUrl"></param>
        /// <param name="platformType"></param>
        /// <returns></returns>
        ICustomWebDriver GetRemoteWebDriver(Browser browser,
            Uri gridUrl = null,
            PlatformType platformType = PlatformType.Any, bool headless=false);

        /// <summary>
        /// Convenience method for setting the Window Size of a WebDriver to common values. (768P, 1080P and fullscreen)
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        ICustomWebDriver SetWindowSize<Toutput>(ICustomWebDriver driver, WindowSize windowSize);
    }
}

