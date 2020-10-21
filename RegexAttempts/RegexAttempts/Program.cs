using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using MaxMind.GeoIP2;
using System.Net;
using MaxMind.GeoIP2.Responses;
using System.Net.Sockets;

namespace RegexAttempts
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex IPaddress = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b");
            //string regexIP = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            //string regextimestamp = @"(?P<timestamp>\d{2}\/\\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4})";
            Regex Timestamp = new Regex(@"\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}");
            Regex IPandTimestamp = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b - - \[\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}\] ");
            string IpTimestamppattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b - - \[\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}\]";
            DatabaseReader ipcountry = new DatabaseReader(@"C:\Users\cerbe\Downloads\maxmind\GeoLite2-Country.mmdb");
            DatabaseReader ipASN = new DatabaseReader(@"C:\Users\cerbe\Downloads\maxmind\GeoLite2-ASN.mmdb");
            //int linecount = File.ReadLines(@"C:\yek\access.log").Count();
            Logfileentries logfileentry = new Logfileentries();
            List<Logfileentries> logfiles = new List<Logfileentries>();
            string hostname = "";
            foreach(String line in File.ReadLines(@"C:\yek\access.log"))
            {
                Match iptimestamp = IPandTimestamp.Match(line);
                Match ipaddress = IPaddress.Match(iptimestamp.Value);
                Match timestamp = Timestamp.Match(iptimestamp.Value);
                CountryResponse countryresponse = ipcountry.Country(ipaddress.Value);
                AsnResponse ASNresponse = ipASN.Asn(ipaddress.Value);
                try
                {
                    hostname = Dns.GetHostEntry(ipaddress.Value).HostName;
                }
                catch (SocketException)
                {
                  
                }
                if (hostname == "")
                {
                    hostname = "unknown domain";
                }
                string[] logmethod = Regex.Split(line, IpTimestamppattern); //kun interesseret i pos 2
                Console.WriteLine("iptimestamp {0} \n ipadresse {1} \n timestamp {2} \n method {3}",iptimestamp.Value,ipaddress.Value,timestamp.Value, logmethod[2]);
                Console.WriteLine("country {0} \n ASN {1} \n DNS {2}", countryresponse.Country,ASNresponse.AutonomousSystemOrganization, hostname);
                logfileentry.Logipaddress = ipaddress.Value;
                logfileentry.Logtimestamp = iptimestamp.Value;
                logfileentry.Logmethod = logmethod[2];
                logfileentry.Logcountry = countryresponse.Country.ToString();
                logfileentry.LogASN = ASNresponse.AutonomousSystemOrganization.ToString();
                logfileentry.LogDNS = hostname;
                logfiles.Add(logfileentry);
            }
            //Console.WriteLine(linecount);

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
        private string logASN;
        public string LogASN
        {
            get { return logASN; }
            set { logASN = value; }
        }
        private string logDNS;
        public string LogDNS
        {
            get { return logDNS; }
            set { logDNS = value; }
        }
    }
}
