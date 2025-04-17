using System;
using System.Text.Json;

class Program
{
    // Classe para armazenar os dados
    public class Pessoa
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string Email { get; set; }
    }

    static void Main(string[] args)
    {
        // Coleta de dados
        Console.Write("Digite o nome: ");
        string nome = Console.ReadLine();

        Console.Write("Digite a idade: ");
        int idade = int.Parse(Console.ReadLine());

        Console.Write("Digite o e-mail: ");
        string email = Console.ReadLine();

        // Criação do objeto
        Pessoa pessoa = new Pessoa
        {
            Nome = nome,
            Idade = idade,
            Email = email
        };

        // Serializa para JSON
        string json = JsonSerializer.Serialize(pessoa, new JsonSerializerOptions { WriteIndented = true });

        // Exibe o JSON
        Console.WriteLine("\nResultado em JSON:");
        Console.WriteLine(json);
    }
}
