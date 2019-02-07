using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace AoT.WebDriverFactory
{
    /// <summary>
    /// Wrapper class that extends RemoteWebDriver implementing TakesScreenshot interface
    /// </summary>
    public class CustomRemoteWebDriver : RemoteWebDriver, ITakesScreenshot, ICustomWebDriver
    {
        public CustomRemoteWebDriver(Uri RemoteAdress, ICapabilities capabilities, TimeSpan commandTimeOut)
            : base(RemoteAdress, capabilities, commandTimeOut)
        {
        }

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public new Screenshot GetScreenshot()
        {
            // Get the screenshot as base64.
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();

            // ... and convert it.
            return new Screenshot(base64);
        }
    }

}

