using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_bus_station
{
    public partial class SettingsForm : Form
    {

        // Объект вывода диалоговых окон
        IniFile INI = new IniFile("config.ini");
        private Infomation Info = new Infomation();

        public SettingsForm()
        {
            InitializeComponent();
            // Загрузка настроек
            if (!(INI.ReadINI("dev", "edit_identifier").Length > 0))
            {
                INI.Write("dev", "edit_identifier", "0");
            }
            if (!(INI.ReadINI("dev", "func_refund").Length > 0))
            {
                INI.Write("dev", "func_refund", "1");
            }
            if (!(INI.ReadINI("dev", "func_edit_tables").Length > 0))
            {
                INI.Write("dev", "func_edit_tables", "1");
            }
            if (!(INI.ReadINI("dev", "func_login").Length > 0))
            {
                INI.Write("dev", "func_login", "1");
            }
            if (!(INI.ReadINI("dev", "func_sale").Length > 0))
            {
                INI.Write("dev", "func_sale", "1");
            }
            checkBox1.Checked = CheckedINI("dev", "edit_identifier");
            checkBox2.Checked = CheckedINI("dev", "func_refund");
            checkBox3.Checked = CheckedINI("dev", "func_edit_tables");
            checkBox4.Checked = CheckedINI("dev", "func_login");
            checkBox5.Checked = CheckedINI("dev", "func_sale");
            //textBox1.Text = INI.ReadINI("dev", "load_tables_dalay");
        }

        private bool CheckedINI(string Section, string Key)
        {
            int value = Convert.ToInt32(INI.ReadINI(Section, Key));

            switch(value)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        private string CheckedINIresult(CheckBox checkBox)
        {
            bool value = checkBox.Checked;

            switch (value)
            {
                case true:
                    return 1.ToString();
                default:
                    return 0.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            INI.Write("dev", "edit_identifier", CheckedINIresult(checkBox1));
            INI.Write("dev", "func_refund", CheckedINIresult(checkBox2));
            INI.Write("dev", "func_edit_tables", CheckedINIresult(checkBox3));
            INI.Write("dev", "func_login", CheckedINIresult(checkBox4));
            INI.Write("dev", "func_sale", CheckedINIresult(checkBox5));

            var message = MessageBox.Show(
                "Настройки успешно применены!\nПерезапустите программу для того, что бы изменения вступили в силу.",
                "Информация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            if (message == DialogResult.OK)
            {
                Close();
            }
        }
    }
}
