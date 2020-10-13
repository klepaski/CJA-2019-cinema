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
    /// Логика взаимодействия для createFilm.xaml
    /// </summary>
    public partial class createFilm : Window
    {
        public createFilm()
        {
            InitializeComponent();
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                if (Name.Text == "" || category.Text == "" || age.Text == "" || calendar.Text == "" || writer.Text == "" || directors.Text == "" || description.Text == "")
                {
                    throw new Exception("Все поля должны быть заполнены");
                }

                // название процедуры
                string sqlExpression = "CREATEFILM";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // добавляем параметр
                    command.Parameters.Add(new SqlParameter("@title", Name.Text));
                    command.Parameters.Add(new SqlParameter("@category", category.Text));

                    command.Parameters.Add(new SqlParameter("@allowable_age", Int32.Parse(age.Text)));
                    command.Parameters.Add(new SqlParameter("@premiere", DateTime.Parse(calendar.Text)));

                    command.Parameters.Add(new SqlParameter("@writer", writer.Text));
                    command.Parameters.Add(new SqlParameter("@directors", directors.Text));
                    command.Parameters.Add(new SqlParameter("@description", description.Text));

                    
                    if (command.ExecuteNonQuery() != 0)
                    {
                        MessageBox.Show("Новый фильм добавлен.");                        
                        connection.Close();
                    }
                    else
                    { MessageBox.Show(" Что-то не так."); }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        
    }
}
