using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AIS_bus_station
{
    class Infomation
    {
        // Вывод диалогового окна с информацией
        public bool Info(string message = "")
        {
            MessageBox.Show(
                message,
                "Информация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            return false;
        }
        // Вывод диалогового окна с ошибкой
        public void Error(string message = "")
        {
            MessageBox.Show(
                message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        // Вывод диалогового окна с предупреждением
        public void Warning(string message = "")
        {
            MessageBox.Show(
                message,
                "Предупреждение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}
