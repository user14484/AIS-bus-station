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
    public partial class delticket : Form
    {
        private database db = new database();
        private Infomation Info = new Infomation();

        private Dictionary<int, string> ticket = new Dictionary<int, string>();
        public delticket()
        {
            db.Open();
            InitializeComponent();
            LoadTickets();
        }

        private void LoadTickets()
        {
            ticket.Clear();
            Dictionary<int, Dictionary<string, string>> routes = db.QuaryMas("SELECT * FROM routes");

            foreach (Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM tickets").Values)
            {
                int id = Convert.ToInt32(dict["id_route"]);
                ticket.Add(Convert.ToInt32(dict["id"]), $"{routes[id]["number"]} | {routes[id]["departure_point"]} - {routes[id]["destination"]} | {dict["time_start"]} - {dict["time_end"]} | Цена: {dict["price"]}");
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(ticket, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void delticket_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(comboBox1.SelectedValue);

            try
            {
                db.Quary($"DELETE FROM tickets WHERE id={id}");
                Info.Info("Билет успешно удалён!");
            }
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
