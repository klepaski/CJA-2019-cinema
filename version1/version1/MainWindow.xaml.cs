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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "" && Pass.Password == "")
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();
                    if (LogInUser(Name.Text, Pass.Password))
                    {
                        StaticClass.activeLogin = Name.Text;
                        StaticClass.activePassword = Pass.Password;
                        createWindow();
                        if (StaticClass.activeRole != "admin")
                        {
                            UserWindow userWindow = new UserWindow();
                            this.Close();
                            userWindow.Show();
                        }
                        else
                        {
                            AdminWindow adminWindow = new AdminWindow();
                            this.Close(); adminWindow.Show();
                        }
                    }
                    else
                    { MessageBox.Show("Неверный логин или пароль"); }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void createWindow()
        {
            try
            {
                string sqlExpression = "GETUSER";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    command.Parameters.Add(new SqlParameter("@login", StaticClass.activeLogin));
                    

                    SqlParameter id_user = new SqlParameter
                    {
                        ParameterName = "@id_user",
                        SqlDbType = SqlDbType.Int
                    };
                    id_user.Direction = ParameterDirection.Output;///выходной парам
                    command.Parameters.Add(id_user);

                    
                    SqlParameter nameUser = new SqlParameter
                    {
                        ParameterName = "@nameUser",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 50
                    };
                    nameUser.Direction = ParameterDirection.Output;
                    command.Parameters.Add(nameUser);

                    SqlParameter surnameUser = new SqlParameter
                    {
                        ParameterName = "@surnameUser",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 50
                    };
                    surnameUser.Direction = ParameterDirection.Output;
                    command.Parameters.Add(surnameUser);
                    SqlParameter emailUser = new SqlParameter
                    {
                        ParameterName = "@emailUser",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 100
                    };
                    emailUser.Direction = ParameterDirection.Output;
                    command.Parameters.Add(emailUser);

                    SqlParameter role = new SqlParameter
                    {
                        ParameterName = "@role",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 50
                    };
                    role.Direction = ParameterDirection.Output;
                    command.Parameters.Add(role);
                    command.ExecuteNonQuery();

                    StaticClass.activeName = command.Parameters["@nameUser"].Value.ToString();
                    StaticClass.activeSurname = command.Parameters["@surnameUser"].Value.ToString();
                    StaticClass.activeId = Convert.ToInt32(command.Parameters["@id_user"].Value.ToString());
                    StaticClass.activeEmail = command.Parameters["@emailUser"].Value.ToString();
                    StaticClass.activeRole = command.Parameters["@role"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            Close();
            registration.Show();
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            this.Close();
            StaticClass.activeLogin = null;
            userWindow.Show();
        }

        private static bool LogInUser(string name, string password)
        {
            string sqlExpression = "ENTRANCE";  ///есть 1, нету 0

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;



                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@login",
                    Value = name
                };
                command.Parameters.Add(nameParam);


                SqlParameter passParam = new SqlParameter
                {
                    ParameterName = "@password",
                    Value = password.GetHashCode().ToString()
                };
                command.Parameters.Add(passParam);

                var result = command.ExecuteScalar();


                var returnParam = new SqlParameter
                {
                    ParameterName = "@return",
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnParam);

                command.ExecuteNonQuery();

                var isLocked = (int)returnParam.Value;

                connection.Close();
                if (isLocked != 0) { return true; }
                else { return false; }

            }
        }


        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10); //копируем 0х10 символов из src начиная с 1 байта в dst c 0 байте
            byte[] buffer2 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer2, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer2, buffer);
        }

        private static bool ByteArraysEqual(byte[] firstHash, byte[] secondHash)
        {
            int _minHashLength = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < _minHashLength; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }
    }
}
