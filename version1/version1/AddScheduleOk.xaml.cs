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
    /// Логика взаимодействия для AddScheduleOk.xaml
    /// </summary>
    public partial class AddScheduleOk : Window
    {
        public AddScheduleOk()
        {
            InitializeComponent();
            Name.Text = StaticClass.activeTitleFilm;
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (calendar.Text == "" || hour.Text == "" || min.Text == "" ||CountTicket.Text == "")
                {
                    throw new Exception("Все поля должны быть заполнены");
                }
               
                if (CountTicket.Text=="0")
                {
                    throw new Exception("Количество билетов должно быть больше нуля!");
                }
                string sqlExpression = "CREATESCHEDULE";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                command.Parameters.Add(new SqlParameter("@id_film", StaticClass.activeIdFilm));
                command.Parameters.Add(new SqlParameter("@datetime", DateTime.Parse(calendar.Text)));

                command.Parameters.Add(new SqlParameter("@time", hour.Text + ":" + min.Text));
                command.Parameters.Add(new SqlParameter("@countTicket", Int32.Parse( CountTicket.Text)));

                    if (command.ExecuteNonQuery() != 0)
                    {
                        MessageBox.Show("Расписание обоновлено.");
                        this.Close();
                    };


            }
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
