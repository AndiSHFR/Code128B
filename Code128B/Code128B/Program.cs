/**
 * Program.cs - application entry point.
 * 
 * Copyright (C) 2015 Andreas Schaefer <andreas.schaefer@schaefer-it.net>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
namespace Code128B {

   /**
    * Required 3rd party assemblies.
    */
   using System;
   using System.Collections.Generic;
   using System.Text;
   using System.Drawing;
   using System.Drawing.Imaging;
   using System.Collections.ObjectModel;
   using Mono.Options;
   using System.Reflection;

  class Program {

    public enum ExitCodes : int {
      Success = 0,
      UnknownError = -1,
      OptionParseError = -2,
    }

    /**
     * Well - Main()
     */
    static void Main(string[] args) {

      ExitCodes exitCode = ExitCodes.Success;
      
      try {

         #region Declare variables width default settings
          /**
          * Application defaults
          */
         string parseError = String.Empty;
         bool showHelp = false;
         int imageWidth = 1024;
         int imageHeight = 320;
         string foregroundColorName = "Black";
         string backgroundColorName = "White";
         Color foregroundColor = Color.Black;
         Color backgroundColor = Color.White;
         int fontSize = 48;
         bool noText = false;
         bool addQuietZone = false;
         ImageFormat imageFormat = ImageFormat.Png;
         string barcodeText = "Code128B";
         string fileName = "barcode.png";
         #endregion

         #region Command line parsing
         /**
          * To process the command line arguments we use the Mono.Options class Options.
          * Caution: The : (optional) and = (required) specifier for an option define
          *          if a parmeter for this specific option is either optional or required.
          *          It does _not_ define if the option itself is optional or required.
          */                  
         var optionSet = new OptionSet() {
            { "?|help", "Show help message", (int? v) => showHelp = (null!=v) },
            { "w=|width=", "Optional: Image width", (int v) => imageWidth = v },
            { "h=|height=", "Optional: Image height", (int v) => imageHeight = v },
            { "f=|foreground=", "Optional: Foreground color. You can use HTML color codes. i.e. #0080FF", (string v) => foregroundColorName = v },
            { "b=|background=", "Optional: Background color. You can use HTML color codes. i.e. #0080FF", (string v) => backgroundColorName = v },
            { "s=|size=", "Optional: Font size", (int v) => fontSize = v },
            { "n:|notext:", "Optional: Display the barcode text on the barcode. Otherwise only the barcode will be drawn.", (bool? v) => noText = (null==v) },
            { "q:|quietzone:", "Optional: Add leading and trailing quiet zone", (int? v) => addQuietZone = (null!=v) },
         };


         /**
          * Try to parse the command line according to the option set.
          */
         try {
            List<string> extraOptions = optionSet.Parse(args);

            /**
             * Check the number of extra options
             *   2 or more -> All is OK.
             *   1         -> Well at least a filename is specified but no barcode text at all
             *   0         -> Neither filename nor barcode text is specified
             */
            if(extraOptions.Count >= 2) {
               // Get the filename from the 1st extra option
               fileName = extraOptions[0];

               // Combine all other extra options into one string
               string[] opts = new string[extraOptions.Count - 1];
               Array.Copy(extraOptions.ToArray(), 1, opts, 0, opts.Length);
               barcodeText = String.Join(" ", opts);

            } else if( extraOptions.Count >= 1 ) {
               parseError = "Please specify the text to create the barcode from.";
            }else if ( extraOptions.Count >= 0 ) {
               parseError = "Please specify a filename and the text to create the barcode from.";
            }
         }
         catch(OptionException ex) {
            /**
             * If there was a parse error keep it for later reference
             */
            parseError = ex.Message;
         }
         #endregion
         
         /**
          * Now create colors from the textual options
          */
         try {
            foregroundColor = System.Drawing.ColorTranslator.FromHtml(foregroundColorName);
         } catch (Exception ex) {
            parseError = String.Format("Error parsing foreground color: {0}", ex.Message);
         }

         try {
            backgroundColor = System.Drawing.ColorTranslator.FromHtml(backgroundColorName);
         } catch (Exception ex) {
            parseError = String.Format("Error parsing background color: {0}", ex.Message);
         }

         /**
          * If we have to show the help, had an error parsing the command line or
          * have no extra parameter (the text for the barcode) we bail out the 
          * help for this utility.
          * 
          */
         if (showHelp || parseError.Length > 0) {
            Console.WriteLine(String.Format("Code128B Version {0}", Assembly.GetExecutingAssembly().GetName().Version ));
            Console.WriteLine("Copyright 2015 (c) by Andreas Schaefer <andreas.schaefer@schaefer-it.net>");

            // If we have an error message show it now.
            if (parseError.Length > 0) {
              exitCode = ExitCodes.OptionParseError;
              Console.WriteLine();
               Console.WriteLine("Error: {0}", parseError);
            }

            Console.WriteLine();
            Console.WriteLine("Create an image containing a code128 b barcode.");
            Console.WriteLine("The default width and height is 1024x320 if not specified.");
            Console.WriteLine();
            Console.WriteLine("Usage: Code128B [OPTIONS]+ <image filename> <text> <text>");
            Console.WriteLine("Example: Code128B product.png \"Example ABC\"");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
            return;
         }


         /**
          * Create the bitmap and save it.
          */
         using(Bitmap bitmap = Code128B.CreateCode128BBitmap(
            imageWidth, imageHeight, 
            fontSize, 
            foregroundColor, backgroundColor, 
            barcodeText, 0, noText, addQuietZone )) {
           bitmap.Save(fileName, imageFormat );
           Console.WriteLine(String.Format("Barcode \"{0}\" saved to file \"{1}\".", barcodeText, fileName));
         }

       } catch (Exception ex) {
          Console.Error.WriteLine(ex.ToString());
          exitCode = ExitCodes.UnknownError;
       }

       Environment.ExitCode = (int)exitCode;
       return;
    }
  }

}
