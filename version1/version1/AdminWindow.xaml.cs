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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            usersMenuItem.Header ="          " +  StaticClass.activeLogin;
        }
        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void profile_Click(object sender, RoutedEventArgs e)
        {            
             if (StaticClass.activeLogin != null)
             {
                    UserPage userPage = new UserPage();
                    Nullable<bool> dialogResult = userPage.ShowDialog();                    
             }
             else
             {
                    MessageBoxResult result = MessageBox.Show("\nДля перехода на свою страницу совершите вход. \nПерейти на вход?", "Переход на вход", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        MainWindow mainWindow = new MainWindow();
                        this.Close();
                        mainWindow.Show();
                    }
             }            
        }

        private void films_Click(object sender, RoutedEventArgs e)
        {
            listFilm.Text = "";
            string sqlExpression = "GETFILM";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                UserNow.Text = " Наши фильмы: ";
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string title = reader.GetString(1);
                        string category = reader.GetString(2);
                        string age = reader.GetInt32(3).ToString();
                        string date = reader.GetDateTime(4).Date.ToString();
                        StringBuilder sb = new StringBuilder(date);
                        for (int i = 11; i < sb.Length; i++) { sb[i] = ' '; }
                        string newdate = sb.ToString();

                        listFilm.Text += category + "  \"  " + title + "\"    " + age + "+\n";
                        listFilm.Text += "  Премьера: " + newdate + "\n  Сценарист: " + reader.GetString(5) +
                            "\n  Режиссер: " + reader.GetString(6) + "\n  Описание: " + reader.GetString(7) + "\n\n\n";
                    }

                }
                reader.Close();
            }
        }

        private void editFilms_Click(object sender, RoutedEventArgs e)
        {
            EditFilms editFilms = new EditFilms();
            this.Close();
            editFilms.Show();
        }

        private void editSchedule_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule editSchedule = new EditSchedule();
            this.Close();
            editSchedule.Show();
        }

        private void exitUser_Click(object sender, RoutedEventArgs e)
        {
            StaticClass.activeLogin = null;
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void newAdmin_Click(object sender, RoutedEventArgs e)
        {
            StaticClass.pageOrRegistartion = false;
            Registration registration = new Registration();
            Nullable<bool> dialogResult = registration.ShowDialog();
        }

        private void xml_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("\nПроизвести импорт?", "\tИмпорт", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string sqlExpression = "IMPORTXML";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    { MessageBox.Show(ex.Message); }
                }
            }
            MessageBoxResult result1 = MessageBox.Show("\nПроизвести экспорт?", "\tЭкспорт", MessageBoxButton.YesNo);
            if (result1 == MessageBoxResult.Yes)
            {
                string sqlExpression = "EXPORTXML";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.ExecuteNonQuery();                        
                    }
                    catch(Exception ex)
                    { MessageBox.Show(ex.Message); }
                }
            }
        }
    }
}
