using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Data;
using System.Data.SQLite;

namespace AIS_bus_station
{
    class database
    {
        // Переменная для подключения к БД
        string connectString = "Data Source=" + Properties.Resources.database + ";Version=3;";
        SQLiteConnection connect;
        string connectionStr;
        public database(string ConnectStr = null)
        {
            if (string.IsNullOrEmpty(ConnectStr))
                ConnectStr = connectString;
            // создаём объект для подключения к БД
            connect = new SQLiteConnection(ConnectStr);
            connectionStr = ConnectStr;
        }

        public string GetConnectStr()
        {
            return connectionStr;
        }

        public void Open()
        {
            // Открываем соединение
            connect.Open();
        }

        public void Close()
        {
            // Закрываем соединение
            connect.Close();
        }

        // Возращает 1 строчку запроса SQL
        public string QuaryStr(string quary)
        {
            // объект для выполнения SQL-запроса
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connect;
            cmd.CommandText = quary;

            // выполняем запрос и получаем ответ
            return cmd.ExecuteScalar().ToString();
        }

        public void Quary(string quary)
        {
            // объект для выполнения SQL-запроса
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connect;
            cmd.CommandText = quary;

            // выполняем запрос и получаем ответ
            cmd.ExecuteNonQuery();
        }

        public Dictionary<int, Dictionary<string, string>> QuaryMas(string quary)
        {
            Dictionary<int, Dictionary<string, string>> result = new Dictionary<int, Dictionary<string, string>>();
            // объект для выполнения SQL-запроса
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = connect;
            cmd.CommandText = quary;
            // объект для чтения ответа сервера
            SQLiteDataReader reader = cmd.ExecuteReader();
            // читаем результат
            while (reader.Read())
            {
                Dictionary<string, string> buffer = new Dictionary<string, string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    buffer.Add(reader.GetName(i).ToString(), reader[i].ToString());
                }
                result.Add(Convert.ToInt32(reader["id"]), buffer);
            }
            reader.Close(); // закрываем reader
            return result;
        }

        public DataSet QuaryData(string quary)
        {
            DataSet data = new DataSet();
            SQLiteDataAdapter dataAdapter;
            dataAdapter = new SQLiteDataAdapter(quary, connect);
            dataAdapter.Fill(data);
            return data;
        }
        public void QuaryDataUpdate(string quary, DataSet data)
        {
            SQLiteDataAdapter dataAdapter;
            dataAdapter = new SQLiteDataAdapter(quary, connect);
            SQLiteCommandBuilder cb = new SQLiteCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = cb.GetUpdateCommand();
            dataAdapter.InsertCommand = cb.GetInsertCommand();
            dataAdapter.Update(data.Tables[0]);
        }

        public string GetMD5Hash(string input)
        {
            byte[] hash = Encoding.ASCII.GetBytes(input);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hashenc = md5.ComputeHash(hash);
            string result = "";
            foreach (var b in hashenc)
            {
                result += b.ToString("x2");
            }
            return result;
        }

        public void error(string message)
        {
            MessageBox.Show(
                        message,
                        "Ошибка!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
        }

        public void success(string message)
        {
            MessageBox.Show(
                        message,
                        "Успех!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
        }

        public string serializeArray(int[] array)
        {
            string Arraystring = array[0].ToString();

            for (int i = 1; i < array.Length; i++)
            {
                Arraystring += "," + array[i].ToString();
            }

            return Arraystring;
        }

        public int[] deserializeArray(string array)
        {
            string[] tokens = array.Split(',');

            int[] myItems = Array.ConvertAll<string, int>(tokens, int.Parse);

            return myItems;
        }
    }
}
