from pydantic import Field, field_validator
from typing import List, Optional, Literal
from Modelos.modeloBase import ModeloBase
from Utilidades.formateador import Formateador

# Definir como es un miembro individual dentro del grupo
class MiembroGrupo(ModeloBase):
    correoUsuario: str = Field(..., description="Correo del usuario")
    rol: Literal["admin", "miembro"] = "miembro"

    @field_validator('correoUsuario', mode='before')
    @classmethod
    def correo_a_minusculas(cls, valor):
        return Formateador.conversor_minusculas(valor)

class SolicitudInvitacion(ModeloBase):
    correoUsuario: str
    comentario: Optional[str] = Field(None, max_length=300)
    fechaSolicitud: str = Field(default_factory=Formateador.obtener_fecha_actual)

class PedirInvitacionEntrada(ModeloBase):
    comentario: Optional[str] = Field(None, max_length=300, description="Es opcional el mensaje")

# Crear grupos
class GrupoCreacion(ModeloBase):
    nombreGrupo: str = Field(..., min_length=3, max_length=50)
    descripcion: Optional[str] = "Bienvenidos a nuestro grupo de ayuda"
    fotoGrupo: str = "group_default.png"

# Devuelve el servidor
class GrupoPublico(GrupoCreacion):
    id: str = Field(..., alias="_id")

    # MInimo 1 (el creador del grupo)
    miembros: List[MiembroGrupo] = Field(..., min_items=1)
    solicitudes: List[SolicitudInvitacion] = []
    fechaCreacion: str

# Edicion del grupo
class GrupoEdicion(ModeloBase):
    nombreGrupo: Optional[str] = Field(None, min_length=3, max_length=50)
    descripcion: Optional[str] = None
    fotoGrupo: Optional[str] = None

    model_config = {
        "json_schema_extra": {
            "examples": [
                {
                "nombreGrupo": "",
                "descripcion": "",
                "fotoGrupo": ""
                }
            ]
        }
    }
