from pydantic import Field, field_validator, model_validator
from typing import Optional, Literal
from Modelos.modeloBase import ModeloBase
from Utilidades.formateador import Formateador

# Modelo para crear favores
class FavorCreacion(ModeloBase):
    titulo: str = Field(..., min_length=5, max_length=100)
    descripcion: str = Field(..., min_length=10)

    # El usuario elige primero la modalidad
    modalidad: Literal["remoto", "presencial"] = Field(...)
    
    # Ubicacion solo sera obligatorio si es "presencial"
    ubicacion: Optional[str] = Field(None, description="Enlace de Google Maps o direccion exacta")
   # Logica de compensacion adaptada
    tipoCompensacion: Literal["favor", "dinero", "ambos"] = Field(...) 
    cantidadDinero: Optional[float] = Field(None, ge=0)
    recompensaFavor: Optional[str] = Field(None, description="Descripcion del favor que pides a cambio")

    @field_validator("tipoCompensacion", "modalidad", mode="before")
    @classmethod
    def normalizar_textos(cls, valor):
        return Formateador.conversor_minusculas(valor)

    # Controlar si es remoto no rellenar ubicacion y si es presencial hay que rellenarlo
    @model_validator(mode="after")
    def validar_logica_completa(self):
        # Validacion de ubicación
        if self.modalidad == "presencial" and not self.ubicacion:
            raise ValueError("Es obligatorio indicar la ubicación para favores presenciales")
        if self.modalidad == "remoto":
            self.ubicacion = None

        # Limpia campos sobrantes dependiendo de tipoCompensacion
        if self.tipoCompensacion == "dinero":
            self.recompensaFavor = None 
            if self.cantidadDinero is None or self.cantidadDinero <= 0:
                raise ValueError("La cantidad de dinero debe ser mayor a 0")
            
        elif self.tipoCompensacion == "favor":
            self.cantidadDinero = None 
            if not self.recompensaFavor:
                self.recompensaFavor = "Favor se acuerda en el chat"

        elif self.tipoCompensacion == "ambos":
            if not self.recompensaFavor or (self.cantidadDinero or 0) <= 0:
                raise ValueError("Para ambos, debes rellenar dinero y texto del favor")
            
        return self
    
# Devuelve el servidor
class FavorPublico(FavorCreacion):
    id: str = Field(..., alias="_id")
    estado: Literal["disponible", "aceptado", "finalizado", "cancelado"] = "disponible"
    fechaCreacion: str
    idUsuarioOfrece: str
    idUsuarioSolicita: Optional[str] = None

# Registrar que se ha cumplido
class RegistroDevolucion(ModeloBase):
    idFavorOriginal: str
    fechaCumplimiento: str = Field(default_factory=Formateador.obtener_fecha_actual)
    comentarios: Optional[str] = "Favor devuelto correctamente"