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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Configuration;
using System.Data;

namespace version1
{
    /// <summary>
    /// Логика взаимодействия для EditFilms.xaml
    /// </summary>
    public partial class EditFilms : Window
    {
        public EditFilms()
        {
            InitializeComponent();
            Window_Loaded();
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        SqlDataAdapter adapter;
        DataTable filmsTable;
        public string sql = "SELECT *from  FilmInfo ";

        public void Window_Loaded()
        {
            filmsTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(filmsTable);
                x.ItemsSource = filmsTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
        DataSet ds = new DataSet();
       
               

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (x.SelectedItems != null)
            {
                for (int i = 0; i < x.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = x.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        DataRow dataRow = (DataRow)datarowView.Row;
                        int id = Int32.Parse(dataRow[0].ToString());
                        MessageBoxResult result = MessageBox.Show("\n\n Вы действительно хотите удалить данный фильм из фильмографии и расписания ?", "Удалить фильм", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string sqlExpression = "DELETEFILM";

                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();
                                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                                    command.CommandType = System.Data.CommandType.StoredProcedure;

                                  
                                    command.Parameters.Add(new SqlParameter("@id_film", id));
                                    
                                    command.ExecuteNonQuery();

                                    if (command.ExecuteNonQuery() != 0 )
                                    {
                                        dataRow.Delete();
                                        MessageBox.Show("Удаление прошло успешно.\t");
                                    }
                                    else  { MessageBox.Show("Что-то пошло не так. Страница будет перезагружена.");
                                        EditFilms editFilms = new EditFilms(); 
                                        this.Close();
                                        editFilms.Show();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
        }


        private void back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.Show();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            createFilm createFilm = new createFilm();
            Nullable<bool> dialogResult = createFilm.ShowDialog();
            Window_Loaded();

        }
    }

   
}
