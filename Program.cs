using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.DataClassification;
using System.Data;
using System.Data.Common;
using System;
using Microsoft.AspNetCore.Builder;

// SqlConnection conexao = new SqlConnection("");

  
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



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Endpoint GET /api/dados que chama a stored procedure
app.MapGet("/api/dados", async () =>
{
    // *** Ajuste sua connection string aqui! ***
    var connectionString = "";

    await using var conn = new SqlConnection(connectionString);
    await using var cmd = new SqlCommand("API", conn)
    {
        CommandType = CommandType.StoredProcedure
    };

    // Se a sua SP tiver parâmetros, faça algo tipo:
    cmd.Parameters.AddWithValue("@Location", 1);

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


// using System.Data;
// using System.Data.SqlClient;

// string conexaoString = "sua_string_de_conexao_aqui";
// using (SqlConnection conexao = new SqlConnection(conexaoString))
// {
//     conexao.Open();

//     // Cria o comando para executar a Stored Procedure
//     using (SqlCommand comando = new SqlCommand("NomeDaSuaProcedure", conexao))
//     {
//         comando.CommandType = CommandType.StoredProcedure;

//         // Cria o parâmetro
//         SqlParameter parametroId = new SqlParameter("@IdCliente", SqlDbType.Int);
//         parametroId.Value = 1; // valor inicial

//         SqlParameter parametroNome = new SqlParameter("@NomeCliente", SqlDbType.NVarChar, 100);
//         parametroNome.Value = "João"; // valor inicial

//         // Adiciona os parâmetros ao comando
//         comando.Parameters.Add(parametroId);
//         comando.Parameters.Add(parametroNome);

//         // Agora, se quiser alterar o valor do parâmetro antes de executar:
//         parametroId.Value = 5; // novo valor
//         parametroNome.Value = "Maria"; // novo valor

//         // Executa a procedure
//         comando.ExecuteNonQuery();
//     }
// }






