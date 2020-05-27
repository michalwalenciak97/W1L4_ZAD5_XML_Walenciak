using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string tabela = null;
        int ilosc_wierszy=0;
        List<string> Tabele = new List<string>
        {
            "Adres",
            "Dane",
            "Pracownik",
            "Producent",
            "Czesci",
            "Faktura",
            "Klient",
            "Pojazd",
            "Usluga",
            "Pozycje_faktura"
        };
        public MainWindow()
        {
            InitializeComponent();
            comboTabela.ItemsSource = Tabele;
            comboTabela.SelectedItem = Tabele[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ilosc_wierszy = int.Parse(textBox.Text);
                if (tabela == Tabele[0])
                {
                    try
                    {
                        SqlKomendy.InsertAdres(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
                if (tabela == Tabele[1])
                {
                    try
                    {
                        SqlKomendy.InsertDane(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[2])
                {
                    try
                    {
                        SqlKomendy.InsertPracownik(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[3])
                {
                    try
                    {
                        SqlKomendy.InsertProducent(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[4])
                {
                    try
                    {
                        SqlKomendy.InsertCzesci(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[5])
                {
                    try
                    {
                        SqlKomendy.InsertFaktura(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[6])
                {
                    try
                    {
                        SqlKomendy.InsertKlient(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[7])
                {
                    try
                    {
                        SqlKomendy.InsertPojazd(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[8])
                {
                    try
                    {
                        SqlKomendy.InsertUsluga(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (tabela == Tabele[9])
                {
                    try
                    {
                        SqlKomendy.InsertPozycjeFaktura(ilosc_wierszy);
                        MessageBox.Show("Koniec");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabela = comboTabela.SelectedItem.ToString();
        }

        private void Button_Click_XML(object sender, RoutedEventArgs e)
        {
            try
            {
                ExportXML.example(tabela);
                MessageBox.Show("Koniec");
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_XML_Import(object sender, RoutedEventArgs e)
        {
            try
            {
                ImportXML.example(tabela);
                MessageBox.Show("Koniec");

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
