# WTT

Based on [UAssetAPI](https://github.com/atenfyr/UAssetAPI).

It replaces existing strings, so you can't add new ones. Tested on version 3.7.10.

**Beware:** There is no error handling since this tool was originally meant for my personal use.

### Compilation

Compile like a normal .NET 6 application. WTT.sln has everything you will need if you use Visual Studio.

### Usage

When it asks, put in the path to your `Message` directory. Usually, this would be `C:\whatever\WindowsNoEditor\Mercury\Content\Message`.

Then, choose a mode:

* **bulk_export** - bulk export all the `uasset` files in the path given as toml files in the same directory
* **bulk_import** - bulk import all the `toml` files
	* directory of TOML files: where your TOML files are
	* uasset directory: where your uasset files are (useful as a way to back up the original uasset files)
	* output uasset directory: where your new uasset files will be put
* **e** - export single toml file from uasset
* **i** - import single uasset file from toml
