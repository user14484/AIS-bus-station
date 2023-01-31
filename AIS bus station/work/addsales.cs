using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace AIS_bus_station.work
{
    public partial class addsales : Form
    {
        private Infomation Info = new Infomation();
        private database db = new database();

        private Dictionary<int, string> ticket = new Dictionary<int, string>();
        Dictionary<int, Dictionary<string, string>> Allroutes = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, Dictionary<string, string>> AllBuses = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, Dictionary<string, string>> AllTickets = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, Dictionary<string, string>> AllSales = new Dictionary<int, Dictionary<string, string>>();

        private string result;

        public addsales()
        {
            db.Open();
            InitializeComponent();
            Allroutes = db.QuaryMas("SELECT * FROM routes");
            AllBuses = db.QuaryMas("SELECT * FROM buses");
            AllTickets = db.QuaryMas("SELECT * FROM tickets");
            AllSales = db.QuaryMas("SELECT * FROM sales");
            LoadTickets("SELECT * FROM tickets");
        }

        private void LoadTickets(string query)
        {
            ticket.Clear();

            foreach (Dictionary<string, string> dict in db.QuaryMas(query).Values)
            {
                int id = Convert.ToInt32(dict["id_route"]);
                int id_bus = Convert.ToInt32(Allroutes[Convert.ToInt32(AllTickets[Convert.ToInt32(dict["id"])]["id_route"])]["id_bus"]);
                //Console.WriteLine(
                //    $"Количество мест в автобусе: {Convert.ToInt32(AllBuses[id_bus]["seats"])}\n" +
                //    $"Количество проданных билетов: {Convert.ToInt32(db.QuaryMas($"SELECT * FROM sales WHERE id_ticket={dict["id"]}").Count)}\n\n"
                //    );
                if (Convert.ToInt32(Allroutes[id]["status"]) == 0)
                    if(Convert.ToInt32(AllBuses[id_bus]["seats"]) > Convert.ToInt32(db.QuaryMas($"SELECT * FROM sales WHERE id_ticket={dict["id"]}").Count))
                        ticket.Add(Convert.ToInt32(dict["id"]), $"{Allroutes[id]["number"]} | {Allroutes[id]["departure_point"]} - {Allroutes[id]["destination"]} | {dict["time_start"]} - {dict["time_end"]} | Цена: {dict["price"]}");
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(ticket, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox2.Text))
            {
                Info.Error("Поле с данными покупателя не может быть пустым!");
                return;
            }

            int id_ticket = Convert.ToInt32(comboBox1.SelectedValue);
            int id_route = Convert.ToInt32(Allroutes[Convert.ToInt32(AllTickets[Convert.ToInt32(comboBox1.SelectedValue)]["id_route"])]["id"]);
            int id_bus = Convert.ToInt32(AllBuses[Convert.ToInt32(Allroutes[Convert.ToInt32(AllTickets[Convert.ToInt32(comboBox1.SelectedValue)]["id_route"])]["id_bus"])]["id"]);
            string name = textBox2.Text;
            string identifier = DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();

            string route = 
                $"Номер маршрута: {Allroutes[id_route]["number"]}\n" +
                $"Пункт отправления: {Allroutes[id_route]["departure_point"]}\n" +
                $"Пункт назначения: {Allroutes[id_route]["destination"]}";

            string bus = $"Марка автобуса: {AllBuses[id_bus]["mark"]}";

            string ticket =
                $"Время отправления: {AllTickets[id_ticket]["time_start"]}\n" +
                $"Время прибытия: {AllTickets[id_ticket]["time_end"]}\n" +
                $"Стоимость билета: {AllTickets[id_ticket]["price"]}";

            result =
                $"Уникальный индентификатор: {identifier}\n\n{route}\n{bus}\n{ticket}\nФИО покупателя: {name}";

            try
            {
                if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                {
                    printDocument1.Print();
                }
                db.Quary($"INSERT INTO sales (id_ticket, name, identifier) VALUES ({id_ticket}, '{name}', '{identifier}')");
                Info.Info("Билет успешно продан!");
            }
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }

        }


        private void addsales_Load(object sender, EventArgs e)
        {

        }

        private void addsales_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(result, new Font("Centuey Gothic", 14, FontStyle.Regular), Brushes.Black, new PointF(0, 0));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string search = textBox1.Text;

            if(string.IsNullOrEmpty(search))
            {
                Info.Warning("Поле поиска не может быть пустым!");
                return;
            }

            string query =
                $"SELECT tickets.* " +
                $"FROM tickets INNER JOIN routes ON(tickets.id_route=routes.id) " +
                $"WHERE routes.departure_point LIKE '%{search}%' OR routes.destination LIKE '%{search}%' OR routes.number LIKE '%{search}%' " +
                $"OR tickets.time_start LIKE '%{search}%' OR tickets.time_end LIKE '%{search}%'";

            if(search.All(char.IsDigit))
            {
                query += $"  OR tickets.price={search}";
            }

            LoadTickets(query);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            LoadTickets("SELECT * FROM tickets");
        }
    }
}
