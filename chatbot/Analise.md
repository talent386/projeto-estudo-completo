
---

### 🧠 1. **Importação da Biblioteca da API Gemini**

```js
import { GoogleGenerativeAI } from "https://esm.run/@google/generative-ai";
```

Importa a SDK da Google Gemini (modelo de IA generativa). Essa biblioteca é usada para conversar com a IA que vai responder às mensagens.

---

### 🗂️ 2. **Inicialização e variáveis globais**

```js
const API_KEY = ""; 
const genAI = new GoogleGenerativeAI(API_KEY);
let manualText = "";
let historicoMensagens = [];
```

- `API_KEY`: a chave da API (deve ser preenchida).
    
- `genAI`: instancia a API do Gemini.
    
- `manualText`: armazenará o conteúdo dos arquivos Markdown.
    
- `historicoMensagens`: mantém o histórico da conversa (para manter o contexto).
    

---

### 📄 3. **Carregamento do manual em Markdown**

```js
async function carregarManualMD() {
  const mdUrls = ["data/Manual_WMS_Seguranca.md", "data/Mapa.md"];
  let textoCompleto = "";

  for (const mdUrl of mdUrls) {
    const response = await fetch(mdUrl);
    let markdownText = await response.text();
    textoCompleto += "\n ----- \n" + markdownText ;
  }

  manualText = textoCompleto;
}
```

- Carrega os arquivos `.md` do manual.
    
- Junta tudo em uma única string (`manualText`) que será usada como base para as respostas da IA.
    

---

### 🖼️ 4. **Conversão de imagem para base64**

```js
async function carregarImagemComoBase64(caminho) {
  const response = await fetch(caminho);
  const blob = await response.blob();
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onloadend = () => resolve(reader.result.split(',')[1]);
    reader.onerror = reject;
    reader.readAsDataURL(blob);
  });
}
```

Converte uma imagem (via URL) para base64 — essa função não está sendo usada no script final, mas serve como utilitário.

---

### 💬 5. **Função para enviar mensagem textual**

```js
async function enviarMensagem(mensagemUsuario) {
  historicoMensagens.push({ role: "user", parts: [{ text: mensagemUsuario }] });
  showTypingIndicator();

  const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });
  const chat = model.startChat({
    history: historicoMensagens,
    generationConfig: {
      temperature: 0.7,
    },
  });

  const result = await chat.sendMessage(`
    Use o manual abaixo para responder a dúvidas sobre o sistema WMS da Talent.

    Manual:
    ${manualText}

    Regras:
    - ...
  `);

  const response = await result.response;
  const texto = await response.text();
  hideTypingIndicator(); 
  historicoMensagens.push({ role: "model", parts: [{ text: texto }] });
  addMessage("Talia: " + texto);
}
```

Resumo do que acontece:

1. Adiciona a mensagem do usuário no histórico.
    
2. Mostra o indicador de "digitando".
    
3. Cria o modelo de chat com histórico.
    
4. Envia a pergunta + manual + regras.
    
5. Recebe e mostra a resposta da IA.
    
6. Atualiza o histórico com a resposta.
    

---

### 🧷 6. **Eventos de interface**

```js
window.addEventListener('DOMContentLoaded', () => {
  carregarManualMD(); // Carrega os arquivos .md ao carregar a página

  // Abrir e fechar o chat
  document.getElementById('chatbotBtn').addEventListener('click', () => {
    document.getElementById('chatContainer').style.display = 'flex';
  });

  document.getElementById('closeBtn').addEventListener('click', () => {
    document.getElementById('chatContainer').style.display = 'none';
  });

  // Enviar mensagem
  document.getElementById('sendBtn').addEventListener('click', () => {
    const userInput = document.getElementById('userInput').value.trim();
    if (userInput) {
      addMessage("Você: " + userInput);
      document.getElementById('userInput').value = '';
      enviarMensagem(userInput);
    }
  });
});
```

Esses eventos controlam a **interface do chat** e **o envio de mensagens de texto**.

---

### 💡 7. **Função para adicionar mensagem na interface**

```js
function addMessage(message) {
  ...
}
```

Essa função cria elementos HTML dinamicamente para:

- Adicionar mensagens da Talia ou do usuário no chat.
    
- Estilizar de acordo com o remetente.
    
- Converter URLs em links e imagens Markdown em `<img>` com a função `linkify()`.
    

---

### 🌐 8. **Função `linkify()` para formatar links/imagens**

```js
function linkify(text) {
  ...
}
```

- Converte URLs em links clicáveis.
    
- Converte imagens Markdown (como `![img](caminho.png)`) em elementos `<img>`.
    
- Formata o texto para HTML (ex.: `\n` vira `<br>`).
    

---

### ✏️ 9. **Indicador de "Talia está digitando..."**

```js
function showTypingIndicator() { ... }
function hideTypingIndicator() { ... }
```

Adiciona ou remove um pequeno indicador de digitação da assistente.

---

### 🖼️ 10. **Envio e análise de imagem (print de tela)**

```js
document.getElementById('imageInput').addEventListener('change', async (event) => {
  const file = event.target.files[0];
  ...
  await enviarImagemParaGemini(base64);
});
```

Quando o usuário seleciona uma imagem:

1. Converte a imagem em base64.
    
2. Mostra a imagem no chat.
    
3. Envia para a IA analisar com a função `enviarImagemParaGemini()`.
    

---

### 🧠 11. **Função para a IA analisar a imagem**

```js
async function enviarImagemParaGemini(base64Image) {
  showTypingIndicator();
  const model = genAI.getGenerativeModel({ model: "gemini-2.0-flash" });

  const result = await model.generateContent([
    {
      inlineData: {
        mimeType: "image/png",
        data: base64Image
      }
    },
    {
      text: `
        Analise essa imagem de tela do sistema WMS da Talent.
        ...
      `
    }
  ]);

  const response = await result.response;
  const texto = await response.text();
  hideTypingIndicator();
  historicoMensagens.push({ role: "model", parts: [{ text: texto }] });
  addMessage("Talia: " + texto);
}
```

Essa função:

- Envia a imagem como input para o Gemini.
    
- Adiciona um prompt textual com instruções.
    
- Recebe e exibe a resposta da IA.
    

---

### ✅ Conclusão

Este código implementa um **chatbot interativo** com:

- **Entrada de texto** + resposta da IA com base em manual.
    
- **Entrada de imagem** com análise visual.
    
- Interface bonita, com rolagem automática, e UX bem cuidada.
    

