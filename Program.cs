using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.DataClassification;
using System.Data;
using System.Data.Common;
using System;
using Microsoft.AspNetCore.Builder;



// Utilização do CORS
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

    var table = dataSet.Tables[0];
    var resultados = new List<Dictionary<string, object>>();

    foreach (DataRow row in table.Rows)
    {
        var linha = new Dictionary<string, object>();
        foreach (DataColumn column in table.Columns)
        {
            linha[column.ColumnName] = row[column];
        }
        resultados.Add(linha);
    }

    return Results.Ok(resultados);
});




app.Run();












