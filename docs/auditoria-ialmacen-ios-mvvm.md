# Auditoría técnica — iAlmacen (.NET MAUI), head de iOS y arquitectura MVVM

**Fecha:** 4 de septiembre de 2026
**Alcance:** solución `iAlmacen.slnx` — proyecto compartido `MauiApp1/MauiApp1/iAlmacen.csproj` (net10.0) y head de iOS `MauiApp1/MauiApp1.iOS/iAlmacen.iOS.csproj` (net10.0-ios, `SupportedOSPlatformVersion` 15.0).
**Naturaleza:** informe de solo lectura. **No se modificó ningún archivo del proyecto.**

---

## 1. Resumen ejecutivo

| Severidad | Hallazgos |
|---|---:|
| Crítico | 9 |
| Alto | 14 |
| Medio | 9 |
| **Total** | **32** |

Cinco conclusiones:

1. **El head de iOS no puede comunicarse con el API tal como está.** La URL base es HTTP en claro (`ConfigAPI.cs:23`) y `Info.plist` no declara `NSAppTransportSecurity`. App Transport Security bloquea la petición antes de que salga del dispositivo, en simulador y en hardware. Todo lo demás en iOS es secundario frente a esto.
2. **MVVM está presente de nombre, no de hecho.** Los 10 code-behind más grandes suman ~7,400 líneas frente a 2,535 de toda la carpeta `ViewModels/`. La lógica de negocio —incluidas transacciones completas de almacén— vive en manejadores de eventos `async void`.
3. **La capa HTTP son dos pilas paralelas e incompatibles.** `ConfigAPI` y `APIService` mantienen cada una su `HttpClient` estático, con timeouts distintos (5 s y 20 s) y manejo de token distinto. La de `APIService` congela el token en el constructor estático, por lo que deja de funcionar en cuanto hay un refresh.
4. **Hay secretos en el historial de git** y las credenciales del usuario se guardan sin cifrar en `Preferences` (`NSUserDefaults` en iOS).
5. **No existe ninguna red de seguridad**: cero proyectos de prueba, cero CI. Cualquier refactor sobre procesos operativos (salidas de almacén, inventarios) se hace hoy a ciegas.

### Comandos para revalidar los conteos

Desde `C:\Proyectos MAPSA\iAlmacen\MauiApp1`:

```bash
grep -rn --include=*.cs -E '\.Result|\.Wait\(\)|GetAwaiter\(\)\.GetResult\(\)' . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 197
grep -rn --include=*.cs 'async void'  . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 116
grep -rn --include=*.cs 'PushAsync'   . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 50
grep -rn --include=*.cs 'GoToAsync'   . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 4
grep -rn --include=*.cs 'Preferences' . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 31
grep -rn --include=*.cs 'MainPage *=' . | grep -v '/obj/' | grep -v '/bin/' | wc -l   # 39
grep -rn --include=*.cs 'SecureStorage' . | grep -v '/obj/' | grep -v '/bin/'          # 1, y está comentada
```

---

## 2. Bloqueadores de iOS

Rutas relativas a `MauiApp1/`.

### C-1 · Crítico — Sin `NSAppTransportSecurity` frente a un API en HTTP claro

`MauiApp1.iOS/Info.plist` (clave ausente) vs `MauiApp1/WebApi/ConfigAPI.cs:23`

```csharp
public static string Servidor = "http://192.168.0.204:8055/";
```

Desde iOS 9, ATS bloquea todo HTTP sin cifrar salvo excepción explícita. No hay ninguna en el plist. **Efecto:** cada llamada falla con `NSURLErrorAppTransportSecurityRequiresSecureConnection`, y como `ConfigAPI.GetAPI` captura solo `WebException` (`ConfigAPI.cs:260`) —no `HttpRequestException`, que es lo que realmente se lanza— la excepción escapa sin manejar.

**Remediación acordada:** añadir la excepción de dominio para la IP LAN.

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsLocalNetworking</key>
    <true/>
    <key>NSExceptionDomains</key>
    <dict>
        <key>192.168.0.204</key>
        <dict>
            <key>NSExceptionAllowsInsecureHTTPLoads</key>
            <true/>
            <key>NSIncludesSubdomains</key>
            <false/>
        </dict>
    </dict>
