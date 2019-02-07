using OpenQA.Selenium;
using System;

namespace AoT.WebDriverFactory
{
    public class CustomLocalWebDriver<TDriverIn> :  ITakesScreenshot, ICustomWebDriver
    {

        private TDriverIn _driver;

        public  CustomLocalWebDriver(TDriverIn driver)
        {
            _driver = driver;
        }
        public IWebDriver GetLocalDriver()
        {
            if(_driver == null)
            {
                throw new ArgumentNullException(" The local driver has not been initialized yet");
            }
            return _driver as IWebDriver;
        }
        

        /// <summary>
        /// object representing the image of the page on the screen.
        /// </summary>
        /// <returns></returns>
        public Screenshot GetScreenshot()
        {
            Screenshot screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            return screenshot;
        }
}

}
