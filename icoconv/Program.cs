using System;
using System.IO;

namespace icoconv
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("ICO Converter version 1.1\nCopyright (c) 2019 David Simunič. All rights reserved.\n\nUsage: ICOCONV.EXE <input filename> <output filename>");
                Environment.Exit(0);
            }

            string input = args[0]; //Input filename
            string output = args[1]; //Output filename
            short firstWord; //The first word in the file

            //Read the first word to determine the format of the input file
            using (BinaryReader b = new BinaryReader(File.Open(input, FileMode.Open)))
            {
                firstWord = b.ReadInt16();
            }

            if (firstWord == 1) //Input file is in old format, converting to new
            {
                IcoOldToNew(input, output);
            }
            else if (firstWord == 0) //Input file is in new format, converting to old
            {
                IcoNewToOld(input, output);
            }
        }

        //Converts icon in old format to new format - THIS ASSUMES A DEVICE-INDEPENDENT OLD ICON THAT'S 64x64 IN SIZE AND MONOCHROME!
        static void IcoOldToNew(string input, string output)
        {
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
            byte[] newHeader = { 00, 00, 01, 00, 01, 00, 0x40, 0x40, 02, 00, 01, 00, 01, 00, 0x30, 04, 00, 00, 0x16, 00, 00, 00 };

            using (BinaryReader b = new BinaryReader(File.Open(input, FileMode.Open)))
            {
                int pos = 0xE; //At offset 0xE is the first bitmap
                b.BaseStream.Seek(pos, SeekOrigin.Begin);

                //Read each bitmap into its own byte array
                firstBitmap = b.ReadBytes(512);
                secondBitmap = b.ReadBytes(512);
            }

            //Reverse the row order for each bitmap (aka mirror the bitmap vertically)
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newFirstBitmap[504 - i * 8 + j] = secondBitmap[i * 8 + j];
                    newSecondBitmap[504 - i * 8 + j] = firstBitmap[i * 8 + j];
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

        //Converts icon in new format to old format - THIS ASSUMES A SINGLE IMAGE 64x64 MONOCHROME NEW FORMAT ICON!
        static void IcoNewToOld(string input, string output)
        {
            byte[] firstBitmap; //The first bitmap
            byte[] secondBitmap; //The second bitmap
            byte[] newFirstBitmap = new byte[512]; //The new first bitmap (=vertically mirrored second bitmap)
            byte[] newSecondBitmap = new byte[512]; //The new second bitmap (=vertically mirrored first bitmap)

            //The following header is currently hardcoded, though I don't imagine there's much variety possible here considering the limitations
            //of the old ICO format.
            byte[] oldHeader = { 01, 00, 00, 00, 00, 00, 0x40, 00, 0x40, 00, 0x08, 00, 00, 00 };

            using (BinaryReader b = new BinaryReader(File.Open(input, FileMode.Open)))
            {
                int pos = 0x46; //At offset 0x46 is the first bitmap
                b.BaseStream.Seek(pos, SeekOrigin.Begin);

                //Read each bitmap into its own byte array
                firstBitmap = b.ReadBytes(512);
                secondBitmap = b.ReadBytes(512);
            }

            //Reverse the row order for each bitmap (aka mirror the bitmap vertically)
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newFirstBitmap[504 - i * 8 + j] = secondBitmap[i * 8 + j];
                    newSecondBitmap[504 - i * 8 + j] = firstBitmap[i * 8 + j];
                }
            }

            //Write the new data to the output file in new format
            using (BinaryWriter b = new BinaryWriter(File.Open(output, FileMode.Create)))
            {
                b.Write(oldHeader);
                b.Write(newFirstBitmap);
                b.Write(newSecondBitmap);
            }

            Console.WriteLine("New format icon " + input + " has been successfully converted to old format icon " + output + ".");
            Console.ReadLine();
        }
    }
}