</dict>
```

> Esto desbloquea iOS hoy, pero el tráfico —incluidas credenciales, ver A-2— sigue viajando sin cifrar por la red interna. La solución correcta a plazo medio es TLS en el servidor y un nombre DNS en lugar de una IP.

### C-2 · Crítico — `UIRequiredDeviceCapabilities = armv7` en una app arm64

`MauiApp1.iOS/Info.plist:19-22`

.NET para iOS genera binarios exclusivamente arm64; armv7 desapareció con iOS 11. Declararlo hace que App Store Connect considere la app incompatible con todo el hardware moderno. **Remediación:** eliminar la clave, o sustituir el valor por `arm64`.

### C-3 · Crítico — Login HTTP síncrono y bloqueante en el constructor de `App`

`MauiApp1/App.xaml.cs:26-34`

```csharp
HttpStatusCode httpStatusCode = Funciones.Login(Global.clave_usuario, Global.pass);
if (httpStatusCode != HttpStatusCode.OK)
{
    MainPage = new LoginView();
}
MainPage = new AppShell();
```

Tres defectos superpuestos:

- `Funciones.Login` es síncrono y bloquea en `.Result` sobre una llamada de red. Corre en el hilo principal durante `FinishedLaunching`. iOS mata la app con el watchdog (`0x8badf00d`) si el arranque tarda demasiado.
- Bloquear el hilo principal impide que se muestre el diálogo de permiso de red local que la propia petición necesita (requiere un run loop vivo).
- **Falta un `return`**: cuando el estado no es `OK` se asigna `LoginView` en la línea 31 y acto seguido la línea 33 lo sobrescribe con `AppShell` incondicionalmente. Un login fallido entra igualmente a la app.

**Remediación:** mover la validación a un arranque asíncrono (`Window.Created` / `OnStart`) mostrando una pantalla de carga, y añadir el `return` que falta.

### C-4 · Crítico — `UIDeviceFamily = [2]` limita la app a iPad

`MauiApp1.iOS/Info.plist:13-16`, con la clave de orientaciones de iPhone presente en `:25-30`, lo que evidencia que la intención era soportar ambos.

**Remediación acordada:** `[1, 2]`. Requiere revisar los layouts diseñados para pantalla grande antes de publicar.

### C-5 · Crítico — Firma de código sin perfil

`MauiApp1.iOS/iAlmacen.iOS.csproj:19-22`

```xml
<ProvisioningType>manual</ProvisioningType>
<CodesignKey>iPhone Developer</CodesignKey>
<CodesignProvision></CodesignProvision>
```

Con aprovisionamiento manual y `CodesignProvision` vacío no hay perfil que seleccionar: la compilación para dispositivo o IPA no puede firmar. Además `Entitlements.plist` está vacío (`<dict/>`) y **ningún csproj declara `CodesignEntitlements`**, así que ni siquiera se incluiría.

### C-6 · Crítico — Paquete nativo de Android referenciado desde iOS

`MauiApp1.iOS/iAlmacen.iOS.csproj:74` y `MauiApp1/iAlmacen.csproj:57`

```xml
<PackageReference Include="SQLitePCLRaw.lib.e_sqlite3.android" Version="2.1.12" />
```

Ese paquete solo contiene binarios `.so` para Android. En el head de iOS no aporta nada y contamina la resolución de assets nativos. Ambas referencias son cambios sin commitear.

### A-1 · Alto — `PrivacyInfo.xcprivacy` incompleto → rechazo en App Store

`MauiApp1.iOS/Resources/PrivacyInfo.xcprivacy:39-48`

El bloque `NSPrivacyAccessedAPICategoryUserDefaults` sigue **comentado** en la plantilla, pese a que la app usa `Preferences` en 31 lugares. Apple exige la declaración con el código de razón `CA92.1`.

### M-1 · Medio — `UIMainStoryboardFile` heredado

`Info.plist:52-53` fija `UIMainStoryboardFile` a `LaunchScreen`, además de `UILaunchStoryboardName` en `:17-18`. Una app MAUI no debe declarar storyboard principal: iOS intenta instanciar un view controller inicial desde el launch screen. Coexisten también dos pantallas de arranque, `LaunchScreen.storyboard` y `LaunchScreen.xib`.

### M-2 · Medio — Versionado invertido y `BuildIpa` en Debug

`Info.plist:9-10` (`CFBundleVersion` 3.0.1) frente a `:54-55` (`CFBundleShortVersionString` 3.0.0): el número de build es mayor que la versión pública. Y `iAlmacen.iOS.csproj:25-29` activa `BuildIpa=True` también en Debug, lo que genera un IPA en cada F5.

### M-3 · Medio — Permiso de red local sin manejo del rechazo

`Info.plist:23-24` declara `NSLocalNetworkUsageDescription`, correcto para acceder a `192.168.x.x`, pero no hay `NSBonjourServices` ni ninguna ruta de código que detecte que el usuario denegó el permiso: las peticiones fallan y el usuario acaba en la pantalla de login sin explicación.

### M-4 · Medio — `Main.cs` oculta el frame real del crash

`MauiApp1.iOS/Main.cs:19` envuelve la excepción nativa en `ApplicationException`, perdiendo el stack original justo donde más se necesita para diagnosticar crashes de arranque.

---

## 3. Seguridad

### C-7 · Crítico — JWT real commiteado en el código fuente

`MauiApp1/Clases/Global.cs:18` — `public static string tokenAPI` tiene como valor por defecto un token HS256 real (usuario `shernan`, ya caducado), presente en el historial de git.

**Remediación:** eliminar el literal **y rotar la clave de firma en el servidor** *(acordado)*. Borrarlo del código no basta: sigue en el historial, y quien tenga el repositorio puede recuperarlo.

### C-8 · Crítico — Bypass total de validación de certificados

`MauiApp1/WebApi/ConfigAPI.cs:83-86`

```csharp
public static bool AceptarTodosLosCertificados(...) => true;
```

Instalado en `ServicePointManager.ServerCertificateValidationCallback` desde **10 sitios** (`ConfigAPI.cs:271, 324, 381, 551, 604, 657`, entre otros). Es un proceso global: anula la validación TLS de toda la aplicación, no solo de esas llamadas. Cualquier man-in-the-middle en la red interna puede interceptar y modificar el tráfico.

### A-2 · Alto — Credenciales y clave AES en la URL, sobre HTTP

`MauiApp1/Handlers/RefreshTokenHandler.cs:112-113`

```csharp
string Parametros = $"{Convert.ToBase64String(SecurityManager.Encrypt(Global.clave_usuario, Global.Key, Global.IV))},{Global.pass}";
request = new HttpRequestMessage(HttpMethod.Get, ConfigAPI.Servidor + $"/api/login/RefreshJWTTokens/?...&parametros={Parametros}");
```

La **contraseña en claro** va en la query string de un GET. Las query strings quedan en logs de acceso del servidor, en proxies y en la caché de `NSURLSession`. Y en `:95-96` la clave y el IV de AES viajan como cabeceras `aes1`/`aes2` junto al texto cifrado que supuestamente protegen — el cifrado no aporta confidencialidad alguna frente a quien vea el tráfico.

### A-3 · Alto — Tokens y contraseña en `Preferences` en vez de `SecureStorage`

`MauiApp1/App.xaml.cs:14-18`, `Handlers/RefreshTokenHandler.cs:132-137`

En iOS `Preferences` es `NSUserDefaults`: texto plano en el contenedor de la app, incluido en backups no cifrados. Ahí se guardan `tokenAPI`, `refreshTokenAPI`, `Guid`, `clave_usuario` y **`Password`**. `SecureStorage` (Keychain) aparece exactamente una vez en todo el repositorio, comentada (`Clases/UnauthorizeInterceptorHandler.cs:12`).

### A-4 · Alto — SQL dirigido desde el cliente

`MauiApp1/WebApi/ConfigAPI.cs:245` construye la URL concatenando `Tabla`, `Condicion`, `Accion` y `Campos`, que llegan como parámetros desde la página. Los endpoints son `api/Operacion/SQL` con métodos `wsp_execute_qwerty` / `ws_fn_EjecutarQuerySQL`. Es decir: **la app móvil decide la tabla, el WHERE, el verbo (`SELECT`/`UPDATE`/`INSERT INTO`/`DELETE`) y las columnas**, y el servidor los ejecuta.

Ejemplo en `Almacen_Refacciones/Salida_Almacen/Almacen_Salidas.xaml.cs:766-887`: lectura de folio, `UPDATE` de folio, `UPDATE` de detalle, `UPDATE` de orden de recolección, `SELECT` e `INSERT INTO` en requisiciones y `DELETE` de detalle, todo con nombres de tabla y condiciones armados en el cliente.

Cualquiera que intercepte el tráfico (trivial: es HTTP en claro) puede reescribir esos parámetros y ejecutar operaciones arbitrarias sobre la base de datos con el token del usuario. **Remediación:** sustituir por endpoints tipados por caso de uso en el servidor; es el trabajo de fondo de la ola 4.

### M-5 · Medio — Contraseña AES literal

`MauiApp1/Clases/SecurityManager.cs:8` — literal comentado, pero presente en el archivo y en el historial.

### M-6 · Medio — Datos de contacto y SMTP embebidos

`MauiApp1/Services/Mail.cs:169` (host SMTP de hosting compartido), `:107, 112, 117` (direcciones de correo fijas, una de ellas personal en Yahoo, usada como destinatario de respaldo).

---

## 4. Arquitectura MVVM

### A-5 · Alto — La lógica de negocio vive en el code-behind

41 archivos `.xaml.cs`. Los diez mayores:

| Líneas | Archivo |
|---:|---|
| 1105 | `Almacen_Refacciones/Salida_Almacen/Almacen_Salidas.xaml.cs` |
| 836 | `Almacen_Refacciones/Salida_Almacen/Almacen_Salidas_Articulos.xaml.cs` |
| 755 | `Almacen_Refacciones/Herramientas_v2/frmCapturaInventarioH.xaml.cs` |
| 680 | `Almacen_Refacciones/Salida_Almacen/Orden_Recoleccion.xaml.cs` |
| 667 | `Almacen_Refacciones/InventarioR/CrearPlantilla.xaml.cs` |
| 535 | `Almacen_Refacciones/InventarioR/CapturaInventario_v2.xaml.cs` |
| 531 | `Almacen_Refacciones/Entrada_Almacen/Page_Head_OrdenCompra.xaml.cs` |
| 525 | `Almacen_Refacciones/InventarioH/frmCapturaPlantillaH.xaml.cs` |
| 449 | `Almacen_Refacciones/Entrada_Almacen/Page_Detail_OrdenCompra.xaml.cs` |
| 357 | `Almacen_Refacciones/Herramientas_v2/frmConsultaH.xaml.cs` |

~7,400 líneas frente a **2,535** de toda la carpeta `ViewModels/`. Los ViewModels funcionan como contenedores de listas (`BindingContext = new ItemsViewModel_X(...)`), no como capa de lógica.

El caso extremo es `Almacen_Salidas.xaml.cs:699-915`: `btnGenerarSalida_Clicked` es una transacción de almacén completa dentro de un manejador `async void`, sin unidad de trabajo ni compensación — si falla a mitad, los `UPDATE` previos ya se aplicaron.

### A-6 · Alto — `INotifyPropertyChanged` a mano, triplicado

No hay referencia a `CommunityToolkit.Mvvm` en ningún `.csproj` (sí a `CommunityToolkit.Maui 14.0.1`, que es la de UI, no la de MVVM). Hay tres implementaciones independientes del mismo boilerplate:

- `ViewModels/BaseViewModel.cs:41-46`
- `ViewModels/BaseObservableObject.cs:63-68` *(archivo nuevo sin commitear)*
- `ViewModels/AIndicatorViewModel.cs:54-59` *(archivo nuevo sin commitear)*

Ninguna usa `[CallerMemberName]`: cada setter pasa `nameof(...)` a mano. Y hay **11 archivos `BaseViewModel_*.cs`** casi idénticos (~47 líneas cada uno) que solo se diferencian en el tipo de la propiedad `DataStore`.

Además, `BaseObservableObject` —pese al nombre— no es una base general: solo contiene cinco propiedades del banner de conectividad, no expone `SetProperty<T>` y ninguna clase existente hereda de ella. `AIndicatorViewModel.cs:42-49` contiene un `Task.Delay(3000)` con el texto `"Prueba"` que nunca vuelve a poner `Activador` en `false`.

**Remediación:** un único `ObservableObject` de CommunityToolkit.Mvvm con `[ObservableProperty]` y `[RelayCommand]`; los 11 `BaseViewModel_*` colapsan en uno genérico.

### A-7 · Alto — La inyección de dependencias está configurada y nunca se usa

`MauiApp1/MauiProgramExtensions.cs:46-58` registra `RefreshTokenHandler`, un cliente con nombre `"api"` con su handler, y `ConfigAPI`, `APIService` y `Funciones` como transitorios.

Sin embargo:

- `IHttpClientFactory` / `CreateClient` tiene **0 usos** en todo el repositorio. El cliente `"api"` y su handler son código muerto; el handler se conecta a mano en `ConfigAPI.cs:44`.
- Los tres tipos registrados son estáticos, así que registrarlos no sirve de nada — se accede como `APIService.PostAPI_...`, nunca por instancia.
- No hay ninguna página ni ViewModel registrado (`AddSingleton<ItemsViewModel_Recoleccion>()` está comentado en `:64`), por lo que **no existe inyección por constructor en ninguna parte**.

### A-8 · Alto — `DependencyService` resuelve siempre al Mock

`ViewModels/BaseViewModel.cs:11` y sus 10 gemelos:

```csharp
public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>() ?? new MockDataStore();
```

Nada llama nunca a `DependencyService.Register<>`. **En producción el almacén de datos es siempre el mock.** Que la app funcione demuestra que esa abstracción no se usa realmente — los datos llegan por las llamadas directas a `ConfigAPI`.

### A-9 · Alto — Shell existe pero la navegación no lo usa

`AppShell.xaml` es un contenedor de flyout con dos `ShellContent`. Estado real:

- **50** `PushAsync` frente a **4** `GoToAsync` (dos de ellos comentados).
- **Ningún `ShellContent` tiene atributo `Route=`.** MAUI genera nombres del tipo `D_FAULT_ShellContent2` a partir de un contador compartido; esos nombres cambian al reordenar páginas, así que no se puede escribir una ruta absoluta estable.
- Los cinco `Routing.RegisterRoute` de `AppShell.xaml.cs:12-17` (`loginview`, `mainview`, `cv_almacen_entradas_ocompra`, `page_head_ordencompra`, `page_detail_ordencompra`) **no los alcanza ningún `GoToAsync`**. `Page_Head_OrdenCompra.xaml.cs:244` navega a su página de detalle con `PushAsync(new Page_Detail_OrdenCompra())` pese a tener la ruta registrada.
- `LoginViewModel.cs:127` navega a `//LoginView`, ruta que no existe: la registrada es `loginview` (relativa) y ningún `ShellContent` la declara. La navegación es un no-op silencioso.
- Los parámetros se pasan por constructor (`new frmListaOrdenesRecoleccion("N")`, `frmSubmenuSolicitudes.xaml.cs:13`) en vez de por query params de Shell con `IQueryAttributable`. En `Almacen_Salidas.xaml.cs:649` se pasa incluso **una instancia de ViewModel** de una página a otra, acoplando sus ciclos de vida.

