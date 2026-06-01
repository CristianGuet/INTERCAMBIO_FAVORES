from datetime import datetime

class Formateador:
    # Pasar texto a minusculas
    @staticmethod
    def conversor_minusculas(texto):
        if isinstance(texto, str):
            return texto.lower()
        return texto

    # Formato para la fecha actual con este formato Dia/Mes/Año Hora:Minuto:Segundo
    @staticmethod
    def obtener_fecha_actual():
        return datetime.now().strftime("%d/%m/%Y %H:%M:%S")
    