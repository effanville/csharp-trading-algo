using System;
using System.IO;
using System.Text;

namespace TradingConsole
{
    public class ConsoleStreamWriter
    {
        private readonly MemoryStream stream;
        public string filePath;

        public ConsoleStreamWriter(MemoryStream st)
        {
            stream = st;
        }

        public void Write(string text)
        {
            Console.WriteLine(text);
            if (stream != null)
            {
                byte[] array = Encoding.UTF8.GetBytes(text + "\n");
                stream.Write(array, 0, array.Length);
            }
        }

        public void SaveToFile(string file = null)
        {
            if (stream != null)
            {
                try
                {
                    string fileToWriteTo = file ?? filePath;
                    if (string.IsNullOrEmpty(fileToWriteTo))
                    {
                        return;
                    }

                    FileStream fileWrite = new FileStream(fileToWriteTo, FileMode.Create, FileAccess.Write);
                    stream.WriteTo(fileWrite);
                    fileWrite.Close();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
