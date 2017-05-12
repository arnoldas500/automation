//Arnoldas Kurbanovas
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
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

//for using phantomJS
using System.Threading;


using O2S.Components.PDF4NET;
using O2S.Components.PDF4NET.PageObjects;
using O2S.Components.PDF4NET.PDFFile;
using O2S.Components.PDF4NET.Graphics;
using O2S.Components.PDF4NET.Graphics.Fonts;

namespace PDF_Secret_Sharing
{
    class Program
    {
        //creating the reference for our browser
        //IWebPropertiesCollection.driver PropertiesCollection.driver = new ChromePropertiesCollection.driver();

        public static List<PDFDocument> shares;

        static void Main(string[] args)
        {

            List<PDFDocument> pdfshare1 = getPdf(new PDFDocument("white.pdf"), "1");
            List<PDFDocument> pdfshare2 = getPdf(new PDFDocument("pdfimage.pdf"), "2");

        }

        /*public void setShareList(List<PDFDocument> shares)
		{
			this.shares = shares;
		}*/

        public static List<PDFDocument> getSharesList()
        {
            return shares;
        }

        public static List<PDFDocument> getPdf(PDFDocument pdf, String xyz)
        {
            Share[] textShares;
            System.Drawing.Bitmap[] imageShares = new System.Drawing.Bitmap[5];

            //PdfFont font = secretPdf.AddFont("Arial");

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            //timer.Start();
            ////lOOP THROUGH EACH PAGE IN THE SECRET PDF         
            //for (int i = 0; i < secretPdf.PageCount; i++ )
            //{
            //    //Text Extraction
            //    for (int j = 0; j < secretPdf.Pages[i].Canvas.GetTextData().Count; j++ )
            //    {
            //        //int test = secretPdf.Pages[i].Canvas.GetTextData().Count;
            //        PdfTextData secretData = secretPdf.Pages[i].GetWords()[j];  //our pdf text chunk to process
            //        textShares = ShareGenerator.GenerateShares(secretData.Text, n, k);

            //        ////Write the share data to the pdf shares
            //        //for (int l = 0; l < sharePdfs.Count; l++)
            //        //{

            //        //}

            //        System.Console.WriteLine("Chunk Complete");
            //    }

            //    //add next page for future processing
            //    for (int l = 0; l < sharePdfs.Count; l++)
            //        sharePdfs[l].AddPage();

            //    System.Console.WriteLine("Page Complete");

            //}
            //timer.Stop();
            //System.Console.WriteLine("\n\nTime Elapsed: " + timer.ElapsedMilliseconds+"\n\n\n");
            //timer.Reset();

            //secretPdf.Dispose();

            PDFDocument pdfDoc = pdf;//new PDFDocument("pdfimage.pdf");
                                     //PDFDocument secondDoc = new PDFDocument("white.pdf");
            PDFDocument recon = new PDFDocument();


            List<int> usedShares = new List<int>();
            usedShares.Add(2);
            usedShares.Add(3);
            usedShares.Add(4);

            List<PDFDocument> pdfShares = new List<PDFDocument>();
            for (int i = 0; i < 2; i++)
                pdfShares.Add(new PDFDocument());

            PDFImportedPage ip;
            O2S.Components.PDF4NET.Text.PDFTextRunCollection textRuns;

            timer.Start();
            for (int i = 0; i < pdfDoc.Pages.Count; i++)
            {
                ip = (PDFImportedPage)pdfDoc.Pages[i];
                textRuns = ip.ExtractTextRuns();
                PDFPageObjectCollection objects = ip.ExtractPageObjects();

                //add next page for future processing
                for (int l = 0; l < pdfShares.Count; l++)
                    pdfShares[l].AddPage();

                recon.AddPage();

                for (int j = 0; j < textRuns.Count; j++)
                {
                    textShares = ShareGenerator.GenerateShares(textRuns[j].Text, 5, 3);
                    PDFFont font = new PDFFont(PDFFontFace.Helvetica, textRuns[j].FontSize);
                    PDFBrush fontBrush = new PDFBrush(textRuns[j].FillColor);
                    //PDFPen pen = new PDFPen(textRuns[j].StrokeColor);
                    string[] shareString = new string[5];
                    for (int l = 0; l < 2; l++)
                    {
                        shareString[l] = textShares[l].GetCipherText();
                        pdfShares[l].Pages[i].Canvas.DrawText(shareString[l], font, fontBrush, textRuns[j].DisplayBounds.Left, textRuns[j].DisplayBounds.Top);
                        //pdfShares[l].Pages[i].Canvas.DrawTextBox(textShares[l].GetCipherText(), font, pen, fontBrush, textRuns[j].DisplayBounds.Left, textRuns[j].DisplayBounds.Top, textRuns[j].DisplayBounds.Width, textRuns[j].DisplayBounds.Height);
                    }
                    //System.Console.WriteLine("Chunk Complete");
                    string reconString = ShareAssembler.TextReconstruction(shareString, usedShares, 5, 3);
                    //System.Console.WriteLine(reconString);
                    recon.Pages[i].Canvas.DrawText(reconString, font, fontBrush, textRuns[j].DisplayBounds.Left, textRuns[j].DisplayBounds.Top);

                }




                for (int j = 0; j < objects.Count; j++)
                {
                    PDFTextPageObject text;
                    PDFImagePageObject image;
                    PDFFont font;
                    PDFBrush fontBrush;

                    if (objects[j] is PDFTextPageObject)
                    {
                        text = (PDFTextPageObject)objects[j];
                        Console.WriteLine(text.FontName);
                        Console.WriteLine("textpdf");
                        font = new PDFFont(PDFFontFace.Helvetica, text.FontSize);
                        fontBrush = new PDFBrush(text.FillColor);
                        textShares = ShareGenerator.GenerateShares(text.Text, 2, 2);
                        string[] shareString = new string[2];
                        for (int l = 0; l < textShares.Length; l++)
                        {
                            shareString[l] = textShares[l].GetCipherText();
                            pdfShares[l].Pages[i].Canvas.DrawText(shareString[l], font, fontBrush, text.DisplayBounds.Left, text.DisplayBounds.Top);
                        }
                        string reconString = ShareAssembler.TextReconstruction(shareString, usedShares, 5, 3);
                        recon.Pages[i].Canvas.DrawText(reconString, font, fontBrush, text.DisplayBounds.Left, text.DisplayBounds.Top);

                    }

                    else if (objects[j] is PDFImagePageObject)
                    {
                        Console.WriteLine("imagepdf");
                        image = (PDFImagePageObject)objects[j];
                        /*
						 * 
						 * following line is what needs to be revised until end
						 * 
						 * 
						 * 
						 * 
						 * 
						 * 
						 * 
						 */
                        //imageShares = ShareGenerator.GenerateReducedShares(image.Image, 5, 3, 15);
                        //imageShares = ShareGenerator.GenerateShares(image.Image, 5, 3);
                        ShareGenerator genShares = new ShareGenerator();
                        imageShares = genShares.GenShares(image.Image, 2, 2);
                        for (int l = 0; l < imageShares.Length; l++)

                            pdfShares[l].Pages[i].Canvas.DrawImage(imageShares[l], image.DisplayBounds.Left, image.DisplayBounds.Top, image.DisplayBounds.Width, image.DisplayBounds.Height);


                        shares = pdfShares;
                        ShareAssembler assembler = new ShareAssembler();
                        System.Drawing.Bitmap reconImage = assembler.ImageConstruction(imageShares, 2, 2);
                        recon.Pages[i].Canvas.DrawImage(reconImage, image.DisplayBounds.Left, image.DisplayBounds.Top, image.DisplayBounds.Width, image.DisplayBounds.Height);
                    }
                    else
                    {

                    }
                }




                System.Console.WriteLine("Page Complete");
                //return shares;
            }

            timer.Stop();
            System.Console.WriteLine("\n\nTime Elapsed: " + timer.ElapsedMilliseconds + "\n\n\n");
            timer.Reset();


            for (int i = 0; i < pdfShares.Count; i++)
            {
                pdfShares[i].Save("Share" + (i + 1) + ".pdf");
                pdfShares[i].Dispose();
            }

            recon.Save(@"D:\Recon123-" + xyz + ".pdf");
            recon.Dispose();

            return shares;
        }

