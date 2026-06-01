from pydantic import Field
from typing import Optional
from Modelos.modeloBase import ModeloBase
from Utilidades.formateador import Formateador

class Notificacion(ModeloBase):
    usuarioDestino: str = Field(..., description="Correo del usuario que recibe la notificacion")
    mensaje: str
    tipo: str = Field("info", description="puede ser: info, exito, alerta")
    leida: bool = False
    idReferencia: Optional[str] = None  # ID del favor o grupo relacionado
    fecha: str = Field(default_factory=Formateador.obtener_fecha_actual)