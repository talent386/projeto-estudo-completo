
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

string caminhoArquivo = "Data.json";

// Modelo Produto
public class Produto
{
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public string SKU { get; set; }
    public string Corredor { get; set; }
    public string Prateleira { get; set; }
}

// Endpoint POST - Adiciona um novo produto
app.MapPost("/api/produtos", async (Produto novoProduto) =>
{
    List<Produto> produtos = new();

    if (File.Exists(caminhoArquivo))
    {
        string conteudo = await File.ReadAllTextAsync(caminhoArquivo);
        if (!string.IsNullOrWhiteSpace(conteudo))
            produtos = JsonSerializer.Deserialize<List<Produto>>(conteudo) ?? new List<Produto>();
    }

    produtos.Add(novoProduto);

    string jsonAtualizado = JsonSerializer.Serialize(produtos, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(caminhoArquivo, jsonAtualizado);

    return Results.Ok(new { mensagem = "Produto salvo com sucesso!" });
});

// Endpoint GET - Buscar produto por SKU
app.MapGet("/api/produtos/{sku}", (string sku) =>
{
    if (!File.Exists(caminhoArquivo))
        return Results.NotFound("Arquivo não encontrado.");

    string conteudo = File.ReadAllText(caminhoArquivo);
    var produtos = JsonSerializer.Deserialize<List<Produto>>(conteudo);
    var produto = produtos?.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

    if (produto == null)
        return Results.NotFound("Produto não encontrado.");

    return Results.Ok(produto);
});

app.Run();
