using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PDF_Secret_Sharing
{
    public class ShareGenerator
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
        private static Random random = new Random(System.DateTime.Now.Millisecond);
        private static Bitmap secretImage;

        /// <summary>
        /// Generates Shamir Secret shares for a given string of text, using the (n,k) scheme
        /// where n shares are generated and k shares are required to reconstruct the secret.
        /// </summary>
        /// <param name="secret">The text secret we wish to share.</param>
        /// <param name="n">The number of shares to produce from our secret.</param>
        /// <param name="k">The number of shares required to reproduce the secret.</param>
        /// <returns>An array of share values generated from our secret of length n.</returns>
        public static Share[] GenerateShares(string secret, int n, int k)
        {

            string[] shares = new string[n]; //The list that will eventually store our shares
            StringBuilder[] shareArray = new StringBuilder[n]; // temporarly stores share values
            for (int i = 0; i < shareArray.Length; i++)
                shareArray[i] = new StringBuilder();

            //Loop through each character in our secret string
            for (int i = 0; i < secret.Length; i++)
            {
                char[] shareChars = GenerateSecretChars(secret[i], n, k);

                //now we append the corresponding character share values to our string share values
                for (int j = 0; j < shareArray.Length; j++)
                    shareArray[j].Append(shareChars[j]);
            }

            //Convert our share array of StringBuilder objects into a list of strings.
            for (int i = 0; i < shareArray.Length; i++)
                shares[i] = shareArray[i].ToString();

            Share[] shares2 = new Share[shares.Length];
            for (int i = 0; i < shares.Length; i++)
            {
                shares2[i] = new Share(i);
                shares2[i].Append(shares[i]);
            }
            return shares2;  //Return shares
        }

        /// <summary>
        /// Generates Shamir Secret Shares for a given Bitmap image, using the (n,k) scheme
        /// where n shares are generated and k shares are required to reconstruct the secret.
        /// </summary>
        /// <param name="secret">The secret Bitmap Image we wish to share</param>
        /// <param name="n">The number of shares to produce in the scheme</param>
        /// <param name="k">The number of shares required to reconstruct the original secret.</param>
        /// <returns>An array of share values generated from our secret image</returns>
        public static Bitmap[] GenerateShares(Bitmap secret, int n, int k)
        {
            Bitmap[] shares = new Bitmap[n];    //our share images
            secretImage = secret;
            //Instantiate each share image to be the same size as the original
            for (int i = 0; i < shares.Length; i++)
                shares[i] = new Bitmap(secretImage.Width, secretImage.Height);

            //loop through each pixel in the original to create shares
            for (int x = 0; x < secretImage.Width; x++)
            {
                for (int y = 0; y < secretImage.Height; y++)
                {
                    Color[] sharePixel = GenerateSecretPixel(secretImage.GetPixel(x, y), n, k);  //create an array of share pixels based on secret pixel

                    //Add the share pixels to the corresponding share images
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(x, y, sharePixel[i]);
                }

                //System.Console.WriteLine("done column: " + (x + 1));
            }
            return shares;  //return our share images
        }


        /// <summary>
        /// Generate Secret Shares for a given Bitmap image using the (n,k) scheme.  Where n
        /// shares are generated and k shares are requried to reconstruct the image.  Also size reduction
        /// techniques are applied to ensure the shares are of smaller size than the origianl image
        /// </summary>
        /// <param name="secret">The secret Bitmap Image we wish to share</param>
        /// <param name="n">The number of shares to produce in the scheme</param>
        /// <param name="k">The number of shares required to reconstruct the original secret.</param>
        /// <param name="threshold">The threshold value used to determine similar pixels.</param>
        /// <returns>An array of share values generated from our secret image</returns>
        public static Bitmap[] GenerateReducedShares(Bitmap secret, int n, int k, int threshold)
        {
            Bitmap[] shares = new Bitmap[n];    //our share images
            secretImage = secret;
            //Make our share images all the same size as the original
            for (int i = 0; i < n; i++)
            {
                shares[i] = new Bitmap(secretImage.Width, secretImage.Height);

            }

            Color[] sharePixel;
            int setXValue;  //this keeps track of where we are placing the pixels in the share compared to what pixel in the secret is being processed(the x vaues aren't identical)
            int similarPixels;
            List<int> similarPixelVals = new List<int>();
            //Loop through each pixel in our secret, from a left to right fashion, line by line


            for (int currY = 0; currY < secretImage.Height; currY++)
            {
                setXValue = 0;

                for (int currX = 0; currX < secretImage.Width;)
                {
                    //Create a Share from the first pixel
                    sharePixel = GenerateSecretPixel(secretImage.GetPixel(currX, currY), n, k);  //creates an array of share pixels based on secret pixel

                    //Update our share images with the shared color pixel
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, sharePixel[i]);
                    setXValue++;

                    //determine how many pixels are 'similar' to our current secret pixel
                    similarPixels = DetermineNumberOfSimilarPixels(currX, currY, threshold);

                    similarPixelVals.Add(similarPixels);    //add this value to our list of similar pixel values
                    currX = currX + similarPixels;  //update our current pixel to the next 'non' similar pixel

                    //If we have 3 values in similarPixelVals, then create a new pixel in our share images
                    if (similarPixelVals.Count == 3)
                    {
                        //currX++;    //increment currX by 1 since we'll be adding a new pixel
                        Color c = Color.FromArgb(similarPixelVals[0], similarPixelVals[1], similarPixelVals[2]);
                        //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                        //add the share pixels to our shares
                        for (int i = 0; i < shares.Length; i++)
                            shares[i].SetPixel(setXValue, currY, c);

                        setXValue++;
                        similarPixelVals.Clear();   //empty our list so we can add more values again
                    }

                    similarPixels = 0;  //reset similar pixels to 0 for next pixel to be processed


                }

                //flush out ramining siimilarPixelValues
                if (similarPixelVals.Count == 1)
                {
                    //currX++;    //increment currX by 1 since we'll be adding a new pixel
                    Color c = Color.FromArgb(similarPixelVals[0], 0, 0);
                    //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                    //add the share pixels to our shares
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, c);


                    similarPixelVals.Clear();   //empty our list so we can add more values again
                }
                else if (similarPixelVals.Count == 2)
                {

                    Color c = Color.FromArgb(similarPixelVals[0], similarPixelVals[1], 0);
                    //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                    //add the share pixels to our shares
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, c);


                    similarPixelVals.Clear();   //empty our list so we can add more values again
                }
                //System.Console.WriteLine("done row: " + (currY + 1));
            }

            return shares;
        }

        /// <summary>
        /// Takes an individual pixel color and creates SSS shares for it based on the (n,k) scheme
        /// </summary>
        /// <param name="color">The color we wish to share</param>
        /// <param name="n">The number of shares to generate</param>
        /// <param name="k">The number of shares required to reconstruct this color</param>
        /// <returns>An array of colors that are the shares of the original color value</returns>
        private static Color[] GenerateSecretPixel(Color color, int n, int k)
        {
            int[] coefficients = new int[k - 1];    //Random coeffiecent values for our polynomial
            Color[] shareColors = new Color[n];    //an array of colors which are the share values of our secret color

            //Generate random coefficents
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = random.Next(IMAGE_MODULUS);

            //Make sure any r,g,b values that are over 251 are turned to 250 to deal with the prime number of 251 issue
            if (color.R >= 251 || color.G >= 251 || color.B >= 251)
            {

                int red;
                int green;
                int blue;

                if (color.R >= 251)
                    red = 250;
                else
                    red = color.R;

                if (color.G >= 251)
                    green = 250;
                else
                    green = color.G;

                if (color.B >= 251)
                    blue = 250;
                else
                    blue = color.B;

                color = Color.FromArgb(red, green, blue);
            }



            //Generate share values for each R,G,B component in the secret color
            int[] redShareValues = GenerateShareValues(color.R, n, k, coefficients, true);
            int[] greenShareValues = GenerateShareValues(color.G, n, k, coefficients, true);
            int[] blueShareValues = GenerateShareValues(color.B, n, k, coefficients, true);

            //Generate the share colors by creating them from the corresponding share color components
            for (int i = 0; i < shareColors.Length; i++)
                shareColors[i] = Color.FromArgb(redShareValues[i], greenShareValues[i], blueShareValues[i]);


            return shareColors; //return our share color array
        }

        /// <summary>
        /// Takes a character and creates SSS shares for it based on the (n,k) scheme.
        /// </summary>
        /// <param name="c">the char value to be shared.</param>
        /// <param name="n">the number of shares to produce for the character.</param>
        /// <param name="k">the number of shares reuired to reconstruct the character.</param>
        /// <returns>an array of chars that are the shares of the original character value.</returns>
        private static char[] GenerateSecretChars(char c, int n, int k)
        {
            int[] coefficients = new int[k - 1]; //Random coefficent values for our polynomial
            char[] shareChars = new char[n];    //An array of characters which are the share values of our secret char

            //Generate the random values
            for (int i = 0; i < coefficients.Length; i++)
                coefficients[i] = random.Next(TEXT_MODULUS); //make sure our coefficients are less than the modulus value

            //Gets the integer share values for our secret char
            int[] rawShareValues = GenerateShareValues((int)c, n, k, coefficients, false);

            //convert the int values into characters
            for (int i = 0; i < shareChars.Length; i++)
                shareChars[i] = (char)(rawShareValues[i] + CHAR_OFFSET);

            return shareChars;  //returns the character share values of the secret character
        }

        /// <summary>
        /// Generates integer share values for a given integer secret.
        /// </summary>
        /// <param name="secretVal">The integer we wish to share.</param>
        /// <param name="n">Number of shares to generate.</param>
        /// <param name="k">Subset of shares required to reconstruct secret</param>
        /// <param name="coefficients">Random coefficent array used in share generation.</param>
        /// <param name="image">Is this value being used for image or text SS.</param>
        /// <returns>An integer array which is the share values of the secret.</returns>
        private static int[] GenerateShareValues(int secretVal, int n, int k, int[] coefficients, bool image)
        {
            int[] rawShareValues = new int[n];

            //Create n polynomials based off the secret and coefficients
            for (int i = 0; i < n; i++)
            {
                int result = 0; //the result of the polynomial expression

                //Starting at the lowest degree of our polynomial (which is 1) and go up to the highest
                //(based on k - 1)
                for (int j = 1; j <= (k - 1); j++)
                {
                    int degreeValue = (int)Math.Pow((double)(i + 1), (double)j); //The value of our x value raised to its power
                    result += coefficients[j - 1] * degreeValue; //add this value to our result
                }
                result += secretVal; //Add the result of our secret now

                //We must mod the entire result now before storing its value
                if (image == false)
                    result = Mod(result, TEXT_MODULUS);
                else
                    result = Mod(result, IMAGE_MODULUS);

                rawShareValues[i] = result;   //store the value as a character in our share char array

            }

            return rawShareValues;  //return the raw share values
        }

        /// <summary>
        /// Determine How many pixels after our current pixel are 'similar' based on a threshold, 
        /// inorder to reduce the number of share values beeing generated
        /// </summary>
        /// <param name="currX">The current x value of the pixel being shared</param>
        /// <param name="currY">The current y value of the pixel being shared</param>
        /// <returns>The number of pixels after the current pixel that are similar</returns>
        private static int DetermineNumberOfSimilarPixels(int currX, int currY, int threshold)
        {
            int similarPixels = 1;  //the number of similar successive pixels

            Color currentPixel = secretImage.GetPixel(currX, currY);    //current pixel being shared
            if (currX == (secretImage.Width - 1))
                return 1;
            Color nextPixel = secretImage.GetPixel(++currX, currY);     //next pixel after the current pixel


            //loop through as many pixels in a row as possible comparing the 'similarity' of them to the current pixel
            while (true)
            {
                //Determine the similarity of the two succeeding pixels
                int test = currentPixel.R - nextPixel.R;
                if ((test >= threshold) || (test <= -1 * threshold))
                    break;  //break out of the loop if difference of R value is greter than threshold
                test = currentPixel.G - nextPixel.G;
                if ((test >= threshold) || (test <= -1 * threshold))
                    break;  //break out of the loop if difference of G value is greter than threshold
                test = currentPixel.B - nextPixel.B;
                if ((test >= threshold) || (test <= -1 * threshold))
                    break;  //break out of the loop if difference of B value is greter than threshold

                //if the pixels are simiar, increment similarPixels by 1 and grab the next pixel
                similarPixels++;

                //move onto th next pixel in the row unless that was the last pixel in the row
                if (currX == (secretImage.Width - 1))
                    break;  //break out of the loop if that was the last pixel in the row
                else
                    nextPixel = secretImage.GetPixel(++currX, currY);   //move onto the next pixel

                if (similarPixels == 255)
                    return similarPixels;

            }

            return similarPixels;
        }

        /// <summary>
        /// A modulus function
        /// </summary>
        /// <param name="x">The x value in the case of x mod n</param>
        /// <param name="n">The n value in the case of x mond n</param>
        /// <returns></returns>
        private static int Mod(int x, int n)
        {
            int mod = x % n;
            if (mod < 0)
            {
                mod = mod + n;
            }

            return mod;

            //return (Math.Abs(x * n) + x) % n;
        }

        public Bitmap[] GenShares(Bitmap original, int n, int k)
        {
            Bitmap[] shares = new Bitmap[n];
            Bitmap secret = original;
            //Bitmap copy;
            Color[] coef = new Color[n];

            for (int index = 0; index < n; index++)
            {
                shares[index] = new Bitmap(secret.Width, secret.Height);
            }

            Color[] sharePix;
            int xPos = 0;
            int xVal = 0;

            for (int yPos = 0; yPos < secret.Height; yPos++)
            {
                for (xPos = 0; xPos < secret.Width; xPos++)
                {
                    Color pixel = original.GetPixel(xPos, yPos);
                    //Color coef1 = getSecretColor(pixel, 1);
                    //Color coef2 = getSecretColor(pixel, 2);
                    for (int i = 0; i < n; i++)
                    {
                        coef[i] = getSecretColor(pixel, i + 7);
                        int alpha = coef[i].A;
                        //Console.WriteLine("in genshares");
                        //Console.WriteLine(alpha);
                        shares[i].SetPixel(xPos, yPos, coef[i]);
                    }
                }
            }

            return shares;

        }

        /*
		 * public static Bitmap[] GenerateReducedShares(Bitmap secret, int n, int k, int threshold)
        {
            Bitmap[] shares = new Bitmap[n];    //our share images
            secretImage = secret;
            //Make our share images all the same size as the original
            for (int i = 0; i < n; i++)
            {
                shares[i] = new Bitmap(secretImage.Width, secretImage.Height);
                
            }

            Color[] sharePixel;
            int setXValue;  //this keeps track of where we are placing the pixels in the share compared to what pixel in the secret is being processed(the x vaues aren't identical)
            int similarPixels;
            List<int> similarPixelVals = new List<int>();
            //Loop through each pixel in our secret, from a left to right fashion, line by line


			for (int currY = 0; currY < secretImage.Height; currY++)
            {
                setXValue = 0;

                for (int currX = 0; currX < secretImage.Width; )
                {
                    //Create a Share from the first pixel
                    sharePixel = GenerateSecretPixel(secretImage.GetPixel(currX, currY), n, k);  //creates an array of share pixels based on secret pixel

                    //Update our share images with the shared color pixel
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, sharePixel[i]);
                    setXValue++;

                    //determine how many pixels are 'similar' to our current secret pixel
                    similarPixels = DetermineNumberOfSimilarPixels(currX, currY, threshold);

                    similarPixelVals.Add(similarPixels);    //add this value to our list of similar pixel values
                    currX = currX + similarPixels;  //update our current pixel to the next 'non' similar pixel

                    //If we have 3 values in similarPixelVals, then create a new pixel in our share images
                    if (similarPixelVals.Count == 3)
                    {
                        //currX++;    //increment currX by 1 since we'll be adding a new pixel
                        Color c = Color.FromArgb(similarPixelVals[0], similarPixelVals[1], similarPixelVals[2]);
                        //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                        //add the share pixels to our shares
                        for (int i = 0; i < shares.Length; i++)
                            shares[i].SetPixel(setXValue, currY, c);

                        setXValue++;
                        similarPixelVals.Clear();   //empty our list so we can add more values again
                    }

                    similarPixels = 0;  //reset similar pixels to 0 for next pixel to be processed
                    
                    
                }

                //flush out ramining siimilarPixelValues
                if (similarPixelVals.Count == 1)
                {
                    //currX++;    //increment currX by 1 since we'll be adding a new pixel
                    Color c = Color.FromArgb(similarPixelVals[0], 0, 0);
                    //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                    //add the share pixels to our shares
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, c);

                    
                    similarPixelVals.Clear();   //empty our list so we can add more values again
                }
                else if (similarPixelVals.Count == 2)
                {
                    
                    Color c = Color.FromArgb(similarPixelVals[0], similarPixelVals[1], 0);
                    //sharePixel = GenerateSecretPixel(c, 5, 3);  //generate secret values of this pixel

                    //add the share pixels to our shares
                    for (int i = 0; i < shares.Length; i++)
                        shares[i].SetPixel(setXValue, currY, c);


                    similarPixelVals.Clear();   //empty our list so we can add more values again
                }
				//System.Console.WriteLine("done row: " + (currY + 1));
            }
            
            return shares;
        }
		 */

        private static Color getSecretColor(Color pixel, int xVal)
        {
            //Console.WriteLine("in secret color");
            Color newColor;
            int secret = pixel.ToArgb();

            //int secreVal = secret + (xVal * 10);

            int red = pixel.R;
            //int alpha = pixel.A;
            int blue = pixel.B;
            int green = pixel.G;

            //Console.WriteLine("r: " + red);
            //if (alpha >= 251)
            //	alpha = 250;
            if (red >= 251)
                red = 250;
            if (blue >= 251)
                blue = 250;
            if (green >= 251)
                green = 250;

            //Console.WriteLine(red);
            int secretRed = (red + (xVal * 2 * 237)) % 251;
            //int secretAlpha = (alpha + (xVal * 10)) % 251;
            int secretBlue = (blue + (xVal * 2 * 237)) % 251;
            int secretGreen = (green + (xVal * 2 * 237)) % 251;

            //Console.WriteLine(secretRed);
            //int secretArgb = secretAlpha << 24 + secretRed << 16 + secretGreen << 8 + secretBlue;

            newColor = Color.FromArgb(secretRed, secretGreen, secretBlue);
            //newColor = Color.FromArgb(secretArgb);
            //Console.WriteLine(newColor.R);
            return newColor;
        }
    }
}
