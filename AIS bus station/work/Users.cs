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
    public partial class Users : Form
    {
        // Объект с шрифтами
        private Fonts fonts = new Fonts();
        // Объект работы с БД
        private database db = new database();
        // Объект отладки
        private debug Debug = new debug();
        // Объект вывода диалоговых окон
        private Infomation Info = new Infomation();

        private int UserId = 0;

        Dictionary<int, Dictionary<string, string>> ALLUsers = new Dictionary<int, Dictionary<string, string>>();
        public Users()
        {
            db.Open();
            InitializeComponent();
            LoadUsers("SELECT * FROM users");
        }

        private void LoadUsers(string query)
        {
            int i = 0;
            Dictionary<int, string> Users = new Dictionary<int, string>();
            Dictionary<int, string> Access = new Dictionary<int, string>() {
                { 0, "Кассир" },
                { 1, "Администратор" }
            };
            ALLUsers = db.QuaryMas(query);

            foreach (Dictionary<string, string> temp in db.QuaryMas(query).Values)
            {
                if(i == 0 && UserId == 0)
                {
                    i++;
                    UserId = Convert.ToInt32(temp["id"]);
                }
                Users.Add(Convert.ToInt32(temp["id"]), $"{temp["login"]} - {temp["name"]}");
            }

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = new BindingSource(Users, null);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Refresh();
            
            comboBox2.DisplayMember = "Value";
            comboBox2.ValueMember = "Key";
            comboBox2.DataSource = new BindingSource(Access, null);
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Refresh();

            comboBox3.DisplayMember = "Value";
            comboBox3.ValueMember = "Key";
            comboBox3.DataSource = new BindingSource(Access, null);
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.Refresh();

            LoadUser(UserId);
        }

        private void Users_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Convert.ToInt32(((ComboBox)sender).SelectedValue)
            //Info.Info(Convert.ToInt32(((ComboBox)sender).SelectedValue).ToString());
            LoadUser(Convert.ToInt32(((ComboBox)sender).SelectedValue));
        }

        private async void LoadUser(int id)
        {
            Dictionary<string, string> User = ALLUsers[id];

            textBox2.Text = User["login"];
            textBox3.Text = User["name"];
            UserId = Convert.ToInt32(User["id"]);
            await EditComboBox(User);
            //comboBox2.SelectedValue = Convert.ToInt32(User["access"]);
        }


        private async Task EditComboBox(Dictionary<string, string> User)
        {
            await Task.Yield();
            comboBox2.SelectedValue = Convert.ToInt32(User["access"]);
            comboBox1.SelectedValue = Convert.ToInt32(User["id"]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(Control ctrl in tabPage1.Controls)
            {
                if(ctrl.GetType() == typeof(TextBox) && ctrl.Name != "textBox4" && ctrl.Name != "textBox1" && string.IsNullOrEmpty(ctrl.Text))
                {
                    Info.Error("Заполните логин или ФИО!");
                    return;
                }
            }

            string login = textBox2.Text;
            string name = textBox3.Text;
            string password = textBox4.Text;
            int access = Convert.ToInt32(comboBox2.SelectedValue);
            string query;

            if (string.IsNullOrEmpty(password))
            {
                query = $"UPDATE users SET login='{login}', name='{name}', access={access} WHERE id={UserId}";
            } 
            else
            {
                query = $"UPDATE users SET login='{login}', name='{name}', password='{db.GetMD5Hash(password)}', access={access} WHERE id={UserId}";
            }

            try
            {
                db.Quary(query);
                Info.Info("Пользователь успешно изменён!");
                LoadUsers("SELECT * FROM users");
            } catch(Exception ex)
            {
                Info.Error(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string search = textBox1.Text;


            if(string.IsNullOrEmpty(search))
            {
                Info.Warning("Поле поиска не может быть пустым!");
                return;
            }

            LoadUsers($"SELECT * FROM users WHERE login LIKE '%{search}%' OR name LIKE '%{search}%'");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadUsers("SELECT * FROM users");
            textBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {

            foreach (Control ctrl in tabPage2.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox) && string.IsNullOrEmpty(ctrl.Text))
                {
                    Info.Error("Не все поля заполнены!");
                    return;
                }
            }

            string login = textBox7.Text;
            string name = textBox6.Text;
            string password = db.GetMD5Hash(textBox5.Text);
            int access = Convert.ToInt32(comboBox3.SelectedValue);

            try
            {
                db.Quary($"INSERT INTO users (login, name, password, access) VALUES ('{login}', '{name}', '{password}', {access})");
                Info.Info("Пользователь успешно добавлен!");
                LoadUsers("SELECT * FROM users");
            } 
            catch(Exception ex)
            {
                Info.Error(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Вы уверены?", "Подтверждение дейстия",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    db.Quary($"DELETE FROM users WHERE id={UserId}");
                    Info.Info("Пользователь успешно удалён!");
                    UserId = 0;
                    LoadUsers("SELECT * FROM users");
                }
                catch(Exception ex)
                {
                    Info.Error(ex.Message);
                }
            }
        }
    }
}
