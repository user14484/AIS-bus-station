using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_bus_station.work
{
    public partial class refundsales : Form
    {
        private Infomation Info = new Infomation();
        private database db = new database();
        // Объект с шрифтами
        private Fonts fonts = new Fonts();

        private int id_sales = 0;

        Dictionary<int, Dictionary<string, string>> Allroutes = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, Dictionary<string, string>> AllBuses = new Dictionary<int, Dictionary<string, string>>();
        Dictionary<int, Dictionary<string, string>> AllTickets = new Dictionary<int, Dictionary<string, string>>();
        public refundsales()
        {
            db.Open();
            Allroutes = db.QuaryMas("SELECT * FROM routes");
            AllBuses = db.QuaryMas("SELECT * FROM buses");
            AllTickets = db.QuaryMas("SELECT * FROM tickets");
            InitializeComponent();
            button1.Enabled = false;
            textBox2.Font = fonts.UseGaret(10.0F);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text))
            {
                Info.Error("Введите индентификатор продажи!");
                return;
            }
            if(id_sales <= 0)
            {
                Info.Warning("Продажа по данному идентификатору не найдена!");
                return;
            }

            try
            {
                db.Quary($"DELETE FROM sales WHERE id={id_sales}");
                Info.Info("Продажа успешно удалена!");
                button1.Enabled = false;
                textBox2.Text = "";
                textBox1.Text = "";
                id_sales = 0;
            } 
            catch (Exception ex)
            {
                Info.Error(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string identifier = textBox1.Text;
            Dictionary<string, string> Sale = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(identifier))
            {
                Info.Error("Введите индентификатор продажи!");
                return;
            }

            if (Convert.ToInt32(db.QuaryStr($"SELECT COUNT(*) FROM sales WHERE identifier='{identifier}'")) > 0)
            {
                Sale = db.QuaryMas($"SELECT * FROM sales WHERE identifier='{identifier}'").OrderBy(kvp => kvp.Key).First().Value;
                button1.Enabled = true;
            }
            else
            {
                Info.Warning("Продажа по данному идентификатору не найдена!");
                textBox2.Text = "";
                button1.Enabled = false;
                id_sales = 0;
                return;
            }

            int id_ticket = Convert.ToInt32(Sale["id_ticket"]);
            int id_route = Convert.ToInt32(Allroutes[Convert.ToInt32(AllTickets[id_ticket]["id_route"])]["id"]);
            int id_bus = Convert.ToInt32(AllBuses[Convert.ToInt32(Allroutes[Convert.ToInt32(AllTickets[id_ticket]["id_route"])]["id_bus"])]["id"]);
            string name = Sale["name"];

            string route =
                $"Номер маршрута: {Allroutes[id_route]["number"]}{Environment.NewLine}" +
                $"Пункт отправления: {Allroutes[id_route]["departure_point"]}{Environment.NewLine}" +
                $"Пункт назначения: {Allroutes[id_route]["destination"]}";

            string bus = $"Марка автобуса: {AllBuses[id_bus]["mark"]}";

            string ticket =
                $"Время отправления: {AllTickets[id_ticket]["time_start"]}{Environment.NewLine}" +
                $"Время прибытия: {AllTickets[id_ticket]["time_end"]}{Environment.NewLine}" +
                $"Стоимость билета: {AllTickets[id_ticket]["price"]}";

            string result =
                $"Уникальный индентификатор: {identifier}{Environment.NewLine}{Environment.NewLine}{route}{Environment.NewLine}{bus}{Environment.NewLine}{ticket}{Environment.NewLine}ФИО покупателя: {name}";

            textBox2.Text = result;
            id_sales = Convert.ToInt32(Sale["id"]);
        }

        private void refundsales_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }
    }
}
