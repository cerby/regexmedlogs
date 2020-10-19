using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace RegexAttempts
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex filter = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b");
            //string regexIP = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            //string regextimestamp = @"(?P<timestamp>\d{2}\/\\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4})";
            Regex filter2 = new Regex(@"\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}");
            Regex filter3 = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b - - \[\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}\] ");

            int linecount = File.ReadLines(@"C:\yek\access.log").Count();
            Console.WriteLine(linecount);

        }
    }

    public class Logfileentries
    {
        private string logipaddress;
        public string Logipaddress
        {
            get { return logipaddress; }
            set { logipaddress = value; }
        }
        private string logtimestamp;
        public string Logtimestamp
        {
            get { return logtimestamp; }
            set { logtimestamp = value; }
        }
        private string logmethod;
        public string Logmethod
        {
            get { return logmethod; }
            set { logmethod = value; }
        }
        private string logcountry;
        public string Logcountry
        {
            get { return logcountry; }
            set { logcountry = value; }
        }
    }
}
