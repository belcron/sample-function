using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FunctionApp2
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string str)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(str);
            streamWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
