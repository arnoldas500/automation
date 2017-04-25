using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium
{
    class Program
    {
        //creating the reference for our browser
        IWebDriver driver = new ChromeDriver();

        static void Main(string[] args)
        {
            

        }

        [SetUp]
        public void Initilize()
        {
            /*
            //Navigate to google page
            driver.Navigate().GoToUrl("http://www.google.com");
            Console.WriteLine("Opened URL");
            */

            //Navigate to google page
            driver.Navigate().GoToUrl("http://executeautomation.com/demosite/index.html?UserName=&amp;Password=&amp;Login=Login﻿");
            Console.WriteLine("Opened URL");

        }

        [Test]
        public void ExecuteTest()
        {

            //selecting the title
            //going to call the custom method that i made
            SeleniumSetMethods.SelectDropDown(driver, "TitleId", "Mr.", "Id");

            //initial
            SeleniumSetMethods.EnterText(driver, "Initial", "executeautomation", "Name");

            //testing out get method
            Console.WriteLine("The value from my Title is:" + SeleniumGetMethods.GetText(driver, "TitleId","Id"));

            Console.WriteLine("The value from my Initial is:" + SeleniumGetMethods.GetText(driver, "Initial", "Name"));

            //The click button
            SeleniumSetMethods.Click(driver, "Save", "Name");


            /*
            //EnterText(element, value, type)

            //to find this you go to chrome, inspect elements after rigth clicking
            //and then find the text box and the name for the text box which is "q"
            IWebElement element = driver.FindElement(By.Name("q"));

            //What will actually get send to the google search text box
            element.SendKeys("This is the shit!!!");

            Console.WriteLine("Executed test");
        */
        }

        [Test]
        public void NextTest()
        {
            Console.WriteLine("This is the next test");
        }

        [TearDown]
        public void CleanUp()
        {
            //closes the browser for you after its done with everything
            driver.Close();
            Console.WriteLine("Closed the browser");
        }

    }
}
