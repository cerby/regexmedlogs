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
            Logfileentries logfileentry = new Logfileentries();
            List<Logfileentries> logfiles = new List<Logfileentries>();
            string MMcountrydbpath = "";
            string MMAsndbpath = "";
            string logpath1 = "";
            string logpath2 = "";
            string logpath3 = "";

            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath1));
            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath2));
            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath3));
            foreach (Logfileentries log in logfiles)
            {
                Console.WriteLine(log.Logipaddress+"||"+log.Logtimestamp+"||"+log.Logmethod+"||"+log.Logcountry+"||"+log.LogASN+"||"+log.LogDNS);
            }
        }
        public static List<Logfileentries> logfilesentry(string dbcountrypath, string dbasnpath, string logpath)
        {
            Logfileentries logfileentry = new Logfileentries();
            List<Logfileentries> logentries = new List<Logfileentries>();
            Regex IPaddress = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b");
            Regex Timestamp = new Regex(@"\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}");
            Regex IPandTimestamp = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b - - \[\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}\] ");
            DatabaseReader ipcountry = new DatabaseReader(dbcountrypath);
            DatabaseReader ipASN = new DatabaseReader(dbasnpath);
            string hostname = "";
            foreach (var line in File.ReadLines(logpath))
            {
                Match iptimestamp = IPandTimestamp.Match(line);
                Match ipaddress = IPaddress.Match(iptimestamp.Value);
                Match timestamp = Timestamp.Match(iptimestamp.Value);
                CountryResponse countryresponse = ipcountry.Country(ipaddress.Value);
                AsnResponse ASNresponse = ipASN.Asn(ipaddress.Value);
                try{ hostname = Dns.GetHostEntry(ipaddress.Value).HostName; }
                catch (SocketException){}
                if (hostname == ""){ hostname = "unknown domain";}
                string[] logmethod = Regex.Split(line, IPandTimestamp.ToString());
                logfileentry.Logipaddress = ipaddress.Value;
                logfileentry.Logtimestamp = iptimestamp.Value;
                logfileentry.Logmethod = logmethod[2];
                logfileentry.Logcountry = countryresponse.Country.ToString();
                logfileentry.LogASN = ASNresponse.AutonomousSystemOrganization.ToString();
                logfileentry.LogDNS = hostname;
                logentries.Add(logfileentry);
            }
            return logentries;
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
