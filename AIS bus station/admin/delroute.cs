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
    public partial class delroute : Form
    {
        private database db = new database();
        private Infomation Info = new Infomation();

        private Dictionary<int, string> routes = new Dictionary<int, string>();
        public delroute()
        {
            db.Open();
            InitializeComponent();
            LoadRoutes();
        }

        private void LoadRoutes()
        {
            routes.Clear();

            foreach(Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM routes").Values)
            {
                routes.Add(Convert.ToInt32(dict["id"]), $"{dict["number"]} | {dict["departure_point"]} - {dict["destination"]}");
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(routes, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void delroute_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(comboBox1.SelectedValue);

            try {
                db.Quary($"DELETE FROM routes WHERE id={id}");
                Info.Info("Маршрут успешно удалён!");
            }
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }
}
    }
}