**Remediación:** dar `Route=` explícito a cada `ShellContent`, registrar las páginas de detalle, y migrar a `GoToAsync` con `ShellNavigationQueryParameters` recibidos vía `IQueryAttributable` en el ViewModel.

---

## 5. Asincronía y estabilidad en iOS

### C-9 · Crítico — `APIService` congela el token de autenticación

`MauiApp1/WebApi/APIService.cs:15-33`

```csharp
static APIService()
{
    _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(20), BaseAddress = new Uri(ConfigAPI.Servidor) };
    ...
    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Global.tokenAPI}");
    _httpClient.DefaultRequestHeaders.Add("RefreshToken", $"Bearer {Global.refreshTokenAPI}");
}
```

El constructor estático corre **una sola vez**, al primer contacto con el tipo, y fija ahí el token. Cuando `RefreshTokenHandler.cs:127` renueva `Global.tokenAPI`, este cliente sigue enviando el viejo: **401 permanentes** en las siete operaciones de `APIService` (guardar inventario, generar salida, firma, números de serie…). Peor aún, si el tipo se toca antes del login, la cabecera queda con el JWT caducado de `Global.cs:18`.

Este cliente tampoco lleva el `RefreshTokenHandler`, así que no tiene ningún manejo de 401.

Defectos menores en el mismo archivo: `:40` concatena `ConfigAPI.Servidor` (absoluta, terminada en `/`) sobre un cliente que ya tiene `BaseAddress` igual, produciendo `http://...//api/...`, mientras `:55` usa ruta relativa; y cada `catch` envuelve todo en `ApplicationException` (`:46`), perdiendo el tipo `HttpRequestException` que es justamente donde iOS reporta los fallos de ATS y red local.

