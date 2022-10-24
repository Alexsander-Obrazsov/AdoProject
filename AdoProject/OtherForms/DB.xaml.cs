using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AdoProject.OtherForms
{
    public partial class DB : Window
    {
        public static string? NameServer { get; set; }
        public static string? NameDataBase { get; set; }
        private string? connectionString;
        private string? SelectedTable { get; set; }
        private SqlConnection? connection;
        private SqlDataAdapter? dataAdapter;
        private SqlCommand? command;
        private SqlDataReader? reader;
        private DataSet? dataSet;
        private string? query;
        private Dictionary<int, TextBox>? createTextBox;
        private Dictionary<int, RowDefinition>? DeleteRowDefinitions;
        private Dictionary<int, Label>? DeleteLabel;

        private ObservableCollection<object>? Maincollection;

        public DB()
        {
            InitializeComponent();
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            SelectedTable = (string?)ListBox.SelectedItem;
            LoadData();
        }
        private void DataBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void AddLineButton_Click(object sender, RoutedEventArgs e)
        {
            AddRowInTable(true);
            AddRows();
        }
        private void EditLineButton_Click(object sender, RoutedEventArgs e)
        {
            AddRowInTable(true);
            EditRows();
            if (DataBase.SelectedItem is not null)
            {
                for (int i = 0; i < createTextBox.Count; i++)
                {
                    createTextBox[i].Text = ((object[])DataBase.SelectedItem)[i].ToString();
                }
            }
            else
            {
                MessageBox.Show("Change row");
            }
        }
        private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataBase.SelectedItem is not null)
            {
                connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
                query = $"Delete From {SelectedTable} Where ID = {((object[])DataBase.SelectedItem)[0]}";
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                LoadData();
            }
            else
            {
                MessageBox.Show("Change row");
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Row.Visibility = Visibility.Hidden;

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
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Row.Visibility = Visibility.Hidden;
            AddRowInTable(false);
            
            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                Row.RowDefinitions.Remove(DeleteRowDefinitions[i]);
                Row.Children.Remove(DeleteLabel[i]);
                Row.Children.Remove(createTextBox[i]);
            }
        }
        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No){}
            else
            {
                connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
                query = $"DELETE FROM {SelectedTable}";
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
                query = $"DBCC CHECKIDENT ('{SelectedTable}', RESEED, 0)";
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                LoadData();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (createTextBox is not null)
            {
                string stqQueryCol = "";
                string strQueryVal = "";
                for (int i = 1; i < createTextBox.Count; i++)
                {
                    if (double.TryParse(createTextBox[i].Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedNumber))
                    {
                        if (i == createTextBox.Count - 1)
                        {
                            stqQueryCol += DataBase.Columns[i].Header;
                            strQueryVal += createTextBox[i].Text;
                        }
                        else
                        {
                            stqQueryCol += DataBase.Columns[i].Header + ", ";
                            strQueryVal += createTextBox[i].Text + ", ";
                        }
                    }
                    else
                    {
                        if (i == createTextBox.Count - 1)
                        {
                            stqQueryCol += DataBase.Columns[i].Header;
                            strQueryVal += "'" + createTextBox[i].Text + "'";
                        }
                        else
                        {
                            stqQueryCol += DataBase.Columns[i].Header + ", ";
                            strQueryVal += "'" + createTextBox[i].Text + "', ";
                        }
                    }
                }
                connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
                query = $"Insert Into {SelectedTable} ({stqQueryCol}) Values ({strQueryVal})";
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                LoadData();
                Row.Visibility = Visibility.Hidden;
                AddRowInTable(false);
            }

            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                Row.RowDefinitions.Remove(DeleteRowDefinitions[i]);
                Row.Children.Remove(DeleteLabel[i]);
                Row.Children.Remove(createTextBox[i]);
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            string strQuery = "";
            for (int i = 1; i < createTextBox.Count; i++)
            {
                if (double.TryParse(createTextBox[i].Text, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture, out var parsedNumber))
                {
                    if (i == createTextBox.Count - 1)
                    {
                        strQuery += DataBase.Columns[i].Header + " = " + createTextBox[i].Text;
                    }
                    else
                    {
                        strQuery += DataBase.Columns[i].Header + " = " + createTextBox[i].Text + ", ";
                    }
                }
                else
                {
                    if (i == createTextBox.Count - 1)
                    {
                        strQuery += DataBase.Columns[i].Header + " = '" + createTextBox[i].Text + "'";
                    }
                    else
                    {
                        strQuery += DataBase.Columns[i].Header + " = '" + createTextBox[i].Text + "', ";
                    }
                }
            }
            if (createTextBox is not null)
            {
                connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
                query = $"UPDATE {SelectedTable} SET {strQuery} WHERE ID = {createTextBox[0].Text}";
                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                LoadData();
                Row.Visibility = Visibility.Hidden;
                AddRowInTable(false);
            }

            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                Row.RowDefinitions.Remove(DeleteRowDefinitions[i]);
                Row.Children.Remove(DeleteLabel[i]);
                Row.Children.Remove(createTextBox[i]);
            }
        }

        private void AddRowInTable(bool enable)
        {
            if (enable)
            {
                ListBox.IsEnabled = false;
                AddLineButton.IsEnabled = false;
                EditLineButton.IsEnabled = false;
                DeleteLineButton.IsEnabled = false;
                Back.IsEnabled = false;
            }
            else if (enable == false)
            {
                ListBox.IsEnabled = true;
                AddLineButton.IsEnabled = true;
                EditLineButton.IsEnabled = true;
                DeleteLineButton.IsEnabled = true;
                Back.IsEnabled = true;
            }
        }
        private void EditRows()
        {
            Row.Visibility = Visibility.Visible;
            createTextBox = new Dictionary<int, TextBox>();
            DeleteRowDefinitions = new Dictionary<int, RowDefinition>();
            DeleteLabel = new Dictionary<int, Label>();

            Button button = new Button();
            Row.Children.Add(button);
            button.Name = "Edit";
            button.Content = "Edit";
            button.Width = 100;
            button.Height = 30;
            Grid.SetColumn(button, 3);
            Grid.SetRow(button, 0);
            button.Click += new RoutedEventHandler(Edit_Click);
            button.Margin = new Thickness(10, 0, 0, 0);

            Row.UpdateLayout();

            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Name = $"{DataBase.Columns[i].Header}" + "Row";
                DeleteRowDefinitions.Add(i, row);
                Row.RowDefinitions.Add(row);

                TextBox textBox = new TextBox();
                if (i == 0) { textBox.IsEnabled = false; }
                Row.Children.Add(textBox);
                textBox.Name = $"{DataBase.Columns[i].Header}" + "TextBox";
                textBox.Width = 450;
                textBox.Height = 30;
                textBox.VerticalContentAlignment = VerticalAlignment.Center;
                textBox.FontSize = (double)14;
                Grid.SetColumn(textBox, 2);
                Grid.SetRow(textBox, i);
                createTextBox.Add(i, textBox);

                Label label = new Label();
                Row.Children.Add(label);
                label.Height = 30;
                label.Content = (string)DataBase.Columns[i].Header;
                Grid.SetColumn(label, 1);
                Grid.SetRow(label, i);
                label.Foreground = Brushes.White;
                DeleteLabel.Add(i, label);
            }
        }
        private void AddRows()
        {
            Row.Visibility = Visibility.Visible;
            createTextBox = new Dictionary<int, TextBox>();
            DeleteRowDefinitions = new Dictionary<int, RowDefinition>();
            DeleteLabel = new Dictionary<int, Label>();

            Button button = new Button();
                Row.Children.Add(button);
                button.Name = "Save";
                button.Content = "Save";
                button.Width = 100;
                button.Height = 30;
                Grid.SetColumn(button, 3);
                Grid.SetRow(button, 0);
                button.Click += new RoutedEventHandler(Save_Click);
                button.Margin = new Thickness(10, 0, 0, 0);

            Row.UpdateLayout();

            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                    row.Name = $"{DataBase.Columns[i].Header}" + "Row";
                    DeleteRowDefinitions.Add(i, row);
                    Row.RowDefinitions.Add(row);

                TextBox textBox = new TextBox();
                    if (i == 0){textBox.IsEnabled = false;}
                    Row.Children.Add(textBox);
                    textBox.Name = $"{DataBase.Columns[i].Header}" + "TextBox";
                    textBox.Width = 450;
                    textBox.Height = 30;
                    textBox.VerticalContentAlignment = VerticalAlignment.Center;
                    textBox.FontSize = (double)14;
                    Grid.SetColumn(textBox, 2);
                    Grid.SetRow(textBox, i);
                    createTextBox.Add(i, textBox);

                Label label = new Label();
                    Row.Children.Add(label);
                    label.Height = 30;
                    label.Content = (string)DataBase.Columns[i].Header;
                    Grid.SetColumn(label, 1);
                    Grid.SetRow(label, i);
                    label.Foreground = Brushes.White;
                    DeleteLabel.Add(i, label);
            }
        }
        private void LoadData()
        {
            connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
            query = $"Select * From {SelectedTable}";

            using (connection = new SqlConnection(connectionString))
            {
                dataAdapter = new SqlDataAdapter(query, connection);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet, SelectedTable);
                var dataTable = dataSet.Tables[0];

                DataBase.Columns.Clear();
                DataBase.AutoGenerateColumns = false;
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    DataBase.Columns.Add(new DataGridTextColumn()
                    {
                        Header = dataTable.Columns[i].ColumnName,
                        Binding = new Binding { Path = new PropertyPath("[" + i.ToString() + "]") }
                    });
                }
                Maincollection = new ObservableCollection<object>();
                foreach (DataRow row in dataTable.Rows)
                {
                    Maincollection.Add(row.ItemArray);
                }
                DataBase.ItemsSource = Maincollection;
            }
        }
    }
}
