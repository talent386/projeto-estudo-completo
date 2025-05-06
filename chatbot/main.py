# To run this code you need to install the following dependencies:
# pip install google-genai

import base64
import os
from google import genai
from google.genai import types
from fastapi import FastAPI
from pydantic import BaseModel

from fastapi.middleware.cors import CORSMiddleware



app = FastAPI()




app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3002"],  # o domínio da sua interface
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)






class Message(BaseModel):
    text: str

@app.post("/chat")
def chat(message: Message):
    return {"response": f"Você disse: {message.text}"}



def generate():
    client = genai.Client(
        api_key="AIzaSyA96BV2KDavAS3vCjNJPg7IDYgQKSq-IZk"
    )

    files = [
        # Please ensure that the file is available in local system working direrctory or change the file path.
        client.files.upload(file="data\manual_WMS.pdf"),
    ]
    model = "models/gemini-2.5-pro-exp-03-25"
    contents = [
        types.Content(
            role="user",
            parts=[
                types.Part.from_uri(
                    file_uri=files[0].uri,
                    mime_type=files[0].mime_type,
                ),
                types.Part.from_text(text=f"""{pergunta_input}"""),
            ],
        ),
    ]
    generate_content_config = types.GenerateContentConfig(
        response_mime_type="text/plain",
        system_instruction=[
            types.Part.from_text(text="""Considere que você é um solucionador de problemas de um sistema WMS, e com base nos materiais enviados, você vai entender o problema que o cliente apresentar, e vai retornar a ele a solução, contanto se o cliente ainda não tiver sucesso com a solução, você vai indicar que ele contate outro facilitador por meio do wahtzap de suporte """),
        ],
    )

    for chunk in client.models.generate_content_stream(
        model=model,
        contents=contents,
        config=generate_content_config,
    ):
        print(chunk.text, end="")

if __name__ == "__main__":
    generate()
