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
    public partial class addbus : Form
    {
        private database db = new database();
        // Объект вывода диалоговых окон
        private Infomation Info = new Infomation();
        public addbus()
        {
            db.Open();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            foreach(Control ctrl in this.Controls)
            {
                if(typeof(TextBox) == ctrl.GetType())
                {
                    if(string.IsNullOrEmpty(ctrl.Text))
                    {
                        Info.Warning("Не все поля заполнены!");
                        return;
                    }
                }
            }

            if (!textBox2.Text.All(char.IsDigit))
            {
                Info.Warning("Поле с количеством мест должно быть числом!");
                return;
            }

            try
            {
                db.Quary($"INSERT INTO buses (mark, seats) VALUES ('{textBox1.Text}', {textBox2.Text})");
                Info.Info("Автобус успешно добавлен!");
            }
            catch(Exception ex)
            {
                Info.Error(ex.Message);
            }
        }

        private void addbus_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }
    }
}
