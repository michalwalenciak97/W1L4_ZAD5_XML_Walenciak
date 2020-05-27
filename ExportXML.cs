using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfApp1
{
    public class ExportXML
    {
        static string connectionstring = "";
        public int columnCount;
        public string xml;
        
        
        public static void example(string table) {
            string fileName = table + ".xml";
            List<string> final = new List<string>();
            final.Add("<?xml version="+'"'+"1.0"+'"'+"?>");
            final.Add("<"+table+">");
            
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                OracleCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM " + table;
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    for (int i = 0; i <= reader.FieldCount; i++)
                    {
                        if (i == 0)
                        {
                            final.Add("<" + reader.GetName(i) +" id=\""+ reader.GetValue(i)+"\">");
                        }
                        else if (i == reader.FieldCount)
                        {

                            final.Add("</" + reader.GetName(0) + ">");
                        }
                        else
                        {
                            final.Add("<" + reader.GetName(i) + ">" + reader.GetValue(i).ToString() + "</" + reader.GetName(i) + ">");
                        }
                    }
                }
                connection.Close();
            }
            final.Add("</" + table + ">");
            try
            {   
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                } 
                using (StreamWriter ws = File.CreateText(fileName))
                {  
                    for(int i = 0; i < final.Count; i++)
                    {
                        ws.WriteLine(final[i]);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
    }
}
