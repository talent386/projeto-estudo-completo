using System; // Necessário para usar Console.WriteLineusing System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Geradordetroco // Agrupa o código sob o nome Helloworld
{
	class Program // Classe principal do programa
	{
		static void Main(string[] args) // Ponto de entrada
		{
			string inicio = "Insira montante: ";
			Console.Write(inicio);
			
			string textMontante = Console.ReadLine();
			float textMontante = float.Parse(textMontante);
			float valor = 524.50f;
			float age = dataNascimento - valor; 
			
			string complemento = "seu troco é: ";
			Console.WriteLine(complemento + age);
		}
	}
}