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

        // Настройки программы
        private Dictionary<string, string> Settings = new Dictionary<string, string>();

        public main(Dictionary<string, string> data)
        {
            // Загрузка настроек
            IniFile INI = new IniFile("config.ini");
            if (!INI.KeyExists("dev", "load_tables_dalay"))
            {
                INI.Write("dev", "load_tables_dalay", "400");
            }
            Settings.Add("load_tables_dalay", INI.ReadINI("dev", "load_tables_dalay"));

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
            button15.Font = fonts.UseGaret(8.0F);
            button16.Font = fonts.UseGaret(8.0F);
            button17.Font = fonts.UseGaret(8.0F);
            button18.Font = fonts.UseGaret(8.0F);
            button19.Font = fonts.UseGaret(8.0F);
            button20.Font = fonts.UseGaret(8.0F);
            label1.Font = fonts.UseGaret(13.0F);
            label10.Font = fonts.UseGaretBold(8.0F);
            label11.Font = fonts.UseGaretBold(8.0F);
            label12.Font = fonts.UseGaretBold(8.0F);
            label13.Font = fonts.UseGaretBold(8.0F);

            LoadBuses("SELECT * FROM buses");
            LoadRoutes("SELECT * FROM routes");
            LoadTickets("SELECT * FROM tickets");
            LoadSales("SELECT * FROM sales");
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

        // Функция загрузки маршрутов из бд
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
                    Name = "RouteEdit_" + route["id"],
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
            await EditTextBoxRoutes(table);
        }

        private async Task EditTextBoxRoutes(TableLayoutPanel table)
        {
            //if (table.InvokeRequired)
            //{
            //    await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            //    table.Invoke(new MethodInvoker(delegate
            //    {
            //        foreach (Dictionary<string, string> route in AllRoutes.Values)
            //        {
            //            ((ComboBox)table.Controls["RouteBus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["id_bus"]);
            //            ((ComboBox)table.Controls["RouteStatus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["status"]);
            //        }
            //    }));
            //}
            //await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            await Task.Yield();
            foreach (Dictionary<string, string> route in AllRoutes.Values)
            {
                ((ComboBox)table.Controls["RouteBus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["id_bus"]);
                ((ComboBox)table.Controls["RouteStatus_" + route["id"]]).SelectedValue = Convert.ToInt32(route["status"]);
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

        // Функция загрузки билетов из бд
        private async void LoadTickets(string query)
        {
            Dictionary<int, string> DictionaryRoutes = new Dictionary<int, string>();

            foreach (Dictionary<string, string> route in AllRoutes.Values)
            {
                DictionaryRoutes.Add(Convert.ToInt32(route["id"]), route["number"]);
            }

            TableLayoutPanel table = tableLayoutPanel10;
            AllTickets = db.QuaryMas(query);
            table.RowCount = 0;
            table.Controls.Clear();
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.Controls.Add(new Label()
            {
                Name = "TicketsLabel1",
                Text = "Маршрут",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 0, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "TicketsLabel2",
                Text = "Время отправления",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 1, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "TicketsLabel3",
                Text = "Время прибытия",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 2, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "TicketsLabel4",
                Text = "Цена",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 3, table.RowCount - 1);
            foreach (Dictionary<string, string> ticket in AllTickets.Values)
            {
                table.RowCount = table.RowCount + 1;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                table.Controls.Add(new ComboBox()
                {
                    Name = "TicketsRoute_" + ticket["id"],
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Value",
                    ValueMember = "Key",
                    DataSource = new BindingSource(DictionaryRoutes, null),
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 0, table.RowCount - 1);

                table.Controls.Add(new TextBox()
                {
                    Name = "TicketsTimeStart_" + ticket["id"],
                    Text = ticket["time_start"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 1, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "TicketsTimeEnd_" + ticket["id"],
                    Text = ticket["time_end"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 2, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "TicketsPrice_" + ticket["id"],
                    Text = ticket["price"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 3, table.RowCount - 1);
                table.Controls.Add(new Button()
                {
                    Name = "TicketsEdit_" + ticket["id"],
                    Text = "Изменить",
                    Font = fonts.UseGaret(8.0F),
                    Dock = DockStyle.Fill
                }, 4, table.RowCount - 1);
            }
            foreach (Control button in table.Controls)
            {
                if (button.GetType() == typeof(Button))
                {
                    button.Click += new System.EventHandler(this.TicketsButton_Click);
                }
            }
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            await EditTextBoxTickets(table);
        }

        private async Task EditTextBoxTickets(TableLayoutPanel table)
        {
            //if (table.InvokeRequired)
            //{
            //    await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            //    table.Invoke(new MethodInvoker(delegate
            //    {
            //        foreach (Dictionary<string, string> ticket in AllTickets.Values)
            //        {
            //            ((ComboBox)table.Controls["TicketsRoute_" + ticket["id"]]).SelectedValue = Convert.ToInt32(ticket["id_route"]);
            //        }
            //    }));
            //}
            //await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            await Task.Yield();
            foreach (Dictionary<string, string> ticket in AllTickets.Values)
            {
                ((ComboBox)table.Controls["TicketsRoute_" + ticket["id"]]).SelectedValue = Convert.ToInt32(ticket["id_route"]);
            }
        }

        /* Применение изменений билета к самой БД */
        private void TicketsButton_Click(object sender, EventArgs e)
        {
            TableLayoutPanel table = tableLayoutPanel10;
            Button button = (Button)sender;
            int id = Convert.ToInt32(button.Name.ToString().Split('_')[1]);
            int id_route = Convert.ToInt32(((ComboBox)table.Controls[$"TicketsRoute_{id}"]).SelectedValue);
            string time_start = ((TextBox)table.Controls[$"TicketsTimeStart_{id}"]).Text;
            string time_end = ((TextBox)table.Controls[$"TicketsTimeEnd_{id}"]).Text;
            string price = ((TextBox)table.Controls[$"TicketsPrice_{id}"]).Text;

            //Console.WriteLine(
            //    $"id_route = {id_route}\n" +
            //    $"time_start = {time_start}\n" +
            //    $"time_end = {time_end}\n" +
            //    $"price = {price}\n"
            //    );

            db.Quary($"UPDATE tickets SET " +
                $"id_route={id_route}, " +
                $"time_start='{time_start}', " +
                $"time_end='{time_end}',  " +
                $"price={price} " +
                $"WHERE id={id}");

            Info.Info("Билет успешно отредактирован!");
        }

        // Функция загрузки продаж из бд
        private async void LoadSales(string query)
        {
            Dictionary<int, string> DictionaryTickets = new Dictionary<int, string>();

            //foreach (Dictionary<string, string> ticket in AllTickets.Values)
            //{
            //    DictionarySales.Add(Convert.ToInt32(ticket["id"]), AllRoutes);
            //}

            Dictionary<int, Dictionary<string, string>> routes = db.QuaryMas("SELECT * FROM routes");

            foreach (Dictionary<string, string> dict in db.QuaryMas("SELECT * FROM tickets").Values)
            {
                int id = Convert.ToInt32(dict["id_route"]);
                DictionaryTickets.Add(Convert.ToInt32(dict["id"]), $"{routes[id]["number"]} | {routes[id]["departure_point"]} - {routes[id]["destination"]} | {dict["time_start"]} - {dict["time_end"]} | Цена: {dict["price"]}");
            }

            TableLayoutPanel table = tableLayoutPanel6;
            AllSales = db.QuaryMas(query);
            table.RowCount = 0;
            table.Controls.Clear();
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            table.Controls.Add(new Label()
            {
                Name = "SalesLabel1",
                Text = "Билет",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 0, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "SalesLabel2",
                Text = "ФИО покупателя",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 1, table.RowCount - 1);
            table.Controls.Add(new Label()
            {
                Name = "SalesLabel3",
                Text = "Идентификатор",
                Font = fonts.UseGaret(8.0F),
                Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            }, 2, table.RowCount - 1);
            foreach (Dictionary<string, string> sales in AllSales.Values)
            {
                table.RowCount = table.RowCount + 1;
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
                table.Controls.Add(new ComboBox()
                {
                    Name = "SalesTicket_" + sales["id"],
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Value",
                    ValueMember = "Key",
                    DataSource = new BindingSource(DictionaryTickets, null),
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 0, table.RowCount - 1);

                table.Controls.Add(new TextBox()
                {
                    Name = "SalesName_" + sales["id"],
                    Text = sales["name"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 1, table.RowCount - 1);
                table.Controls.Add(new TextBox()
                {
                    Name = "SalesIdentifier_" + sales["id"],
                    Text = sales["identifier"],
                    Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)))
                }, 2, table.RowCount - 1);
                table.Controls.Add(new Button()
                {
                    Name = "SalesEdit_" + sales["id"],
                    Text = "Изменить",
                    Font = fonts.UseGaret(8.0F),
                    Dock = DockStyle.Fill
                }, 4, table.RowCount - 1);
            }
            foreach (Control button in table.Controls)
            {
                if (button.GetType() == typeof(Button))
                {
                    button.Click += new System.EventHandler(this.SalesButton_Click);
                }
            }
            table.RowCount = table.RowCount + 1;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            await EditTextBoxSales(table);
        }

        private async Task EditTextBoxSales(TableLayoutPanel table)
        {
            //if (table.InvokeRequired)
            //{
            //    await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            //    table.Invoke(new MethodInvoker(delegate
            //    {
            //        foreach (Dictionary<string, string> ticket in AllTickets.Values)
            //        {
            //            ((ComboBox)table.Controls["TicketsRoute_" + ticket["id"]]).SelectedValue = Convert.ToInt32(ticket["id_route"]);
            //        }
            //    }));
            //}
            //await Task.Delay(Convert.ToInt32(Settings["load_tables_dalay"]));
            await Task.Yield();
            foreach (Dictionary<string, string> sales in AllSales.Values)
            {
                ((ComboBox)table.Controls["SalesTicket_" + sales["id"]]).SelectedValue = Convert.ToInt32(sales["id_ticket"]);
            }
        }

        /* Применение изменений билета к самой БД */
        private void SalesButton_Click(object sender, EventArgs e)
        {
            TableLayoutPanel table = tableLayoutPanel6;
            Button button = (Button)sender;
            int id = Convert.ToInt32(button.Name.ToString().Split('_')[1]);
            int id_ticket = Convert.ToInt32(((ComboBox)table.Controls[$"SalesTicket_{id}"]).SelectedValue);
            string name = ((TextBox)table.Controls[$"SalesName_{id}"]).Text;
            string identifier = ((TextBox)table.Controls[$"SalesIdentifier_{id}"]).Text;

            //Console.WriteLine(
            //    $"id_ticket = {id_ticket}\n" +
            //    $"name = {name}\n" +
            //    $"identifier = {identifier}\n"
            //    );

            db.Quary($"UPDATE sales SET " +
                $"id_ticket={id_ticket}, " +
                $"name='{name}', " +
                $"identifier='{identifier}' " +
                $"WHERE id={id}");

            Info.Info("Продажа успешно отредактирована!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            work.Users Users = new work.Users();
            Users.FormClosed += ((s, ev) => { this.Show(); });
            Users.Show();
            this.Hide();
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

        private void button12_Click(object sender, EventArgs e)
        {
            string search = textBox4.Text;
            if (string.IsNullOrEmpty(search))
            {
                Info.Error("Поле поиска не может быть пустым!");
                return;
            }
            LoadTickets($"SELECT * FROM tickets WHERE time_start LIKE '%{search}%' OR time_end LIKE '%{search}%' OR price LIKE '%{search}%'");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            LoadTickets("SELECT * FROM tickets");
        }

        // Функция открытия формы настроек
        private void OpenSettingsForm()
        {
            SettingsForm SettingsForm = new SettingsForm();
            //SettingsForm.FormClosed += ((s, e) => { this.Show(); });
            SettingsForm.Show();
            //this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenSettingsForm();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            admin.addbus addbus = new admin.addbus();
            //SettingsForm.FormClosed += ((s, e) => { this.Show(); });
            addbus.Show();
            //this.Hide();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            admin.delbus delbus = new admin.delbus();
            delbus.Show();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            admin.addroute addroute = new admin.addroute();
            addroute.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            admin.delroute delroute = new admin.delroute();
            delroute.Show();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            admin.addtickets addtickets = new admin.addtickets();
            addtickets.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            admin.delticket delticket = new admin.delticket();
            delticket.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            work.addsales addsales = new work.addsales();
            //addsales.FormClosed += ((s, ev) => { this.Show(); });
            addsales.Show();
            //this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            LoadSales("SELECT * FROM sales");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string search = textBox2.Text;
            if (string.IsNullOrEmpty(search))
            {
                Info.Error("Поле поиска не может быть пустым!");
                return;
            }
            LoadSales($"SELECT * FROM sales WHERE name LIKE '%{search}%' OR identifier LIKE '%{search}%'");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            work.refundsales refundsales = new work.refundsales();
            refundsales.Show();
        }

        //private void button12_Click(object sender, EventArgs e)
        //{
        //    string search = textBox4.Text;
        //    if (string.IsNullOrEmpty(search))
        //    {
        //        Info.Error("Поле поиска не может быть пустым!");
        //        return;
        //    }
        //    LoadTickets($"SELECT * FROM tickets WHERE time_start LIKE '%{search}%' OR time_end LIKE '%{search}%' OR price LIKE '%{search}%'");
        //}

        //private void button11_Click(object sender, EventArgs e)
        //{
        //    textBox4.Text = "";
        //    LoadTickets("SELECT * FROM tickets");
        //}
    }
}
