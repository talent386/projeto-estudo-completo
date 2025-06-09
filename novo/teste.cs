using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public partial class Chat_Atendimento : System.Web.UI.Page
{
    // 1. DECLARE A PROPRIEDADE PÚBLICA PARA ARMAZENAR O JSON
    // A página .aspx irá ler o valor desta propriedade.
    public string UsuarioJson { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (G_Session.Empresa.Cd_Empresa > 0 && !String.IsNullOrEmpty(G_Session.Empresa.CNPJ_CPF_Filial))
        {
            JObject jUsuario = new JObject();
            jUsuario.Add("idusuario", G_Session.Usuario.Usuario);
            jUsuario.Add("ds_grupo_usuario", G_Session.Usuario.Ds_Grupo_Usu);
            jUsuario.Add("nome", G_Session.Usuario.Nome);
            jUsuario.Add("ind_admin_sistema", G_Session.Usuario.Ind_Administrador_Sistema);
            jUsuario.Add("cnpj_cpf", G_Session.Empresa.CNPJ_CPF_Filial);
            jUsuario.Add("cd_empresa", G_Session.Empresa.Cd_Empresa);
            jUsuario.Add("cd_filial", G_Session.Empresa.Cd_Filial);
            jUsuario.Add("empresa", G_Session.Empresa.Nome_Fantasia_Filial);
            jUsuario.Add("ds_nucleo", G_Session.Nucleo.Ds_Nucleo); // A propriedade no JSON será "ds_nucleo"
            jUsuario.Add("cd_usuario_terceiro", G_Session.Usuario.Cd_Terceiro);

            TalentCS_Chat.ConexaoUsuario.addUsuario(jUsuario);
            
            // 2. CONVERTA O JObject PARA STRING E ATRIBUA À PROPRIEDADE
            // Agora a variável UsuarioJson tem o valor que o ASPX precisa.
            this.UsuarioJson = jUsuario.ToString(Formatting.None); 
        }
    }
}
