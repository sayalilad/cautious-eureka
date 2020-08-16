using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filepath = "D:\\Moana\\24062020\\"; //Filepath where you kept file
        private void button1_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(filepath, "*.csv");
            List<string> Normalized_Data;// = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                string[] AllRows = File.ReadAllLines(files[i]);
                Normalized_Data = new List<string>();
                Normalized_Data.Add(AllRows[0]);
                for (int j = 1; j < AllRows.Length; j++)
                {
                    Get_PhoneColumn_value(AllRows[j], ref Normalized_Data);


                }
                
                //Writing csv file with contents of List
                using (StreamWriter sw = new StreamWriter(new FileStream(files[i].Split('\\')[3].Split('.')[0] + "_new.csv", FileMode.Create)))
                {
                    for (int p = 0; p < Normalized_Data.Count; p++)
                    {
                        sw.WriteLine(Normalized_Data[p]);
                    }
                }
            }
        }

        private string GetNormalized_Phone_number(string v)
        {
            ulong phone_num = 0;
            phone_num = ulong.Parse(v);

            if (v.Contains('+'))
                return v;

            if (ulong.TryParse(v, out phone_num))
            {
                int[] ProperFormat = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


                int End_index = 12 - phone_num.ToString().Length;
                for (int i = 11, j = phone_num.ToString().Length - 1; i >= End_index && j >= 0; i--, j--)
                {
                    
                    ProperFormat[i] = Int32.Parse(((char)phone_num.ToString().ElementAt(j)).ToString());
                }

                if (phone_num.ToString().Length < 9 || phone_num.ToString().Length == 9)
                {
                    ProperFormat[0] = 3;
                    ProperFormat[1] = 5;
                    ProperFormat[2] = 3;
                }

                StringBuilder temp = new StringBuilder();
                for (int i = 0; i < ProperFormat.Length; i++)
                {
                    temp.Append(ProperFormat[i]);
                }
                phone_num = Convert.ToUInt64(temp.ToString());
            }

            return "+" + phone_num.ToString();
        }

        private void Get_PhoneColumn_value(string HeaderRow, ref List<string> lst)
        {
            string temp2 = "";
            StringBuilder temp1 = new StringBuilder().Append(HeaderRow.Split(',')[0]);
            if (HeaderRow.Split(',')[1].Contains('"') && HeaderRow.Split('"')[1].Contains(','))  //Address in English
            {
                temp1.Append(",\"" + HeaderRow.Split('"')[1] + "\"" + ",");
                if (!HeaderRow.Split(new string[] { "\"," }, StringSplitOptions.None)[1].Contains(','))
                {
                    temp2 = GetNormalized_Phone_number(HeaderRow.Split(new string[] { "\"," }, StringSplitOptions.None)[1]);
                    temp1.Append(temp2);
                }
                else
                {
                    temp2 = GetNormalized_Phone_number(HeaderRow.Split(new string[] { "\"," }, StringSplitOptions.None)[1].Split(',')[0]);
                    temp1.Append(temp2);
                    for (int k = 1; k < HeaderRow.Split(new string[] { "\"," }, StringSplitOptions.None)[1].Split(',').Length; k++)
                    {
                        temp1.Append("," + HeaderRow.Split(new string[] { "\"," }, StringSplitOptions.None)[1].Split(',')[k]);
                    }
                }
            }
            else // Chinese characters address without ""
            {
                temp1.Append(HeaderRow.Split(',')[1] + ",");
                temp2 = GetNormalized_Phone_number(HeaderRow.Split(',')[2]);
                temp1.Append(temp2);
                //Content after phone number
                if (HeaderRow.Split(',').Length > 3)
                {
                    for (int i = 3; i < HeaderRow.Split(',').Length; i++)
                    {
                        temp1.Append("," + HeaderRow.Split(',')[i]);
                    }

                }
            }

            //add to list
            lst.Add(temp1.ToString());




        }
    }
}
