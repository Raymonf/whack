# WTT

Based on [UAssetAPI](https://github.com/atenfyr/UAssetAPI).

It replaces existing EnglishMessageUSA strings, so you can't add new ones for whatever reason. Tested on version 3.07.01.

**Beware:** There is very little error handling since this tool was originally meant for my personal use. **You should check the changelog out if you've used this in the past.**

### Compilation

Compile like a normal .NET 6 application. WTT.sln has everything you will need if you use Visual Studio.

### Usage

When it asks, put in the path to your `Message` directory. Usually, this would be `C:\whatever\WindowsNoEditor\Mercury\Content\Message`.

Then, decide whether you want to use override mode or not. The default option (when no input is received) is to enable it. When override mode is enabled, bulk import mode will not ask for a path to the uasset directory or the output uasset directory, instead opting to read in and overwrite the uasset files chosen in the first step.

Finally, choose a mode:

* **bulk_export** or **be** - bulk export all the `uasset` files in the path given as toml files in the same directory
* **bulk_import** or **bi** - bulk import all the `toml` files
	* directory of TOML files: where your TOML files are
	* uasset directory: where your uasset files are (useful as a way to back up the original uasset files)
	* output uasset directory: where your new uasset files will be put
* **e** - export single toml file from uasset
* **i** - import single uasset file from toml

If you want to toggle override mode, use the **o** command.

### Changelog

### 3/4/2023

Hello, it's 2023! I hackily added settings. See `ExportSettings.json` and `ImportSettings.json`.

Valid values for `Language` in `ImportSettings.json` are:  `Japanese`, `EnglishUSA`, `EnglishSG`, `TraditionalChineseTW`, `TraditionalChineseHK`, `SimplifiedChinese`, `Korean`

### 11/8/2022

WTT now focuses on editing the `EnglishMessageUSA` column in the strings. Feel free to ask me how you can make your game read that column instead of the `JapaneseMessage` column if you don't know how!

The side effect is that previously exported TOML files need to be updated. Commit `3cfb5fbc63a536f0307e34107f154763869ceb4e` is perfect for updating your files. It'll read in the `JapaneseMessage` values from your translated TOML files, and update the `EnglishMessageUSA` columns **if the TOML `JapaneseMessage` string is not equal to the value in the uasset `JapaneseMessage`**.

In other words, you should use that specific commit and specify a path to a Japanese (_initially_ untouched) `Message` folder. The importer in that commit will update the `EnglishMessageUSA` column instead of the Japanese column, even though it's reading from `JapaneseMessage` in your TOML values.

After that, the latest commit (`dedc8f9fa6fe0d946d83050b0d7a74462ce1436c` or newer) will export Japanese, English, and Korean as separate strings in the TOML file, while outputting `false` for nonexistent (null) values.
