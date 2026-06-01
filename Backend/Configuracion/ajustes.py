import os
from dotenv import load_dotenv

load_dotenv()

# Configuracion de Base de Datos
URL_MONGODB = os.getenv("URL_MONGODB")
NOMBRE_BASE_DATOS = os.getenv("NOMBRE_BASE_DATOS")

# Configuracion de JWT
CLAVE_SECRETA_API = os.getenv("JWT_CLAVE")
ALGORITMO_SEGURIDAD = os.getenv("JWT_ALGORITMO")