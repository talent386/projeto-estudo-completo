<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        /* Seu CSS existente */
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: Arial, sans-serif;
            overflow-x: hidden;
        }

        .chat-container {
            position: fixed;
            height: 100%;
            display: flex;
            flex-direction: column;
            background-color: white;
        }

        .chat-box {
            display: flex;
            flex-direction: column;
            height: 100%;
        }

        .chat-header {
            background-color: #2C6AA0;
            color: white;
            display: flex;
            justify-content: space-between;
            padding: 10px;
            align-items: center;
        }

        .chat-messages {
            flex-grow: 1;
            padding: 10px;
            overflow-y: auto;
            max-height: 700px;
            border-bottom: 1px solid #ccc;
            display: flex;
            flex-direction: column;
        }

        .chat-input {
            display: flex;
            padding: 10px;
            background-color: #f9f9f9;
            border-bottom-left-radius: 8px;
            border-bottom-right-radius: 8px;
        }

            .chat-input input {
                flex-grow: 1;
                padding: 10px;
                border: 1px solid #ccc;
                border-radius: 4px;
                margin-right: 10px;
                font-size: 14px;
            }

            .chat-input button {
                padding: 10px 15px;
                background-color: #2C6AA0;
                color: white;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            }

                .chat-input button:hover {
                    background-color: #0F99CE;
                }

        .user-message {
            background-color: #f1f1f1;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 10px;
            max-width: 80%;
            text-align: right;
            width: fit-content;
            align-self: flex-end !important;
        }

        .talia-message {
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 10px;
            max-width: 80%;
            width: fit-content;
            background-color: #C2EEFF;
            text-align: justify;
            align-self: flex-start;
        }

        .message-wrapper {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            margin-bottom: 10px;
        }

        .talia-wrapper {
            align-items: flex-start;
        }

        .message-sender {
            font-size: 12px;
            color: #02007c;
            margin-top: 0px;
            padding: 0 8px;
        }

        .typing-indicator {
            display: inline-block;
            font-style: italic;
            font-size: 14px;
            color: #02006d;
            margin: 5px;
            padding: 10px;
            background-color: #ccd3e7;
            border-radius: 8px;
            max-width: 200px;
            animation: blink 1.5s infinite;
        }

        @keyframes blink {
            0% {
                opacity: 0.3;
            }

            50% {
                opacity: 1;
            }

            100% {
                opacity: 0.3;
            }
        }

        .info {
            display:inline-flex
        }
    </style>

    <script type="module" >
        import { GoogleGenerativeAI } from "https://esm.run/@google/generative-ai";

        // --- INÍCIO DO CÓDIGO INJETADO DO CODE-BEHIND ---
        // Pega o JSON do code-behind e o transforma em um objeto JavaScript
        var usuarioData = <%= this.UsuarioJson ?? "null" %>;
        // --- FIM DO CÓDIGO INJETADO DO CODE-BEHIND ---

        const API_KEY = ""; // Sua chave de API
        const genAI = new GoogleGenerativeAI(API_KEY);
        let manualText = "";
        let historicoMensagens = [];
        let manualCarregado = false;

        async function enviarMensagem(mensagemUsuario, imagemBase64 = null) {
            historicoMensagens.push({ role: "user", parts: [{ text: mensagemUsuario }] });
            showTypingIndicator();

            const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

            const partes = [];
            if (mensagemUsuario) partes.push({ text: mensagemUsuario });
            if (imagemBase64) {
                partes.push({
                    inlineData: {
                        mimeType: "image/png",
                        data: imagemBase64
                    }
                });
            }

            partes.push({
                text: `
Use o manual do sistema WMS da Talent para responder dúvidas, seguindo estas regras:

${manualText}

* Não cite o manual nem faça referência direta a ele.
* Apenas dê sugestões de utilização de parâmetros se o usuário citar que quer isso, não é aconselhável que o usuário altere parâmetros sem consciência das consequências dessas alterações
* O usuário não tem acesso ao manual; não tente fornecer links ou partes dele.
* Sempre retorne links de imagens relacionadas, se existirem.
* Se a imagem mostrar erro ou informação documentada, explique o significado e oriente claramente a solução.
* Se o problema na imagem não estiver no manual, informe que não é possível ajudar e oriente contato com suporte via WhatsApp.
* Responda de forma educada, profissional e clara, como um atendente experiente.
* Não use palavras de saudação ou tratamento; seja direto.
* Não responda a assuntos fora do sistema WMS, não permita que o usuário desvie o assunto.
* Se não conseguir resolver, oriente contato com suporte via WhatsApp como último recurso.
* Exemplo de link de imagem: ![alt text](link)
* Antes de dar orientações ao usuário, faça perguntas para entender melhor qual o problema dele

Responda sempre com objetividade e clareza, mantendo profissionalismo.


Responda a mensagem:
${mensagemUsuario} ou ${imagemBase64}

` });

            const result = await model.generateContent({
                contents: [historicoMensagens, { role: "user", parts: partes }],
                generationConfig: {
                    temperature: 0.7,
                },
            });

            const response = await result.response;
            const texto = await response.text();
            hideTypingIndicator();
            historicoMensagens.push({ role: "model", parts: [{ text: texto }] });
            addMessage("Talia: " + texto);
        }

        window.addEventListener('DOMContentLoaded', () => {
            document.getElementById('sendBtn').addEventListener('click', async () => {
                const userInput = document.getElementById('userInput').value.trim();
                if (!manualCarregado) {
                    alert("Aguarde, carregando informações do sistema...");
                    return;
                }
                if (userInput) {
                    addMessage("Você: " + userInput);
                    document.getElementById('userInput').value = '';
                    await enviarMensagem(userInput);
                }
            });

            // Adicione esta chamada para iniciar o carregamento do manual
            // usando o nucleoId obtido do code-behind
            if (usuarioData && usuarioData.ds_nucleo) {
                controleAcesso(usuarioData.ds_nucleo);
            } else {
                console.warn("Dados do usuário ou núcleo não disponíveis para carregar o manual.");
                // Opcional: Trate o caso em que o núcleo não está disponível, talvez mostrando um alerta.
            }
        });

        function addMessage(message) {
            const chatMessages = document.getElementById('chatMessages');

            const isTalia = message.startsWith("Talia:");
            const cleanText = message.replace("Talia:", "").replace("Você:", "").trim();

            const wrapper = document.createElement('div');
            wrapper.classList.add('message-wrapper');
            wrapper.classList.add(isTalia ? 'talia-wrapper' : 'user-wrapper');

            const msg = document.createElement('div');
            msg.classList.add('message-class');
            msg.classList.add(isTalia ? 'talia-message' : 'user-message');
            msg.innerHTML = linkify(cleanText);

            const nome = document.createElement('div');
            nome.classList.add('message-sender');
            nome.textContent = isTalia ? 'Talia' : 'Você';

            wrapper.appendChild(msg);
            wrapper.appendChild(nome);
            chatMessages.appendChild(wrapper);
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }

        function addImageToChat(base64) {
            const chatMessages = document.getElementById('chatMessages');
            const img = document.createElement('img');
            img.classList.add('user-message');
            img.src = 'data:image/png;base64,' + base64;
            img.style.maxWidth = '100%';
            img.style.margin = '10px 0';
            chatMessages.appendChild(img);
            chatMessages.scrollTop = chatMessages.scrollHeight;
        }

        function linkify(text) {
            const urlRegex = /(\bhttps?:\/\/[^\s]+)/g;
            const imagemInternaRegex = /(?:!\[.*?\]|\[.*?\])?\((\/data\/imagens_md\/[^\s)]+?\.(?:png|jpg|jpeg|gif))\)/gi;

            let html = text
                .replace(/\n/g, "<br>")
                .replace(/\*\*/g, "")
                .replace(/\*/g, "&bull;");

            html = html.replace(imagemInternaRegex, (match, path) => {
                return `<br><img src="${path}" alt="Imagem do manual" style="max-width:100%; border-radius:6px; margin:10px 0;">`;
            });

            html = html.replace(urlRegex, (url) => {
                return `<a href="${url}" target="_blank" rel="noopener noreferrer">${url}</a>`;
            });

            return html;
        }

        let typingIndicator;

        function showTypingIndicator() {
            const chatMessages = document.getElementById('chatMessages');
            if (!typingIndicator) {
                typingIndicator = document.createElement('div');
                typingIndicator.classList.add('message-wrapper', 'talia-wrapper');
                const indicatorContent = document.createElement('div');
                indicatorContent.classList.add('typing-indicator');
                indicatorContent.textContent = 'Talia está digitando...';
                typingIndicator.appendChild(indicatorContent);
                chatMessages.appendChild(typingIndicator);
                chatMessages.scrollTop = chatMessages.scrollHeight;
            }
        }

        function hideTypingIndicator() {
            if (typingIndicator && typingIndicator.parentNode) {
                typingIndicator.parentNode.removeChild(typingIndicator);
                typingIndicator = null;
            }
        }

        document.getElementById('imageInput').addEventListener('change', async (event) => {
            if (!manualCarregado) {
                alert("Aguarde, carregando informações do sistema...");
                return;
            }

            const file = event.target.files[0];
            if (!file) return;

            const base64 = await fileToBase64(file);
            addImageToChat(base64);
            await enviarMensagem('', base64);
        });

        function fileToBase64(file) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.onloadend = () => resolve(reader.result.split(',')[1]);
                reader.onerror = reject;
                reader.readAsDataURL(file);
            });
        }

        // A variável nucleoId agora será lida do objeto usuarioData
        // let nucleoId = G_Session.Nucleo.Ds_Nucleo; // Remova esta linha, não será mais usada

        async function controleAcesso(nucleoIdFromData) { // Renomeado para evitar conflito com a global nucleoId
            const nucleosMap = {
                "Segurança": ["/manuais/Manual_nucleo_Seguranca.md"],
                "Contrato": ["manuais/Mapa_Talent.md", "manuais/Manual_nucleo_Contrato.md"],
                "WMS": ["manuais/Mapa_Talent.md", "/manuais/Manual_nucleo_WMS.md", "/manuais/Manual_nucleo_WMS_Parametros.md", "/manuais/teste.md", "/manuais/Manual_CFOP.md"],
                "Portaria": ["manuais/Mapa_Talent.md", "/manuais/teste_.md"],
                "5": ["manuais/Mapa_Talent.md", "manuais/Manual_nucleo_Financeiro.md"],
                "6": ["manuais/Mapa_Talent.md", "manuais/Manual_nucleo_Fatura_Servico.md"],
                "7": ["manuais/Mapa_Talent.md", "manuais/Manual_nucleo_Contabilidade.md", "/manuais/Manual_CFOP.md"],
                "8": ["manuais/Mapa_Talent.md", "manuais/Manual_nucleo_Emissor_NFe.md"],
            };

            const arquivos = nucleosMap[nucleoIdFromData]; // Use o parâmetro da função
            if (!arquivos) {
                alert("Módulo não encontrado para o núcleo: " + nucleoIdFromData);
                return;
            }

            manualText = "";
            manualCarregado = false;

            for (const arquivo of arquivos) {
                const response = await fetch(arquivo);
                const texto = await response.text();
                manualText += "\n ----- \n" + texto;
            }

            manualCarregado = true;

            // Removendo as linhas abaixo, pois você não tem o botão associado diretamente
            // const nomeModulo = document.querySelector(`button[value="${nucleoId}"]`).textContent;
            // addMessage("Você: Estou no núcleo " + nomeModulo);
            // await enviarMensagem("Estou no núcleo " + nomeModulo);
        }
    </script>
