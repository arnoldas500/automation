using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;


namespace PDF_Secret_Sharing
{
    class ShareImage
    {
        private int share_no;
        private Bitmap shareImg;

        public ShareImage(int share_no)
        {
            this.share_no = share_no;
            this.shareImg = new Bitmap(256, 256);
        }

        public void setImgSize(int width, int height)
        {
            shareImg = new Bitmap(shareImg, width, height);
        }

        public void append(int xpos, int ypos, Color rgb)
        {
            shareImg.SetPixel(xpos, ypos, rgb);
        }


        public Bitmap getShareImage()
        {
            return shareImg;
        }

        public System.IO.MemoryStream getShareImageStream()
        {
            System.IO.MemoryStream temp = new System.IO.MemoryStream();
            shareImg.Save(temp, ImageFormat.Png);
            return temp;
        }

        public int getWidth()
        {
            return shareImg.Width;
        }

        public int getHeight()
        {
            return shareImg.Height;
        }

        public int getShareNo()
        {
            return share_no;
        }

        /// <summary>
        /// Takes an array of shares and prints them out
        /// </summary>
        /// <param name="shares">an array of shares to print to a image file</param>
        public static void PrintShares(ShareImage[] shares)
        {

            for (int i = 0; i < shares.Length; i++)
            {
                shares[i].getShareImage().Save("ImageShare" + (i + 1));
            }
        }


        /*
        public void SetImage(BitMiracle.Docotic.Pdf.PdfImage pdfImage)
        {
            System.IO.MemoryStream temp = new System.IO.MemoryStream();
            pdfImage.Save(temp);
            shareImg = new Bitmap(temp);
        }
         */
    }
}
