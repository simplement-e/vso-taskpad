using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimplementE.TaskPad.Business
{
    public static class Logger
    {

        static object _lockForLogEntries = new object();

        static string _logEntriesToken = "9814e73f-aaad-44a6-86a6-4f712725528d";
        static TcpClient _traceClient = null;
        static NetworkStream _traceStream = null;


        static void OpenLogEntriesSocket()
        {
            try
            {
                _traceClient = new TcpClient("data.logentries.com", 10000);
                _traceStream = _traceClient.GetStream();
            }
            catch
            {
                _traceClient = null;
                _traceStream = null;
            }
        }

        public static void LogException(Exception e, string categorie = null)
        {
            string tmp = e.ToString();
            tmp = tmp.Replace("\n", " ").Replace("\r", "");
            if (string.IsNullOrEmpty(categorie))
                categorie = "-";
            Log("error|" + categorie + "|" + tmp, true);
        }

        public static void LogError(string content, string categorie = null)
        {
            content = content.Replace("\n", " ").Replace("\r", "");
            if (string.IsNullOrEmpty(categorie))
                categorie = "-";
            Log("error|" + categorie + "|" + content, true);
        }

        public static void LogInfo(string content, string categorie=null)
        {
            content = content.Replace("\n", " ").Replace("\r", "");
            if (string.IsNullOrEmpty(categorie))
                categorie = "-";
            Log("info|" + categorie + "|" + content);
        }


        private static void Log(string trace, bool estErreur = false, bool retry = false)
        {
            lock (_lockForLogEntries)
            {
                StringBuilder blr = new StringBuilder();
                blr.Append(_logEntriesToken);
                blr.Append(" ");
                blr.Append(Environment.MachineName);
                blr.Append("|");
                blr.Append(DateTime.Now.ToString("HH:mm:ss"));
                blr.Append("|");
                blr.Append(trace);
                blr.Append("\r\n");

                if (estErreur)
                    Trace.TraceError(blr.ToString());
                else
                    Trace.TraceInformation(blr.ToString());


                if (_traceStream == null)
                {
                    if (!string.IsNullOrEmpty(_logEntriesToken))
                        OpenLogEntriesSocket();
                    if (_traceStream == null)
                        return;
                }

                try
                {
                    byte[] bs = Encoding.UTF8.GetBytes(blr.ToString());
                    _traceStream.Write(bs, 0, bs.Length);
                    _traceStream.Flush();
                }
                catch (IOException)
                {
                    try
                    {
                        _traceClient.Close();
                    }
                    catch { }
                    _traceClient = null;
                    _traceStream = null;
                    if (retry)
                        Log(trace, estErreur, false);
                }
                catch (ObjectDisposedException)
                {
                    try
                    {
                        _traceClient.Close();
                    }
                    catch { }
                    _traceClient = null;
                    _traceStream = null;
                    if (retry)
                        Log(trace, estErreur, false);
                }
                catch
                {
                }
            }
        }
    }
}
