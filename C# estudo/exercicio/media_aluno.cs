
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
			Console.WriteLine("Insira o nome do aluno:");
			var aluno = Console.ReadLine();
			Console.WriteLine("Insira as notas: ");
		 
			var nota1T = Console.ReadLine();
			var nota2T = Console.ReadLine();
			var nota3T = Console.ReadLine();
			var nota4T = Console.ReadLine();
			var nota5T = Console.ReadLine();
			var nota6T = Console.ReadLine();
			
			int nota1 = Convert.ToInt32(nota1T);
			int nota2 = Convert.ToInt32(nota2T);
			int nota3 = Convert.ToInt32(nota3T);
			int nota4 = Convert.ToInt32(nota4T);
			int nota5 = Convert.ToInt32(nota5T);
			int nota6 = Convert.ToInt32(nota6T);
		 
		 Console.WriteLine("A média do " + aluno + " é: " + (nota1+nota2+nota3+nota4+nota5+nota6)/6);
			
			

		}
	}
}