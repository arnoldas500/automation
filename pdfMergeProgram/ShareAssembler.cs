using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PDF_Secret_Sharing
{
    public class ShareAssembler
    {
        /// <summary>
        /// Private fields for Share Generation
        /// TEXT_MOUDULUS: The number that will be used in mod arithmitic for text shares.
        /// IMAGE_MODULUS: The number that will be used in mod arithmitic for image shares.
        /// CHAR_OFFSET: Since some char values that will be produced by shares are unreadable in pdf files
        ///              We offset them in order to find a field of numbers in which all 127 characters can
        ///              be read again by the pdf files in file reconstruction.
        /// </summary>
        private const int TEXT_MODULUS = 127;
        private const int IMAGE_MODULUS = 251;
        private const int CHAR_OFFSET = 556; //Change to 556 when working with the pdf files.

        /// <summary>
        /// A method for taking an array of share values and using them to reconstruct their secret based
        /// on Shamirs (n,k) thereshold.
        /// </summary>
        /// <param name="shares">The array of shares used to generate the original secret</param>
        /// <param name="sharesToUse">The list of shares that we wish to use from our array.</param>
        /// <param name="n">The number of shares created in the generation process.</param>
        /// <param name="k">The number of shares required to reconstruct the secret.</param>
        /// <returns></returns>
        public static string TextReconstruction(string[] shares, List<int> shareNumbers, int n, int k)
        {
            Dictionary<int, int> sharesUsed = new Dictionary<int, int>();  //A dictionary datatype used to store sharenumbers and share values
            StringBuilder reconstruction = new StringBuilder();  //String that is being reconstructed
            int secret = 0; //Stores the integer secret value we reconstruct each itteration
            //make sure the list of shares to use is the appropriate size
            if (shareNumbers.Count < k)
                return "To few shares used in reconstruction! Use " + k + " shares to reconstruct secret. Reconstruction failed.";

            //If the count in ShareNumbers matches k exactly, use all of the strings in shares[]
            else if (shareNumbers.Count == k)
            {
                //loop through each character in the shares and peform the following:
                //we use shares[0].length here since each share value is the same length so it 
                //is an arbitrary choice.
                for (int i = 0; i < shares[0].Length; i++)
                {
                    //For each item in shareNumbers, use that value as the 'key' in our dictionary
                    //and then use that value as the index in our shares array to get the value
                    for (int j = 0; j < shareNumbers.Count; j++)
                    {
                        sharesUsed.Add(shareNumbers[j] + 1, (int)(shares[shareNumbers[j]][i]) - CHAR_OFFSET);
                    }

                    //Call lagrange polynomial on our dictionary to reconstruct secret
                    secret = LagrangePolynomial(sharesUsed, false);
                    reconstruction.Append((char)secret);    //add our secret value to our string reconstruction
                    //Clear our dictionary object
                    sharesUsed.Clear();
                }

            }
            else
            {
                return "To many shares selected to be used. Select " + k + " shares to reconstruct secret.  Reconstruction failed.";
            }

            return reconstruction.ToString();
        }

        public static Bitmap ImageReconstruction(Bitmap[] shares, List<int> shareNumbers, int n, int k)
        {

            Dictionary<int, int> redShare = new Dictionary<int, int>();
            Dictionary<int, int> blueShare = new Dictionary<int, int>();
            Dictionary<int, int> greenShare = new Dictionary<int, int>();
            Bitmap reconstruction = new Bitmap(shares[0].Width, shares[0].Height); //Create our reconstructed image object, arbitratily pick one of the shares to use for sizing purposes
            Color reconColor;   //Our reconstruction pixel color
            int red, green, blue; //Reconstructed color components of a pixel

            //make sure the list of shares to use is the appropriate size
            if (shareNumbers.Count < k)
                System.Console.WriteLine("To few shares used in reconstruction! Use " + k + " shares to reconstruct secret. Reconstruction failed.");



            //Loop through each x pixel in the reconstruction
            for (int x = 0; x < reconstruction.Width; x++)
            {
                //loop through each y pixel in the reconstruction
                for (int y = 0; y < reconstruction.Height; y++)
                {
                    //For each item in shareNumbers, use that value as the 'key' in our dictionary
                    //and then use that value as the index in our shares array to get the value
                    for (int i = 0; i < shareNumbers.Count; i++)
                    {
                        redShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(x, y).R);
                        greenShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(x, y).G);
                        blueShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(x, y).B);
                    }

                    //Reconstruct inndividual color components
                    red = LagrangePolynomial(redShare, true);
                    green = LagrangePolynomial(greenShare, true);
                    blue = LagrangePolynomial(blueShare, true);

                    reconColor = Color.FromArgb(red, green, blue);
                    reconstruction.SetPixel(x, y, reconColor);

                    //Clears out our color component dictionaries
                    redShare.Clear();
                    greenShare.Clear();
                    blueShare.Clear();

                }
            }
            return reconstruction;
        }

        public static Bitmap ImageReconstructionReduced(Bitmap[] shares, List<int> shareNumbers, int n, int k)
        {

            //Bitmap reconImage = new Bitmap();
            Bitmap reconImage = new Bitmap(shares[0].Width + 200, shares[0].Height + 200);
            int batchCount;
            int currPixelXVal;
            Color runColor = new Color();

            Dictionary<int, int> redShare = new Dictionary<int, int>();
            Dictionary<int, int> blueShare = new Dictionary<int, int>();
            Dictionary<int, int> greenShare = new Dictionary<int, int>();

            Color reconColor;   //Our reconstruction pixel color
            int reconXPos;
            int red, green, blue; //Reconstructed color components of a pixel

            for (int y = 0; y < shares[0].Height;)
            {
                reconXPos = 0;
                //determine how many pixels we actually need to process in the current row
                int numPixelsUsed = 0;  //number of pixels to process in this row
                for (int i = 0; i < shares[0].Width; i++)
                {
                    if (shares[0].GetPixel(i, y).R == 0 && shares[0].GetPixel(i, y).G == 0 && shares[0].GetPixel(i, y).B == 0)
                    {
                        //break;
                    }
                    else
                        numPixelsUsed++;

                }


                for (int shareXPos = 0; shareXPos < numPixelsUsed;)
                {


                    //determine how many pixels we are able to process
                    if (numPixelsUsed - shareXPos >= 4)
                        batchCount = 3;
                    else if (numPixelsUsed - shareXPos == 3)
                        batchCount = 2;
                    else
                        batchCount = 1;

                    runColor = shares[0].GetPixel(shareXPos + batchCount, y);   //the pixel that contains 'count' information of the preceeding colors

                    //loop through the pixels in the batch
                    for (int j = 0; j < batchCount; j++)
                    {
                        Console.WriteLine("batch count is " + batchCount);
                        currPixelXVal = shareXPos + j;

                        //For each item in shareNumbers, use that value as the 'key' in our dictionary
                        //and then use that value as the index in our shares array to get the value
                        //////////took away +1 for red share gree
                        /// for (int i = 0; i < shareNumbers.Count; i++)
                        /*{
							redShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).R);
							greenShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).G);
							blueShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).B);
						}*/

                        for (int i = 0; i < shareNumbers.Count; i++)
                        {
                            redShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).R);
                            greenShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).G);
                            blueShare.Add(shareNumbers[i] + 1, shares[shareNumbers[i]].GetPixel(currPixelXVal, y).B);
                        }

                        //Reconstruct inndividual color components
                        red = LagrangePolynomial(redShare, true);
                        green = LagrangePolynomial(greenShare, true);
                        blue = LagrangePolynomial(blueShare, true);

                        reconColor = Color.FromArgb(red, green, blue);

                        if (reconColor.ToArgb() > 251)
                        {
                            Console.WriteLine("theres a value greater than 251");
                        }
                        else if (reconColor.ToArgb() < 250)
                        {
                            //Console.WriteLine("values less than 251 are 250");
                            //Console.WriteLine(reconColor.ToArgb());
                        }

                        //////////				//////////Console.WriteLine(reconColor.ToArgb());
                        //determine which R,G or B value to use in our Run Pixel adn create a 'run' of that many pixels in reconsturction image
                        if (j == 0)
                        {
                            for (int l = 0; l < runColor.R; l++)
                            {
                                //System.Console.WriteLine("pixelating...");
                                if (reconColor == null)
                                {

                                }
                                else
                                {
                                    reconImage.SetPixel(reconXPos, y, reconColor);
                                    reconXPos++;
                                }
                            }

                        }

                        else if (j == 1)
                        {
                            for (int l = 0; l < runColor.G; l++)
                            {
                                if (reconColor == null)
                                {

                                }
                                else
                                {
                                    reconImage.SetPixel(reconXPos, y, reconColor);
                                    reconXPos++;
                                }
                            }

                        }

                        else
                        {
                            for (int l = 0; l < runColor.B; l++)
                            {

                                reconImage.SetPixel(reconXPos, y, reconColor);
                                reconXPos++;
                            }

                        }

                        //Clears out our color component dictionaries
                        redShare.Clear();
                        greenShare.Clear();
                        blueShare.Clear();

                    }



                    shareXPos += batchCount;

                    //skip over to the next pixel to be the first to be processed in the next batch
                    if ((shareXPos + 1) != reconImage.Width)
                        shareXPos++;

                }

                y++;
            }

            return reconImage;
        }


        /// <summary>
        /// Calculates the lagrange polynomial on a set of share numbers and their value to reconstruct7
        /// an original secret.
        /// </summary>
        /// <param name="sharesUsed">The dictionary used to store share numbers and their values.</param>
        /// <returns>Returns the reconstructed secret integer value.</returns>
        private static int LagrangePolynomial(Dictionary<int, int> sharesUsed, bool isImage)
        {
            Dictionary<int, int>.KeyCollection temp = sharesUsed.Keys;
            List<int> shareNumbers = temp.ToList<int>();

            int l1;
            int l2;
            int l3;
            //Determine legrange polynomial 1
            if (!isImage)
            {
                int templ1 = shareNumbers[1] * shareNumbers[2];
                if (templ1 < 0)
                {
                    templ1 = TEXT_MODULUS + templ1;
                }
                int minv = (shareNumbers[0] - shareNumbers[1]) * (shareNumbers[0] - shareNumbers[2]);
                if (minv < 0)
                {
                    minv = TEXT_MODULUS + minv;
                }
                l1 = Mod(templ1, TEXT_MODULUS) * MultiplicatveInverse(minv, TEXT_MODULUS);
            }
            else
            {
                int temp4 = (int)((shareNumbers[0] - shareNumbers[1]) * (shareNumbers[0] - shareNumbers[2]));
                if (temp4 < 0)
                {
                    temp4 = IMAGE_MODULUS + temp4;
                }
                int temp8 = shareNumbers[1] * shareNumbers[2];
                if (temp8 < 0)
                {
                    temp8 = IMAGE_MODULUS + temp8;
                }
                l1 = Mod(temp8, IMAGE_MODULUS) * MultiplicatveInverse(temp4, IMAGE_MODULUS);
            }

            //for legrange polynomial 2 there is a chance the constant term is negative, so if it is we must
            //account for this
            if (!isImage)
            {
                int templ2 = shareNumbers[0] * shareNumbers[2];
                if (templ2 < 0)
                {
                    templ2 = TEXT_MODULUS + templ2;
                }
                int minv1 = (shareNumbers[1] - shareNumbers[0]) * (shareNumbers[1] - shareNumbers[2]);
                if (minv1 < 0)
                {
                    minv1 = TEXT_MODULUS + minv1;
                }
                //if its not negative do this
                l2 = Mod(templ2, TEXT_MODULUS) * MultiplicatveInverse(minv1, TEXT_MODULUS);
            }
            else
            {
                int temp5 = (shareNumbers[1] - shareNumbers[0]) * (shareNumbers[1] - shareNumbers[2]);
                if (temp5 < 0)
                {
                    temp5 = IMAGE_MODULUS + temp5;
                }
                int temp9 = shareNumbers[0] * shareNumbers[2];
                if (temp9 < 0)
                {
                    temp9 = IMAGE_MODULUS + temp9;
                }
                l2 = Mod(temp9, IMAGE_MODULUS) * MultiplicatveInverse(temp5, IMAGE_MODULUS);
                /*
                if ((shareNumbers[1] - shareNumbers[0]) * (shareNumbers[1] - shareNumbers[2]) < 0)
                {
                    int num = Mod(shareNumbers[0] * shareNumbers[2], IMAGE_MODULUS) * -1;
                    int denom = (shareNumbers[1] - shareNumbers[0]) * (shareNumbers[1] - shareNumbers[2]);
                    denom *= -1;
                    l2 = num * MultiplicatveInverse(denom, IMAGE_MODULUS);
                }
                //if its not negative do this
                else
                    l2 = Mod(shareNumbers[0] * shareNumbers[2], IMAGE_MODULUS) * MultiplicatveInverse((shareNumbers[1] - shareNumbers[0]) * (shareNumbers[1] - shareNumbers[2]), IMAGE_MODULUS);
				*/
            }
            //determine legrange polynomial 3
            if (!isImage)
            {
                int templ3 = shareNumbers[0] * shareNumbers[1];
                if (templ3 < 0)
                {
                    templ3 = TEXT_MODULUS + templ3;
                }
                int minv3 = (shareNumbers[2] - shareNumbers[0]) * (shareNumbers[2] - shareNumbers[1]);
                if (minv3 < 0)
                {
                    minv3 = TEXT_MODULUS + minv3;
                }
                l3 = Mod(templ3, TEXT_MODULUS) * MultiplicatveInverse(minv3, TEXT_MODULUS);
            }
            else
            {
                int temp6 = (shareNumbers[2] - shareNumbers[0]) * (shareNumbers[2] - shareNumbers[1]);
                if (temp6 < 0)
                {
                    temp6 = IMAGE_MODULUS + temp6;
                }
                int temp11 = shareNumbers[0] * shareNumbers[1];
                if (temp11 < 0)
                {
                    temp11 = IMAGE_MODULUS + temp11;
                }
                l3 = Mod(temp11, IMAGE_MODULUS) * MultiplicatveInverse(temp6, IMAGE_MODULUS);
            }

            int result = 0;

            //Sum the polynomials
            result += l1 * sharesUsed[shareNumbers[0]];
            result += l2 * sharesUsed[shareNumbers[1]];
            result += l3 * sharesUsed[shareNumbers[2]];

            if (!isImage)
                return Mod(result, TEXT_MODULUS);   //mod the final result
            else
                return Mod(result, IMAGE_MODULUS);
        }

        /// <summary>
        /// A modulus function
        /// </summary>
        /// <param name="x">The x value in the case of x mod n</param>
        /// <param name="n">The n value in the case of x mond n</param>
        /// <returns></returns>
        private static int Mod(int x, int n)
        {
            return (Math.Abs(x * n) + x) % n;
        }

        /// <summary>
        /// A function to determine the Mulitplicative inverse of a number and a prime
        /// </summary>
        /// <param name="a">the number</param>
        /// <param name="mod">the prime number</param>
        /// <returns>The value of the multiplicative inverse</returns>
        private static int MultiplicatveInverse(int a, int mod)
        {
            /*
            int dividend = a % mod;
            int divsor = mod;

            int lastX = 1;
            int currX = 0;

            while (divsor > 0)
            {
                int quotient = dividend / divsor;
                int remainder = dividend % divsor;
                if (remainder <= 0)
                    break;

                int nextX = lastX - currX * quotient;
                lastX = currX;
                currX = nextX;

                dividend = divsor;
                divsor = remainder;
            }


            if (divsor != 1)
                throw new Exception("Numbers a and mod are not realitive primes!");

            return (currX < 0 ? currX + mod : currX);*/

            int index = 0;
            int inverse = 0;

            while (index < mod)
            {
                if ((index * a) % mod == 1)
                {
                    inverse = index;
                    index = mod;
                }

                index++;
            }

            return inverse;

            /*
			 * if (index == 251)
				throw new Exception("Numbers a and mod are not relative primes");

			return inverse;*/
        }
        public Bitmap ImageConstruction(Bitmap[] shares, int k, int n)
        {
            Bitmap original = new Bitmap(shares[0].Width, shares[0].Height);

            int size = shares.Length;
            Color[] color = new Color[n];
            //Color originalColor;
            for (int yPos = 0; yPos < shares[0].Height; yPos++)
            {
                for (int xPos = 0; xPos < shares[0].Width; xPos++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        color[i] = shares[i].GetPixel(xPos, yPos);
                    }
                    original.SetPixel(xPos, yPos, getOriginalColor(color));
                }
            }

            return original;
        }

        public Color getOriginalColor(Color[] color)
        {
            //System.Console.WriteLine("in get original color");
            Color original = new Color();
            Color share1 = color[0];
            Color share2 = color[1];
            int xVal1 = 7 * 2;
            int xVal2 = 8 * 2;

            //int secretAlpha1 = share1.A;
            //int secretAlpha2 = share2.A;
            int secretRed2 = share2.R;
            int secretRed1 = share1.R;
            //Console.WriteLine(secretRed1);
            int secretGreen1 = share1.G;
            int secretBlue1 = share1.B;
            int slope = ((secretRed2 - secretRed1) * MultiplicatveInverse(xVal2 - xVal1, 251)) % 251;

            //Console.WriteLine(slope);
            //int secretAlpha = (secretAlpha1 - (slope * xVal1)) % 251;
            int red = (secretRed1 - (slope * xVal1)) % 251;
            if (red < 0)
            {
                red = 251 + red;
            }
            int green = (secretGreen1 - (slope * xVal1)) % 251;
            if (green < 0)
            {
                green = 251 + green;
            }
            int blue = (secretBlue1 - (slope * xVal1)) % 251;
            if (blue < 0)
            {
                blue = 251 + blue;
            }
            original = Color.FromArgb(red, green, blue);
            //Console.WriteLine(original.A);
            return original;
        }

    }
}