</asp:Content>

---

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="chat-container" id="chatContainer">
        <div class="chat-box">
            <div class="chat-header">
                <h2>Suporte</h2>
            </div>
            <span class="badge badge-info">Núcleo: <span id="nucleoInfo"></span></span>
            <span class="badge badge-warning">Protocolo: <span id="protocoloInfo"></span></span>
            <span class="badge badge-info">Aberto em: <span id="dataCadastroInfo"></span></span>
            
            <div class="chat-messages" id="chatMessages">
                <div class="message-wrapper talia-wrapper">
                    <div class="message-class talia-message">
                        Olá, <span id="nomeUsuarioChat"></span>! Me chamo <span style="color: #5a06c9;">Talia</span> e sou sua Assistente de Suporte Nível 1. Como posso te ajudar hoje? <br>
                        Você pode descrever o seu problema ou, se preferir, me enviar um print da sua tela, o que for mais fácil para você. Estou à disposição!
                    </div>
                    <div class="message-sender">Talia</div>
                </div>
            </div>
            <div class="chat-input">
                <input type="text" id="userInput" placeholder="Digite sua mensagem..." />
                <button id="sendBtn">Enviar</button>
            </div>
            <input type="file" id="imageInput" accept="image/*" />
        </div>
    </div>

    <script type="text/javascript">
        // Adicione este script para preencher as informações dinâmicas
        $(document).ready(function () {
            if (usuarioData !== null) {
                // Preenchendo as informações na UI
                $('#nucleoInfo').text(usuarioData.ds_nucleo);
                $('#nomeUsuarioChat').text(usuarioData.nome);

                // Como você mencionou "protocolo", se ele vier de uma fonte diferente,
                // você precisará de uma propriedade similar no code-behind para ele.
                // Por agora, estou assumindo que 'protocolo' não está diretamente no 'usuarioData'.
                // Se 'protocolo.nr_protocolo' e 'protocolo.dt_cadastro' estiverem disponíveis,
                // você precisará passá-los de forma semelhante (outra propriedade JSON, ou direto na página).

                // Exemplo se você tivesse um protocolo no code-behind como 'ProtocoloJson'
                // var protocoloData = <%= this.ProtocoloJson ?? "null" %>;
                // if (protocoloData !== null) {
                //     $('#protocoloInfo').text(protocoloData.nr_protocolo);
                //     $('#dataCadastroInfo').text(protocoloData.dt_cadastro); // Formatando a data pode precisar de uma função JS
                // }
                // Por enquanto, os campos de protocolo ficarão vazios ou você precisará
                // buscar essas informações de outra forma.
            }
        });
    </script>
</asp:Content>
