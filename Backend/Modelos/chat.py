from pydantic import BaseModel, Field, field_validator, EmailStr
from typing import List, Literal
from Modelos.modeloBase import ModeloBase
from Utilidades.formateador import Formateador

# Estructura de los mensajes individuales
class Mensaje(ModeloBase):
    emisor: EmailStr = Field(..., description="Correo del usuario que envía el mensaje")
    contenido: str = Field(..., min_length=1)
    fechaEnvio: str = Field(default_factory=Formateador.obtener_fecha_actual)
    leido: bool = False

    @field_validator('emisor', mode='before')
    @classmethod
    def emisor_a_minusculas(cls, valor):
        if isinstance(valor, str):
            return valor.lower().strip()
        return valor

# Lo que pide el frontend para crear/obtener un chat
class ChatCreacionRequest(BaseModel):
    participantes: List[EmailStr] = Field(..., min_items=1, description="Lista de correos validos")

# Estructura del Chat en la Base de Datos
class Chat(ModeloBase):
    tipo: Literal["privado", "grupo"] = "privado"  # Diferencia chats privados de grupales
    participantes: List[EmailStr] = Field(..., min_items=1)
    mensajes: List[Mensaje] = []
    ultimaActividad: str = Field(default_factory=Formateador.obtener_fecha_actual)

# Lo que devuelve la API convertido a JSON publico
class ChatPublico(ModeloBase):
    id: str = Field(..., alias="_id")
    tipo: Literal["privado", "grupo"]
    participantes: List[EmailStr]
    mensajes: List[Mensaje] = []
    ultimaActividad: str

class MensajeEntrada(BaseModel):
    contenido: str = Field(..., min_length=1, description="Contenido del mensaje")