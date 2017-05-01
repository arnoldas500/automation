using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium
{
    class EAPageObject
    {

        //creating constructor/ objects for this particular page
        public EAPageObject()
        {
            PageFactory.InitElements(PropertiesCollection.driver, this);
        }

        //for drop down
        [FindsBy(How = How.Id, Using ="TitleId")]
        public IWebElement ddlTitleID { get; set; }

        //for text box
        [FindsBy(How = How.Name, Using = "Initial")]
        public IWebElement txtInitial { get; set; }

        //for button
        [FindsBy(How = How.Name, Using = "Save")]
        public IWebElement btnSave { get; set; }

    }
}
