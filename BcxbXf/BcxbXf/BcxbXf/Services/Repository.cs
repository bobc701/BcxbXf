using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using System.Linq;


namespace BcxbXf.Services
{
   static class Repository {

      public static StreamReader GetTextFileOnDisk(string fName) {
      // --------------------------------------------------------------------------------
      // This returns StreamReader for file stored under Resources.
      // FileName should include folders separated by '.'.
      // EG: Model.cfeng1 <-- Note: Case sensitive!
      // --------------------------------------------------------------------------------
         var assembly = typeof(MainPage).GetTypeInfo().Assembly;
         string path = @"BcxbXf.Resources." + fName;

         // For testing, look at these...
         /* Strings returned have these 5 parts, all separated by dots...
          * - Default namespace ('BcxbXf')
          * - 'Resources'
          * - Folder structure ('Model' or 'Teams' etc)
          * - File name
          * - Extension ('bcxt')
          */ 
         var files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
         files = assembly.GetManifestResourceNames();

         Stream strm = assembly.GetManifestResourceStream(path);
         //strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
         return new StreamReader(strm);

      }


      public static string[] GetFileListFromDisk (string pattern) {
      // -----------------------------------------------------------------
         string[] files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
         return files.Where(f => f.Contains(pattern)).ToArray();
      
      }


      //      public static StreamReader GetTextFileOnLine(string token) {
      //         // ---------------------------------------------------------------
      //         //WebClient client = new WebClient(); 
      //         string path = "";
      //         Stream strm;

      //         switch (token) {
      //            case "DataDate": path = @"http://www.4bcx.com/NflData/DataDate1.txt"; break;
      //            case "Spread": path = @"http://www.4bcx.com/NflData/SpreadTable3.txt"; break;
      //            case "Results": path = @"http://www.4bcx.com/NflData/ResultsTemplate1.txt"; break;
      //            case "Schedule": path = @"http://www.4bcx.com/NflData/Schedule2.txt"; break;
      //         }

      //      // ----------------------------------------------------
      //      // Found this approach on Web.
      //      // Uses HttpWebRequest i/o WebClient, and so allows you 
      //      // to set the timeout period.
      //      // HttpWebRequest --> HttpWebResponse --> Stream --> StreamReader
      //      // ----------------------------------------------------
      //         var request = (HttpWebRequest)WebRequest.Create(path);
      //         request.Timeout = 30000;
      //         request.ReadWriteTimeout = 30000;
      //         //-bc 1807.01: using 'using' per StackOverflow...
      //         //using (var wresp = (HttpWebResponse)request.GetResponse()) { 
      //         //   strm = wresp.GetResponseStream();
      //         //}
      //         var wresp = (HttpWebResponse)request.GetResponse();
      //         strm = wresp.GetResponseStream();

      //         //strm = client.OpenRead(path);
      //         return new StreamReader(strm);

      //      }



   }
}
