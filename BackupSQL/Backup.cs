using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using BackupSQL.Utilities;

namespace BackupSQL
{
    public class Backup
    {
     
        private string connectionString;
        private string URLBackup;
        private string Database;
        private string DBInstance;
        private string User;
        private string Password;
        private string FileName;
        private string FullBackupURL;
        private int BackupRetention;
        private bool Debug;


        public Backup()
        {
            System.Console.WriteLine("Reading parameters");
            //EN: Getting app.config parameters
            //ES: Obteniendo los parámetros del app.config 
            this.URLBackup = ConfigurationManager.AppSettings["URLBackup"];
            this.Database = ConfigurationManager.AppSettings["Database"];
            this.DBInstance = ConfigurationManager.AppSettings["DBInstance"];
            this.User = ConfigurationManager.AppSettings["User"];
            this.Password = ConfigurationManager.AppSettings["Password"];
            this.BackupRetention = int.Parse(ConfigurationManager.AppSettings["BackupRetention"].ToString());
            this.Debug = bool.Parse(ConfigurationManager.AppSettings["Debug"].ToString());
            
            //EN: Creating connection string to connecto with SQL SERVER EXPRESS
            //ES: Creando cadena de conexión para conectarse con SQL SERVER EXPRESS
            connectionString = @"Data source=" + DBInstance 
                               + ";Initial Catalog=" + Database 
                               + "; user="  + User+ "; password=" + Password 
                               + "; Trusted_Connection=false;";

            
            
        }

        private bool ExecuteSQLBackup()
        {
            //Name of the file wihtout extension
            //Nombre del archivo sin extensión
            FileName = "backup" + Utilities.Utilities.DateFormat(DateTime.Now);
            FullBackupURL = URLBackup + "\\TMP\\" + FileName + ".bak";

            //Check if bakcup already exist on folder
            //Revisar si el backup ya existia en la carpeta
            if (File.Exists(FullBackupURL))
            {
                System.Console.WriteLine("Backup already exist!");
                return true;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"BACKUP DATABASE [" + Database + "] TO  DISK = N'" + FullBackupURL + "';";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (SqlException e)
                    {
                        System.Console.WriteLine(e.Message.ToString(), "Error Message");
                        connection.Close();
                        return false;
                    }

                }


            }

            //If backup existe return true;
            if (File.Exists(FullBackupURL))
                return true;
            else
                return false;

        }

        private bool CreateZipBackup()
        {

            try
            {

                if (File.Exists(FullBackupURL))
                {
                    ZipFile.CreateFromDirectory(URLBackup + "\\TMP\\", URLBackup + FileName + ".zip");
                    if (File.Exists(URLBackup + FileName + ".zip"))
                        return true;
                    else
                        return false;


                }
                else
                    return false;

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message.ToString(), "Error Message");
                return false;
            }
        }

        public bool BackupDatabase()
        {

            //Create TMP directory

            //Step 1 creating la carpeta tempora TMP
            //Paso 1 creating la carpeta temporal TMP
            System.Console.WriteLine("\nSTEP 1: Creating TMP (temporal) folder...");

            if (Utilities.Utilities.CreateDirectory(URLBackup + "TMP"))
                System.Console.WriteLine("Success!");
            else
                System.Console.WriteLine("Failed!");

            //Step 2 Creating backup from SQL SERVER Instance
            //Paso 1 Creando backup desde la instancia de SQL SERVER
            System.Console.WriteLine("\nSTEP 2: Creating Backup from SQL SERVER Express Instance...");

            if (ExecuteSQLBackup())
            {

                System.Console.WriteLine("Success!");

                System.Console.WriteLine("\nSTEP 3: Creating ZIP file from SQL SERVER Backup...");

                //STEP 3: Creating Zip from SQL Backup
                //Paso 3: Creando Zip del Backup de SQL
                if(CreateZipBackup())
                    System.Console.WriteLine("Success!");
                else
                    System.Console.WriteLine("Failed!");

            }
            else
                System.Console.WriteLine("Failed!");

            System.Console.WriteLine("\nSTEP 4: Deleting TMP folder...");

            if(Utilities.Utilities.DeleteDirectory(URLBackup + "TMP"))
                System.Console.WriteLine("Success!");
            else
                System.Console.WriteLine("Failed!");

            System.Console.WriteLine("\nSTEP 5: Deleting old backups based on file creation date...");

            if(CleanUpBackups())
                System.Console.WriteLine("Success!");
            else
                System.Console.WriteLine("Failed!");


            Console.WriteLine("\nBackupSQL v1.0.0 free tool created by Rodolfo Borja [rborja.net]");
            Console.WriteLine("Report bugs to rborja.net@gmail.com");
            Console.WriteLine("Donate through PayPal https://www.paypal.me/RodolfoBorjaLopez");

            if(Debug)
            Console.ReadKey();

            return true;

           
        }

       private bool CleanUpBackups()
       {

           if (BackupRetention < 1)
               return true;

           try
           {

               DirectoryInfo info = new DirectoryInfo(URLBackup);
               FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTimeUtc).ToArray();

               if (files.Length > BackupRetention)
               {
                   int limit = files.Length - BackupRetention;

                   for (int i = 0; i < limit; i++)
                   {
                       System.Console.WriteLine("File to delete: " + files[i].Name);
                       File.Delete(files[i].FullName);
                       System.Console.WriteLine("File " + files[i].Name + " deleted");
                   }

                   return true;

               }
               else
                   return true;

           }
           catch (Exception e)
           { 
              System.Console.WriteLine(e.Message.ToString(), "Error Message");  
              return false;
           }

       }
        
       

    }
}
