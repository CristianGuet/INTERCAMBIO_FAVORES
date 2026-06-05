import os
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from dotenv import load_dotenv

load_dotenv()

def enviar_correo_notificacion(usuario_destino: str, asunto: str, contenido_texto: str):
    """
    Envía un correo usando el SMTP de Gmail.
    usuario_destino: correo del destinatario (puede ser Gmail, Outlook, etc.)
    """
    remitente = os.getenv("EMAIL_REMITENTE")
    password = os.getenv("EMAIL_PASSWORD")
    smtp_server = os.getenv("SMTP_SERVER", "smtp.gmail.com")
    smtp_port = int(os.getenv("SMTP_PORT", 587))

    if not remitente or not password:
        print("[SMTP ERROR]: Faltan credenciales en el archivo .env")
        return

    # Crear el mensaje
    msg = MIMEMultipart("alternative")
    msg["Subject"] = asunto
    msg["From"] = remitente
    msg["To"] = usuario_destino

    # Versión texto plano
    parte_texto = MIMEText(contenido_texto, "plain")
    msg.attach(parte_texto)

    # Versión HTML
    contenido_html = f"""
    <!DOCTYPE html>
    <html>
    <head><meta charset="UTF-8"></head>
    <body style="font-family: 'Segoe UI', sans-serif; color: #333333; background-color: #f4f4f7; padding: 20px;">
        <div style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; padding: 20px;">
            <h2 style="color: #4F46E5;">FavorApp</h2>
            <p>{contenido_texto}</p>
            <hr>
            <p style="font-size: 12px; color: #9ca3af;">Este es un correo automático de FavorApp.</p>
        </div>
    </body>
    </html>
    """
    parte_html = MIMEText(contenido_html, "html")
    msg.attach(parte_html)

    # Enviar
    try:
        with smtplib.SMTP(smtp_server, smtp_port) as server:
            server.starttls()
            server.login(remitente, password)
            server.sendmail(remitente, usuario_destino, msg.as_string())
        print(f"[SMTP SUCCESS]: Correo enviado a {usuario_destino}")
    except Exception as e:
        print(f"[SMTP ERROR]: No se pudo enviar a {usuario_destino}. Motivo: {str(e)}")