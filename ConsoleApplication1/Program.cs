using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCachingSingleton;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var bytes = Singleton.Instance.GetBytesFromFile("WatchPicture");

                Console.WriteLine(bytes.ToString());
            }
        }
    }
}
