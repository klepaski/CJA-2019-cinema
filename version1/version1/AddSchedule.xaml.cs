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
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace version1
{
    /// <summary>
    /// Логика взаимодействия для AddSchedule.xaml
    /// </summary>
    public partial class AddSchedule : Window
    {
         public AddSchedule()
         {
            InitializeComponent();
            string sql = "GETFILM";
            contains = 5;
            Window_Loaded(sql);
         }


        SqlDataAdapter adapter;
        DataTable scheduleTable;
        
        private void Window_Loaded(string sql)
        {
            scheduleTable = new DataTable();
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                if (contains == 0)
                    command.Parameters.Add(new SqlParameter("@text", word.Text));

                else if (contains == 1)
                    command.Parameters.Add(new SqlParameter("@text", wordForAndOr.Text));

                else if (contains == 2)
                {
                    string s = "";
                    s = s + "\"" + wordWithStar.Text + "\"";
                    command.Parameters.Add(new SqlParameter("@text", s));
                }

                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(scheduleTable);
                x.ItemsSource = scheduleTable.DefaultView;
                x.CanUserAddRows = false;
                contains = 0;
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

        
        private void profile_Click(object sender, RoutedEventArgs e)
        {

            Window adminWindow = new AdminWindow();
            this.Close();
            adminWindow.Show();
        }
        

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void showAll_Click(object sender, RoutedEventArgs e)
        {
            string sql = "GETFILM";
            contains = 5;
            Window_Loaded(sql);
        }

        private void freetext_Click(object sender, RoutedEventArgs e)
        {
            if (word.Text != "")
            {
                string sql = "freetextUser";
                Window_Loaded(sql);
            }
        }

        private void contains_Click(object sender, RoutedEventArgs e)
        {
            if (word.Text != "")
            {
                string sql = "containsUser";
                Window_Loaded(sql);

            }
        }

        public static int contains = 0;
        private void containsAndOR_Click(object sender, RoutedEventArgs e)
        {
            string sql = "containsUser";
            contains = 1;
            Window_Loaded(sql);
        }

        private void containWithStar_Click(object sender, RoutedEventArgs e)
        {
            string sql = "containsUser";
            contains = 2;
            Window_Loaded(sql);
        }


        private void ID_Click(object sender, RoutedEventArgs e)
        {
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
                        StaticClass.activeIdFilm = id;
                        StaticClass.activeTitleFilm = dataRow[2].ToString() + " \"" + dataRow[1].ToString() + "\"";
                        AddScheduleOk ok = new AddScheduleOk();
                        Nullable<bool> dialogResult = ok.ShowDialog();
                        string sql = "GETFILM";
                        contains = 5;
                        Window_Loaded(sql);
                    }
                }

            }
        }

        private void newStoplist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // string sql = " alter fulltext stoplist yourStoplist add N'" + newStopList.Text + "' language 1033; ";
                string sql = "newStopList";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@word", "'" + newStopList.Text + "'"));

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteStoplist_Click(object sender, RoutedEventArgs e)
        {
            if (deleteStopList.Text != "")
            {
                try
                {
                    string sql = "deleteStopList";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@word", "'" + deleteStopList.Text + "'"));

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Введите слово для удаления.");
        }

        private void WordForAndOr_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}