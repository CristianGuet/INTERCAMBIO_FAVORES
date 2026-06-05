from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
import uvicorn
import os
from dotenv import load_dotenv
from Rutas.usuario import router as router_usuarios
from Rutas.favor import router as router_favores
from Rutas.grupo import router as router_grupos 
from Rutas.chat import router as router_chats 
from Rutas.perfil import router as router_perfil
from Rutas.notificacion import router as router_notificaciones
from Rutas.admin import router as router_admin


load_dotenv()

# Instancia principal de FastAPI
app = FastAPI(
    title="FavorApp API",
    description="Esta es la api de FavorApp",
    version="1.0.0"
)

# Crea la ruta
ruta_imagenes = os.path.join(os.path.dirname(__file__), "Cargas")

# Si la carpeta no existe, se crea
if not os.path.exists(ruta_imagenes):
    os.makedirs(ruta_imagenes)

# Monta la carpeta en la URL /imagenes
app.mount("/imagenes", StaticFiles(directory=ruta_imagenes), name="imagenes")

# Rutas
app.include_router(router_usuarios)
app.include_router(router_favores)
app.include_router(router_grupos)
app.include_router(router_chats)
app.include_router(router_perfil)
app.include_router(router_notificaciones)
app.include_router(router_admin)

# Configuracion de CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # En desarrollo esta permitido todo. En produccion se cierra
    allow_credentials=True,
    allow_methods=["*"],  # Permite GET, POST, PUT, DELETE ...
    allow_headers=["*"],
)

# Punto de entrada para ejecutar el servidor
if __name__ == "__main__":
    # Todo estrictamente desde el .env
    host = os.getenv("APP_HOST")
    puerto = int(os.getenv("APP_PUERTO"))
    modo = os.getenv("MODO_DEBUG")

    # Host 0.0.0.0 permite que otros dispositivos en tu red local vean el servidor
    uvicorn.run("main:app", host=host, port=puerto, reload=modo)