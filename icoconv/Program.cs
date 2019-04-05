using System;
using System.IO;

namespace icoconv
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("ICO Converter version 1.0\nCopyright (c) 2019 David Simunič. All rights reserved.\n\nNo arguments given. Use /? for help.");
                Environment.Exit(0);
            }

            //The help switch
            if(args.Length == 1 && args[0] == "/?")
            {
                Console.WriteLine("ICO Converter version 1.0\nCopyright (c) 2019 David Simunič. All rights reserved.\n\n" +
                    "Usage: ICOCONV.EXE <options> <input filename> <output filename>\n\nAvailable options:\n" +
                    "  /? : displays this message");
                Environment.Exit(0);
            }

            if(args.Length == 2)
            {
                string input = args[0]; //Input filename
                string output = args[1]; //Output filename
                short firstWord; //The first word in the file

                //Read the first word to determine the format of the input file
                using (BinaryReader b = new BinaryReader(File.Open(input, FileMode.Open)))
                {
                    firstWord = b.ReadInt16();
                    //Console.WriteLine("firstWord: " + firstWord);
                }

                if (firstWord == 1) //Input file is in old format, converting to new
                {
                    OldToNew(input, output);
                }
                else if (firstWord == 0) //Input file is in new format, converting to old
                {
                    NewToOld(input, output);
                }
            }
        }

        //Converts icon in old format to new format
        static void OldToNew(string input, string output)
        {
            short widthPixels; //Width of the bitmap in pixels
            short heightPixels; //Height of the bitmap in pixels
            short widthBytes; //Width of the bitmap in bytes

            byte[] firstBitmap; //The first bitmap
            byte[] secondBitmap; //The second bitmap
            byte[] newFirstBitmap = new byte[512]; //The new first bitmap (=vertically mirrored second bitmap)
            byte[] newSecondBitmap = new byte[512]; //The new second bitmap (=vertically mirrored first bitmap)

            //The following two arrays are currently hardcoded, though I don't imagine there's much variety possible here considering the limitations
            //of the old ICO format.
            //
            //The Windows 3.x DIB header used in the new format
            byte[] bmpHeader = {0x28, 00, 00, 00, 0x40, 00, 00, 00, 0x80, 00, 00, 00, 01, 00, 01, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00,
                                00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 0xFF, 0xFF, 0xFF, 00};

            //The new ICO header
            byte[] newHeader = {00, 00, 01, 00, 01, 00, 0x40, 0x40, 02, 00, 01, 00, 01, 00, 0x30, 04, 00, 00, 0x16, 00, 00, 00};

            using (BinaryReader b = new BinaryReader(File.Open(input, FileMode.Open)))
            {
                int pos = 6; //At offset 6 is the word that contains the width of the bitmap in pixels
                b.BaseStream.Seek(pos, SeekOrigin.Begin);

                //Read the relevant information (icon dimensions) from the header
                widthPixels = b.ReadInt16();
                heightPixels = b.ReadInt16();
                widthBytes = b.ReadInt16();

                b.ReadInt16(); //Advance the stream to the start of the first bitmap

                //Read each bitmap into its own byte array
                firstBitmap = b.ReadBytes(512);
                secondBitmap = b.ReadBytes(512);
            }

            //Console.WriteLine("widthPixels: " + widthPixels + ", heightPixels: " + heightPixels + ", widthBytes: " + widthBytes);

            //Reverse the row order for each bitmap (aka mirror the bitmap vertically)
            for (int i = 0; i < 64; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    newFirstBitmap[504-i*8+j] = secondBitmap[i*8+j];
                    newSecondBitmap[504-i*8+j] = firstBitmap[i*8+j];
                }
            }

            //Write the new data to the output file in new format
            using (BinaryWriter b = new BinaryWriter(File.Open(output, FileMode.Create)))
            {
                b.Write(newHeader);
                b.Write(bmpHeader);
                b.Write(newFirstBitmap);
                b.Write(newSecondBitmap);
            }

            Console.WriteLine("Old format icon " + input + " has been successfully converted to new format icon " + output + ".");
            Console.ReadLine();
        }

        //Converts icon in new format to old format
        static void NewToOld(string input, string output)
        {

        }
    }
}