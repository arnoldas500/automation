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
        public static void EnterText(string element, string value, PropertyType elementType)
        {
            if(elementType == PropertyType.Id)
                PropertiesCollection.driver.FindElement(By.Id(element)).SendKeys(value);
            if (elementType == PropertyType.Name)
                PropertiesCollection.driver.FindElement(By.Name(element)).SendKeys(value);
        }

        //click operation inro a button, checkbox, option, etc
        public static void Click(string element, PropertyType elementType)
        {
            if (elementType == PropertyType.Id)
                PropertiesCollection.driver.FindElement(By.Id(element)).Click();
            if (elementType == PropertyType.Name)
                PropertiesCollection.driver.FindElement(By.Name(element)).Click();
        }

        //method for selecting a drop down control
        public static void SelectDropDown(string element, string value, PropertyType elementType)
        {
            //SelectElement selectElement = new SelectElement();
            if (elementType == PropertyType.Id)
                new SelectElement(PropertiesCollection.driver.FindElement(By.Id(element))).SelectByText(value);
            if (elementType == PropertyType.Name)
                new SelectElement(PropertiesCollection.driver.FindElement(By.Name(element))).SelectByText(value);
        }


    }
}
