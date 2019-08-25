using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Starting process...");
            Backup backup = new Backup();
            backup.BackupDatabase();
        }
    }
}
