using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BooksSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace BooksSQL
{
    public partial class MainWindow : Window
    {
        private DataSet dataSet = new DataSet();
        private SqlDataAdapter authorsAdapter;
        private SqlDataAdapter booksAdapter;

        public MainWindow()
        {
            InitializeComponent();
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                authorsAdapter = new SqlDataAdapter("SELECT * FROM Authors", connection);
                authorsAdapter.Fill(dataSet, "Authors");
                authorsComboBox.ItemsSource = dataSet.Tables["Authors"].DefaultView;
                authorsComboBox.DisplayMemberPath = "FirstName";
                authorsComboBox.SelectedValuePath = "AuthorID";
            }
        }

        private void AuthorsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (authorsComboBox.SelectedValue != null)
            {
                LoadBooks((int)authorsComboBox.SelectedValue);
            }
        }

        private void LoadBooks(int authorId)
        {
            string connectionString = ConfigurationHelper.GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Books WHERE AuthorID = @AuthorID";
                booksAdapter = new SqlDataAdapter(query, connection);
                booksAdapter.SelectCommand.Parameters.AddWithValue("@AuthorID", authorId);
                if (dataSet.Tables["Books"] != null)
                {
                    dataSet.Tables["Books"].Clear();
                }
                booksAdapter.Fill(dataSet, "Books");
                booksDataGrid.ItemsSource = dataSet.Tables["Books"].DefaultView;
            }
        }
    }
}
