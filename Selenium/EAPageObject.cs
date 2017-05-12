//Arnoldas Kurbanovas
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Secret_Sharing
{
    class EAPageObject
    {

        //creating constructor/ objects for this particular page
        public EAPageObject()
        {
            PageFactory.InitElements(PropertiesCollection.driver, this);
        }

        /*
         * 
         WebElement El = driver.findElement(By.id("'fileUploadField'"));
         El.sendKeys("c:\\temp\\test.txt");
        */

        //for drop down
        [FindsBy(How = How.Id, Using ="TitleId")]
        public IWebElement ddlTitleID { get; set; }

        //for selecting files
        [FindsBy(How = How.CssSelector, Using = "[tabindex='1']")]
        public IWebElement fileUploads1 { get; set; }
        //for selecting files
        [FindsBy(How = How.CssSelector, Using = "[tabindex='2']")]
        public IWebElement fileUploads2 { get; set; }

        //for text box
        [FindsBy(How = How.Name, Using = "Initial")]
        public IWebElement txtInitial { get; set; }

        //for button
        [FindsBy(How = How.Name, Using = "Save")]
        public IWebElement btnSave { get; set; }

        //for button
        [FindsBy(How = How.Name, Using = "files")]
        public IWebElement btnChooseFl { get; set; }

        //for button
        [FindsBy(How = How.Id, Using = "btnSubmit")]
        public IWebElement btnMerge { get; set; }

    }
}
