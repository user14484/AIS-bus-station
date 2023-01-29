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
        }

        private void LoadTickets()
        {
            ticket.Clear();

            foreach (Dictionary<string, string> dict in db.QuaryMas("SELECT ticket.*, routes.departure_point as departure_point, route_start FROM ticket").Values)
            {
                ticket.Add(Convert.ToInt32(dict["id"]), $"{dict["time_start"]} | {dict["time_end"]} - {dict["destination"]}");
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
    }
}
