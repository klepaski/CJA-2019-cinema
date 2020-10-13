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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            Window_Loaded();
        }

        private void Window_Loaded()
        {
            if (StaticClass.activeLogin != null)
            {
                UserNow.Text = "\t\t Добро пожаловать, " + StaticClass.activeName + " " + StaticClass.activeSurname + "!";
                usersMenuItem.Header = "             " + StaticClass.activeLogin;
            }
            else
            {
                UserNow.Text = "\t\t Здравствуйте, Гость!";
                usersMenuItem.Header = "              Гость";
            }
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //Фильмография
        private void films_Click(object sender, RoutedEventArgs e)
        {
            getFilms();
        }

        private void getFilms()
        {
            listFilm.Text = "";
            // название процедуры
            string sqlExpression = "GETFILM";
            ///id_film, title, category, allowable_age, premiere, writer, directors, description

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

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


        //   -------  мои заказы    --------
        private void usersorders_Click(object sender, RoutedEventArgs e)
        {
            if (StaticClass.activeLogin == null)
            {
                MessageBoxResult result = MessageBox.Show("Для просмотра ваших заказов совершите вход. \nПерейти на страницу входа?", "Переход на вход", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MainWindow mainWindow = new MainWindow();
                    this.Close();
                    mainWindow.Show();
                }
            }
            else
            {
                UserOrders userOrders = new UserOrders();
                this.Close();
                userOrders.Show();
            }

        }

        //    -----   расписание    --------
        private void schedule_Click(object sender, RoutedEventArgs e)
        {
            Schedule schedule = new Schedule();
            this.Close();
            schedule.Show();
        }

        //      -------  MenuItem  ------------
        private void profile_Click(object sender, RoutedEventArgs e)
        {
            if (StaticClass.activeLogin != null)
            {
                UserPage userPage = new UserPage();
                Nullable<bool> dialogResult = userPage.ShowDialog();
                Window_Loaded();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Для перехода на свою страницу совершите вход. \nПерейти на страницу входа?", "Переход на вход", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MainWindow mainWindow = new MainWindow();
                    this.Close();
                    mainWindow.Show();
                }
            }
        }


        private void exitUser_Click(object sender, RoutedEventArgs e)
        {
            StaticClass.activeLogin = null;
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void deleteUser_Click(object sender, RoutedEventArgs e)
        {

            if (StaticClass.activeLogin != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить страницу?", "Выход", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string sqlExpression = "DELETEUSER";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@id_user", StaticClass.activeId));
                        command.ExecuteNonQuery();
                        if (command.ExecuteNonQuery() == 0)
                        {
                            MessageBox.Show("Что-то пошло не так:( ");
                        }
                        else
                        {

                            MainWindow mainWindow = new MainWindow();
                            this.Close();
                            mainWindow.Show();


                        }
                    }
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Для удаления профиля совершите вход. \nПерейти на страницу входа?", "Переход на вход", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MainWindow mainWindow = new MainWindow();
                    this.Close();
                    mainWindow.Show();
                }
            }
        }
    }
}

