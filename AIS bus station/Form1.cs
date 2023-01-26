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
            Debug.WriteLine(string.Format("login = {0}\npassword = {1}", login, password));

            // Проверка если поле логина пустое
            if(string.IsNullOrEmpty(login))
            {
                Info.Warning("Поле логина не может быть пустым");
            }
        }
    }
}
