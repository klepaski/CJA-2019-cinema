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
    /// Логика взаимодействия для UserOrders.xaml
    /// </summary>
    public partial class UserOrders : Window
    {
        public UserOrders()
        {
            InitializeComponent();
            //usersMenuItem.Header = "           " + StaticClass.activeLogin;
            Window_Load();
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void Window_Load()
        {
            SqlDataAdapter adapter;
            DataTable userOrderTable;
            string sql = "GETUSERORDER";
            userOrderTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@id_user", StaticClass.activeId));

                adapter = new SqlDataAdapter(command);
                connection.Open();

                adapter.Fill(userOrderTable);
                x.ItemsSource = userOrderTable.DefaultView;
                x.CanUserAddRows = false;
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

        private void deleteOrder_Click(object sender, RoutedEventArgs e)
        {
            string sqlExpression = "DELETEORDER";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                if (x.SelectedItems != null)
                {
                    object a = x.SelectedItem;
                    for (int i = 0; i < x.SelectedItems.Count; i++)
                    {
                        DataRowView datarowView = x.SelectedItems[i] as DataRowView;
                        if (datarowView != null)
                        {
                            DataRow dataRow = (DataRow)datarowView.Row;
                            
                            int id = Int32.Parse(dataRow[0].ToString());
                            dataRow.Delete();
                            command.Parameters.Add(new SqlParameter("@id_order", id));

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }      
        }
            /*
            SQL Server заблокировал доступ к процедура "sys.xp_cmdshell" компонента "xp_cmdshell", поскольку он отключен в результате 
            настройки конфигурации безопасности сервера.Использование "xp_cmdshell" может быть разрешено администратором при помощи 
            хранимой процедуры sp_configure.Дополнительные сведения о включении "xp_cmdshell" см.в электронной документации по "xp_cmdshell".
            */


        private void back_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            this.Close();
            userWindow.Show();
        }
    }
}
