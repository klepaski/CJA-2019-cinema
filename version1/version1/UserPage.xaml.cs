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
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Window
    {
        public UserPage()
        {
            InitializeComponent();
            Window_Load();
        }
        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region check text

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && (e.Text == "@" || e.Text == "[" || e.Text == "]" || e.Text == " " || e.Text == "/" || e.Text == "\\" || e.Text == "|" || e.Text == "" || e.Text == ";"))
                e.Handled = true;


        }
        private void number(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        bool IsGood(char c)
        {
            if (c >= '0' && c <= '9')
                return false;

            if (c == '@' || c == ';' || c == '#' || c == '/' || c == '*' || c == ' ' || c == '+') return false;
            else return true;
        }

        #endregion
        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sqlExpression = "UPDATEUSER";
               if(oldPassword.Password == "")
                {
                    MessageBox.Show("Для обновления введите свой старый пароль, \nчтобы мы знали, что это вы.");
                }
                else if (oldPassword.Password != StaticClass.activePassword)
                {
                    MessageBox.Show("Не верный пароль!");
                }               
                else
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@id_user", StaticClass.activeId));
                        command.Parameters.Add(new SqlParameter("@login", Login.Text));
                        command.Parameters.Add(new SqlParameter("@nameUser", Name.Text));
                        command.Parameters.Add(new SqlParameter("@surnameUser", Surname.Text));

                        if (Pass.Password == Pass2.Password && Pass.Password != "")
                        {
                            command.Parameters.Add(new SqlParameter("@password", Pass2.Password.GetHashCode().ToString()));
                            StaticClass.activePassword = Pass2.Password;
                        }
                        else if (Pass.Password == Pass2.Password && Pass.Password == "")
                        {
                            command.Parameters.Add(new SqlParameter("@password", StaticClass.activePassword));
                        }
                        else
                        {
                            throw new Exception("Пароли не совпадают");
                        }
                        command.Parameters.Add(new SqlParameter("@email", Email.Text));

                        if (command.ExecuteNonQuery() != 0)
                        {
                            MessageBox.Show("Страница обновлена!");                           
                            StaticClass.activeLogin = Login.Text;
                            StaticClass.activeName = Name.Text;
                            StaticClass.activeSurname = Surname.Text;
                            StaticClass.activeEmail = Email.Text;
                            Window_Load();
                        }
                        else { MessageBox.Show("Что-то пошло не так :("); }
                        connection.Close();
                    }
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       private void Window_Load()
        {
            Name.Text = StaticClass.activeName;
            Surname.Text = StaticClass.activeSurname;
            Login.Text = StaticClass.activeLogin;
            Email.Text = StaticClass.activeEmail;
            oldPassword.Password = "";
            Pass.Password = "";
            Pass2.Password = "";
        }
    }
}
