using System;
using System.Numerics;

public class RSA
{
	private long p;// p простое число
	private long q;// q простое число

	private long e;// e открытая экспонента

	private long d;// d секретная экспонента

	private long n;// n
	private long Fi;// Ф(n)

	private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	//private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789.,?!*/+-=_()%;:#";//Набор сиволов

	private int[] numericConversion;//Перевод текста в числовую запись

	private long[] encrypt;//Числовой шифр
	private string[] alphabetEncrypt;//Зашифрованное сообщение в символьном представлении

	private long[] decrypt;//Числовой шифр до расшифровки
	private long[] alphabetDecrypt;//Дешифрованное сообщение в численном представлении
	private string OutDecrypt;//Дешифрованное сообщение





	public void getPublicKey(out long e, out long n)//Получить открытый ключ
	{
		e = this.e;
		n = this.n;
	}

	public void getPrivateKey(out long d, out long n)//Получить секретный ключ
	{
		d = this.d;
		n = this.n;
	}

	public RSA()
	{

	}

	public RSA(long e, long p, long q)//Генерация ключей
	{
		try
		{
			this.p = p;
			this.q = q;

			this.n = p * q;// n модуль

			this.Fi = (p - 1) * (q - 1);// Фи(n)

			this.e = e;

			extendedGCD(e, Fi, out long x, out long y, out long d);
			this.d = x + Fi;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
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

	private static long GCD(long A, long B)//Поиск НОД | Алгоритм Евклида
	{
		while (B != 0)
			(A, B) = (B, A % B);
		return A;
	}

	private static void extendedGCD(long a, long b, out long x, out long y, out long d)//Расширенный алгоритм Евклида
	{
		long q, r, x1, x2, y1, y2;


		if (b == 0)
		{
			d = a;
			x = 1;
			y = 0;
			return;
		}
		x2 = 1; y2 = 0;
		x1 = 0; y1 = 1;

		while (b > 0)
		{
			q = a / b; r = a - q * b;
			x = x2 - q * x1; y = y2 - q * y1;
			a = b; b = r;
			x2 = x1; x1 = x; y2 = y1; y1 = y;
		}
		d = a; x = x2; y = y2;
	}




	public void CipherLog()//Лог работы программы
	{
		//(e,n) --> Открытый ключ
		//(d,n) --> Закрытый ключ
		Console.WriteLine($"Сообщение хочет получить абонент B от абонента A");
		Console.WriteLine($"Процесс инициирует абонент B!\n");

		Console.WriteLine($"У абонента B:");
		Console.WriteLine($"Введем p = {p};");
		Console.WriteLine($"Введем q = {q};");
		Console.WriteLine($"Вычислим n из выражения по формуле n = p * q = {n};");
		Console.WriteLine($"Вычислим Ф(n) по формуле Ф(n) = (p-1)*(q-1). Ф(n) = {Fi};");
		Console.WriteLine($"Выбираем ключ e, соответствующий условию алгоритма. e = {e};");
		Console.WriteLine($"Сгенерируем ключ d, соответствующий e^-1 (mod Ф(n)). d = {d};\n");

		Console.WriteLine($"У абонента A:");
		Console.WriteLine($"Формируем алфавит;");

		Console.Write($"Преобразовываем текст в числовой эквивалент: ");
		foreach (var item in numericConversion) Console.Write(item + " ");

		Console.Write($"\nВыполняем цифрование по формуле m^e(mod n): ");
		foreach (var item in encrypt) Console.Write(item + " ");

		Console.Write($"\nПриведение к символьному виду: ");
		foreach (var item in alphabetEncrypt) Console.Write(item + " ");

		Console.WriteLine("\nПередаем зашифрованное сообщение Абоненту B;");

		Console.WriteLine("\nУ абонента B:");
		Console.Write("Преобразование шифра в числовой эквивалент: ");
		foreach (var item in alphabetDecrypt) Console.Write(item + " ");

		Console.Write("\nРасшифровка сообщения: ");
		foreach (var item in decrypt) Console.Write(item + " ");

		Console.Write("\nРасшифрованное текстовое сообщение: ");
		foreach (var item in OutDecrypt) Console.Write(item);
		Console.WriteLine();
	}


	public static BigInteger reSquaring(BigInteger m, BigInteger e, BigInteger n)//c=m^e(mod n)
	{
		BigInteger E = e;

		int i;
		for (i = 1; E != 1; i++)//Проверка количества элементов
			E = E / 2;

		BigInteger[] bynaryN = new BigInteger[i];

		E = e;
		for (int j = 0; j < i; j++)//Степень в бинарном представлении
		{
			bynaryN[j] = E % 2;
			E = E / 2;
		}

		BigInteger b = m % n;
		for (int j = 1; j < i; j++)//Возведение в степень
		{
			b = BigInteger.ModPow(b, 2, n);
			if (bynaryN[j] == 1)
				m = (m * b) % n;
		}
		return m;
	}

	//=========================================================================================================== Шифрование

	private string symbolicRepresentationToString(long C)//Приведение к символьному типу
	{
		string returnStr = "";

		int numberLength = C.ToString().Length;//Количество цифр в числе

		long index;
		long numberC = C;

		for (int i = 1; i <= numberLength; i++)
		{
			index = (numberC / (long)Math.Pow(alphabet.Length, numberLength - i));

			numberC -= (index * (long)Math.Pow(alphabet.Length, numberLength - i));

			returnStr += alphabet[(int)index];
		}
		return returnStr;
	}

	public string[] Encrypt(string m, long e, long n)//Зашифровать
	{
		numericConversion = new int[m.Length];
		for (int i = 0; i < m.Length; i++)//Числовой вид сообщения
		{
			for (int j = 0; j < alphabet.Length; j++)
			{
				if (m[i] == alphabet[j])
				{
					numericConversion[i] = j; break;
				}
			}
		}

		encrypt = new long[numericConversion.Length];//Шифротекст
		for (int i = 0; i < numericConversion.Length; i++)
		{
			encrypt[i] = (long)reSquaring(numericConversion[i], e, n);
		}

		alphabetEncrypt = new string[encrypt.Length];//Шифротекст в символьном представлении
		for (int i = 0; i < encrypt.Length; i++)
		{
			alphabetEncrypt[i] = symbolicRepresentationToString(encrypt[i]);
		}
		return alphabetEncrypt;
	}


	//=========================================================================================================== Дешифрование

	private long antiSymbolicRepresentationToString(string Str)//Перевод символов в числовой эквивалент
	{

		long StrLength = Str.Length;//Количество букв
		long C = 0;//В числовой эквивалент

		for (int i = 0; i < StrLength; i++)
		{
			for (int j = 0; j < alphabet.Length; j++)
			{
				if (Str[i] == alphabet[j])
				{
					C += j * (long)Math.Pow(alphabet.Length, StrLength - i - 1); break;
				}
			}
		}
		return C;//Вернуть число
	}
	public string Decrypt(string[] m, long d, long n)//Дешифровать
	{
		alphabetDecrypt = new long[m.Length];//Шифротекст в символьном представлении
		for (int i = 0; i < m.Length; i++)
		{
			alphabetDecrypt[i] = antiSymbolicRepresentationToString(m[i]);
		}

		decrypt = new long[alphabetDecrypt.Length];//Шифротекст в число
		for (int i = 0; i < alphabetDecrypt.Length; i++)
		{
			decrypt[i] = (long)reSquaring(alphabetDecrypt[i], d, n);
		}

		for (int i = 0; i < m.Length; i++)
		{
			OutDecrypt += alphabet[(int)decrypt[i]].ToString();
		}

		return OutDecrypt;
	}
}
