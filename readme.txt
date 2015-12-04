===========
Everybackup
===========

A backup file list configuration tool using Everything for accelerating.


Usage
=====

0. You need Everything installed and launched. .NET 3.5 needed (for Linq support).
1. Download release (or compile by yourself and put Everything32.dll in the directory).
2. Create backup.lst (or other name you set in the ini file) in any folder you want to backup.
3. Edit backup.lst with include lines (eg. photos *.doc) or exclude lines (start with a pipe, eg. |nouse |*.obj).
4. Run the program and a file list named Everyback.lst will be generated.
5. You can set ExecProgram and ExecArguments in the ini file to run archive software automatically after file list generated.

More?
=====

No more. This project is quite simple (less than 100 lines) so you can fork and add anything you want.