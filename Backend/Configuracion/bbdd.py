from motor.motor_asyncio import AsyncIOMotorClient
from dotenv import load_dotenv 
import os

load_dotenv()
# Configuracion de la URL de conexion
MONGO_URL = os.getenv("URL_MONGODB")

# Crea el cliente asincrono
cliente = AsyncIOMotorClient(MONGO_URL)

# Definir la base de datos principal
BBDD = os.getenv("NOMBRE_BASE_DATOS")
db = cliente[BBDD]

# Accesos directos a las colecciones que son las tablas
coleccion_usuarios = db.usuarios
coleccion_favores = db.favores
coleccion_chats = db.chats
coleccion_grupos = db.grupos
coleccion_notificaciones = db.notificaciones