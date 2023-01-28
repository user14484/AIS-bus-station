using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_bus_station
{
    public partial class main : Form
    {
        // Объект с шрифтами
        private Fonts fonts = new Fonts();
        // Объект работы с БД
        private database db = new database();
        // Объект отладки
        private debug Debug = new debug();
        // Объект вывода диалоговых окон
        private Infomation Info = new Infomation();
        // Информация о пользователе из БД
        private Dictionary<string, string> DataUser = new Dictionary<string, string>();

        // Загружаем все данные из БД
        private Dictionary<int, Dictionary<string, string>> AllBuses = new Dictionary<int, Dictionary<string, string>>();
        private Dictionary<int, Dictionary<string, string>> AllRoutes = new Dictionary<int, Dictionary<string, string>>();
        private Dictionary<int, Dictionary<string, string>> AllSales = new Dictionary<int, Dictionary<string, string>>();
        private Dictionary<int, Dictionary<string, string>> AllTickets = new Dictionary<int, Dictionary<string, string>>();

        public main(Dictionary<string, string> data)
        {
            db.Open();
            DataUser = data;
            InitializeComponent();

            // Показываем юзеру под каким пользователем он залогинен
            label1.Text = $"Пользователь: {DataUser["name"]}";

            // Применение шрифтов
            button1.Font = fonts.UseGaret(13.0F);
            button2.Font = fonts.UseGaret(13.0F);
            button3.Font = fonts.UseGaret(13.0F);
            button4.Font = fonts.UseGaret(13.0F);
            button5.Font = fonts.UseGaret(8.0F);
            button6.Font = fonts.UseGaret(8.0F);
            button7.Font = fonts.UseGaret(8.0F);
            button8.Font = fonts.UseGaret(8.0F);
            button9.Font = fonts.UseGaret(8.0F);
            button10.Font = fonts.UseGaret(8.0F);
            button11.Font = fonts.UseGaret(8.0F);
            button12.Font = fonts.UseGaret(8.0F);
            label1.Font = fonts.UseGaret(13.0F);

            LoadBuses("SELECT * FROM buses");
            LoadRoutes("SELECT * FROM routes");
        }

        // Функция загрузки автобусов из бд
        private void LoadBuses(string query)
        {
            TableLayoutPanel table = tableLayoutPanel4;
            AllBuses = db.QuaryMas(query);
            table.RowCount = 0;
            table.Controls.Clear();
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.Controls.Add(new Label()
            {
                Name = "BusLabel1",
                Text = "Марка автобуса",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 0, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "BusLabel2",
                Text = "Количество мест",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 1, table.RowCount - 1);
            foreach (Dictionary<string, string> bus in AllBuses.Values)
            {
                table.RowCount = table.RowCount + 1;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                table.Controls.Add(new TextBox()
                {
                    Name = "BusMark_" + bus["id"],
                    Text = bus["mark"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 0, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "BusSeats_" + bus["id"],
                    Text = bus["seats"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 1, table.RowCount - 1);
                table.Controls.Add(new Button()
                {
                    Name = "BusEdit_" + bus["id"],
                    Text = "Изменить",
                    Font = fonts.UseGaret(8.0F),
                    Dock = DockStyle.Fill
                }, 2, table.RowCount - 1);
            }
            foreach (Control button in table.Controls)
            {
                if (button.GetType() == typeof(Button))
                {
                    button.Click += new System.EventHandler(this.BusButton_Click);
                }
            }
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        /* Применение изменений автобусов к самой БД */
        private void BusButton_Click(object sender, EventArgs e)
        {
            TableLayoutPanel table = tableLayoutPanel4;
            Button button = (Button)sender;
            int id = Convert.ToInt32(button.Name.ToString().Split('_')[1]);
            string mark = ((TextBox)table.Controls["BusMark_" + id.ToString()]).Text;
            string seats = ((TextBox)table.Controls["BusSeats_" + id.ToString()]).Text;
            //Info.Info($"mark = {mark}\nseats = {seats}\nid = {id}");

            db.Quary($"UPDATE buses SET mark='{mark}', seats={seats} WHERE id={id}");

            Info.Info("Автобус успешно отредактирован!");

        }

        // Функция загрузки автобусов из бд
        private async void LoadRoutes(string query)
        {
            Dictionary<int, string> DictionaryBuses = new Dictionary<int, string>();
            Dictionary<int, string> DictionaryStatus = new Dictionary<int, string>(){
                { 0, "Не завершён" },
                { 1, "Завершён" }
            };

            foreach (Dictionary<string, string> bus in AllBuses.Values)
            {
                DictionaryBuses.Add(Convert.ToInt32(bus["id"]), bus["mark"]);
            }

            TableLayoutPanel table = tableLayoutPanel8;
            AllRoutes = db.QuaryMas(query);
            table.RowCount = 0;
            table.Controls.Clear();
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.Controls.Add(new Label()
            {
                Name = "RouteLabel1",
                Text = "Автобус",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 0, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "RouteLabel2",
                Text = "Пункт отправления",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 1, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "RouteLabel3",
                Text = "Пункт назначения",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 2, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "RouteLabel4",
                Text = "Номер маршрута",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 3, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "RouteLabel5",
                Text = "Статус",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 4, table.RowCount - 1);
            foreach (Dictionary<string, string> route in AllRoutes.Values)
            {
                table.RowCount = table.RowCount + 1;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                table.Controls.Add(new ComboBox()
                {
                    Name = "RouteBus_" + route["id"],
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Value",
                    ValueMember = "Key",
                    DataSource = new BindingSource(DictionaryBuses, null),
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 0, table.RowCount - 1);

                for (int i = 0; i < ((ComboBox)table.Controls[$"RouteBus_{route["id"]}"]).Items.Count; i++)
                {
                    if (((ComboBox)table.Controls[$"RouteBus_{route["id"]}"]).Items[i].ToString() == "МАЗ-241")
                    {
                        ((ComboBox)table.Controls[$"RouteBus_{route["id"]}"]).SelectedIndex = i;
                    }
                }

                table.Controls.Add(new TextBox()
                {
                    Name = "RouteDeparturePoint_" + route["id"],
                    Text = route["departure_point"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 1, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "RouteDestination_" + route["id"],
                    Text = route["destination"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 2, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "RouteNumber_" + route["id"],
                    Text = route["number"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 3, table.RowCount - 1);
                table.Controls.Add(new ComboBox()
                {
                    Name = "RouteStatus_" + route["id"],
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Value",
                    ValueMember = "Key",
                    DataSource = new BindingSource(DictionaryStatus, null),
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 4, table.RowCount - 1);
                table.Controls.Add(new Button()
                {
                    Name = "BusEdit_" + route["id"],
                    Text = "Изменить",
                    Font = fonts.UseGaret(8.0F),
                    Dock = DockStyle.Fill
                }, 5, table.RowCount - 1);
            }
            foreach (Control button in table.Controls)
            {
                if (button.GetType() == typeof(Button))
                {
                    button.Click += new System.EventHandler(this.RouteButton_Click);
                }
            }
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            await Task.Run(() => EditTextBox(table));
        }

        private async void EditTextBox(TableLayoutPanel table)
        {
            if (table.InvokeRequired)
            {
                await Task.Delay(100);
                table.Invoke(new MethodInvoker(delegate
                {
                    foreach (Dictionary<string, string> route in AllRoutes.Values)
                    {
                        ((ComboBox)table.Controls["RouteBus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["id_bus"]);
                        ((ComboBox)table.Controls["RouteStatus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["status"]);
                    }
                }));
            }
        }

        /* Применение изменений маршрутов к самой БД */
        private void RouteButton_Click(object sender, EventArgs e)
        {
            TableLayoutPanel table = tableLayoutPanel8;
            Button button = (Button)sender;
            int id = Convert.ToInt32(button.Name.ToString().Split('_')[1]);
            int id_bus = Convert.ToInt32(((ComboBox)table.Controls[$"RouteBus_{id}"]).SelectedValue);
            string departure_point = ((TextBox)table.Controls[$"RouteDeparturePoint_{id}"]).Text;
            string destination = ((TextBox)table.Controls[$"RouteDestination_{id}"]).Text;
            string number = ((TextBox)table.Controls[$"RouteNumber_{id}"]).Text;
            int status = Convert.ToInt32(((ComboBox)table.Controls[$"RouteStatus_{id}"]).SelectedValue);

            db.Quary($"UPDATE routes SET " +
                $"id_bus={id_bus}, " +
                $"departure_point='{departure_point}', " +
                $"destination='{destination}',  " +
                $"number='{number}', " +
                $"status={status} " +
                $"WHERE id={id}");

            Info.Info("Маршрут успешно отредактирован!");
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string search = textBox1.Text;
            if(string.IsNullOrEmpty(search))
            {
                Info.Error("Поле поиска не может быть пустым!");
                return;
            }
            LoadBuses($"SELECT * FROM buses WHERE mark LIKE '%{search}%'");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            LoadBuses("SELECT * FROM buses");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string search = textBox3.Text;
            if (string.IsNullOrEmpty(search))
            {
                Info.Error("Поле поиска не может быть пустым!");
                return;
            }
            LoadRoutes($"SELECT * FROM routes WHERE departure_point LIKE '%{search}%' OR destination LIKE '%{search}%' OR number LIKE '%{search}%'");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            LoadRoutes("SELECT * FROM routes");
        }
    }
}
