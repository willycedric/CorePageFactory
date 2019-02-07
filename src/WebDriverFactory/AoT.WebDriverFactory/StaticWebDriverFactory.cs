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

        /// <summary>
        /// Return a local webdriver of the given browser type with default settings.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="headless"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(Browser browser, bool headless = false)
        {
            if (headless && !(browser == Browser.Chrome || browser == Browser.Firefox))
            {
                throw new ArgumentException($"Headless mode is not currently supported for {browser}.");
            }
            switch (browser)
            {
                case Browser.Firefox:
                    return GetLocalWebDriver(StaticDriverOptionsFactory.GetFirefoxOptions(headless));

                case Browser.Chrome:
                    return GetLocalWebDriver(StaticDriverOptionsFactory.GetChromeOptions(headless));

                case Browser.InternetExplorer:
                    return GetLocalWebDriver(StaticDriverOptionsFactory.GetInternetExplorerOptions());

                case Browser.Edge:
                    return GetLocalWebDriver(StaticDriverOptionsFactory.GetEdgeOptions());

                case Browser.Safari:
                    return GetLocalWebDriver(StaticDriverOptionsFactory.GetSafariOptions());

                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }
        }


        /// <summary>
        /// Return a Local Chrome WebDriver instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(ChromeOptions options, WindowSize windowSize = WindowSize.Hd)
        {
            
            CustomLocalWebDriver driver = new CustomLocalWebDriver(new ChromeDriver(DriverPath, options));
            return SetWindowSize<ICustomWebDriver>(driver, windowSize);
        }

      

        /// <summary>
        /// Return a local Firefox WebDriver instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(FirefoxOptions options, WindowSize windowSize = WindowSize.Hd)  {
            
            CustomLocalWebDriver driver = new CustomLocalWebDriver(new FirefoxDriver(DriverPath, options));
            return SetWindowSize<ICustomWebDriver>(driver, windowSize);
        }

        /// <summary>
        /// Return a local Edge WebDriver instance. (Only supported on Microsoft Windows 10)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(EdgeOptions options, WindowSize windowSize = WindowSize.Hd)
        {
            if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.WinNT))
            {
                throw new PlatformNotSupportedException("Microsoft Edge is only available on Microsoft Windows.");
            }

            CustomLocalWebDriver driver = new CustomLocalWebDriver(new EdgeDriver(DriverPath, options));
            return SetWindowSize<ICustomWebDriver>(driver, windowSize);
        }

        /// <summary>
        /// Return a local Internet Explorer WebDriver instance. (Only supported on Microsoft Windows)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(InternetExplorerOptions options, WindowSize windowSize = WindowSize.Hd)
        {
            if (!Platform.CurrentPlatform.IsPlatformType(PlatformType.WinNT))
            {
                throw new PlatformNotSupportedException("Microsoft Internet Explorer is only available on Microsoft Windows.");
            }

            CustomLocalWebDriver driver = new CustomLocalWebDriver(new InternetExplorerDriver(DriverPath, options)) ;
            return SetWindowSize<ICustomWebDriver>(driver, windowSize);
        }

        /// <summary>
        /// Return a local Safari WebDriver instance. (Only supported on Mac Os)
        /// </summary>
        /// <param name="options"></param>
        /// <param name="windowSize"></param>
        /// <returns></returns>
        public static ICustomWebDriver GetLocalWebDriver(SafariOptions options, WindowSize windowSize = WindowSize.Maximise)
        {
            //Platform.CurrentPlatform returns Unix on OSX so using the .Net Core RuntimeInformation class instead
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new PlatformNotSupportedException("Safari is only available on Mac Os.");
            }
            // I suspect that the SafariDriver is already on the path as it is within the Safari executable.
            // I currently have no means to test this
            CustomLocalWebDriver driver = new CustomLocalWebDriver(new SafariDriver(options));
            return SetWindowSize<ICustomWebDriver>(driver, windowSize);
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
        public static T SetWindowSize<T>(T driver, WindowSize windowSize)
        {

            switch (windowSize)
            {
                case WindowSize.Unchanged:
                    return driver;

                case WindowSize.Maximise:
                    if ( driver as ICustomWebDriver!=null)
                    {
                        var d = driver as CustomLocalWebDriver;
                        d.GetLocalDriver().Manage().Window.Maximize();
                        
                    }
                    else
                    {
                        var d = driver as CustomRemoteWebDriver;
                        d.Manage().Window.Maximize();
                        
                    }
                    return driver;
                case WindowSize.Hd:
                    if (driver as CustomLocalWebDriver != null)
                    {
                        var d = driver as CustomLocalWebDriver;
                        try
                        {
                            d.GetLocalDriver().Manage().Window.Position = Point.Empty;
                            d.GetLocalDriver().Manage().Window.Size = new Size(1366, 768);
                        }
                        catch (Exception)
                        {
                            d.GetLocalDriver().Manage().Window.Size = new Size(1366, 768);
                        }
                       
                    }
                    else
                    {
                        var d = driver as CustomRemoteWebDriver;
                        try
                        {
                            d.Manage().Window.Position = Point.Empty;
                            d.Manage().Window.Size = new Size(1366, 768);
                        }
                        catch (Exception)
                        {
                            d.Manage().Window.Size = new Size(1366, 768);
                        }
                       

                    }
                    return driver;
                case WindowSize.Fhd:                  
                    if ( driver as CustomLocalWebDriver !=null)
                    {
                        var d = driver as CustomLocalWebDriver;
                        try
                        {
                            d.GetLocalDriver().Manage().Window.Position = Point.Empty;
                            d.GetLocalDriver().Manage().Window.Size = new Size(1920, 1080);
                        }
                        catch (Exception)
                        {
                            d.GetLocalDriver().Manage().Window.Size = new Size(1920, 1080);
                        }
                        
                    }
                    else
                    {
                        var d = driver as CustomRemoteWebDriver;
                        try
                        {
                            d.Manage().Window.Position = Point.Empty;
                            d.Manage().Window.Size = new Size(1920, 1080);
                        }
                        catch (Exception)
                        {
                            d.Manage().Window.Size = new Size(1920, 1080);
                        }
                       
                    }
                    
                    return driver;

                default:
                    return driver;
            }
        }
    }
}
