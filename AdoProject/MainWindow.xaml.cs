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
        /// <summary>
        /// Initialize window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Button "Connect to the DataBase"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectDataBase_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox2.SelectedItem != null)
            {
                DB db = new DB();
                DB.NameServer = TextBox.Text;
                DB.NameDataBase = ListBox2.SelectedItem.ToString();
                db.Show();
                this.Close();
            }
        }
        /// <summary>
        /// Button "Connect to the Server"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectServer_Click(object sender, RoutedEventArgs e)
        {
            ListBox2.Items.Clear();
            string connectionString = $"Server={TextBox.Text};Trusted_Connection=True;Encrypt=False;";
            string query = "select name from sysdatabases";
            StringBuilder errorMessages = new StringBuilder();
            if (TextBox.Text != "")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    try
                    {
                        connection.Open();
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
                                errorMessages.Append("Table not found");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Server not found");
                        }
                        MessageBox.Show(errorMessages.ToString());
                    }
                }
                ListBox2.Items.Remove("master");
                ListBox2.Items.Remove("tempdb");
                ListBox2.Items.Remove("model");
                ListBox2.Items.Remove("msdb");
            }
            else
            {
                MessageBox.Show("Server not found");
            }
        }
        /// <summary>
        /// Button "Exit"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
