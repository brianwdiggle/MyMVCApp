using System;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;


namespace SeleniumTests
{
    using System.Threading;

    /// <summary>
    /// Uses Selenium 2 (Webdriver) and NUNit
    /// the tests in here which exercize jQuery autocomplete have not been implemented correctly
    /// </summary>
    [TestFixture]
    public class AddWalkFirefoxTests
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest()
        {
            driver = new FirefoxDriver();
            baseURL = "http://localhost:4845/";
            verificationErrors = new StringBuilder();
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        
        [Test]
        public void TheAddWalkOverallSpeedAutoCalculationTest()
        {
            // open | / | 
            driver.Navigate().GoToUrl(baseURL + "/");
            // click | link=Add Walk | 
            driver.FindElement(By.LinkText("Add Walk")).Click();
            // sendKeys | id=CartographicLength | 20
            driver.FindElement(By.Id("CartographicLength")).SendKeys("20");
            // sendKeys | id=total_time_hours | 10
            driver.FindElement(By.Id("total_time_hours")).SendKeys("10");
            // click | id=total_time_mins | 
            driver.FindElement(By.Id("total_time_mins")).Click();
            // sendKeys | id=total_time_mins | 0
            driver.FindElement(By.Id("total_time_mins")).SendKeys("0");
            // click | id=WalkAverageSpeedKmh | 
            driver.FindElement(By.Id("WalkAverageSpeedKmh")).Click();
            // focus | id=WalkAverageSpeedKmh | 2.0
            // ERROR: Caught exception [ERROR: Unsupported command [focus | id=WalkAverageSpeedKmh | ]]
        }

        [Test]
        public void TheAddWalkWalkAreaTest()
        {
            // open | / | 
            driver.Navigate().GoToUrl(baseURL + "/");
            // click | link=Add Walk | 
            driver.FindElement(By.LinkText("Add Walk")).Click();
            // sendKeys | id=WalkAreaName | iona

            driver.FindElement(By.Id("WalkAreaName")).Click();
            driver.FindElement(By.Id("WalkAreaName")).SendKeys("sca");



            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            IWebElement myDynamicElement = wait.Until<IWebElement>((d) =>
            {
                return d.FindElement(By.ClassName("ac_results"));
            });

           
            driver.FindElement(By.ClassName("ac_results")).SendKeys(Keys.ArrowDown);

        
            driver.FindElement(By.ClassName("ac_results")).SendKeys(Keys.Enter);


            Thread.Sleep(1000);
            // assertValue | id=WalkAreaName | Iona, Type:I, Ref:I44
            Assert.AreEqual("Iona, Type:I, Ref:I44", driver.FindElement(By.Id("WalkAreaName")).GetAttribute("value"));
            // assertValue | id=WalkAreaID | I44
            Assert.AreEqual("I44", driver.FindElement(By.Id("WalkAreaID")).GetAttribute("value"));
        }

        [Test]
        public void TheAddWalkCircularWalkStartSameAsEndTest()
        {
            // open | / | 
            driver.Navigate().GoToUrl(baseURL + "/");
            // click | link=Add Walk | 
            driver.FindElement(By.LinkText("Add Walk")).Click();
            // select | id=WalkTypes | label=Valley - Circular
            new SelectElement(driver.FindElement(By.Id("WalkTypes"))).SelectByText("Valley - Circular");
            // sendKeys | id=WalkStartPoint | Car Park
            driver.FindElement(By.Id("WalkStartPoint")).SendKeys("Car Park");
            // assertValue | id=WalkEndPoint | Car Park
            Assert.AreEqual("Car Park", driver.FindElement(By.Id("WalkEndPoint")).GetAttribute("value"));
        }

        [Test]
        public void TheAddWalkVisitedSummit1AutocompleteTest()
        {
            // open | /HillAscent/Index?OrderBy=DateDesc | 
            driver.Navigate().GoToUrl(baseURL + "/HillAscent/Index?OrderBy=DateDesc");
            // click | link=Add Walk | 
            driver.FindElement(By.LinkText("Add Walk")).Click();
            // click | VisitedSummit1 | 
            // ERROR: Caught exception [Error: locator strategy either id or name must be specified explicitly.]
            // sendKeys | id=VisitedSummit1 | kidsty
            driver.FindElement(By.Id("VisitedSummit1")).SendKeys("kidsty");
            // click | css=li.ac_even > strong | 
            driver.FindElement(By.CssSelector("li.ac_even > strong")).Click();
            // assertValue | id=VisitedSummit1 | Kidsty Pike, 780m, 2559ft, NY447125, N W B
            Assert.AreEqual("Kidsty Pike, 780m, 2559ft, NY447125, N W B", driver.FindElement(By.Id("VisitedSummit1")).GetAttribute("value"));
            // assertVisible | id=VisitedSummit2 | 
            Assert.IsTrue(driver.FindElement(By.Id("VisitedSummit2")).Displayed);
            // assertValue | id=VisitedSummit1HillID | 2536
            Assert.AreEqual("2536", driver.FindElement(By.Id("VisitedSummit1HillID")).GetAttribute("value"));
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}
