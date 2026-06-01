from pydantic import BaseModel, ConfigDict

# Hace mapeo de todos los campos (por ejemplo _id lo pasa a id) automaticamente
class ModeloBase(BaseModel):
    model_config = ConfigDict(populate_by_name=True)