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
        
        /// <summary>
        /// Initialize window
        /// </summary>
        public DB()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Table selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            SelectedTable = (string?)SelectTable.SelectedItem;
            LoadData();
        }
        /// <summary>
        /// Button "Add line"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectTable.SelectedItems.Count > 0)
            {
                AddRowInTable(true);
                AddRows();
            }
            else
            {
                MessageBox.Show("Select table");
            }
        }
        /// <summary>
        /// Button "Edit line"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectTable.SelectedItems.Count > 0)
            {
                if (DataBase.SelectedItems.Count > 0)
                {

                    AddRowInTable(true);
                    EditRows();
                    if (DataBase.SelectedItem is not null)
                    {
                        for (int i = 0; i < createTextBox!.Count; i++)
                        {
                            createTextBox[i].Text = ((object[])DataBase.SelectedItem)[i].ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select row");
                }
            }
            else
            {
                MessageBox.Show("Select table");
            }
        }
        /// <summary>
        /// Button "Delete line"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataBase.SelectedItem is not null)
            {
                ConnectLine();
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
                MessageBox.Show("Select row");
            }
        }
        /// <summary>
        /// Event "Window load"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Row.Visibility = Visibility.Hidden;

            ConnectLine();
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
                            SelectTable.Items.Add(reader[0].ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Tables not find");
                    }
                }
            }
            SelectTable.Items.Remove("sysdiagrams");
        }
        /// <summary>
        /// Button "Back"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        /// <summary>
        /// Button "Cancel" in the add line window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Row.Visibility = Visibility.Hidden;
            AddRowInTable(false);
            
            for (int i = 0; i < DataBase.Columns.Count; i++)
            {
                Row.RowDefinitions.Remove(DeleteRowDefinitions![i]);
                Row.Children.Remove(DeleteLabel![i]);
                Row.Children.Remove(createTextBox![i]);
            }
        }
        /// <summary>
        /// Button "Clear table"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearTable_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No){}
            else
            {
                if (SelectTable.SelectedItems.Count > 0)
                {
                    ConnectLine();
                    query = $"DELETE FROM {SelectedTable}";
                    using (connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                    }
                    query = $"DBCC CHECKIDENT ('{SelectedTable}', RESEED, 0)";
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
                    MessageBox.Show("Select table");
                }
            }
        }
        /// <summary>
        /// Button "Save" in the add line window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                ConnectLine();
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
                Row.RowDefinitions.Remove(DeleteRowDefinitions![i]);
                Row.Children.Remove(DeleteLabel![i]);
                Row.Children.Remove(createTextBox![i]);
            }
        }
        /// <summary>
        /// Button "Edit" in the add line window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            string strQuery = "";
            for (int i = 1; i < createTextBox!.Count; i++)
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
                ConnectLine();
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
                Row.RowDefinitions.Remove(DeleteRowDefinitions![i]);
                Row.Children.Remove(DeleteLabel![i]);
                Row.Children.Remove(createTextBox![i]);
            }
        }
        /// <summary>
        /// Method for disabling the other interface when the add lines window appears 
        /// </summary>
        /// <param name="enable"></param>
        private void AddRowInTable(bool enable)
        {
            if (enable)
            {
                SelectTable.IsEnabled = false;
                AddLineButton.IsEnabled = false;
                EditLineButton.IsEnabled = false;
                DeleteLineButton.IsEnabled = false;
                Back.IsEnabled = false;
            }
            else if (enable == false)
            {
                SelectTable.IsEnabled = true;
                AddLineButton.IsEnabled = true;
                EditLineButton.IsEnabled = true;
                DeleteLineButton.IsEnabled = true;
                Back.IsEnabled = true;
            }
        }
        /// <summary>
        /// Method for creating a window for editing a line
        /// </summary>
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
        /// <summary>
        /// Method for creating a window for adding a line
        /// </summary>
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
        /// <summary>
        /// Method for loading the table
        /// </summary>
        private void LoadData()
        {
            ConnectLine();
            query = $"Select * From {SelectedTable}";

            using (connection = new SqlConnection(connectionString))
            {
                dataAdapter = new SqlDataAdapter(query, connection);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet, SelectedTable!);
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
        /// <summary>
        /// Method with connection string 
        /// </summary>
        private void ConnectLine()
        {
            connectionString = $"Server={NameServer};DataBase={NameDataBase};Trusted_Connection=True;Encrypt=False;";
        }
    }
}
