using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium
{
    //Strong variable passing (outside of class)
    enum PropertyType
    {
        Id,
        Name,
        LinkText,
        CssName,
        ClassName,
        CssSelector
    }

    class PropertiesCollection
    {
        

        //auto-implemented property
        public static IWebDriver driver
        {
            get;
            set;
        }
    }
}
