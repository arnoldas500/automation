﻿using NUnit.Framework;
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
        //IWebPropertiesCollection.driver PropertiesCollection.driver = new ChromePropertiesCollection.driver();

        static void Main(string[] args)
        {
            

        }

        [SetUp]
        public void Initilize()
        {
            /*
            //Navigate to google page
            PropertiesCollection.driver.Navigate().GoToUrl("http://www.google.com");
            Console.WriteLine("Opened URL");
            */

            PropertiesCollection.driver = new ChromeDriver();

            //Navigate to google page
            PropertiesCollection.driver.Navigate().GoToUrl("http://executeautomation.com/demosite/index.html?UserName=&amp;Password=&amp;Login=Login﻿");
            Console.WriteLine("Opened URL");

        }

        [Test]
        public void ExecuteTest()
        {

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
            //EnterText(element, value, type)

            //to find this you go to chrome, inspect elements after rigth clicking
            //and then find the text box and the name for the text box which is "q"
            IWebElement element = PropertiesCollection.driver.FindElement(By.Name("q"));

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
            PropertiesCollection.driver.Close();
            Console.WriteLine("Closed the browser");
        }

    }
}
