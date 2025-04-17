
using System; // Necessário para usar Console.WriteLineusing System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ageguesser // Agrupa o código sob o nome Helloworld
{
	class Program // Classe principal do programa
	{
		static void Main(string[] args) // Ponto de entrada
		{
			string inicio = "Insira seu ano de nascimento: ";
			Console.Write(inicio);
			
			string textDataNascimento = Console.ReadLine();
			int dataNascimento = Convert.ToInt32(textDataNascimento);
			int ano = 2025;
			int age = ano - dataNascimento; 
			
			string complemento = "sua idade é: ";
			Console.WriteLine(complemento + age);
		}
	}
}