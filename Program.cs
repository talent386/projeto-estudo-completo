using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.DataClassification;
using System.Data;
using System.Data.Common;
using System;
using Microsoft.AspNetCore.Builder;


  
//         conexao.Open();
//         Console.WriteLine(conexao.State.ToString());
//         SqlCommand command= conexao.CreateCommand();
//          command.CommandText = "teste";

//         command.CommandTimeout = 15;
//         command.CommandType = CommandType.StoredProcedure;

//                  SqlDataAdapter adapter = new SqlDataAdapter(command);
//                  DataSet DSteste = new DataSet();
//                  adapter.Fill(DSteste);

//         conexao.Close();
//               Console.WriteLine(conexao.State.ToString());


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://127.0.0.1:3000") // <-- CORREÇÃO AQUI
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Endpoint GET /api/dados que chama a stored procedure
app.MapGet("/api/dados/{location}", async (int location) =>
{
    // *** Ajuste sua connection string aqui! ***
    var connectionString = "";

    await using var conn = new SqlConnection(connectionString);
    await using var cmd = new SqlCommand("API", conn)
    {
        CommandType = CommandType.StoredProcedure
    };

    // Se a sua SP tiver parâmetros, faça algo tipo:
    cmd.Parameters.AddWithValue("@Location", location);

    await conn.OpenAsync();
    await using var reader = await cmd.ExecuteReaderAsync();

    var resultados = new List<Dictionary<string, object>>();
    while (await reader.ReadAsync())
    {
        var linha = new Dictionary<string, object>();
        for (int i = 0; i < reader.FieldCount; i++)
            linha[reader.GetName(i)] = reader.GetValue(i);
        resultados.Add(linha);
    }

    return Results.Ok(resultados);
});


app.Run();












