using SRModCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPlaylistDownloaderTests
{
    public class TestLogger : SRLogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message, Exception e)
        {
            Console.WriteLine(message + ": " + e.Message);
        }

        public void Msg(string message)
        {
            Console.WriteLine(message);
        }
    }
}