### A-10 · Alto — 197 llamadas bloqueantes sobre red

`.Result`, `.Wait()` y `GetAwaiter().GetResult()` sobre operaciones HTTP, muchas en el hilo de UI. Ejemplos representativos:

| Archivo:línea | Llamada |
|---|---|
| `App.xaml.cs:28` | login completo en el constructor de `App` |
| `ViewModels/ItemsViewModel_Inventario.cs:60-61, 146, 150, 153` | `ConfigAPI.GetAPI(...).Result` + `ReadAsStreamAsync().Result` |
| `Handlers/RefreshTokenHandler.cs:119` | `.Result` **dentro** de un `DelegatingHandler` async |
| `Almacen_Salidas.xaml.cs:294, 390, 915` | consultas y POST de firma |
| `frmCapturaInventarioH.xaml.cs:615, 743` | generación de series y guardado |

Con `Timeout = 5s` en `ConfigAPI.cs:51` y `20s` en `APIService.cs:20` —inconsistentes entre sí—, la interfaz se congela hasta ese tiempo en cada operación.

### A-11 · Alto — `ObservableCollection` mutada fuera del hilo de UI

`ViewModels/ItemsViewModel_Inventario.cs:57, 92, 152, 163` y equivalentes en el resto de ViewModels: `Items.Clear()` y `Items.Add(...)` desde continuaciones que no están marshalladas al hilo principal. En Android suele pasar desapercibido; **en iOS mutar la colección enlazada a un `CollectionView` desde otro hilo es un crash nativo**, difícil de reproducir y de diagnosticar.

