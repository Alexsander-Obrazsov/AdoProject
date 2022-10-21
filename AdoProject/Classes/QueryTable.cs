using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Controls;

namespace AdoProject.Classes
{
    public class QueryTable
    {
        public static DataGrid ListSelectedTable(string SelectedTable, string connectionLine)
        {
            string queryTable = $"Select * From {SelectedTable}";
            using (SqlConnection connection = new SqlConnection(connectionLine))
            {
                connection.Open();
                DataGrid dataGrid = new DataGrid();
                SqlCommand command = new SqlCommand(queryTable, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(queryTable, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;

                return dataGrid;
            }
        }
    }
}