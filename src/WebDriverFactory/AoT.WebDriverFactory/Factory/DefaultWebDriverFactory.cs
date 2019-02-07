using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace AoT.WebDriverFactory
{
    public class DefaultWebDriverFactory : IWebDriverFactory
    {
        public DefaultWebDriverFactory(Uri gridUri = null, IDriverOptionsFactory driverOptionsFactory = null, string driverPath = null)
        {
            GridUri = gridUri?? new Uri("http://localhost:4444/wd/hub");
            DriverOptionsFactory = driverOptionsFactory ?? new DefaultDriverOptionsFactory();
            DriverPath = driverPath?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string DriverPath { get; set; }

        public Uri GridUri { get; set; }

        public IDriverOptionsFactory DriverOptionsFactory { get; set; }

        //@TODO: understand the difference between this method and  the one ine StaticWebDriverFactory
        //Futhermore understant the difference between those two factories
        public virtual ICustomWebDriver GetLocalWebDriver(Browser browser, bool headless = false, WindowSize windowSize = WindowSize.Maximise)
        {
            if (headless && !(browser == Browser.Chrome || browser == Browser.Firefox))
            {
                throw new ArgumentException($"Headless mode is not currently supported for {browser}.");
            }
            switch (browser)
            {
                case Browser.Firefox:
                    var customFirefox = new CustomLocalWebDriver<FirefoxDriver>(new FirefoxDriver(StaticDriverOptionsFactory.GetFirefoxOptions(headless)));
                    return SetWindowSize<FirefoxDriver>(customFirefox, windowSize);
                case Browser.Chrome:
                    var customChrome = new CustomLocalWebDriver<ChromeDriver>(new ChromeDriver(StaticDriverOptionsFactory.GetChromeOptions(headless)));
                    return SetWindowSize<ChromeDriver>(customChrome, windowSize);
                case Browser.InternetExplorer:   
                    var customIE = new CustomLocalWebDriver<InternetExplorerDriver>(new InternetExplorerDriver(StaticDriverOptionsFactory.GetInternetExplorerOptions()));
                    return SetWindowSize<InternetExplorerDriver>(customIE, windowSize);
                case Browser.Edge:          
                    var customEdge = new CustomLocalWebDriver<EdgeDriver>(new EdgeDriver(StaticDriverOptionsFactory.GetEdgeOptions()));
                    return SetWindowSize<EdgeDriver>(customEdge, windowSize);
                case Browser.Safari:
                    //Platform.CurrentPlatform returns Unix on OSX so using the .Net Core RuntimeInformation class instead
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        throw new PlatformNotSupportedException($"because {browser} is not supported on {Platform.CurrentPlatform}, is only available on OSX");
                    }
                    var customSafari = new CustomLocalWebDriver<SafariDriver>(new SafariDriver(StaticDriverOptionsFactory.GetSafariOptions()));
                    return SetWindowSize<SafariDriver>(customSafari, windowSize);
                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }
        }



        public virtual ICustomWebDriver GetRemoteWebDriver(DriverOptions options,
            Uri gridUrl,
            WindowSize windowSize = WindowSize.Hd)
        {
            return StaticWebDriverFactory.GetRemoteWebDriver(options, gridUrl, windowSize);
        }

        public virtual ICustomWebDriver GetRemoteWebDriver(Browser browser,
            Uri gridUrl = null,
            PlatformType platformType = PlatformType.Any, bool headless=false)
        {
            Uri actualGridUrl = gridUrl ?? GridUri;
            switch (browser)
            {
                case Browser.Firefox:
                    return GetRemoteWebDriver(DriverOptionsFactory.GetFirefoxOptions(headless,platformType), actualGridUrl);

                case Browser.Chrome:
                    return GetRemoteWebDriver(DriverOptionsFactory.GetChromeOptions(headless, platformType), actualGridUrl);

                case Browser.InternetExplorer:
                    return GetRemoteWebDriver(DriverOptionsFactory.GetInternetExplorerOptions(platformType), actualGridUrl);

                case Browser.Edge:
                    return GetRemoteWebDriver(DriverOptionsFactory.GetEdgeOptions(platformType), actualGridUrl);

                case Browser.Safari:
                    //Platform.CurrentPlatform returns Unix on OSX so using the .Net Core RuntimeInformation class instead
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        throw new PlatformNotSupportedException($"because {browser} is not supported on {Platform.CurrentPlatform}.");
                    }
                    return GetRemoteWebDriver(DriverOptionsFactory.GetSafariOptions(platformType), actualGridUrl);

                default:
                    throw new PlatformNotSupportedException($"{browser} is not currently supported.");
            }
        }

        public virtual ICustomWebDriver SetWindowSize<Toutput>(ICustomWebDriver driver, WindowSize windowSize)
        {
            return StaticWebDriverFactory.SetWindowSize<Toutput>(driver, windowSize);
        }
    }
}