### A-12 · Alto — `async void` en métodos que no son manejadores de eventos

116 ocurrencias en total. Las problemáticas son las que no son handlers de XAML —una excepción ahí no se puede capturar y termina el proceso:

`Almacen_Salidas.xaml.cs:290` y `:386` (`buscar_datos`), `Page_Head_OrdenCompra.xaml.cs:71`, `LoginViewModel.cs:124` (`OnLoginClicked`), y `Verificar_autorizacion` en seis archivos (`frmResguardosEmpleadosCerrados.xaml.cs:104`, `frmArticulosCerradosH.xaml.cs:74`, `frmCapturaPlantillaH.xaml.cs:426`, `CrearPlantilla.xaml.cs:485`, `InvRefaccion.xaml.cs:190`, `xamlArticulosCerrados.xaml.cs:117`), más `InvRefaccion.xaml.cs:292` (`AplicarInventario`).

Se suman siete de los ocho `MainThread.BeginInvokeOnMainThread` que reciben una lambda `async` —`async void` encubierto—, entre ellos `RefreshTokenHandler.cs:80-83`, que además llama a `Shell.Current.GoToAsync` cuando `Shell.Current` puede ser `null` (si `MainPage` es un `LoginView` suelto y no un `AppShell`). `MainThread.InvokeOnMainThreadAsync` no se usa nunca.

