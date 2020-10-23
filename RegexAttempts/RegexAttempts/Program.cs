using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using MaxMind.GeoIP2;
using System.Net;
using MaxMind.GeoIP2.Responses;
using System.Net.Sockets;
using System.Data.SQLite;
using System.Data.Common;
using System.Linq;

namespace RegexAttempts
{
    class Program
    {
        static void Main(string[] args)
        {
            Logfileentries logfileentry = new Logfileentries();
            List<Logfileentries> logfiles = new List<Logfileentries>();
            string MMcountrydbpath = @"C:\Users\cerbe\Downloads\maxmind\GeoLite2-Country.mmdb";
            string MMAsndbpath = @"C:\Users\cerbe\Downloads\maxmind\GeoLite2-ASN.mmdb";
            string logpath1 = @"C:\yek\accesslogcombinedamazon.log";
            string logpath2 = @"C:\yek\accesslogcombinedfrankfurt.log";
            string logpath3 = @"C:\yek\accesslogcombinedlondon.log";

            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath1));
            Console.WriteLine("amazon added");
            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath2));
            Console.WriteLine("frankfurt added");
            logfiles.AddRange(logfilesentry(MMcountrydbpath, MMAsndbpath, logpath3));
            Console.WriteLine("london added");
            int counter = 0;
            
            SQLiteConnection connection = new SQLiteConnection(@"Data Source=C:\yek\Nginx logs.db");
            connection.Open();
            SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO Accesslogs (AccessID, IPAddress, Timestamp, Logmethod, Logcountry, LogASN, LogDNS)" +
                                                        " VALUES (@AccessID, @IPAddress, @Timestamp, @Logmethod, @Logcountry, @LogASN, @LogDNS)",connection);
            //SqliteConnection connection = new SqliteConnection(@"Data Source=C:\yek\Nginx logs.db");
            //connection.Open();
            List<string> counts = new List<string>();
            Console.WriteLine("connection open");
            //SqliteCommand insertSQL = new SqliteCommand("INSERT INTO Accesslogs (AccessID, IPAddress, Timestamp, Logmethod, Logcountry, LogASN, LogDNS");
            
            foreach (Logfileentries log in logfiles)
            {
                Console.WriteLine(counter);
                //Console.WriteLine(log.Logipaddress+"||"+log.Logtimestamp+"||"+log.Logmethod+"||"+log.Logcountry+"||"+log.LogASN+"||"+log.LogDNS);
                counts.Add(counter.ToString());
                insertSQL.Parameters.AddWithValue("AccessID", counter);
                insertSQL.Parameters.AddWithValue("IPAddress", log.Logipaddress);
                insertSQL.Parameters.AddWithValue("Timestamp", log.Logtimestamp);
                insertSQL.Parameters.AddWithValue("Logmethod", log.Logmethod);
                insertSQL.Parameters.AddWithValue("Logcountry", log.Logcountry);
                insertSQL.Parameters.AddWithValue("LogASN", log.LogASN);
                insertSQL.Parameters.AddWithValue("LogDNS", log.LogDNS);
                insertSQL.ExecuteNonQuery();
                
                counter = counter + 1;
            }
            
            connection.Close();
            Console.WriteLine("connection closed");
            
        }
        public static List<Logfileentries> logfilesentry(string dbcountrypath, string dbasnpath, string logpath)
        {
            
            List<Logfileentries> logentries = new List<Logfileentries>();
            Regex IPaddress = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b");
            Regex Timestamp = new Regex(@"\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}");
            Regex IPandTimestamp = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b - - \[\d{2}\/\w{3}\/\d{4}:\d{2}:\d{2}:\d{2} (\+|\-)\d{4}\] ");
            DatabaseReader ipcountry = new DatabaseReader(dbcountrypath);
            DatabaseReader ipASN = new DatabaseReader(dbasnpath);
            string hostname = "";
            foreach (var line in File.ReadLines(logpath))
            {
                Logfileentries logfileentry = new Logfileentries();
                Match iptimestamp = IPandTimestamp.Match(line);
                Match ipaddress = IPaddress.Match(iptimestamp.Value);
                Match timestamp = Timestamp.Match(iptimestamp.Value);
                //Console.WriteLine(ipaddress.Value);
                CountryResponse countryresponse = ipcountry.Country(ipaddress.Value);
                AsnResponse ASNresponse = ipASN.Asn(ipaddress.Value);
                try{ hostname = Dns.GetHostEntry(ipaddress.Value).HostName; }
                catch (SocketException){}
                if (hostname == ""){ hostname = "unknown domain";}
                string[] logmethod = Regex.Split(line, IPandTimestamp.ToString());
                logfileentry.Logipaddress = ipaddress.Value;
                logfileentry.Logtimestamp = timestamp.Value;
                logfileentry.Logmethod = logmethod[2];
                logfileentry.Logcountry = countryresponse.Country.ToString();
                logfileentry.LogASN = ASNresponse.AutonomousSystemOrganization.ToString();
                logfileentry.LogDNS = hostname;
                logentries.Add(logfileentry);
                //Console.WriteLine(logfileentry.Logipaddress + "||" + logfileentry.Logtimestamp + "||" + logfileentry.Logmethod + "||" + logfileentry.Logcountry + "||" + logfileentry.LogASN + "||" + logfileentry.LogDNS);
                
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
