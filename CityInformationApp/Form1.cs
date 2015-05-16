using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityInformationApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["CityInformationConnString"].ConnectionString;

        private bool isUpdateMode = false;
        private int cityId;

         

        private void saveButton_Click(object sender, EventArgs e)
        {
            City city = new City();
            city.cityName = cityNameTextBox.Text;
            city.about = aboutTextBox.Text;
            city.country = countryTextBox.Text;

            if (isUpdateMode)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                string insertQuery = "UPDATE cityInfoTable SET About='" + city.about + "',Country='" + city.country + "' WHERE ID='"+cityId+"'";
                SqlCommand command = new SqlCommand(insertQuery, connection);
                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();
                if (rowAffected > 0)
                {
                    MessageBox.Show(@"City Information Successfully Update...!");
                    isUpdateMode = false;
                    saveButton.Text = "Save";
                    cityId = 0;
                    ShowAllData();
                    cityNameTextBox.Clear();
                    aboutTextBox.Clear();
                    countryTextBox.Clear();
                }
                else
                {
                    MessageBox.Show(@"Update Failed...!");
                }
            }
            else
            {
                if (city.cityName != "" && city.country != "")
                {
                    if (city.cityName.Length >= 4)
                    {
                        if (IsCityExists(city.cityName))
                        {
                            MessageBox.Show(@"City Name already Exists.");
                            return;
                        }
                        SqlConnection connection = new SqlConnection(connectionString);

                        string insertQuery = "INSERT INTO cityInfoTable VALUES('" + city.cityName + "','" + city.about + "','" + city.country + "')";

                        SqlCommand command = new SqlCommand(insertQuery, connection);
                        connection.Open();
                        int rowAffected = command.ExecuteNonQuery();
                        connection.Close();
                        if (rowAffected > 0)
                        {
                            MessageBox.Show(@"City Information Successfully Inserted...!");
                            ShowAllData();
                        }
                        else
                        {
                            MessageBox.Show(@"Isnertion Failed...!");
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"City Name Length at Least four(4) Characters");
                    }
                }
                else
                {
                    MessageBox.Show(@"Fill Up Requirement");
                }
            }
            
        }

        public bool IsCityExists(string cityName)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string selectQuery = "SELECT * FROM cityInfoTable WHERE CityName='"+cityName+"'";

            SqlCommand command = new SqlCommand(selectQuery, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            bool isCityExists = false;
            while (reader.Read())
            {
                isCityExists = true;
            }
            reader.Close();
            connection.Close();
            return isCityExists;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowAllData();
        }
        public void ShowAllData()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string selectQuery = "SELECT * FROM cityInfoTable";

            SqlCommand command = new SqlCommand(selectQuery, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<City> cityList = new List<City>();

            while (reader.Read())
            {
                City city = new City();
                city.id = int.Parse(reader["ID"].ToString());
                city.cityName = reader["CityName"].ToString();
                city.about = reader["About"].ToString();
                city.country = reader["Country"].ToString();
                cityList.Add(city);
            }
            reader.Close();
            connection.Close();
            LoadShowListView(cityList);
        }

        public void LoadShowListView(List<City> citiyList)
        {
            
            int i = 1;
            showListView.Items.Clear();
            foreach (var city in citiyList)
            {
                ListViewItem item = new ListViewItem(city.id.ToString());
                item.SubItems.Add(i.ToString());
                item.SubItems.Add(city.cityName);
                item.SubItems.Add(city.about);
                item.SubItems.Add(city.country);
                showListView.Items.Add(item);
                i = i + 1;
            }

        }

        private void showListView_DoubleClick_1(object sender, EventArgs e)
        {
            ListViewItem item = showListView.SelectedItems[0];

            int id = int.Parse(item.Text.ToString());

            City city = GetCityById(id);
            if (city != null)
            {
                isUpdateMode = true;
                saveButton.Text = "Update";
                cityId = city.id;
                cityNameTextBox.ReadOnly = true;
                cityNameTextBox.Text = city.cityName;
                aboutTextBox.Text = city.about;
                countryTextBox.Text = city.country;
            }
        }

        public City GetCityById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string selectQuery = "SELECT * FROM cityInfoTable WHERE ID='"+id+"'";
            SqlCommand command = new SqlCommand(selectQuery, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            List<City> cityList = new List<City>();

            while (reader.Read())
            {
                City city = new City();
                city.id = int.Parse(reader["ID"].ToString());
                city.cityName = reader["CityName"].ToString();
                city.about = reader["About"].ToString();
                city.country = reader["Country"].ToString();
                cityList.Add(city);
            }
            reader.Close();
            connection.Close();
            return cityList.FirstOrDefault();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string search = searchTextBox.Text;
            SearchAllData(search);
        }

        public void SearchAllData(string byCity)
        {
            if (searchByCityRadioButton.Checked)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string selectQuery = "SELECT * FROM cityInfoTable WHERE CityName LIKE'%" + byCity + "%'";

                SqlCommand command = new SqlCommand(selectQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<City> cityList = new List<City>();

                while (reader.Read())
                {
                    City city = new City();
                    city.id = int.Parse(reader["ID"].ToString());
                    city.cityName = reader["CityName"].ToString();
                    city.about = reader["About"].ToString();
                    city.country = reader["Country"].ToString();
                    cityList.Add(city);
                }
                reader.Close();
                connection.Close();
                LoadShowListView(cityList);
            }
            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string selectQuery = "SELECT * FROM cityInfoTable WHERE Country LIKE'%" + byCity + "%'";

                SqlCommand command = new SqlCommand(selectQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<City> cityList = new List<City>();

                while (reader.Read())
                {
                    City city = new City();
                    city.id = int.Parse(reader["ID"].ToString());
                    city.cityName = reader["CityName"].ToString();
                    city.about = reader["About"].ToString();
                    city.country = reader["Country"].ToString();
                    cityList.Add(city);
                }
                reader.Close();
                connection.Close();
                LoadShowListView(cityList);
            }
        }
       
    }
}
