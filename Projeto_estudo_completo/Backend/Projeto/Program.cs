// See https://aka.ms/new-console-template for more information
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;


class Program
{
    // Classe para armazenar os dados
    public class Produto
    {
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public string SKU { get; set; }
        public string Corredor { get; set; }
        public string Prateleira { get; set; }
    }

    static void Main(string[] args)
    {
        // Coleta de dados
        

        // Criação do objeto
        Produto produto = new Produto
        {
            Produto = produto,
            SKU = sku,
            Quantidade = quantidade,
            localização {
                Corredor = corredor
                Prateleira = prateleira
            }

        };

        // Serializa para JSON
       string json = JsonSerializer.Serialize(produto, new JsonSerializerOptions { WriteIndented = true });

        // Caminho do arquivo JSON
        string caminho = "Data.json";

        // Escrevendo no arquivo (append)
        File.AppendAllText(caminho, json + Environment.NewLine);

        Console.WriteLine("\nProduto salvo em JSON:");
        Console.WriteLine(json);
    }
}
