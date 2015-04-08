/** 
 * Code128B.cs - Create barcode bitmap.
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

   using System;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Drawing.Text;
  using System.Text;

   /// <summary>
   /// Creates a png image that contains a barcode of a specified text.
   /// The barcode is of CODE128 and will be centered on the image.
   /// The code tries to fill the image size as much as possible.
   /// </summary>
   class Code128B {

      public enum Code128Alphabet { 
         AlphabetA,
         AlphabetB,
         AlphabetC
      }

    #region PatternHelper
    private class PatternHelper {
      public int Index { get; set; }
      public char Character { get; set; }
      public string Pattern { get; set; }

      public PatternHelper(int index, char character, string pattern) {
        this.Index = index;
        this.Character = character;
        this.Pattern = pattern;
      }
    }
    #endregion

    private static readonly int _borderLeft = 0;  // Left border on the image before the barcode sequence starts
    private static readonly int _borderRight = 0; // Right border on the image before the barcode sequence starts

    #region Code128 Pattern Table
    private static readonly int _indexStartCodeB = 104;         // Index of the startcode for alphabet B
    private static readonly int _indexStopCode   = 106;         // Index of the stopcode for alphabet B
    private static readonly int _moduleLen       =  11;         // Each pattern is made up from 11 modules
    private static readonly int _charCount       = 103;         // Number of characters in alphabet.
    private static readonly string _sequenceTerminator = "11";  // Termination sequence

    private static readonly PatternHelper[] _patterns = new PatternHelper[] {
      new PatternHelper(   0, ' ', "11011001100"),               // 0
      new PatternHelper(   1, '!', "11001101100"),               //  1
      new PatternHelper(   2, '~', "11001100110"),                 //  2
      new PatternHelper(   3, '#', "10010011000"),                 //  3
      new PatternHelper(   4, '$', "10010001100"),                 //  4
      new PatternHelper(   5, '%', "10001001100"),                 //  5
      new PatternHelper(   6, '&', "10011001000"),                 //  6
      new PatternHelper(   7, '\'', "10011000100"),               //  7
      new PatternHelper(   8, '(', "10001100100"),                 //  8
      new PatternHelper(   9, ')', "11001001000"),                 //  9
      new PatternHelper(  10, '*', "11001000100"),                //  10
      new PatternHelper(  11, '+', "11000100100"),                //  11
      new PatternHelper(  12, ',', "10110011100"),                //  12
      new PatternHelper(  13, '-', "10011011100"),                //  13
      new PatternHelper(  14, '.', "10011001110"),                //  14
      new PatternHelper(  15, '/', "10111001100"),                //  15
      new PatternHelper(  16, '0', "10011101100"),                //  16
      new PatternHelper(  17, '1', "10011100110"),                //  17
      new PatternHelper(  18, '2', "11001110010"),                //  18
      new PatternHelper(  19, '3', "11001011100"),                //  19
      new PatternHelper(  20, '4', "11001001110"),                //  20
      new PatternHelper(  21, '5', "11011100100"),                //  21
      new PatternHelper(  22, '6', "11001110100"),                //  22
      new PatternHelper(  23, '7', "11101101110"),                //  23
      new PatternHelper(  24, '8', "11101001100"),                //  24
      new PatternHelper(  25, '9', "11100101100"),                //  25
      new PatternHelper(  26, ':', "11100100110"),                //  26
      new PatternHelper(  27, ';', "11101100100"),                //  27
      new PatternHelper(  28, '{', "11100110100"),                //  28
      new PatternHelper(  29, '=', "11100110010"),                //  29
      new PatternHelper(  30, '}', "11011011000"),                //  30
      new PatternHelper(  31, '?', "11011000110"),                //  31
      new PatternHelper(  32, '@', "11000110110"),                //  32
      new PatternHelper(  33, 'A', "10100011000"),                //  33
      new PatternHelper(  34, 'B', "10001011000"),                //  34
      new PatternHelper(  35, 'C', "10001000110"),                //  35
      new PatternHelper(  36, 'D', "10110001000"),                //  36
      new PatternHelper(  37, 'E', "10001101000"),                //  37
      new PatternHelper(  38, 'F', "10001100010"),                //  38
      new PatternHelper(  39, 'G', "11010001000"),                //  39
      new PatternHelper(  40, 'H', "11000101000"),                //  40
      new PatternHelper(  41, 'I', "11000100010"),                //  41
      new PatternHelper(  42, 'J', "10110111000"),                //  42
      new PatternHelper(  43, 'K', "10110001110"),                //  43
      new PatternHelper(  44, 'L', "10001101110"),                //  44
      new PatternHelper(  45, 'M', "10111011000"),                //  45
      new PatternHelper(  46, 'N', "10111000110"),                //  46
      new PatternHelper(  47, 'O', "10001110110"),                //  47
      new PatternHelper(  48, 'P', "11101110110"),                //  48
      new PatternHelper(  49, 'Q', "11010001110"),                //  49
      new PatternHelper(  50, 'R', "11000101110"),                //  50
      new PatternHelper(  51, 'S', "11011101000"),                //  51
      new PatternHelper(  52, 'T', "11011100010"),                //  52
      new PatternHelper(  53, 'U', "11011101110"),                //  53
      new PatternHelper(  54, 'V', "11101011000"),                //  54
      new PatternHelper(  55, 'W', "11101000110"),                //  55
      new PatternHelper(  56, 'X', "11100010110"),                //  56
      new PatternHelper(  57, 'Y', "11101101000"),                //  57
      new PatternHelper(  58, 'Z', "11101100010"),                //  58
      new PatternHelper(  59, '[', "11100011010"),                //  59
      new PatternHelper(  60, '\\', "11101111010"),               //  60
      new PatternHelper(  61, ']', "11001000010"),                //  61
      new PatternHelper(  62, '^', "11110001010"),                //  62
      new PatternHelper(  63, '_', "10100110000"),                //  63
      new PatternHelper(  64, '`', "10100001100"),                //  64
      new PatternHelper(  65, 'a', "10010110000"),                //  65
      new PatternHelper(  66, 'b', "10010000110"),                //  66
      new PatternHelper(  67, 'c', "10000101100"),                //  67
      new PatternHelper(  68, 'd', "10000100110"),                //  68
      new PatternHelper(  69, 'e', "10110010000"),                //  69
      new PatternHelper(  70, 'f', "10110000100"),                //  70
      new PatternHelper(  71, 'g', "10011010000"),                //  71
      new PatternHelper(  72, 'h', "10011000010"),                //  72
      new PatternHelper(  73, 'i', "10000110100"),                //  73
      new PatternHelper(  74, 'j', "10000110010"),                //  74
      new PatternHelper(  75, 'k', "11000010010"),                //  75
      new PatternHelper(  76, 'l', "11001010000"),                //  76
      new PatternHelper(  77, 'm', "11110111010"),                //  77
      new PatternHelper(  78, 'n', "11000010100"),                //  78
      new PatternHelper(  79, 'o', "10001111010"),                //  79
      new PatternHelper(  80, 'p', "10100111100"),                //  80
      new PatternHelper(  81, 'q', "10010111100"),                //  81
      new PatternHelper(  82, 'r', "10010011110"),                //  82
      new PatternHelper(  83, 's', "10111100100"),                //  83
      new PatternHelper(  84, 't', "10011110100"),                //  84
      new PatternHelper(  85, 'u', "10011110010"),                //  85
      new PatternHelper(  86, 'v', "11110100100"),                //  86
      new PatternHelper(  87, 'w', "11110010100"),                //  87
      new PatternHelper(  88, 'x', "11110010010"),                //  88
      new PatternHelper(  89, 'y', "11011011110"),                //  89
      new PatternHelper(  90, 'z', "11011110110"),                //  90
      new PatternHelper(  91, '{', "11110110110"),                //  91
      new PatternHelper(  92, '|', "10101111000"),                //  92
      new PatternHelper(  93, '}', "10100011110"),                //  93
      new PatternHelper(  94, 'E', "10001011110"),                //  94
      new PatternHelper(  95, 'E', "10111101000"),                //  95
      new PatternHelper(  96, 'E', "10111100010"),                //  96
      new PatternHelper(  97, 'E', "11110101000"),                //  97
      new PatternHelper(  98, 'E', "11110100010"),                //  98
      new PatternHelper(  99, 'I', "10111011110"),                //  99
      new PatternHelper( 100, 'I', "10111101110"),               //  100
      new PatternHelper( 101, 'I', "11101011110"),               //  101
      new PatternHelper( 102, 'I', "11110101110"),               //  102
      new PatternHelper( 103, '*', "11010000100"),               //  103  Start Code A
      new PatternHelper( 104, '*', "11010010000"),               //  104  Start Code B
      new PatternHelper( 105, '*', "11010011100"),               //  105  Start Code C
      new PatternHelper( 106, '*', "11000111010")                //  106  STOP
    };
    #endregion


    private static int LookupIndexByChar(char c) {
      int index = -1;
      
      for (int pos = 0; pos < _charCount; pos++) {
        if (c == _patterns[pos].Character) {
          index = pos;
          break;
        }
      }

      return index;
    }

    public static Bitmap CreateCode128BBitmap(int imageWidth, int imageHeight, float fontSize, Color foregroundColor, Color backgroundColor, string barcodeText, int moduleWidth, bool doNotShowText, bool addQuietZone ) {

      StringBuilder moduleSequence = new StringBuilder();

      /**
       * Initialize the checksum with the start character index.
       * We also might use a START char #104 for Table B or #105.
       */
      int index = 0;
      int checksum = _indexStartCodeB;
      moduleSequence.Append(_patterns[_indexStartCodeB].Pattern); 
     
      for (int charPos = 0; charPos < barcodeText.Length; charPos++) {

        index = LookupIndexByChar(barcodeText[charPos]);
        
        // Invalid characters will be replaced by a space character
        if (index < 0 || index > _charCount) { 
          index = 0;
        }

        moduleSequence.Append(_patterns[index].Pattern);
        checksum += (1 + charPos) * index;
      }

      /**
       * Now calculate the checksum.
       * Use the checksum as the index into our barcode table
       * to retrive the barcode pattern for this value.
       */
      index = checksum % _charCount;
      moduleSequence.Append(_patterns[index].Pattern);

       /**
        * Place the STOP char at the end of the bar code.
        */
      moduleSequence.Append(_patterns[_indexStopCode].Pattern);

      /**
       * Place the TERMINATION at the end of the bar code.
       */
      moduleSequence.Append(_sequenceTerminator);

      // Add leading and trailing quietzone if needed.
      if (addQuietZone) {
        String quietZoneSequence = new String('0', _moduleLen - 1 );
        moduleSequence.Insert(0, quietZoneSequence);
        moduleSequence.Append(quietZoneSequence);
      }

      /**
       * Calculate the moduleWidth if in AUTO mode.
       * Otherwise resize the image to fit the complete sequence.
       */
      if(moduleWidth <= 0) {
         moduleWidth = Math.Max(1, (int)Math.Floor((double)(imageWidth - _borderLeft - _borderRight) / (double)moduleSequence.Length));
      } else {
      // Make sure it is not smaller than 1;
      moduleWidth = Math.Max(1, moduleWidth);
      // Now calc the image size according to the moduleWidth;
      imageWidth = moduleWidth * moduleSequence.Length;
      }

      /**
       * Check to see if the barcode fits on the image width.
       * Main target is to draw the barcode and _not_ to keep the correct image width.
       */
      if ((_borderLeft + (moduleWidth * moduleSequence.Length) + _borderRight) > imageWidth) {
         imageWidth = _borderLeft + (moduleWidth * moduleSequence.Length) + _borderRight;
      }

      Brush foregroundBrush = new SolidBrush(foregroundColor);

      Bitmap bitmap = new Bitmap(imageWidth, imageHeight);
      
      using (Graphics gfx = Graphics.FromImage(bitmap)) {

        /**
         * Enable high qualitity rendering...
         */
        gfx.InterpolationMode = InterpolationMode.High;
        gfx.CompositingQuality = CompositingQuality.HighQuality;

        /**
         * Clear the complete image w/ the background color.
         */
         gfx.Clear(backgroundColor);

        /**
         * Now loop thru all the characters in the moduleSequence string.
         * For an "0" character draw a filled rectangle with the module width.
         * No rocket sience. ;-)
         */
        int firstModulePos = _borderLeft +  (imageWidth - _borderLeft - _borderRight - (moduleWidth * moduleSequence.Length)) / 2;
        int posX = firstModulePos;
          foreach (char c in moduleSequence.ToString()) {
            if ('1' == c) {
              gfx.FillRectangle(foregroundBrush, posX, 5, moduleWidth, imageHeight - 10);
            }
            posX += moduleWidth;
          }
          int lastModulePos = posX;

         /**
          * If we have to show the text then print it centered on the bottom edge of the barcode.
          * We will show the text in a horizontal stretched manner.
          */
          if (!doNotShowText) {

            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.TextRenderingHint = TextRenderingHint.AntiAlias;

            // Create the font - we will use an Arial font face of bold type.
            Font drawFont = new Font("Arial", (float)fontSize, FontStyle.Bold);

            // Determine the overall size of the the text.
            SizeF sizeOfText = gfx.MeasureString(barcodeText, drawFont);

            // Calculate the origin of the text to be printed and the rectangle that will hold the text.
            Point textOrigin = new Point((int)(imageWidth - sizeOfText.Width ) / 2, (int)(imageHeight - sizeOfText.Height + 5));
            Rectangle textRect = new Rectangle(textOrigin, new Size((int)sizeOfText.Width, (int)sizeOfText.Height));

            using (Bitmap textImage = new Bitmap((int)sizeOfText.Width, (int)sizeOfText.Height)) { 
              using (Graphics textGfx = Graphics.FromImage(textImage)) {
                textGfx.InterpolationMode = InterpolationMode.High;
                textGfx.CompositingQuality = CompositingQuality.HighQuality;
                textGfx.SmoothingMode = SmoothingMode.AntiAlias;
                textGfx.TextRenderingHint = TextRenderingHint.AntiAlias;
                textGfx.FillRectangle(new SolidBrush(backgroundColor), new Rectangle(new Point(0, 0), textImage.Size));
                textGfx.DrawString(barcodeText, drawFont, foregroundBrush, 0f, 0f);
                textGfx.Flush();
              }

            // Right Border = Left border = 2 * moduleWidth * moduleLen
            textRect.X = firstModulePos + (moduleWidth * _moduleLen);
            textRect.Width = lastModulePos - (moduleWidth * _moduleLen) - textRect.X; 
            gfx.DrawImage(textImage, textRect);
            }

          }

          
          gfx.Flush();
      }

      return bitmap;
    }

  }
}
