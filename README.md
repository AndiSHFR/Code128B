## Code128B
A windows console application to create code128 barcode images (B character set).

If you want to get more familiar with the barcode itself feel free to read the wikipedia article [Code 128](http://http://en.wikipedia.org/wiki/Code_128).


##Get the source code 

Either download the [zip archive](https://github.com/AndiSHFR/Code128B/archive/master.zip) with the complete source code or clone the repository using your git client.
Links can be found on this page.


##Compile the source code

To use `Code128B.exe` you need to compile the program from source code.
To compile the programm from sources you need Microsoft Visual Studio 2010 or higher.
To run the application you need at least .Net Framework 3.0.


##Usage

`Code128B.exe` is a windows console application. 
You need to call the program and supply additional commandline options. 

To run the application open a command prompt and call the program.
Go to the start-orb, type in CMD.EXE and press the enter-key. 
You will get a black window - the command prompt.
Now use the change directory command (cd) to change your working directory to the folder with the `Code128B.exe` application binary.

Type in the programm name, the name of the png image and the text you want to appear on the barcode.

Example:

   `Code128B.exe test.png "This is a test."`
   
Generally you can specify command options to change various aspects of the image content.

   `Code128B.exe [OPTIONS]+ <image filename> <text> <text>`

If there is already a file with the same image name the file will be replaced without any further notice.

##Program exit code

On exit the application will set its exit code.

   * 0   no error.
   * -1  an unspecified error occured.
   * -2  invalid command line option error.


##Command line options

There are various command line options to control the look of the barcode image.

   `
   * ?|help          Show the help message.

   * w|width         Optional: Image width in pixels.
   * h|height        Optional: Image height in pixels.
   * f|foreground    Optional: Foreground color. You can use HTML color codes. i.e. #0080FF.
   * b|background    Optional: Background color. You can use HTML color codes. i.e. #0080FF.
   * s|size          Optional: Font size in pixels.
   * n|notext        Optional: Do not display the barcode text on the barcode.
   * q|quietzone     Optional: Add leading and trailing quiet zone.
   `

##Examples

* Create a 1024x320 image named barcode.png with the barcode "PU4711".

   `Code128B.exe barcode.png PDU4711`


* Create a 1024x320 image named barcode.png with the barcode "PU4711" and yellow background and blue foreground.

   `Code128B.exe --background yellow --foreground blue barcode.png PDU4711`

   You can specify colors in HTML notation too - like #FFFF00

   `Code128B.exe --background #FFFF00 --foreground #0000FF barcode.png PDU4711`


* Create a 1024x320 image named barcode.png with the barcode "PU4711" and yellow background and blue foreground.

   `Code128B.exe --background yellow --foreground blue barcode.png PDU4711`
   

##Remarks

The main focus is on creating the barcode image and not on keeping the desired image size.
If the barcode will not fit on the specified image width the programm will extend the image to make the barcode fit on the image.

If you want to test the barcode w/o having a barcode reader at hand start searching the internet for "online barcode reader".
You will find various sites where you can upload an image and the site tries to read the barcode in the image for you.


##License
The MIT License (MIT)

Copyright (c) 2015 Andreas Schaefer

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
