﻿using System;
using System.Configuration;
using Aliases.Common.Configuration;
using Aliases.Common.Output;
using Aliases.Common.Shared;
using Aliases.Common.Shared.Enumerations;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Safari;

namespace Aliases.Drivers.Selenium.Configuration
{
    public class SeleniumTestConfiguration : ITestConfiguration, ICloneable
    {
        public string BaseTestUrl { get; set; }

        public EnvironmentType? TestEnvironmentType { get; set; }

        public ITestableWebPage BaseTestPageType { get; set; }

        public Browser Browser { get; set; }

        public ITestOutput TestOutput { get; set; }

        public SeleniumTestConfiguration() {}

        public SeleniumTestConfiguration(string baseTestUrl, EnvironmentType? testEnvironmentType, ITestableWebPage baseTestPageType, ITestOutput testOutput, int actionTimeout)
        {
            BaseTestUrl = baseTestUrl;
            TestEnvironmentType = testEnvironmentType;
            BaseTestPageType = baseTestPageType;
            TestOutput = testOutput;

            BaseTestPageType.DefaultActionTimeout = actionTimeout;
        }

        /// <summary>
        /// Builds the test configuration based on either environment variables or app.config setup
        /// 
        /// Looks for process level environment variables first, if those do not exist it will use the app.config entries
        /// </summary>
        /// <param name="testOutput"></param>
        /// <param name="baseTestUrl"></param>
        /// <param name="testEnvironmentType"></param>
        /// <param name="baseTestPageType"></param>
        /// <returns></returns>
        public ITestConfiguration Create(ITestOutput testOutput, string baseTestUrl = null, EnvironmentType? testEnvironmentType = null, ITestableWebPage baseTestPageType = null)
        {
            //Set Test base url
            if (string.IsNullOrWhiteSpace(baseTestUrl))
            {
                baseTestUrl = Environment.GetEnvironmentVariable("TestAutomationUrl", EnvironmentVariableTarget.Process) ?? ConfigurationManager.AppSettings["TestAutomationUrl"];
            }

            //Set Test EnvironmentType
            if (!testEnvironmentType.HasValue)
            {
                string environment = Environment.GetEnvironmentVariable("TestAutomationEnvironment", EnvironmentVariableTarget.Process) ?? ConfigurationManager.AppSettings["TestAutomationEnvironment"];

                EnvironmentType environmentType;
                Enum.TryParse(environment, true, out environmentType);

                testEnvironmentType = environmentType;
            }
            
            Browser browserType = Browser.Chrome;

            //Set Page Type
            if (baseTestPageType == null)
            {
                string browser = Environment.GetEnvironmentVariable("TestAutomationBrowser", EnvironmentVariableTarget.Process) ?? ConfigurationManager.AppSettings["TestAutomationBrowser"];

                Enum.TryParse(browser, true, out browserType);

                IWebDriver driver = GetWebDriver(browserType);
                baseTestPageType = new SeleniumWebPage(driver);
            }
            
            return new SeleniumTestConfiguration(baseTestUrl, testEnvironmentType, baseTestPageType, testOutput, 60)
            {
                Browser = browserType
            };
        }

        public virtual IWebDriver GetWebDriver(Browser browser)
        {
            IWebDriver driver;
            switch (browser)
            {

                case Browser.IE:
                    driver = StartIE();
                    break;
                case Browser.Firefox:
                    driver = StartFirefox();
                    break;
                case Browser.Chrome:
                    driver = StartChrome();
                    break;
                case Browser.Safari:
                    driver = StartSafari();
                    break;
                case Browser.PhantomJS:
                    driver = StartPhantom();
                    break;
                default:
                    driver = StartChrome();
                    break;
            }

            driver.Manage().Cookies.DeleteAllCookies();

            return driver;
        }

        public virtual IWebDriver StartFirefox()
        {
            var profile = GetFirefoxProfile();

            if (profile != null)
            {
                return new FirefoxDriver(profile);
            }

            return new FirefoxDriver();
        }

        public virtual FirefoxProfile GetFirefoxProfile()
        {
            return null;
        }

        public virtual IWebDriver StartChrome()
        {
            //I hate disabling the extensions but a popup window sometimes jumps out on tests in windows if you don't
            var options = GetChromeOptions();

            if (options != null)
            {
                return new ChromeDriver(options);
            }

            return new ChromeDriver();
        }

        public virtual ChromeOptions GetChromeOptions()
        {
            //I hate disabling the extensions but a popup window sometimes jumps out on tests in windows if you don't
            var options = new ChromeOptions();
            options.AddArguments("chrome.switches", "--disable-extensions");

            return options;
        }

        public virtual IWebDriver StartIE()
        {
            var options = GetIEOptions();

            if (options != null)
            {
                return new InternetExplorerDriver(options);
            }

            return new InternetExplorerDriver();
        }

        public virtual InternetExplorerOptions GetIEOptions()
        {
            var options = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                IgnoreZoomLevel = true
            };

            return options;
        }

        public virtual IWebDriver StartSafari()
        {
            var options = GetSafariOptions();

            if (options != null)
            {
                return new SafariDriver(options);
            }

            return new SafariDriver();
        }

        public virtual SafariOptions GetSafariOptions()
        {
            return null;
        }

        public virtual IWebDriver StartPhantom()
        {
            var options = GetPhantomOptions();

            if (options != null)
            {
                return new PhantomJSDriver(options);
            }

            return new PhantomJSDriver();
        }

        public virtual PhantomJSOptions GetPhantomOptions()
        {
            return null;
        }

        public virtual void Dispose()
        {
            BaseTestPageType.Dispose();
        }

        public virtual object Clone()
        {
            return new SeleniumTestConfiguration(BaseTestUrl, TestEnvironmentType, BaseTestPageType.Clone() as ITestableWebPage, TestOutput, BaseTestPageType.DefaultActionTimeout)
            {
                Browser = Browser
            };
        }
    }
}
