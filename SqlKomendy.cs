using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class SqlKomendy
    {
        static string connectionstring = "Data Source=217.173.198.135:1522/orcltp.iaii.local; User Id=s95578; Password=s95578;";
        public static int GetRowCount(string tabela)
        {
            int rowCount = 0;
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                OracleCommand command = connection.CreateCommand();
                command.CommandText = "SELECT *" + " FROM " + tabela;
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rowCount++;
                }
                connection.Close();
            }

            return rowCount;
        }
        public static List<int> GetIds(string columnName, string tableName)
        {
            List<int> ids = new List<int>();
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                OracleCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " + columnName + " FROM " + tableName;
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(Convert.ToInt32(reader[columnName]));
                }
                connection.Close();
            }

            return ids;
        }
        public static List<string> Getvin(string columnName, string tableName)
        {
            List<string> ids = new List<string>();
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                OracleCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " + columnName + " FROM " + tableName;
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(Convert.ToString(reader[columnName]));
                }
                connection.Close();
            }

            return ids;
        }
        public static List<DateTime> GetDate(string columnName, string tableName)
        {
            List<DateTime> date = new List<DateTime>();
            using (OracleConnection connection = new OracleConnection(connectionstring))
            {
                OracleCommand command = connection.CreateCommand();
                command.CommandText = "SELECT " + columnName + " FROM " + tableName;
                connection.Open();
                OracleDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    date.Add(Convert.ToDateTime(reader[columnName]));
                }
                connection.Close();
            }

            return date;
        }
        public static void InsertAdres(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Adres (adres_id,kod_poczt,miejsc,ulica,nr_domu,nr_mieszk) values(:adres_id, :kod_poczt, :miejsc, :ulica, :nr_domu, :nr_mieszk)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        command.Parameters.Add(new OracleParameter("adres_id", GetRowCount("Adres")));
                        command.Parameters.Add(new OracleParameter("kod_poczt", Generatory.generatorKod_poczt()));
                        command.Parameters.Add(new OracleParameter("miejsc", Generatory.generatorZnakow(10, false)));
                        command.Parameters.Add(new OracleParameter("ulica", Generatory.generatorZnakow(10, true)));
                        command.Parameters.Add(new OracleParameter("nr_domu", Generatory.generatorLiczb(4, 0)));
                        command.Parameters.Add(new OracleParameter("nr_mieszk", Generatory.generatorLiczb(4, 0)));
                        commandsToTextFile.Add("insert into Adres (adres_id,kod_poczt,miejsc,ulica,nr_domu,nr_mieszk) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " 
                            + command.Parameters[3].Value.ToString() + ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                    }
                }
            }
            Generatory.zapis_plik("Adres", commandsToTextFile);

        }
        public static void InsertDane(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> adresId = new List<int>();
            List<int> foreignKeys = new List<int>();
            int liczba_rand = 0;
            foreignKeys.Clear();
            adresId.Clear();
            adresId = GetIds("adres_id", "Adres");
            foreignKeys = GetIds("adres_adres_id", "Dane");
            adresId.RemoveAll(i => foreignKeys.Contains(i));
            if (adresId.Count == 0)
            {
                throw new Exception("brak mozliwych do uzycia wierszy w tabeli adres");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Dane (dane_id, e_mail , adres_adres_id, telefon) values(:dane_id, :e_mail, :adres_adres_id, :telefon)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        liczba_rand = rnd.Next(adresId.Count - 1);
                        command.Parameters.Add(new OracleParameter("dane_id", GetRowCount("Dane")));
                        command.Parameters.Add(new OracleParameter("e_mail",Generatory.generatorZnakow(15,false)));
                        command.Parameters.Add(new OracleParameter("adres_adres_id", adresId[liczba_rand]));
                        command.Parameters.Add(new OracleParameter("telefon", Convert.ToString(Generatory.generatorTelefon())));
                        commandsToTextFile.Add("insert into Dane (dane_id, e_mail, adres_adres_id, telefon) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                        adresId.Remove((int)command.Parameters[2].Value);
                    }
                }
            }
            foreignKeys.Clear();
            adresId.Clear();
            Generatory.zapis_plik("Dane", commandsToTextFile);
        }
        public static void InsertPracownik(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> daneId = new List<int>();
            List<DateTime> daty = GetDate("data_zatrudnienia", "Pracownik");
            DateTime data;
            if (daty.Count == 0)
            {
                data = new DateTime(1995, 1, 1);
            }
            else
            {
                data = daty[daty.Count - 1];
            }
            List<int> foreignKeys = new List<int>();
            int liczba_rand = 0;
            foreignKeys.Clear();
            daneId.Clear();
            daneId = GetIds("dane_id", "Dane");
            foreignKeys = GetIds("dane_dane_id", "Pracownik");
            daneId.RemoveAll(i => foreignKeys.Contains(i));
            if (daneId.Count == 0)
            {
                throw new Exception("brak mozliwych do uzycia wierszy w tabeli dane");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Pracownik (PRACOWNIK_ID, imie, nazwisko, fax, dane_dane_id, data_zatrudnienia) values(:PRACOWNIK_ID, :imie, :nazwisko, :fax, :dane_dane_id, :data_zatrudnienia)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        int range = (DateTime.Today - data).Days;
                        data = data.AddDays(rnd.Next(range));
                        liczba_rand = rnd.Next(daneId.Count - 1);
                        command.Parameters.Add(new OracleParameter("PRACOWNIK_ID", GetRowCount("Pracownik")));
                        command.Parameters.Add(new OracleParameter("imie", Generatory.generatorZnakow(9, false)));
                        command.Parameters.Add(new OracleParameter("nazwisko", Generatory.generatorZnakow(9, true)));
                        command.Parameters.Add(new OracleParameter("fax", Convert.ToString(Generatory.generatorTelefon())));
                        command.Parameters.Add(new OracleParameter("dane_dane_id", daneId[liczba_rand]));
                        command.Parameters.Add(new OracleParameter("data_zatrudnienia", data));
                        commandsToTextFile.Add("insert into Pracownik (PRACOWNIK_ID, imie, nazwisko, fax, dane_dane_id, data_zatrudnienia) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " 
                            + command.Parameters[3].Value.ToString() + ", "+ command.Parameters[4].Value.ToString() + ", "+ command.Parameters[5].Value.ToString() + ")");
                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                        daneId.Remove((int)command.Parameters[4].Value);
                    }
                }
            }
            foreignKeys.Clear();
            daneId.Clear();
            Generatory.zapis_plik("Pracownik", commandsToTextFile);
        }
        public static void InsertProducent(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Producent (id_producent,nazwa_producent) values(:id_producent, :nazwa_producent)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        command.Parameters.Add(new OracleParameter("id_producent", GetRowCount("Producent")));
                        command.Parameters.Add(new OracleParameter("nazwa_producent", Generatory.generatorZnakow(10,true)));
                        commandsToTextFile.Add("insert into Producent (id_producent,nazwa_producent) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                    }
                }
            }
            Generatory.zapis_plik("Producent", commandsToTextFile);

        }
        public static void InsertCzesci(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> producentIds = new List<int>();
            int liczba_rand=0;
            producentIds.Clear();

            producentIds = GetIds("id_producent", "Producent");
            if(producentIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Producent");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Czesci (id_czesci, kod_produktu, stan_czesci, cena_netto, cena_brutto, procent_vat, Producent_id_producent)" +
                    " values(:id_czesci, :kod_produktu, :stan_czesci, :cena_netto, :cena_brutto, :procent_vat, :Producent_id_producent)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        liczba_rand = rnd.Next(producentIds.Count - 1);
                        command.Parameters.Add(new OracleParameter("id_czesci", GetRowCount("Czesci")));
                        command.Parameters.Add(new OracleParameter("kod_produktu", Generatory.generatorZnakow(3,false)+Generatory.generatorLiczb(8,0)));
                        command.Parameters.Add(new OracleParameter("stan_czesci", Generatory.generatorZnakow(7,true)));
                        command.Parameters.Add(new OracleParameter("cena_netto", double.Parse(Generatory.generatorLiczb(6,2))));
                        command.Parameters.Add(new OracleParameter("cena_brutto", double.Parse(Generatory.generatorLiczb(6, 2))));
                        command.Parameters.Add(new OracleParameter("procent_vat", 23));
                        command.Parameters.Add(new OracleParameter("Producent_id_producent", producentIds[liczba_rand]));
                        commandsToTextFile.Add("insert into Czesci (id_czesci, kod_produktu, stan_czesci, cena_netto, cena_brutto, procent_vat, Producent_id_producent) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() + 
                            ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ", " + command.Parameters[6].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();

                    }
                }
            }
            Generatory.zapis_plik("Czesci", commandsToTextFile);
            producentIds.Clear();
        }
        public static void InsertKlient(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> daneId = new List<int>();
            List<int> foreignKeys = new List<int>();
            int liczba_rand = 0;
            foreignKeys.Clear();
            daneId.Clear();
            daneId = GetIds("dane_id", "Dane");
            foreignKeys = GetIds("dane_dane_id", "Klient");
            daneId.RemoveAll(i => foreignKeys.Contains(i));
            if (daneId.Count == 0)
            {
                throw new Exception("brak mozliwych do uzycia wierszy w tabeli dane");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Klient (id_klient, imie, nazwisko, nr_dow_osobistego, dane_dane_id) values(:id_klient, :imie, :nazwisko, :nr_dow_osobistego, :dane_dane_id)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        liczba_rand = rnd.Next(daneId.Count - 1);
                        command.Parameters.Add(new OracleParameter("id_klient", GetRowCount("Klient")));
                        command.Parameters.Add(new OracleParameter("imie", Generatory.generatorZnakow(9, false)));
                        command.Parameters.Add(new OracleParameter("nazwisko", Generatory.generatorZnakow(9, true)));
                        command.Parameters.Add(new OracleParameter("nr_dow_osobistego", Generatory.generatorZnakow(3,true)+Generatory.generatorLiczb(6,0)));
                        command.Parameters.Add(new OracleParameter("dane_dane_id", daneId[liczba_rand]));
                        commandsToTextFile.Add("insert into Klient (id_klient, imie, nazwisko, nr_dow_osobistego, dane_dane_id) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", "
                            + command.Parameters[3].Value.ToString() + ", " + command.Parameters[4].Value.ToString() + ")");
                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();
                        daneId.Remove((int)command.Parameters[4].Value);
                    }
                }
            }
            foreignKeys.Clear();
            daneId.Clear();
            Generatory.zapis_plik("Pracownik", commandsToTextFile);
        }
        public static void InsertFaktura(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> klientIds = new List<int>();
            List<DateTime> daty = GetDate("data_uslugi", "Faktura");
            DateTime data;
            if (daty.Count == 0)
            {
                data = new DateTime(2010, 1, 1);
            }
            else
            {
                data = daty[daty.Count - 1];
            }
            int liczba_rand = 0;
            klientIds.Clear();

            klientIds = GetIds("id_klient", "Klient");
            if (klientIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Klient");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Faktura (id_faktura, data_uslugi, wartosc_netto, wartosc_brutto, nr_faktury_sprzedazy, wartosc_vat, forma_platnosci, Klient_id_klient)" +
                    " values(:id_faktura, :data_uslugi, :wartosc_netto, :wartosc_brutto, :nr_faktury_sprzedazy, :wartosc_vat, :forma_platnosci, :Klient_id_klient)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        int range = (DateTime.Today - data).Days;
                        data = data.AddDays(rnd.Next(range));
                        liczba_rand = rnd.Next(klientIds.Count - 1);
                        command.Parameters.Add(new OracleParameter("id_faktura", GetRowCount("Faktura")));
                        command.Parameters.Add(new OracleParameter("data_uslugi", data));
                        command.Parameters.Add(new OracleParameter("wartosc_netto", Generatory.generatorLiczb(6, 2)));
                        command.Parameters.Add(new OracleParameter("wartosc_brutto", Generatory.generatorLiczb(6, 2)));
                        command.Parameters.Add(new OracleParameter("nr_faktury_sprzedazy", Generatory.generatorZnakow(2,true)+"/"+Generatory.generatorLiczb(1,0)+"/"+Generatory.generatorLiczb(4,0)));
                        command.Parameters.Add(new OracleParameter("wartosc_vat", "23"));
                        command.Parameters.Add(new OracleParameter("forma_platnosci", Generatory.generatorZnakow(8,false)));
                        command.Parameters.Add(new OracleParameter("Klient_id_klient", klientIds[liczba_rand]));
                        commandsToTextFile.Add("insert into Faktura (id_faktura, data_uslugi, wartosc_netto, wartosc_brutto, nr_faktury_sprzedazy, wartosc_vat, forma_platnosci, Klient_id_klient) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() +
                            ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ", " + command.Parameters[6].Value.ToString() + command.Parameters[7].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();

                    }
                }
            }
            Generatory.zapis_plik("Faktura", commandsToTextFile);
            klientIds.Clear();
        }
        public static void InsertPojazd(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<string> vin = new List<string>();
            string nr_Vin;
            bool wyjscie =true;
            vin.Clear();
            
            vin = Getvin("nr_vin", "Pojazd");
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Pojazd (nr_vin, marka, model, rok_produkcji, nr_rej, jaki_pojazd) values(:nr_vin, :marka, :model, :rok_produkcji, :nr_rej, :jaki_pojazd)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        nr_Vin = Generatory.generatorZnakow(4, true) + Generatory.generatorLiczb(6, 0)+ Generatory.generatorLiczb(7, 0);
                        while (true)
                        {
                            for(int j = 0; j < vin.Count; j++)
                            {
                                wyjscie =true;
                                if (vin[j] == nr_Vin)
                                {
                                    wyjscie = false;
                                }
                            }
                            if (wyjscie)
                            {
                                break;
                            }
                            nr_Vin = Generatory.generatorZnakow(4, true) + Generatory.generatorLiczb(6, 0) + Generatory.generatorLiczb(7, 0);
                        }
                        command.Parameters.Add(new OracleParameter("nr_vin", nr_Vin));
                        command.Parameters.Add(new OracleParameter("marka", Generatory.generatorZnakow(7,true)));
                        command.Parameters.Add(new OracleParameter("model", Generatory.generatorZnakow(7, false)));
                        command.Parameters.Add(new OracleParameter("rok_produkcji", int.Parse("201"+Generatory.generatorLiczb(1, 0))));
                        command.Parameters.Add(new OracleParameter("nr_rej", Generatory.generatorZnakow(2, true) + " " + Generatory.generatorLiczb(5, 0)));
                        command.Parameters.Add(new OracleParameter("jaki_pojazd", Generatory.generatorZnakow(6,false)));
                        commandsToTextFile.Add("insert into Pojazd (nr_vin, marka, model, rok_produkcji, nr_rej, jaki_pojazd) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() +
                            ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ", " +")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();

                    }
                }
            }
            Generatory.zapis_plik("Pojazd", commandsToTextFile);
        }
        public static void InsertUsluga(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> klientIds = new List<int>();
            List<int> pracownikIds = new List<int>();
            List<string> pojazdIds = new List<string>();
            List<DateTime> daty = GetDate("\"data-uslugi\"", "Usluga");
            DateTime data;
            if (daty.Count == 0)
            {
                data = new DateTime(2010, 1, 1);
            }
            else
            {
                data = daty[daty.Count - 1];
            }
            klientIds.Clear();
            pracownikIds.Clear();
            pojazdIds.Clear();

            klientIds = GetIds("id_klient", "Klient");
            pracownikIds = GetIds("pracownik_id", "Pracownik");
            pojazdIds = Getvin("nr_vin", "Pojazd");
            if (klientIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Klient");
            }
            if (pracownikIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Pracownik");
            }
            if (pojazdIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Pojazd");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Usluga (id_usluga, \"data-uslugi\", cena, klient_id_klient, pojazd_nr_vin, pracownik_pracownik_id)" +
                    " values(:id_usluga, :\"data-uslugi\", :cena, :klient_id_klient, :pojazd_nr_vin, :pracownik_pracownik_id)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        int range = (DateTime.Today - data).Days;
                        data = data.AddDays(rnd.Next(range));
                        command.Parameters.Add(new OracleParameter("id_usluga", GetRowCount("Usluga")));
                        command.Parameters.Add(new OracleParameter("\"data-uslugi\"", data));
                        command.Parameters.Add(new OracleParameter("cena", Generatory.generatorLiczb(6, 2)));
                        command.Parameters.Add(new OracleParameter("klient_id_klient", klientIds[rnd.Next(klientIds.Count - 1)]));
                        command.Parameters.Add(new OracleParameter("pojazd_nr_vin", pojazdIds[rnd.Next(pojazdIds.Count-1)]));
                        command.Parameters.Add(new OracleParameter("pracownik_pracownik_id", pracownikIds[rnd.Next(pracownikIds.Count-1)]));
                        commandsToTextFile.Add("insert into Usluga (id_usluga, \"data-uslugi\", cena, klient_id_klient, pojazd_nr_vin, pracownik_pracownik_id) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() +
                            ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();

                    }
                }
            }
            Generatory.zapis_plik("Usluga", commandsToTextFile);
            klientIds.Clear();
        }
        public static void InsertPozycjeFaktura(int rows)
        {
            List<string> commandsToTextFile = new List<string>();
            List<int> uslugaIds = new List<int>();
            List<int> fakturaIds = new List<int>();
            List<int> czesciIds = new List<int>();
            List<int> foreignKeys = new List<int>();
            uslugaIds.Clear();
            fakturaIds.Clear();
            czesciIds.Clear();

            uslugaIds = GetIds("id_usluga", "Usluga");
            fakturaIds = GetIds("id_faktura", "Faktura");
            czesciIds = GetIds("id_czesci", "Czesci");
            foreignKeys = GetIds("Usluga_id_Usluga", "Pozycje_faktura");
            uslugaIds.RemoveAll(i => foreignKeys.Contains(i));
            if (uslugaIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Usluga");
            }
            if (fakturaIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Faktura");
            }
            if (czesciIds.Count == 0)
            {
                throw new Exception("brak wierszy w tabeli Czesci");
            }
            Random rnd = new Random();
            for (int i = 0; i < rows; i++)
            {
                var commandText = "insert into Pozycje_faktura (id_pozycja_faktury, cena_netto, cena_brutto, procent_vat, Usluga_id_Usluga, id_faktura, Czesci_id_czesci)" +
                    " values(:id_pozycja_faktury, :cena_netto, :cena_brutto, :procent_vat, :Usluga_id_Usluga, :id_faktura, :Czesci_id_czesci)";

                using (OracleConnection connection = new OracleConnection(connectionstring))
                {
                    using (OracleCommand command = new OracleCommand(commandText, connection))
                    {
                        command.Parameters.Add(new OracleParameter("id_pozycja_faktury", GetRowCount("Pozycje_faktura")));
                        command.Parameters.Add(new OracleParameter("cena_netto", Generatory.generatorLiczb(6, 2)));
                        command.Parameters.Add(new OracleParameter("cena_brutto", Generatory.generatorLiczb(6, 2)));
                        command.Parameters.Add(new OracleParameter("procent_vat", 23));
                        command.Parameters.Add(new OracleParameter("Usluga_id_Usluga", uslugaIds[rnd.Next(uslugaIds.Count - 1)]));
                        command.Parameters.Add(new OracleParameter("id_faktura", fakturaIds[rnd.Next(fakturaIds.Count - 1)]));
                        command.Parameters.Add(new OracleParameter("Czesci_id_czesci", czesciIds[rnd.Next(czesciIds.Count - 1)]));
                        commandsToTextFile.Add("insert into Pozycje_faktura (id_pozycja_faktury, cena_netto, cena_brutto, procent_vat, Usluga_id_Usluga, id_faktura, Czesci_id_czesci) " +
                            "values(" + command.Parameters[0].Value.ToString() + ", " + command.Parameters[1].Value.ToString() + ", " + command.Parameters[2].Value.ToString() + ", " + command.Parameters[3].Value.ToString() +
                            ", " + command.Parameters[4].Value.ToString() + ", " + command.Parameters[5].Value.ToString() + ", " + command.Parameters[6].Value.ToString() + ")");

                        command.Connection.Open();
                        command.ExecuteNonQueryAsync();
                        command.Connection.Close();

                    }
                }
            }
            Generatory.zapis_plik("Usluga", commandsToTextFile);
            uslugaIds.Clear();
            fakturaIds.Clear();
            czesciIds.Clear();
        }
    }
}
