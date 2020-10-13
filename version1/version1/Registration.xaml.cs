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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region check text

        private bool isText(char[] s)
        {
            for (int i=0; i<s.Length; i++)
            {
                if ((s[i] >= 'a' && s[i] <= 'z') || (s[i] >= 'а' && s[i] <= 'я') ||
                    (s[i] >= 'A' && s[i] <= 'Z') || (s[i] >= 'А' && s[i] <= 'Я'))
                    continue;
                else return false;
            }
            return true;
        }

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && (e.Text == "@" || e.Text == "[" || e.Text == "]" 
                || e.Text == "/" || e.Text == "\\" || e.Text == "|" || e.Text == "" || e.Text == ";"))
                e.Handled = true;
        }

        private void number(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        bool IsGood(char c)///не цифра и не знак
        {
            if (c >= '0' && c <= '9')
                return false;
            if (c == '@' || c == ';' || c == '#' || c == '/' || c == '*' || c == '+')
                return false;
            else return true;
        }

        #endregion

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            bool a = false;
            try
            {
                if (Name.Text == "" || Surname.Text == "" || Login.Text == "")
                {
                    throw new Exception("Все поля должны быть заполнены");
                }
                if (Pass.Password != Pass2.Password)
                {
                    throw new Exception("Пароли не совпадают ");
                }

                string sqlExpression = "CREATEUSER";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    string email = Email.Text + mail.Text;
                    string role;
                    if (StaticClass.pageOrRegistartion == true)
                        role = "user";
                    else
                        role = "admin";
                    

                    command.Parameters.Add(new SqlParameter("@login", Login.Text));
                    command.Parameters.Add(new SqlParameter("@password", Pass.Password.GetHashCode().ToString()));

                    command.Parameters.Add(new SqlParameter("@nameUser", Name.Text));
                    command.Parameters.Add(new SqlParameter("@surnameUser", Surname.Text));
                   
                    command.Parameters.Add(new SqlParameter("@email", email));
                    command.Parameters.Add(new SqlParameter("@role", role));

                    SqlParameter returnValue = new SqlParameter
                    {
                        ParameterName = "@return",
                        SqlDbType = SqlDbType.Int
                    };
                    returnValue.Direction = ParameterDirection.Output;
                    command.Parameters.Add(returnValue);
                    command.ExecuteNonQuery();

                    int registrated = Int32.Parse( command.Parameters["@return"].Value.ToString());


                    if (registrated ==1)
                    {
                        MessageBox.Show("Congratulations! \nYou are registered. \nNow you can use all the features of our application. ");
                        a = true;
                        connection.Close();
                    }
                    else if (registrated == 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.\t");
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (a)
                {
                    if (StaticClass.pageOrRegistartion == true)
                    {
                        MainWindow mainWindow = new MainWindow();
                        this.Close();
                        mainWindow.Show();
                    }
                    else
                    {
                        StaticClass.pageOrRegistartion = true;
                        this.Close();
                    }
                }
            }
        }

        public static string HashPassword(string password)
        {
            ///идент.пароли не приводят к одному хешу
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            //генерация ключа password с размером соли 16 байт и 1000 итераций
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;              //генерация соли
                buffer2 = bytes.GetBytes(0x20); //возвращает значение ключа с числом генерируемых псевдослучайных байтов ключа
            }
            byte[] dst = new byte[0x31];                    //формируем массив для хешированного пароля
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);        //копируем соль в хешированный пароль по 15 байт
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);  // копируем ключ в хешированный пароль с 16 по 49 байт

            return Convert.ToBase64String(dst);
        }
    }
}
