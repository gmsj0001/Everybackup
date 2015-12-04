using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Everybackup
{
    class Program
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpszSection, string lpszKey, string lpszDefault, StringBuilder lpReturnedString, int cchReturnBuffer, string lpszFile);
        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern int Everything_SetSearchW(string lpSearchString);
        [DllImport("Everything32.dll")]
        public static extern bool Everything_QueryW(bool bWait);
        [DllImport("Everything32.dll")]
        public static extern int Everything_GetNumResults();
        [DllImport("Everything32.dll", CharSet = CharSet.Unicode)]
        public static extern void Everything_GetResultFullPathNameW(int nIndex, StringBuilder lpString, int nMaxCount);

        static string GetIniString(string key)
        {
            var retVal = new StringBuilder(260);
            GetPrivateProfileString("Everybackup", key, "", retVal, retVal.Capacity, Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\Everybackup.ini");
            return retVal.ToString();
        }

        static HashSet<string> EverythingSearch(string search)
        {
            Everything_SetSearchW(search);
            Everything_QueryW(true);
            var set = new HashSet<string>();
            var sum = Everything_GetNumResults();
            for (int i = 0; i < sum; ++i)
            {
                var str = new StringBuilder(260);
                Everything_GetResultFullPathNameW(i, str, 260);
                set.Add(str.ToString());
            }
            return set;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(
@"=================================
Everybackup v1.0 beta by gmsj0001
         [http://lxf.me]
=================================");
            var files = new HashSet<string>();
            var confs = EverythingSearch("wfn:" + GetIniString("ListFile")).ToList();
            confs.Sort();
            foreach (var conf in confs)
            {
                var lines = File.ReadAllLines(conf).Select(line => line.Trim()).Where(line => line != "");
                var path = Path.GetDirectoryName(conf).TrimEnd('\\') + '\\';
                foreach (var line in lines)
                {
                    if (line[0] != '|')
                    {
                        Console.WriteLine("Include: " + path + line);
                        files.UnionWith(EverythingSearch("file:" + path + line));
                    }
                    else
                    {
                        Console.WriteLine("Exclude: " + path + line.Substring(1));
                        files.ExceptWith(EverythingSearch("file:" + path + line.Substring(1)));
                    }

                }
            }
            var list = files.ToList();
            list.Sort();
            File.WriteAllLines("Everybackup.lst", list.ToArray(), Encoding.Default);

            var execProgram = GetIniString("ExecProgram");
            if (execProgram != "")
                System.Diagnostics.Process.Start(GetIniString("ExecProgram"), GetIniString("ExecArguments"));
        }
    }
}