### A-13 · Alto — `RefreshTokenHandler` no cumple su función

`MauiApp1/Handlers/RefreshTokenHandler.cs`

- **Nunca reintenta la petición original.** Tras un refresh exitoso (`:68`), el flujo cae hasta `:89` y devuelve la respuesta 401 original. El reintento está comentado en `:73-75`. El refresh funciona, pero el usuario ve el error igual.
- El auto-refresh está **desactivado por defecto** y depende de un checkbox de la pantalla de login: `Global.cs:21` (`isAutoRefreshToken = false`) y `LoginView.xaml.cs:106`. La guarda de `:30` deja el handler inerte salvo que el usuario marque la casilla.
- `:110` crea un `HttpClient` nuevo por intento y nunca lo libera — en iOS eso filtra objetos `NSUrlSession` y sockets.
- `:116` `EnsureSuccessStatusCode()` lanza antes de la comprobación `NotFound` de `:121`, que es código muerto; y la excepción escapa del handler hacia el llamante.
- `:119` bloquea con `.Result` dentro de un método async.
- La guarda de concurrencia (`SemaphoreSlim`, `_isRefreshing`) está **comentada** en `:14-15` y `:32-56`: dos 401 simultáneos disparan dos refrescos en paralelo.

### M-7 · Medio — `double.Parse` sin cultura

