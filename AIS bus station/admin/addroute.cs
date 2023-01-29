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
    public partial class addroute : Form
    {
        private database db = new database();
        private Infomation Info = new Infomation();

        private Dictionary<int, string> Bus = new Dictionary<int, string>();

        public addroute()
        {
            db.Open();
            InitializeComponent();
            LoadBus();
        }

        private void LoadBus()
        {
            Bus.Clear();

            foreach(Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM buses").Values)
            {
                Bus.Add(Convert.ToInt32(dict["id"]), dict["mark"]);
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(Bus, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void addroute_FormClosing(object sender, FormClosingEventArgs e)
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

            try
            {
                db.Quary($"INSERT INTO routes (id_bus, departure_point, destination, number) VALUES (" +
                    $"{comboBox1.SelectedValue}, " +
                    $"'{textBox1.Text}', " +
                    $"'{textBox2.Text}', " +
                    $"'{textBox3.Text}')");
                Info.Info("Маршрут успешно добавлен!");
            }
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }
        }
    }
}
