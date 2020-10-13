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
    /// Логика взаимодействия для Schedule.xaml
    /// </summary>
    public partial class EditSchedule : Window
    {
        public EditSchedule()
        {
            InitializeComponent();
            string sql = "POISKFILM";
            Window_Loaded(sql);
        }

        SqlDataAdapter adapter;
        DataTable scheduleTable;

        public static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void Window_Loaded(string sql)
        {
            scheduleTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(scheduleTable);
                x.ItemsSource = scheduleTable.DefaultView;
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
        private void back_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow userWindow = new AdminWindow();
            this.Close();
            userWindow.Show();
        }
        

        string sql = "";


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
                        MessageBoxResult result = MessageBox.Show("\n\n Вы действительно хотите удалить данный фильм из расписания ?", "Удалить", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                string sqlExpression = "DELETESCHEDULE";

                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();
                                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                                    command.CommandType = System.Data.CommandType.StoredProcedure;


                                    command.Parameters.Add(new SqlParameter("@id_schedule", id));

                                    command.ExecuteNonQuery();

                                    if (command.ExecuteNonQuery() != 0)
                                    {
                                        dataRow.Delete();
                                        MessageBox.Show("Удаление прошло успешно.\t");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Что-то пошло не так. Страница будет перезагружена.");
                                        EditSchedule edit = new EditSchedule();
                                        this.Close();
                                        edit.Show();
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
               
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddSchedule addSchedule = new AddSchedule();
            this.Close();
            addSchedule.Show();
        }
    }
}

