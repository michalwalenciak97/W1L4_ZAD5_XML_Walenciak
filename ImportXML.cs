using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace WpfApp1
{
    public class ImportXML
    {
        static string connectionstring = "";
        public static void example(string table)
        {
            List<string> kolumnyy = new List<string>();
            List<string> wartt = new List<string>();
            XmlTextReader reader = new XmlTextReader(table+".xml");
            var commandText = "insert into " + table;
            var kolumny = " (";
            var wart = "(";
            int count=0;
            int rowCount;
            string first;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.AttributeCount > 0) wartt.Add(reader.GetAttribute("id"));
                        kolumnyy.Add(reader.Name);
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        wartt.Add(reader.Value);
                        break;
                }
            }
            first = kolumnyy[1];
            for(int i = 0; i < kolumnyy.Count-1; i++)
            {
                if (kolumnyy[i+1] == first && i > 1) break;
                kolumny+=kolumnyy[i+1]+",";
                wart += ":" + kolumnyy[i + 1] + ",";
                count++;
            }

            kolumny = kolumny.Remove(kolumny.Length-1,1);
            kolumny += ")";
            wart = wart.Remove(wart.Length - 1, 1);
            wart += ")";
            commandText += kolumny + " values" + wart;
            rowCount = wartt.Count/(count);
            for(int i = 0; i < rowCount; i++)
            {
                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        for(int j = 0; j < count; j++)
                        {
                            //if (j==0) command.Parameters.Add(new OracleParameter(kolumnyy[j+1], i));
                             command.Parameters.Add(new OracleParameter(kolumnyy[j + 1], wartt[(i*count)+j]));
                        }
                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                    }
                }
            }
            
        }
    }
}
