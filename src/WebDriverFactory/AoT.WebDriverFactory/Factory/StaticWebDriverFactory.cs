using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;

namespace AoT.WebDriverFactory
{
    public static class StaticWebDriverFactory
    {
        private static string DriverPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static TimeSpan DefaultTimeOut = TimeSpan.FromSeconds(10);

        private delegate void SetDriverWindowSize(ICustomWebDriver driver, WindowSize size);

        private static void SetLocalDriverWindowsSize(ICustomWebDriver driver, WindowSize size)
        {
            if(driver is CustomRemoteWebDriver)
            {
                var d = driver as CustomRemoteWebDriver;
                //d.Manage().Window.Size = 
            }
        }
        /// <summary>
        /// Return a local webdriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="headless"></param>
        /// <returns></returns>
        public static IWebDriver GetLocalWebDriver(Browser browser, bool headless = false)
        {
            if (headless && !(browser == Browser.Chrome || browser == Browser.Firefox))
            {
                throw new ArgumentException($"Headless mode is not currently supported for {browser}.");
            }
            switch (browser)
            {
                case Browser.Firefox:                    
                    var firefoxDriver= new CustomLocalWebDriver<FirefoxDriver>(new FirefoxDriver(DriverPath, StaticDriverOptionsFactory.GetFirefoxOptions(headless)));
                    return firefoxDriver.GetLocalDriver();
                case Browser.Chrome:
                    var chromeDriver = new CustomLocalWebDriver<ChromeDriver>(new ChromeDriver(DriverPath, StaticDriverOptionsFactory.GetChromeOptions(headless)));
                    return chromeDriver.GetLocalDriver();
                case Browser.InternetExplorer:
                    var ieDriver = new CustomLocalWebDriver<InternetExplorerDriver>(new InternetExplorerDriver(DriverPath, StaticDriverOptionsFactory.GetInternetExplorerOptions()));
                    return ieDriver.GetLocalDriver();
                case Browser.Edge:
                    var edgeDriver = new CustomLocalWebDriver<EdgeDriver>(new EdgeDriver(DriverPath, StaticDriverOptionsFactory.GetEdgeOptions()));
                    return edgeDriver.GetLocalDriver();
                case Browser.Safari:
                    //Platform.CurrentPlatform returns Unix on OSX so using the .Net Core RuntimeInformation class instead
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        throw new PlatformNotSupportedException($"because {browser} is not supported on {Platform.CurrentPlatform}, is only available on OSX");
                    }
                    var safariDriver = new CustomLocalWebDriver<SafariDriver>(new SafariDriver(DriverPath, StaticDriverOptionsFactory.GetSafariOptions()));
                    return safariDriver.GetLocalDriver();
                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }
        }
        /// <summary>
        /// Return a RemoteWebDriver of the given browser type with default settings.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="gridUrl"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetRemoteWebDriver(
            DriverOptions options,
            Uri gridUrl,
            WindowSize windowSize = WindowSize.Hd)
        {
            CustomRemoteWebDriver driver = new CustomRemoteWebDriver(gridUrl, options.ToCapabilities(), DefaultTimeOut);
            return SetWindowSize<CustomRemoteWebDriver>(driver, windowSize);
        }

        /// <summary>
        /// Return a configured RemoteWebDriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="gridUrl"></param>
        /// <param name="platformType"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetRemoteWebDriver(
            Browser browser,
            Uri gridUrl,
            PlatformType platformType = PlatformType.Any, bool headless=false)
        {
            switch (browser)
            {
                case Browser.Firefox:
                    return GetRemoteWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(platformType), gridUrl);

                case Browser.Chrome:
                    return GetRemoteWebDriver(StaticDriverOptionsFactory.GetChromeOptions(platformType), gridUrl);

                case Browser.InternetExplorer:
                    return GetRemoteWebDriver(StaticDriverOptionsFactory.GetInternetExplorerOptions(platformType), gridUrl);

                case Browser.Edge:
                    return GetRemoteWebDriver(StaticDriverOptionsFactory.GetEdgeOptions(platformType), gridUrl);

                case Browser.Safari:
                    return GetRemoteWebDriver(StaticDriverOptionsFactory.GetSafariOptions(platformType), gridUrl);

                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }
        }

        /// <summary>
        /// Convenience method for setting the Window Size of a WebDriver to common values. (768P, 1080P and fullscreen)
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver SetWindowSize<ToutPut>(ICustomWebDriver driver, WindowSize windowSize)
        {
            if (driver is CustomLocalWebDriver<ToutPut>)
            {
                var d = driver as CustomLocalWebDriver<ToutPut>;
                switch (windowSize)
                {
                    case WindowSize.Unchanged:
                        return driver;

                    case WindowSize.Maximise:
                        d.GetLocalDriver().Manage().Window.Maximize();
                        return driver;
                    case WindowSize.Hd:
                        try
                        {
                            d.GetLocalDriver().Manage().Window.Position = Point.Empty;
                            d.GetLocalDriver().Manage().Window.Size = new Size(1366, 768);
                        }
                        catch (Exception)
                        {
                            d.GetLocalDriver().Manage().Window.Size = new Size(1366, 768);
                        }

                        return driver;

                    case WindowSize.Fhd:
                        try
                        {
                            d.GetLocalDriver().Manage().Window.Position = Point.Empty;
                            d.GetLocalDriver().Manage().Window.Size = new Size(1920, 1080);
                        }
                        catch (Exception)
                        {
                            d.GetLocalDriver().Manage().Window.Size = new Size(1920, 1080);
                        }
                        return driver;

                    default:
                        return driver;
                }

            }
            else if(driver is CustomRemoteWebDriver)
            {
                var d = driver as CustomRemoteWebDriver;
                switch (windowSize)
                {
                    case WindowSize.Unchanged:
                        return driver;

                    case WindowSize.Maximise:
                        d.Manage().Window.Maximize();
                        return driver;
                    case WindowSize.Hd:
                        try
                        {
                            d.Manage().Window.Position = Point.Empty;
                            d.Manage().Window.Size = new Size(1366, 768);
                        }
                        catch (Exception)
                        {
                            d.Manage().Window.Size = new Size(1366, 768);
                        }

                        return driver;

                    case WindowSize.Fhd:
                        try
                        {
                            d.Manage().Window.Position = Point.Empty;
                            d.Manage().Window.Size = new Size(1920, 1080);
                        }
                        catch (Exception)
                        {
                            d.Manage().Window.Size = new Size(1920, 1080);
                        }
                        return driver;

                    default:
                        return driver;
                }

            }
            else
            {
                return driver;
            }
        }
    }
}
