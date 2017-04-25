using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace Selenium
{
    class SeleniumSetMethods
    {

        //enter text
        public static void EnterText(IWebDriver driver, string element, string value, string elementType)
        {
            if(elementType == "Id")
                driver.FindElement(By.Id(element)).SendKeys(value);
            if (elementType == "Name")
                driver.FindElement(By.Name(element)).SendKeys(value);
        }

        //click operation inro a button, checkbox, option, etc
        public static void Click(IWebDriver driver, string element, string elementType)
        {
            if (elementType == "Id")
                driver.FindElement(By.Id(element)).Click();
            if (elementType == "Name")
                driver.FindElement(By.Name(element)).Click();
        }

        //method for selecting a drop down control
        public static void SelectDropDown(IWebDriver driver, string element, string value, string elementType)
        {
            //SelectElement selectElement = new SelectElement();
            if (elementType == "Id")
                new SelectElement(driver.FindElement(By.Id(element))).SelectByText(value);
            if (elementType == "Name")
                new SelectElement(driver.FindElement(By.Name(element))).SelectByText(value);
        }


    }
}
