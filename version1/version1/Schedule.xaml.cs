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
    public partial class Schedule : Window
    {
        public Schedule()
        {
            InitializeComponent();
            string sql = "POISKFILM";
            Window_Loaded(sql);              
        }

        SqlDataAdapter adapter;
        DataTable scheduleTable;

        public static string connectionString;
        
        private void Window_Loaded(string sql)
        {             
            scheduleTable = new DataTable();
            SqlConnection connection = null;
            if (StaticClass.activeRole == "user")
            {
               connectionString = @"Data Source=USER-ПК;Initial Catalog=Cinema;Integrated Security=True;User ID = CinemaUser;Password = 0000";
            }
            else if (StaticClass.activeRole == "admin")
            {
                connectionString = @"Data Source=USER-ПК;Initial Catalog=Cinema;Integrated Security=True; User ID = CinemaAdmin;Password = 0000";
            }
            else
            {
                connectionString = @"Data Source=USER-ПК;Integrated Security=True;Initial Catalog=Cinema;User ID = CinemaGuest;Password = 0000";
            }

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
            UserWindow userWindow = new UserWindow();
            this.Close();
            userWindow.Show();
        }


        private void poiskFilm_Click(object sender, RoutedEventArgs e)
        {
            string sql = "POISKFILM";
            scheduleTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                if (category.Text != "")
                    command.Parameters.Add(new SqlParameter("@category", category.Text));
                if (calendar.Text != "")
                    command.Parameters.Add(new SqlParameter("@datetime", DateTime.Parse( calendar.Text)));
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

        private void orderTicket_Click(object sender, RoutedEventArgs e)
        {
            if (StaticClass.activeLogin != null)
            {
                if (countOrder.Text != "" && countOrder.Text != "0")
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

                                MessageBoxResult result = MessageBox.Show("\n" + dataRow[3].ToString() + " \" " + dataRow[4].ToString() + " \"\t\n" + "Количество билетов: " + countOrder.Text + "\n\n  Совершить заказ?", "Ваш заказ", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        string sqlExpression = "NEWORDER";

                                        using (SqlConnection connection = new SqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            SqlCommand command = new SqlCommand(sqlExpression, connection);
                                            command.CommandType = System.Data.CommandType.StoredProcedure;

                                            command.Parameters.Add(new SqlParameter("@id_user", StaticClass.activeId));
                                            command.Parameters.Add(new SqlParameter("@id_schedule", id));
                                            command.Parameters.Add(new SqlParameter("@countOrder", countOrder.Text));

                                            if (command.ExecuteNonQuery() != -1)
                                            {
                                                MessageBox.Show("Ваш заказ принят!");
                                                Window_Loaded("POISKFILM");
                                            }
                                            else
                                            {
                                                MessageBox.Show("Билетов недостаточно или заказ уже совершен!");
                                            }
                                        }
                                    }
                                    catch (SqlException ex)
                                    {
                                        MessageBox.Show(ex.Message);
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
                else { MessageBox.Show("Выберите число билетов!"); }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("\nДля заказa совершите вход. \n  Перейти на страницу входа?", "Переход на вход", MessageBoxButton.YesNo);
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
