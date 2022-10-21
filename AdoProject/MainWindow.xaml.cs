using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdoProject.OtherForms;

namespace AdoProject
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox2.SelectedItem != null)
            {
                DB db = new DB();
                db.NameServer = TextBox.Text;
                db.NameDataBase = ListBox2.SelectedItem.ToString();
                db.Show();
                //this.Close();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ListBox2.Items.Clear();
            string connectionString = $"Server={TextBox.Text};Trusted_Connection=True;Encrypt=False;";
            string connectionDB = "select name from sysdatabases";
            if (TextBox.Text != "")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(connectionDB, connection);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    ListBox2.Items.Add(reader[0].ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("Tables not find");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                ListBox2.Items.Remove("master");
                ListBox2.Items.Remove("tempdb");
                ListBox2.Items.Remove("model");
                ListBox2.Items.Remove("msdb");
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
