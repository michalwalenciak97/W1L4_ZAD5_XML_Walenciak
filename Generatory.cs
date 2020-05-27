using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WpfApp1
{
    public class Generatory
    {
        static string znaki = "mnbvcxzlkjhgfdsapoiuytrewq";
        static string cyfry = "1234567890";
        static class StringHelper
        {
            public static string ReverseString(string myStr)
            {
                char[] myArr = myStr.ToCharArray();
                Array.Reverse(myArr);
                return new string(myArr);
            }
        }
        public static string generatorLiczb(int ilosc_znakow, int ile_przecinek)
        {
            Random rnd = new Random();
            string liczba = null;
            ilosc_znakow -= ile_przecinek;
            liczba = rnd.Next(int.Parse(Math.Pow(10, ilosc_znakow).ToString())).ToString();
            if (ile_przecinek > 0)
            {
                liczba += "," + rnd.Next(int.Parse(Math.Pow(10, ile_przecinek - 1).ToString()), int.Parse(Math.Pow(10, ile_przecinek).ToString())).ToString();
            }
            return liczba;
        }
        public static string generatorZnakow(int ilosc_znakow,bool zmiana)
        {
            string gotowy = null;
            Random rnd = new Random();
            if (zmiana)
            {
                znaki = StringHelper.ReverseString(znaki);
            }
            for (int i = 0; i < ilosc_znakow; i++)
            {
                gotowy += znaki[rnd.Next(znaki.Length)];
            }
            return gotowy;
        }
        public static string generatorKod_poczt()
        {
            string kod_poczt = null;
            Random rnd = new Random();
            for (int i = 0; i < 6; i++)
            {
                if (i == 2)
                {
                    kod_poczt += "-";
                }
                else
                {
                    kod_poczt += rnd.Next(9);
                }
            }
            //Thread.Sleep(4);
            return kod_poczt;
        }
        public static string generatorTelefon()
        {
            string telefon=null;
            Random rnd = new Random();
            for(int i = 0; i < 9; i++)
            {
                telefon += cyfry[rnd.Next(cyfry.Length)];
            }
            return telefon;
        }
        public static void zapis_plik(string tabela,List<string> text)
        {
            System.IO.File.WriteAllLines(tabela+".txt", text);
        }
    }
}