`ViewModels/ItemsViewModel_Inventario.cs:108, 114, 168-170`. En un dispositivo con locale de coma decimal (es-ES, fr-FR, de-DE) lanza `FormatException`; el `catch` de `:128`/`:176` la traduce en expulsar al usuario a la pantalla de login, sin mensaje. Usar `CultureInfo.InvariantCulture` y `TryParse`.

### M-8 · Medio — `Application.MainPage` obsoleto

Asignado en 39 lugares. Está marcado `[Obsolete]` en .NET 9/10 y en camino de eliminarse; el reemplazo es `Application.Current.Windows[0].Page`. Genera 39 advertencias de compilación.

---

## 6. Higiene del repositorio y build

### A-14 · Alto — Copia duplicada obsoleta del proyecto en la raíz

`iAlmacen/` — **4,453 archivos** sin seguimiento: una copia completa y antigua de la solución con **cinco** heads (incluye `.Mac` y `.WinUI`, que no existen en la solución real), código fuente de marzo (51 archivos `.cs` frente a 141 en el proyecto vivo), más `.vs/` con archivos `.suo` e índices semánticos de Copilot. No está en `.gitignore` ni referenciada por `iAlmacen.slnx`.

Riesgo concreto: editar por error el archivo equivocado, o commitear 4,453 archivos de basura. **Acordado: eliminar.** Añadir además `.vs/` y `Server/` al `.gitignore`.

También existe `Server/02b42714aadedd745609be5a3c1da0aa90b1fd2e79f760fff169e8c590ecc2fe/MauiApp1/`, un árbol vacío del build remoto Windows→Mac filtrado a la raíz del repositorio.

### M-9 · Medio — Gestión de paquetes dispersa

- Desalineación de versiones en el proyecto compartido: `Microsoft.Maui.Controls 10.0.41` (`iAlmacen.csproj:51`) frente a `Microsoft.Maui.Controls.Compatibility 10.0.50` (`:53`). Deben ir a la par.
- No hay `Directory.Packages.props` ni `Directory.Build.props`: las versiones se repiten en cada csproj y divergen sin aviso.
- `SkiaSharp 2.88.9` en el head de iOS, sin el paquete de assets nativos correspondiente.
- Cuatro archivos excluidos de la compilación en `iAlmacen.csproj:33-36` y `:40-42` (`almacen_entradas_compra`, `Almacen_Salidas_Captura`, `UnauthorizeInterceptorHandler`, `BarcodeScannerPage`): código muerto que conviene borrar en lugar de excluir.
- Se usan **dos serializadores JSON** en paralelo: `System.Text.Json` en `APIService` y `Newtonsoft.Json` en `ConfigAPI` y `RefreshTokenHandler`.
- Los payloads se modelan como `System.Data.DataTable` serializado a JSON, con acceso posicional a columnas (`r[16]`, `r[1]` en `ItemsViewModel_Inventario.cs`). Es frágil ante cualquier cambio de orden de columnas en el servidor y hostil a trimming/AOT.

---

## 7. Pruebas y CI

**No existe ningún proyecto de prueba.** `iAlmacen.slnx` contiene exactamente tres proyectos; no hay referencia a xUnit, NUnit, MSTest ni `Microsoft.NET.Test.Sdk` en ninguno de los ocho `.csproj` del repositorio (incluida la copia duplicada). No hay ningún workflow de CI: `.github/` solo contiene `copilot-instructions.md`.

