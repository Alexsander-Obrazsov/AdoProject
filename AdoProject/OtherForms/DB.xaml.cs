using AdoProject.Classes;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AdoProject.OtherForms
{
    public partial class DB : Window
    {
        public ObservableCollection<StoreTable>? StoreTable;

        public string? NameServer { get; set; }
        public string? NameDataBase { get; set; }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
            string query = $"Select * From {ListBox.SelectedItem}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                SqlDataAdapter dataAdp = new SqlDataAdapter(command);
                DataTable dt = new DataTable($"{ListBox.SelectedItem}");
                dataAdp.Fill(dt);
                DataBase.ItemsSource = dt.DefaultView;
            }
        }
        private void DataBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataBase.SelectedItem != null)
            {
                EditLineButton.IsEnabled = true;
            }
            else
            {
                EditLineButton.IsEnabled = false;
            }
        }
        private void AddLineButton_Click(object sender, RoutedEventArgs e)
        {
            Row.IsEnabled = true;
            StoreTable = new ObservableCollection<StoreTable>();
            DataBase.ItemsSource = StoreTable;
        }
        private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedTable = (string)ListBox.SelectedItem;
            string connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
            string query = $"Delete {selectedTable} Where ID = {((System.Data.DataRowView)DataBase.SelectedItem).Row.ItemArray[0]}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                DataBase.ItemsSource = QueryTable.ListSelectedTable(selectedTable, connectionString).ItemsSource;
                DataBase.Items.Refresh();
            }
        }

        private void ReplaceRow()
        {
            ObservableCollection<StoreTable> StoreTable = new ObservableCollection<StoreTable>
            {
                //new StoreTable {row = }
            };
            StoreTable.CollectionChanged += StoreTable_CollectionChanged;
            DataBase.ItemsSource = StoreTable;

        }
        private static void StoreTable_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    StoreTable? storeTableAdd = e.NewItems[0] as StoreTable;
                    MessageBox.Show("Object add");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    StoreTable? storeTableRemove = e.OldItems[0] as StoreTable;
                    MessageBox.Show("Object remove");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    StoreTable? storeTableReplaced = e.OldItems[0] as StoreTable;
                    StoreTable? storeTableReplacing = e.NewItems[0] as StoreTable;
                    MessageBox.Show("Object replace");
                    break;
            }

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Row.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EditLineButton.IsEnabled = false;
            Row.Visibility = Visibility.Visible;

            string connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
            string query = $"SELECT TABLE_NAME FROM [{NameDataBase}].INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ListBox.Items.Add(reader[0].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Tables not find");
                    }
                }
            }
            ListBox.Items.Remove("sysdiagrams");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
