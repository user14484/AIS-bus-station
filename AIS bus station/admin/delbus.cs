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
    public partial class delbus : Form
    {
        private database db = new database();
        // Объект вывода диалоговых окон
        private Infomation Info = new Infomation();
        private Dictionary<int, string> Bus = new Dictionary<int, string>();
        public delbus()
        {
            db.Open();
            InitializeComponent();

            LoadBus();
        }

        private void LoadBus()
        {
            Bus.Clear();
            foreach (Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM buses").Values)
            {
                Bus.Add(Convert.ToInt32(dict["id"]), dict["mark"]);
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(Bus, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void delbus_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(comboBox1.SelectedValue);
                db.Quary($"DELETE FROM buses WHERE id={id}");
                Info.Info("Автобус успешно удалён");
                LoadBus();
            } catch(Exception ex)
            {
                Info.Error(ex.Message);
            }
        }
    }
}
