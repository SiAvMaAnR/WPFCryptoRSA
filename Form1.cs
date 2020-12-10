using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFCryptoRSA
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}



		private async void Form1_Load(object sender, EventArgs e)
		{
			for (Opacity = 0; Opacity < 1; Opacity += 0.02)
			{
				await Task.Delay(10);
			}
			textBox2.Text = "36563";
			textBox1.Text = "57731";
		}

		

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape: Close(); break;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(textBox3.Text)|| string.IsNullOrEmpty(textBox2.Text)|| string.IsNullOrEmpty(textBox1.Text))
					throw new Exception("Error: Заполните пустые поля!");

				long p = (IsPrimeNumber(Convert.ToInt64(textBox2.Text))) ? Convert.ToInt64(textBox2.Text) : throw new Exception("Error: p должно быть простым");
				long q = (IsPrimeNumber(Convert.ToInt64(textBox1.Text))) ? Convert.ToInt64(textBox1.Text) : throw new Exception("Error: q должно быть простым");
				long eOpenKey = (((1 < Convert.ToInt64(textBox3.Text) && Convert.ToInt64(textBox3.Text) < (p - 1) * (q - 1))) && (GCD(Convert.ToInt64(textBox3.Text), ((p - 1) * (q - 1))) == 1)) 
					? Convert.ToInt64(textBox3.Text) 
					: throw new Exception("Error: Должно выполняться 2 условия:\n 1 - (1 < e < Ф(n))\n 2 - e и Ф(n) - взаимно простые числа!");

				RSA algorithmRSA = new RSA(eOpenKey, p, q);



				algorithmRSA.getPublicKey(out long e1, out long n1);//Возврат открытых ключей
				algorithmRSA.getPrivateKey(out long d, out long n2);//Возврат закрытых ключей

				this.d = d; this.n = n2;this.e = e1;

				string[] rsaEncrypt = algorithmRSA.Encrypt(textBox6.Text, e1, n1);//Зашифровываем сообщение

				string OutMessage = "";
				foreach (var item in rsaEncrypt) 
					OutMessage += item+ " ";
				PrintOut(OutMessage, true);

			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}

		private void PrintOut(string str, bool B)
		{
			if (B)
				label4.Text = str;
			else 
				label5.Text = str;
		}


		private long e;
		private long d;
		private long n;

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
					throw new Exception("Error: Заполните пустые поля!");

				RSA algorithmRSA = new RSA();
				string[] rsaEncrypt = textBox7.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				string rsaDecrypt = algorithmRSA.Decrypt(rsaEncrypt, Convert.ToInt64(textBox4.Text), Convert.ToInt64(textBox5.Text));//Дешифровываем сообщение
				label5.Text = rsaDecrypt;

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{
			if (!(this.e == 0 || this.d == 0 || this.n == 0))
			{
				MessageBox.Show($"Открытый ключ --> (e,n) --> ({this.e} , {this.n})\nЗакрытый ключ --> (d,n) --> ({this.d} , {this.n})", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			textBox7.Text = label4.Text;
			textBox4.Text = d.ToString();
			textBox5.Text = n.ToString();
			tabControl1.SelectedIndex = 1;
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			textBox6.Text = label5.Text;
			tabControl1.SelectedIndex = 0;
		}

		private static long GCD(long A, long B)//Поиск НОД | Алгоритм Евклида
		{
			while (B != 0)
				(A, B) = (B, A % B);
			return A;
		}

		private static bool IsPrimeNumber(long n)//Проверка на простоту
		{
			bool result = true;

			if (n > 1)
			{
				for (var i = 2u; i < n; i++)
				{
					if (n % i == 0)
					{
						result = false;
						break;
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void label5_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(label5.Text))
			{
				DialogResult DR = MessageBox.Show("Скопировать вывод в буфер обмена?", "Warning!", MessageBoxButtons.YesNo);
				if (DR == DialogResult.Yes)
				{
					Label label = sender as Label;
					if (label != null)
						Clipboard.SetText(label.Text, TextDataFormat.UnicodeText);
				}
			}
		}

		private void label4_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(label5.Text))
			{
				DialogResult DR = MessageBox.Show("Скопировать вывод в буфер обмена?", "Warning!", MessageBoxButtons.YesNo);
				if (DR == DialogResult.Yes)
				{
					Label label = sender as Label;
					if (label != null)
						Clipboard.SetText(label.Text, TextDataFormat.UnicodeText);
				}
			}
		}
	}
}
