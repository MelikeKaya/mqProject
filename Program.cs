using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Data.OracleClient;
using System.Data;
using System.Net.Sockets;

namespace mqProject
{
    public static class Program
    {
        static OracleCommand cmd = new OracleCommand();
        static void Main(string[] args)
        {
            try
            {
                string ipAddress = "";
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.ToString();
                    }
                }

                Console.WriteLine(ipAddress);

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

                dbConnection(connectionString);

                gateBox(ipAddress);
                hostBox(ipAddress);

                dbConnection(null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static OracleConnection dbConnection(string connString)
        {
            try
            {
                OracleConnection con = new OracleConnection(connString);
                if (connString != null)
                {
                    con.Open();
                    cmd = con.CreateCommand();
                    return con;
                }
                else
                {
                    con.Close();
                    return con;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }


        }

        public static void gateBox(string ipAddress)
        {
            try
            {
                cmd.CommandText = "SELECT column1,column2 "
                                            + "FROM table1 "
                                            + "WHERE condition";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                Console.WriteLine(cmd.CommandText);
                Console.WriteLine(reader);

                if (reader != null)
                {
                    Console.WriteLine(dt.Columns.Count);
                    Console.WriteLine(dt.Rows.Count);
                    for (int i = 0; i < dt.Columns.Count ; i++)
                    {
                        for (int j = 0; j < dt.Rows.Count ; j++)
                        {
                            string row = dt.Rows[j][i].ToString().Replace("-", "").Replace(" ", "");
                            if (row != "")
                            {
                                var que = new MessageQueue().CreateIfNotExists(row);
                                Console.WriteLine(row);
                            }
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        public static void hostBox(string ipAddress)
        {
            try
            {
                cmd.CommandText = "SELECT column1,column2 "
                                            + "FROM table2 "
                                            + "WHERE condition";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                Console.WriteLine(cmd.CommandText);
                Console.WriteLine(reader);

                if (reader != null)
                {
                    Console.WriteLine(dt.Columns.Count);
                    Console.WriteLine(dt.Rows.Count);

                    for (int i = 0; i < dt.Columns.Count ; i++)
                    {
                        for (int j = 0; j < dt.Rows.Count ; j++)
                        {
                            string row = dt.Rows[j][i].ToString().Replace("-", "").Replace(" ", "");
                            if (row != "")
                            {
                                var que = new MessageQueue().CreateIfNotExists(row);
                                Console.WriteLine(row);
                            }
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        public static MessageQueue CreateIfNotExists(this MessageQueue messageQueue, string queueName)
        {
            try
            {
                MessageQueue queue;
                //Bu alanda ".\Private$" yolu, oluşturmak/bulmak istediğimiz queue'nin private olduğunu belirtiyor. Ardından dosya yoluna queue ismi ile devam ediyoruz. 
                if (MessageQueue.Exists($@".\Private$\{queueName}"))
                {
                    //Queue mevcut, bu queue için bir C# objesi oluşturup dönüyoruz.
                    queue = new MessageQueue($@".\Private$\{queueName}");
                    queue.Label = $@"private$\{queueName}";
                    queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                }
                else
                {
                    //Queue mevcut değil, "Create" metodu ile yeni bir queue oluşturup, bu queue'nun C# objesini dönüyoruz. Tam yetki veriyoruz.
                    queue = MessageQueue.Create($@".\Private$\{queueName}");
                    queue.Label = $@"private$\{queueName}";
                    queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);
                }
                return queue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

    }
}
