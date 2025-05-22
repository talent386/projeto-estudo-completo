using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.DataClassification;
using System.Data;
using System.Data.Common;
using System;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://127.0.0.1:3000") 
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);



//API com SqlReader 
// app.MapGet("/api/dados/{location}", async (int location) =>
// {
//     // *** Ajuste sua connection string aqui! ***
//     var connectionString = "";

//     await using var conn = new SqlConnection(connectionString);
//     await using var cmd = new SqlCommand("API", conn)
//     {
//         CommandType = CommandType.StoredProcedure
//     };

//     // Se a sua SP tiver parâmetros, faça algo tipo:
//     cmd.Parameters.AddWithValue("@Location", location);

//     await conn.OpenAsync();
//     await using var reader = await cmd.ExecuteReaderAsync();

//     var resultados = new List<Dictionary<string, object>>();
//     while (await reader.ReadAsync())
//     {
//         var linha = new Dictionary<string, object>();
//         for (int i = 0; i < reader.FieldCount; i++)
//             linha[reader.GetName(i)] = reader.GetValue(i);
//         resultados.Add(linha);
//     }

//     return Results.Ok(resultados);
// });






// API com SqlDataAdapter
app.MapGet("/api/dados/{location}", async (int location) =>
{
    var connectionString = "";

    using var conexao = new SqlConnection(connectionString);
    using var command = conexao.CreateCommand();

    command.CommandText = "API"; 
    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.AddWithValue("@Location", location);

    var adapter = new SqlDataAdapter(command);
    var dataSet = new DataSet();

    adapter.Fill(dataSet);


// serialização

var produtos = new List<Dictionary<string, object>>();
var table1 = dataSet.Tables[0];

foreach (DataRow row in table1.Rows)
{
    var dict = new Dictionary<string, object>();
    foreach (DataColumn column in table1.Columns)
    {
        dict[column.ColumnName] = row[column];
    }
    produtos.Add(dict);
}

var card = new Dictionary<string, object>();
if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
{
    var row = dataSet.Tables[1].Rows[0];
    foreach (DataColumn column in dataSet.Tables[1].Columns)
    {
        card[column.ColumnName] = row[column];
    }
}

var resultadoFinal = new
{
    produtos,
    card
};

// Serializa o objeto para JSON com Newtonsoft
string json = JsonConvert.SerializeObject(resultadoFinal, Formatting.Indented);

// Retorna como resposta JSON manual
return Results.Content(json, "application/json");


});


app.MapGet("/api/location", async () =>
{
    var connectionString = "";

    var locais = new List<Dictionary<string, object>>();

    using var conexao = new SqlConnection(connectionString);
    using var command = conexao.CreateCommand();

    command.CommandText = "location";
    command.CommandType = CommandType.StoredProcedure;

    var adapter = new SqlDataAdapter(command);
    var dataSet = new DataSet();

    // Executa o preenchimento do DataSet em uma thread em background
    await Task.Run(() => adapter.Fill(dataSet));

    if (dataSet.Tables.Count > 0)
    {
        var table = dataSet.Tables[0];

        foreach (DataRow row in table.Rows)
        {
            var dict = new Dictionary<string, object>();

            foreach (DataColumn column in table.Columns)
            {
                dict[column.ColumnName] = row[column];
            }

            locais.Add(dict);
        }
    }

    return Results.Json(locais); // Retorno assíncrono compatível
});









