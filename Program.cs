//from packages.config
/*
 * 
 * <package id="ChromePropertiesCollection.driver2.1" version="2.1.210652" targetFramework="net452" />
  <package id="NUnit" version="3.6.1" targetFramework="net452" />
  <package id="Selenium.Support" version="3.3.0" targetFramework="net452" />
  <package id="Selenium.WebPropertiesCollection.driver" version="3.3.0" targetFramework="net452" />
  <package id="Selenium.WebPropertiesCollection.driver.ChromePropertiesCollection.driver" version="2.29.0" targetFramework="net452" />
  <package id="WebPropertiesCollection.driverChromePropertiesCollection.driver" version="2.10" targetFramework="net452" />
 * 
*/


using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//for using phantomJS
using OpenQA.Selenium.PhantomJS;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace Selenium
{
    class Program
    {
        //creating the reference for our browser
        //IWebPropertiesCollection.driver PropertiesCollection.driver = new ChromePropertiesCollection.driver();

        static void Main(string[] args)
        {
            /*
            PropertiesCollection.driver = new ChromeDriver();
            PropertiesCollection.driver.Navigate().GoToUrl("https://www.pdfmerge.com/﻿");
            Console.WriteLine("Opened URL");
            */

            /*
            IWebDriver driver = new PhantomJSDriver();
            driver.Navigate().GoToUrl("https://www.google.com/");
            IWebElement element = driver.FindElement(By.Name("q"));
            element.SendKeys("blaaa");
            //GenericHelper.TakeScreenShot();
            string url = driver.Url;
            Console.WriteLine(url);
            Console.Read();
            
            driver.Close();
            */
            /*
            PhantomJSDriver driver = new PhantomJSDriver();

            driver.Manage().Window.Size = new System.Drawing.Size(1440, 1000);

            driver.Navigate().GoToUrl("http://nonlinearcreations.com");

            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(@"c:\tmp\img1.jpg", ImageFormat.Jpeg);

            driver.Quit();
            */

            

            /*
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", "c:\tmp");
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            //var driver = new ChromeDriver("Driver_Path", chromeOptions);

            var opts = new PhantomJSOptions();
            */
        }

        [SetUp]
        public void Initilize()
        {
            /*
            //Navigate to google page
            PropertiesCollection.driver.Navigate().GoToUrl("http://www.google.com");
            Console.WriteLine("Opened URL");
            */
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");

            //ChromeDriver chromeDriver = new ChromeDriver(options);
            //PropertiesCollection.driver = new ChromeDriver(options);
            PropertiesCollection.driver = new RemoteWebDriver(DesiredCapabilities.HtmlUnitWithJavaScript());

            //Navigate to google page
            //PropertiesCollection.driver.Navigate().GoToUrl("http://executeautomation.com/demosite/index.html?UserName=&amp;Password=&amp;Login=Login﻿");
            PropertiesCollection.driver.Navigate().GoToUrl("https://www.pdfmerge.com/﻿");
            Console.WriteLine("Opened URL");

            //using PhantomJS
            /*
            PropertiesCollection.driver = new PhantomJSDriver();
            PropertiesCollection.driver.Navigate().GoToUrl("https://www.pdfmerge.com/");
            Console.WriteLine("Opened URL");
            string url = PropertiesCollection.driver.Url;
            Console.WriteLine(url);
            Console.Read();
            */
            /*
            var driver = new PhantomJSDriver();
            driver.Navigate().GoToUrl("https://www.pdfmerge.com/");
            string url = driver.Url;
            Console.WriteLine(url);
            Console.Read();
            */
            
            /*
            PropertiesCollection.driver = new PhantomJSDriver();

            PropertiesCollection.driver.Manage().Window.Size = new System.Drawing.Size(1440, 1000);

            PropertiesCollection.driver.Navigate().GoToUrl("https://www.pdfmerge.com/");

            ((ITakesScreenshot)PropertiesCollection.driver).GetScreenshot().SaveAsFile(@"c:\tmp\img1.jpg", ImageFormat.Jpeg);
            */
        }

        
        

    [Test]
        public void ExecuteTest()
        {

            //first need to initliza the page by calling its reference
            EAPageObject page = new EAPageObject();

            //page.btnChooseFl.Click();

            page.fileUploads1.SendKeys("C:/Users/User/Desktop/OSF_CFP_2017[1071].pdf");
            page.fileUploads2.SendKeys("C:/Users/User/Desktop/OSF-CFP-2017.pdf");
            

            page.btnMerge.Click();

            
            /*
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile("https://www.pdfmerge.com/", "c:/tmp/pdf.pdf");
            }
            */

            /*
         * 
         WebElement El = driver.findElement(By.id("'fileUploadField'"));
         El.sendKeys("c:\\temp\\test.txt");
        */

            //can now directly pass text that we want to use
            //page.txtInitial.SendKeys("executeautomation");

            //page.btnSave.Click();

            /*
             * dont need these anymore since we created EAPageObject
            //selecting the title
            //going to call the custom method that i made
            SeleniumSetMethods.SelectDropDown( "TitleId", "Mr.", PropertyType.Id);

            //initial
            SeleniumSetMethods.EnterText("Initial", "executeautomation", PropertyType.Name);

            //testing out get method
            Console.WriteLine("The value from my Title is: " + SeleniumGetMethods.GetTextFromDropDown("TitleId", PropertyType.Id));

            Console.WriteLine("The value from my Initial is: " + SeleniumGetMethods.GetText("Initial", PropertyType.Name));

            //The click button
            SeleniumSetMethods.Click("Save", PropertyType.Name);
            /*

            
            

            /*
            //EnterText(element, value, type)

            //to find this you go to chrome, inspect elements after rigth clicking
            //and then find the text box and the name for the text box which is "q"
            IWebElement element = PropertiesCollection.driver.FindElement(By.Name("q"));

            //What will actually get send to the google search text box
            element.SendKeys("This is the shit!!!");

            Console.WriteLine("Executed test");
        */
        }

        /*
        [Test]
        public void NextTest()
        {
            Console.WriteLine("This is the next test");
        }
        */

        [TearDown]
        public void CleanUp()
        {
            //closes the browser for you after its done with everything
            Thread.Sleep(9900);
            PropertiesCollection.driver.Close();
            Console.WriteLine("Closed the browser");
        }

    }
}