        [SetUp]
        public void Initilize()
        {
            //recon.Save(@"D:\Recon123.pdf");
            //PDFDocument pdfDoc = new PDFDocument("pdfimage.pdf");
            /*
            //Navigate to google page
            PropertiesCollection.driver.Navigate().GoToUrl("http://www.google.com");
            Console.WriteLine("Opened URL");
            */
            ChromeOptions options = new ChromeOptions();
            //options.AddArguments("--headless");
            //options.AddArguments("--no-startup-window");

            //ChromeDriver chromeDriver = new ChromeDriver(options);
            PropertiesCollection.driver = new ChromeDriver();
            
            //PropertiesCollection.driver = new RemoteWebDriver(DesiredCapabilities.HtmlUnitWithJavaScript());
            //PropertiesCollection.driver.Manage().Window().setPosition(new Point(-2000, 0));
            //PropertiesCollection.driver.set_window_position(-2000, 0);

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

            //page.fileUploads1.SendKeys("C:/Users/User/Desktop/OSF_CFP_2017[1071].pdf");
            //page.fileUploads2.SendKeys("C:/Users/User/Desktop/OSF-CFP-2017.pdf");

            //List<PDFDocument> Share1 = getSharesList();

            page.fileUploads1.SendKeys("C:/Users/User/Desktop/Share1Send.pdf");
            page.fileUploads2.SendKeys("C:/Users/User/Desktop/Share2Send.pdf");

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
            Thread.Sleep(9000);
            PropertiesCollection.driver.Close();
            Console.WriteLine("Closed the browser");
        }

    }
}
