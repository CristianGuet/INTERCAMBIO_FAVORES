from pydantic import EmailStr, Field
from typing import Optional
from Modelos.modeloBase import ModeloBase

# Crear usuarios
class UsuarioCreacion(ModeloBase):
    correo: EmailStr
    contrasenia: str = Field(..., min_length=6)
    nombreUsuario: str = Field(..., min_length=3, max_length=40)
    descripcion: Optional[str] = "Me encanta esta aplicacion, es fantastica"
    fotoPerfil: Optional[str] = "default.png"

# Devuelve el servidor
class UsuarioPublico(ModeloBase):
    id: str = Field(..., alias="_id") # Mapear el _id de Mongo a "id" en base.py
    correo: EmailStr
    nombreUsuario: str
    descripcion: str
    fotoPerfil: str
    calificacionMedia: Optional[float] = Field(None, ge=1.0, le=5.0)
    rol: str

class UsuarioEdicion(ModeloBase):
    nombreUsuario: Optional[str] = Field(None, min_length=3, max_length=40)
    descripcion: Optional[str] = None
    fotoPerfil: Optional[str] = None

class Valoracion(ModeloBase):
    puntuacion: float = Field(..., ge=1.0, le=5.0)