# icoconv
A simple tool for converting icons between the old and new ICO formats

### About the ICO format

The ICO format was introduced with Windows 1.0 and was implemented as early as November 1984, but it's likely older than that. However, the original version was much simpler than what we consider to be the ICO format today. This modern format was introduced with Windows 3.0 in 1990 and is incompatible with the old one. I decided to write this tool so it's easier to convert icons back and forth between the old format (used in Windows 1.0 and 2.x) and the new format (used by Windows 3.0 and later).

The differences of the old format compared to the new one are:
* **simpler header** - the old header is smaller and simpler compared to the new one
* **only one icon per file** - the old format can only store one icon per file, while the new one supports multiple icon sizes and color depths in a single file
* **no BMP headers** - the bitmaps do not include their DIB headers like they do in the new format
* **no color support** - all icons are black and white only, though transparency is supported
* **the AND and XOR bitmaps are swapped** - the old format has the AND bitmap first and the XOR bitmap second, the new one swaps them
* **row order is reversed** - in other words, the icon is vertically flipped compared to the new format

Additionally, the icon size is limited to 32x32 pixels. While this is not a limitation of the format itself, Windows 1.0 and 2.x only ever displayed icons of this size, imposing a practical limitation to icon size.

The old format also supports a concept called "device independent" and "device dependent" icons. I imagine this is in relation to the display adapter that's used, but the specifics of this are unclear to me, aside from the fact that device-independent icons are stored as double their size (ie. at 64x64 pixels). This type of icons is also the only I ever encountered in practice.

Much like the modern Windows cursor format (CUR) is just slightly modified modern icon format, the old cursor format was likewise just slightly modified old icon format. Cursor conversion is not supported yet, though.

### About the converter

This tool is written in C# and is built using Visual Studio 2017. It requires .NET Framework 4.0, which is served via Windows Update and preinstalled on the more recent versions of Windows. It is a console application you run from a command interpreter like this:

`ICOCONV.EXE <options> <input filename> <output filename>`

* `<options>`: these are command line switches for advanced users. Currently, only the help (`/?`) switch is supported for displaying usage information.
* `<input filename>`: the name/full path of the icon file to be converted. The converter will automatically detect the existing format and convert to the other.
* `<output filename>`: the name/full path of the new icon file after conversion. If the file does not exist, it will be created. If it already exists, it will be overwritten if possible, otherwise an exception will be thrown.

Currently, it's only possible to convert old format to new format. Some values in the header are hardcoded due to the limitations of the old format.
