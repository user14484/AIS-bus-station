using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace AIS_bus_station
{
    public partial class AuthorizationForm : Form
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


        // Инизиализация самой формы
        public AuthorizationForm()
        {
            Debug.enable = true;
            // Инициализируем все компоненты
            InitializeComponent();

            // Применение шрифтов
            label1.Font = fonts.UseGaret(20.0F);
            label2.Font = fonts.UseGaret(10.0F);
            label3.Font = fonts.UseGaret(10.0F);
            button1.Font = fonts.UseGaret(13.0F);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Записываем логин и пароль в переменные
            string login = textBox_login.Text, password = db.GetMD5Hash(textBox_password.Text);

            // Выводим переменные в консоль
            //Debug.WriteLine(string.Format("login = {0}\npassword = {1}", login, password));

            // Проверка если поле логина пустое
            if(string.IsNullOrEmpty(login))
            {
                Info.Warning("Поле логина не может быть пустым");
                return;
            }

            // Поиск пользователя и проверка пароля пользователя
            db.Open();
            if(Convert.ToInt32(db.QuaryStr($"SELECT COUNT(*) FROM users WHERE login='{login}' AND password='{password}'")) > 0)
            {
                DataUser = db.QuaryMas($"SELECT * FROM users WHERE login='{login}' AND password='{password}'").OrderBy(kvp => kvp.Key).First().Value;

                if(Convert.ToInt32(DataUser["access"]) != 1)
                {
                    IniFile INI = new IniFile("config.ini");
                    if (!(INI.ReadINI("dev", "func_login").Length > 0))
                    {
                        INI.Write("dev", "func_login", "1");
                    }
                    if(CheckedINI(INI.ReadINI("dev", "func_login")))
                    {
                        Info.Warning("Функция авторизации отключена!");
                        db.Close();
                        return;
                    }
                }

                OpenMainForm();
            }
            else
            {
                Info.Error("Неверный логин или пароль!");
            }
            db.Close();
        }

        private bool CheckedINI(string value)
        {
            int temp = Convert.ToInt32(value);

            switch (temp)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        // Функция перехода на основную форму
        private void OpenMainForm()
        {
            main MainForm = new main(DataUser);
            MainForm.FormClosed += ((s, e) => { this.Close(); });
            MainForm.Show();
            this.Hide();
        }
    }
}
