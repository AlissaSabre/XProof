XProof
======

A simple Windows tool to help proofreading XLIFF bilingual files using Microsoft Word.

## What is it

### Functions

* XProof allows you to check the target texts of XLIFF files using Microsoft Word's spell checking and grammar checking functions by easier operations.
* XProof supports XLIFF files, as well as XLIFF-based packages such as wsxz, xlz, or mqxlz files.

### Backgrounds

XLIFF is an OASIS standard format for bilingual documents, which is used in many CAT (computer aided translation) environments.

Microsoft Word comes with a good _Spelling and Grammar_ checker built in.
It is useful for proofreading a translated documents.

A problem is, however, that most CAT environment has no easy interface to do so.
Some CAT software have features to write contents into a Word file,
but the produced files are often not very suitable for using Word's review features;
e.g., due to wrong LangIDs or tag markers disrupting text analyses.
You can combine some tools and/or several manual operations to get a Word file suitable for spell/grammar checking,
but it is time consuming.

XProof is a sort of a quick-hack to solve the issue;
it takes a set of XLIFF files (or packages containing XLIFF files, e.g., wsxz)
and produces a single Word file (docx) that is suitable for the spelling and grammar check functions of Microsoft Word,
then run Microsoft Word's _Spelling and Grammar_ function.

## How to use it

### Prerequisites

* Windows and .NET Framework 4.5.
* Microsoft Word (2010 or later) that is capable of handling your target language.

## Installation

Go to [Releases](https://github.com/AlissaSabre/XProof/releases) page,
download the latest msi file,
then double click on it.

You need an administrator priviledge to install XProof.

### Running

1. Double click on the desktop shortcut that the installer created, or otherwise run the program XProof.exe.
2. Click on the "..." button to choose XLIFF files or packages to proofread.
3. Click on the "Run" button.

You should then see Microsoft Word is starting, and,
if any spelling/grammar errors are found,
Word's dialog box to tell an issue will be shown as always.

Note that **any changes you made on Word are simply lost.**

Instead, you need to use your CAT software to correct any errors detected by XProof.

### Command line tool

In addition to the ordinary GUI application (XProof.exe),
XProof provides a command line tool (XProofCmd.exe).
You can use it in your own batch file, for example.

The command line syntax is: XProofCmd filename...
It has no options/switches.

## Programmers' info.

### Environment and Dependency

* XProof is written in C# with WinForms, targetting .NET Framework 4.5.
* XProof uses [NetOffice] (http://netoffice.codeplex.com) to use Word functions.
* XProof's installer is created with [WiX Toolset] (http://wixtoolset.org/).
* I'm using a free-of-charge Visual Studio 2015 Express.

### How to build

1. Grab the set of source files.
2. Start Visual Studio and open XProof.sln.
3. Go to Tools | NuGet Package Manager | Manage NuGet Packages for Solution. The Visual Studio should say "Some NuGet packages are missing ...", showing two NetOffice packages.  Restore them.
4. Build the solution.  You will get exe files both on Release and Debug builds.  You will get msi file only on Release build.

**Please change GUIDs appropriately** before distribution, if you modify XProof or reuse parts of its files for another project.
