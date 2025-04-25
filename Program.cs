using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.DataClassification;
using System.Data;
using System.Data.Common;

SqlConnection conexao = new SqlConnection("Server=#;Initial Catalog=#;User Id=#;Password=#;TrustServerCertificate=True;");

  
        conexao.Open();
        Console.WriteLine(conexao.State.ToString());
        SqlCommand command= conexao.CreateCommand();
         command.CommandText = "teste";

        command.CommandTimeout = 15;
        command.CommandType = CommandType.StoredProcedure;

                 SqlDataAdapter adapter = new SqlDataAdapter(command);
                 DataSet DSteste = new DataSet();
                 adapter.Fill(DSteste);

        conexao.Close();
              Console.WriteLine(conexao.State.ToString());


