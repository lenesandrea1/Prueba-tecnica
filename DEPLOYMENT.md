# Despliegue gratuito — EventosVivos

Stack recomendado (capa gratuita):

| Componente | Proveedor | Plan |
|------------|-----------|------|
| PostgreSQL | [Neon](https://neon.tech) | Free |
| API .NET | [Render](https://render.com) | Free |
| Angular SPA | [Netlify](https://netlify.com) | Free |

> El plan free de Render **duerme** el servicio tras ~15 min sin tráfico. El primer request puede tardar 30–60 s.

---

## 1. Base de datos (Neon)

1. Crea cuenta en https://neon.tech
2. **New Project** → elige región cercana
3. Copia la connection string **pooled** (modo `postgresql://...`)
4. Asegúrate de que incluya `Ssl Mode=Require` o usa el formato:
   ```
   Host=ep-xxx.region.aws.neon.tech;Database=neondb;Username=xxx;Password=xxx;Ssl Mode=Require
   ```

---

## 2. API en Render

1. Cuenta en https://render.com → **New +** → **Blueprint** o **Web Service**
2. Conecta el repo `lenesandrea1/Prueba-tecnica`
3. Si usas **Blueprint**, Render lee `render.yaml` automáticamente
4. Si creas manualmente:
   - **Runtime**: Docker
   - **Dockerfile path**: `./Dockerfile`
   - **Plan**: Free
   - **Health check path**: `/health`

5. Variables de entorno:

   | Key | Value |
   |-----|-------|
   | `ConnectionStrings__DefaultConnection` | *(connection string de Neon)* |
   | `Cors__AllowedOrigins` | `https://TU-SITIO.netlify.app` *(actualizar tras Netlify)* |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

6. Deploy → anota la URL: `https://eventosvivos-api.onrender.com` (o similar)

7. Verifica: `https://TU-API.onrender.com/health` → `{ "status": "healthy" }`

---

## 3. Frontend en Netlify

1. Cuenta en https://netlify.com → **Add new site** → **Import from Git**
2. Repo: `lenesandrea1/Prueba-tecnica`
3. Netlify detecta `netlify.toml` en la raíz
4. Variable de entorno en **Site settings → Environment variables**:

   | Key | Value |
   |-----|-------|
   | `API_URL` | `https://TU-API.onrender.com/api` |

5. Deploy → URL tipo `https://eventos-vivos.netlify.app`

6. **Vuelve a Render** y actualiza `Cors__AllowedOrigins` con la URL exacta de Netlify (sin `/` final)

7. En Netlify, **Trigger deploy** de nuevo si cambiaste `API_URL`

---

## 4. Verificación end-to-end

1. Abre la URL de Netlify
2. Ve a **Eventos** → debería cargar venues desde la API
3. Crea un evento con fecha futura
4. Reserva entradas → confirma pago en **Reservas**

---

## URLs del proyecto (completar tras desplegar)

| Servicio | URL |
|----------|-----|
| Frontend | `https://____________.netlify.app` |
| API | `https://____________.onrender.com` |
| Swagger | *(solo en Development local)* |

Actualiza estas URLs en el `README.md` principal antes de entregar la prueba.

---

## Problemas frecuentes

**CORS error en el navegador**  
→ Revisa que `Cors__AllowedOrigins` en Render coincida exactamente con la URL de Netlify (https, sin barra final).

**API responde 502 / timeout**  
→ Render free está despertando; espera ~1 min y reintenta.

**Error de conexión a PostgreSQL**  
→ Verifica connection string de Neon y `Ssl Mode=Require`.

**Frontend llama a localhost**  
→ Falta la variable `API_URL` en Netlify o no se redeployó después de configurarla.
