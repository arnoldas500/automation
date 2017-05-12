using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PDF_Secret_Sharing
{

    /// <summary>
    /// A class that defines and manipulates a share for secret sharing encryption/decryption
    /// Based off of Kevin Hildebrand's implementation in Java
    ///
    /// Written by: Taylor Budzan
    /// </summary>
    public class Share
    {
        private int shareNumber; //A shares specific number in a group of shares
        private StringBuilder cipherText;  //the content of the share (cipher text)

        /// <summary>
        /// Creates a share
        /// </summary>
        /// <param name="shareNumber">This shares number</param>
        public Share(int shareNumber)
        {
            this.shareNumber = shareNumber;
            cipherText = new StringBuilder();

        }


        /// <summary>
        /// Append a single character to the end of the share
        /// </summary>
        /// <param name="c">The character to add to the share</param>
        public void Append(char c)
        {
            cipherText.Append(c);
        }

        /// <summary>
        /// Append a string to the end of the share
        /// </summary>
        /// <param name="c">The string to add to the share</param>
        public void Append(string c)
        {
            cipherText.Append(c);
        }

        /// <summary>
        /// Returns the entire cipher text of the share
        /// </summary>
        /// <returns>The cipher text of the share</returns>
        public string GetCipherText()
        {
            return cipherText.ToString();
        }

        /// <summary>
        /// Returns the number of the share
        /// </summary>
        /// <returns>The share's assigned number</returns>
        public int GetShareNumber()
        {
            return shareNumber;
        }

        /// <summary>
        /// Determine if the two shares are identicle
        /// </summary>
        /// <param name="share">Share to be compared to</param>
        /// <returns>Returns true if the shares are identicle, false if not</returns>
        public bool Equals(Share share)
        {
            if (this.shareNumber == share.shareNumber && this.cipherText.Equals(share.cipherText))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Takes an array of shares and prints them out
        /// </summary>
        /// <param name="shares">an array of shares to print to a text file</param>
        public static void PrintShares(Share[] shares)
        {

            for (int i = 0; i < shares.Length; i++)
            {
                StreamWriter sw = new StreamWriter("Share" + (1 + i) + ".txt.");
                sw.Write(shares[i].GetCipherText());
                sw.Close();
            }
        }
    }
}
