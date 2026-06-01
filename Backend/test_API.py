import pytest
import pytest_asyncio
from httpx import AsyncClient
from main import app
from bson import ObjectId
from Rutas.usuario import obtener_usuario_actual

pytestmark = pytest.mark.asyncio

async def mock_usuario_actual():
    return "test_v1@ejemplo.com"

# 3. LA FIXTURE (Usamos pytest_asyncio.fixture en lugar de pytest.fixture)
@pytest_asyncio.fixture(scope="function")
async def cliente():
    # Aquí hacemos la magia del "Override" (Sustitución)
    app.dependency_overrides[obtener_usuario_actual] = mock_usuario_actual
    
    async with AsyncClient(app=app, base_url="http://test") as ac:
        yield ac
    
    # Al terminar los tests, quitamos el "doble" para que la app vuelva a la normalidad
    app.dependency_overrides.clear()

# ID genérico para las URLs de los tests
ID_GENERICO = str(ObjectId())
OTRO_USUARIO = "otro@ejemplo.com"

# --- BLOQUE DE TESTS (Todos los endpoints) ---

@pytest.mark.asyncio
async def test_modulo_usuarios_y_perfil(cliente):
    # Registro
    await cliente.post("/usuarios/registro", json={"nombreUsuario": "Test", "correo": "t@t.com", "password": "123"})
    # Perfil
    res = await cliente.get("/perfil/mis-datos")
    assert res.status_code == 200

@pytest.mark.asyncio
async def test_modulo_favores(cliente):
    # Crear favor
    f_res = await cliente.post("/favores/crear", json={"titulo": "Test", "descripcion": "Desc", "puntos": 1})
    assert f_res.status_code == 200
    id_f = f_res.json().get("_id", ID_GENERICO)
    
    # Acciones de favor
    await cliente.put(f"/favores/{id_f}/aceptar")
    await cliente.put(f"/favores/{id_f}/finalizar")
    await cliente.delete(f"/favores/{id_f}/eliminar")

@pytest.mark.asyncio
async def test_modulo_grupos(cliente):
    # Crear grupo
    g = await cliente.post("/grupos/crear", json={"nombreGrupo": "Grupo de Test"})
    assert g.status_code == 200
    id_g = g.json().get("_id", ID_GENERICO)

    # Editar grupo 
    edit = await cliente.patch(f"/grupos/{id_g}/editar", json={"nombreGrupo": "Nombre Editado"})
    assert edit.status_code in [200, 403] 

    # Cambiar rol
    rol = await cliente.patch(f"/grupos/{id_g}/cambiar-rol", params={"correo_miembro": OTRO_USUARIO, "nuevo_rol": "admin"})
    assert rol.status_code in [200, 422, 404]

@pytest.mark.asyncio
async def test_modulo_chats(cliente):
    # Crear chat
    c = await cliente.post("/chat/obtener-o-crear", json={"idReceptor": OTRO_USUARIO})
    assert c.status_code == 200
    id_chat = c.json().get("_id", ID_GENERICO)

    # Enviar mensaje
    m = await cliente.post(f"/chat/{id_chat}/enviar", json={"contenido": "Hola test"})
    assert m.status_code == 200