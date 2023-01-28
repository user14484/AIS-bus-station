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
            if (!INI.KeyExists("dev", "load_tables_dalay"))
            {
                INI.Write("dev", "load_tables_dalay", "400");
            }
            textBox1.Text = INI.ReadINI("dev", "load_tables_dalay");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            INI.Write("dev", "load_tables_dalay", textBox1.Text);
            Info.Info("Настройки успешно применены!\nПерезапустите программу для того, что бы изменения вступили в силу.");
        }
    }
}
