﻿using FileHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace tgif_clarifi
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string CLARIFI_API_ROOT = "https://api.clarifai.com/v1/";
        private static readonly long MIN_TTL_MS = 60000;
        private static string client_id = "hkOom3PZmyIrKsyyx8mYU7nJ5gM2BU_TT_N8h4jI";
        private static string client_secret = "Xan8IwoAc2qKqPve_T6HzW46y6AJlUM39rqhElVB";
        private static readonly string tgif_file_path = "tgif-v1.0.tsv.txt";
        private static string access_token;
        private static long expiration;
        private static Gif[] gif_array;

        static void Main(string[] args)
        {
            try
            {
                ReadTgifFile();
                string token = RetrieveAccessToken();
                int countId = 1;
                foreach (Gif gif in gif_array)
                {
                    Console.WriteLine(Environment.NewLine + "Runing for image #id = " + countId);
                    GifResult gifRes = RunTagForUrl(gif);

                    string tagsOutput = "";
                    foreach (Tag tag in gifRes.tagsList)
                    {
                        tagsOutput = tagsOutput + tag.ToString() + ",";
                    }
                    if (tagsOutput.Length > 2)
                        tagsOutput.Remove(tagsOutput.Length - 2);

                    // This text is always added, making the file longer over time
                    // if it is not deleted.
                    using (StreamWriter sw = File.AppendText("tgif_clarifi_results.csv"))
                    {
                        sw.WriteLine(countId + ", \"" + gifRes.gif.GifUrl + "\", " + EscapeCSV(gifRes.gif.GifDesc) + ", " + EscapeCSV(tagsOutput) +" ,"+ gifRes.time);
                    }
                    countId++;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(ex);
            }
            
        }

        public static string EscapeCSV(string data)
        {
            if (data.Contains("\""))
            {
                data = data.Replace("\"", "\"\"");
            }

            if (data.Contains(","))
            {
                data = String.Format("\"{0}\"", data);
            }

            if (data.Contains(System.Environment.NewLine))
            {
                data = String.Format("\"{0}\"", data);
            }

            return data;
        }

        static Gif[] ReadTgifFile()
        {
            try
            {
                var engine = new FileHelperEngine<Gif>();

                // To Read Use:
                gif_array = engine.ReadFile(tgif_file_path);

                return gif_array;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.Error(ex);
            }
            return null;
        }

        static string RetrieveAccessToken()
        {
            try
            {
                if (!String.IsNullOrEmpty(Program.access_token) && System.Environment.TickCount < Program.expiration - MIN_TTL_MS)
                    return Program.access_token;

                var client = new RestClient(CLARIFI_API_ROOT);                
                var request = new RestRequest("token", Method.POST);

                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("client_id", client_id);
                request.AddParameter("client_secret",client_secret);

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                
                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content; // raw content as string
                dynamic json = JValue.Parse(content);
                // values require casting

                if (response.StatusCode !=  System.Net.HttpStatusCode.OK)
                {
                    log.Error(content);
                }
                access_token = json.access_token;
                int expiresIn = json.expires_in;
                expiration = System.Environment.TickCount + expiresIn;

                return access_token;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return "";
        }

        /// <summary>
        /// Launch the legacy application with some options set.
        /// </summary>
        static GifResult RunTagForUrl(Gif gif)
        {
            GifResult gifRes = new GifResult();
            try
            {
                gifRes.gif = gif;
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "Run.bat",
                        Arguments = gif.GifUrl,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                // Create new stopwatch.
                Stopwatch stopwatch = new Stopwatch();

                // Begin timing.
                stopwatch.Start();
                proc.Start();
                stopwatch.Stop();
                gifRes.time = stopwatch.Elapsed.TotalMilliseconds + " ms";

                List<Tag> tagsList = new List<Tag>();
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                    if (line.Contains("("))
                    {
                        string[] tagResults = line.Replace(")", "").Split('(');
                        Tag tag = new Tag(tagResults[0].Trim(), Double.Parse(tagResults[1].Trim()));
                        tagsList.Add(tag);
                    }
                }

                gifRes.tagsList = tagsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                log.Error(ex);
            }

            return gifRes;
        }


    }
}