**Recomendación (aprobada):** crear `iAlmacen.Tests` (net10.0, xUnit) **antes** de acometer el refactor MVVM. Los procesos afectados —salidas de almacén, inventarios físicos— son operativos: una regresión ahí tiene coste real. Los primeros candidatos a cubrir son la lógica que hoy está atrapada en `Almacen_Salidas.xaml.cs` y `frmCapturaInventarioH.xaml.cs`, a medida que se extraiga a ViewModels, más la capa de servicios y el manejo de tokens.

---

## 8. Hoja de ruta priorizada

### Ola 1 — Desbloquear iOS (horas)

Sin dependencias, bajo riesgo, todo en `Info.plist` y `iAlmacen.iOS.csproj`:

1. Añadir la excepción ATS para `192.168.0.204` *(acordado)* — C-1
2. Quitar `UIRequiredDeviceCapabilities = armv7` — C-2
3. `UIDeviceFamily` → `[1, 2]` *(acordado; revisar layouts de iPad)* — C-4
4. Quitar `UIMainStoryboardFile` y una de las dos pantallas de arranque — M-1
5. Quitar `SQLitePCLRaw.lib.e_sqlite3.android` de ambos csproj — C-6
6. Configurar `CodesignProvision` y `CodesignEntitlements`; quitar `BuildIpa` de Debug — C-5, M-2
7. Descomentar el bloque `NSPrivacyAccessedAPICategoryUserDefaults` — A-1

### Ola 2 — Seguridad y arranque (días)

8. Rotar la clave de firma del JWT en el servidor y quitar el literal de `Global.cs:18` *(acordado)* — C-7
9. Eliminar `AceptarTodosLosCertificados` y sus 10 instalaciones — C-8
10. Migrar tokens, `Guid` y contraseña a `SecureStorage` — A-3
11. Pasar el refresh de token a POST con cuerpo, sin credenciales en la URL — A-2
12. Mover el login fuera del constructor de `App` a un arranque asíncrono, y añadir el `return` que falta — C-3
13. Corregir `APIService`: cabecera de autorización por petición y el `RefreshTokenHandler` en la cadena — C-9
14. Arreglar `RefreshTokenHandler`: reintento real, `SemaphoreSlim`, sin `HttpClient` por intento, sin `.Result` — A-13

### Ola 3 — Red de seguridad e higiene (días)

15. Eliminar `iAlmacen/` y `Server/`; ampliar `.gitignore` *(acordado)* — A-14
16. Crear `iAlmacen.Tests` (xUnit) — sección 7
17. Añadir `Directory.Packages.props` y alinear las versiones de MAUI — M-9
18. Borrar los cuatro archivos excluidos de la compilación — M-9

### Ola 4 — MVVM de verdad (semanas)

19. Añadir `CommunityToolkit.Mvvm`; un único `BaseViewModel` con `[ObservableProperty]`/`[RelayCommand]`; colapsar los 11 `BaseViewModel_*` — A-6
20. DI real: registrar páginas y ViewModels, consumir `IHttpClientFactory`, eliminar los tipos estáticos y `DependencyService` con Mock — A-7, A-8
21. Sustituir el SQL dirigido desde el cliente por endpoints tipados por caso de uso, y `DataTable` por DTOs — A-4, M-9
22. Migrar página por página, empezando por `Almacen_Salidas` y `Almacen_Salidas_Articulos`: extraer la lógica a ViewModels con `async`/`await`, marshalling correcto al hilo de UI, y navegación Shell con `Route=`, `GoToAsync` e `IQueryAttributable` — A-5, A-9, A-10, A-11, A-12

---

## Anexo — Metodología

Análisis estático de solo lectura sobre el árbol de trabajo en el commit `d99e7d9` más los cambios sin commitear presentes al momento de la auditoría. Las citas críticas (`Info.plist`, `App.xaml.cs`, `APIService.cs`, `ConfigAPI.cs`, `RefreshTokenHandler.cs`, `Global.cs`, `iAlmacen.slnx`, `PrivacyInfo.xcprivacy`, `iAlmacen.iOS.csproj`) se verificaron leyendo los archivos completos; los conteos agregados son reproducibles con los comandos de la sección 1. No se ejecutó la aplicación ni se compiló la solución, por lo que no se listan advertencias del compilador ni hallazgos que requieran ejecución (fugas de memoria en uso real, rendimiento medido, cobertura).
