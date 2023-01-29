using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_bus_station.admin
{
    public partial class addtickets : Form
    {
        private database db = new database();
        private Infomation Info = new Infomation();

        private Dictionary<int, string> route = new Dictionary<int, string>();
        public addtickets()
        {
            db.Open();
            InitializeComponent();
            LoadRoutes();
        }

        private void LoadRoutes()
        {
            route.Clear();

            foreach (Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM routes").Values)
            {
                route.Add(Convert.ToInt32(dict["id"]), $"{dict["number"]} | {dict["departure_point"]} - {dict["destination"]}");
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(route, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void addtickets_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (typeof(TextBox) == ctrl.GetType())
                {
                    if (string.IsNullOrEmpty(ctrl.Text))
                    {
                        Info.Warning("Не все поля заполнены!");
                        return;
                    }
                }
            }

            if (!textBox1.Text.All(char.IsDigit))
            {
                Info.Warning("Поле с ценой билета должно быть числом!");
                return;
            }

            try
            {
                db.Quary($"INSERT INTO tickets (id_route, time_start, time_end, price) VALUES (" +
                    $"{comboBox1.SelectedValue}, " +
                    $"'{textBox2.Text}', " +
                    $"'{textBox3.Text}', " +
                    $"{textBox1.Text})");
                Info.Info("Билет успешно добавлен!");
            }
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }
        }
    }
}
