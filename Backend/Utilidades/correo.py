import os
import resend
from dotenv import load_dotenv

load_dotenv()

resend.api_key = os.getenv("RESEND_API_KEY")

def enviar_correo_notificacion(usuario_destino: str, asunto: str, contenido_texto: str):
    """
    Envía una notificación por correo electrónico utilizando la API profesional de Resend
    """
    if not resend.api_key:
        print("[Resend ERROR]: No se ha configurado RESEND_API_KEY en el archivo .env")
        return

    try:
        # Plantilla HTML
        contenido_html = f"""
        <!DOCTYPE html>
        <html>
        <head><meta charset="UTF-8"></head>
        <body style="font-family: 'Segoe UI', sans-serif; color: #333333; background-color: #f4f4f7; padding: 20px; margin: 0;">
            <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; border: 1px solid #e2e8f0; overflow: hidden;">
                <div style="background-color: #4F46E5; padding: 25px; text-align: center;">
                    <h2 style="color: #ffffff; margin: 0; font-size: 24px;">FavorApp</h2>
                </div>
                <div style="padding: 30px;">
                    <p style="font-size: 16px; color: #1f2937;">Hola,</p>
                    <div style="background-color: #f9fafb; border-left: 4px solid #4F46E5; padding: 20px; margin-bottom: 25px; border-radius: 4px;">
                        <p style="font-size: 16px; color: #4b5563; margin: 0; font-style: italic;">
                            "{contenido_texto}"
                        </p>
                    </div>
                    <p style="font-size: 15px; color: #4b5563;">Inicia sesión en la app para gestionar tus grupos.</p>
                </div>
                <div style="background-color: #f9fafb; padding: 15px; text-align: center; border-top: 1px solid #edf2f7;">
                    <p style="font-size: 12px; color: #9ca3af; margin: 0;">Este es un correo automático de FavorApp.</p>
                </div>
            </div>
        </body>
        </html>
        """

        # Configurar los parámetros de envío
        # Si ya verificaste tu dominio, cambia 'onboarding@resend.dev' por 'no-reply@tudominio.com'
        params = {
            "from": f"FavorApp <{os.getenv('EMAIL_REMITENTE')}>",
            "to": [usuario_destino],
            "subject": asunto,
            "text": contenido_texto,
            "html": contenido_html,
        }

        # Lanzar la petición a Resend
        email = resend.Emails.send(params)
        print(f"[Resend SUCCESS]: Correo enviado correctamente. ID: {email.get('id')} -> {usuario_destino}")

    except Exception as e:
        print(f"[Resend ERROR]: Falló el envío hacia {usuario_destino}. Motivo: {str(e)}")