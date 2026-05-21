using Controls.UserDialogs.Maui;
using iAlmacen.Clases;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using SQLite;
using System.Data;
using System.Net;

namespace iAlmacen.Almacen_Refacciones.InventarioR
{
    public partial class InvRefaccion : ContentPage
    {
        public InvRefaccion()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Atras");
        }

        private string CreateDatabase()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(Global.PathCatalogo);
                connection.CreateTableAsync<RegArticulo>();
                return "Database created";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        private int FindNumberRecords()
        {
            try
            {
                var db = new SQLiteConnection(Global.PathCatalogo);
                // this counts all records in the database, it can be slow depending on the size of the database
                var count = db.ExecuteScalar<int>("SELECT Count(*) FROM RegArticulo");
                // for a non-parameterless query
                // var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");
                return count;
            }
            catch (SQLiteException)
            {
                return 0;
            }
        }

        private async void DescargarCatalogo_Clicked(object sender, EventArgs e)
        {
            if (Funciones.ChkConnected() == false)
            {
                await DisplayAlertAsync("Informacion", "Esta opcion require conexion a la red", "OK");
                return;
            }

            var records_tmp = FindNumberRecords();
            if (records_tmp != 0)
            {
                await DisplayAlertAsync("Advertencia", "Ya existen datos, debe primero eliminar los registros descargados.", "OK");
                return;
            }

            var answer = await DisplayAlertAsync("Descargar", "Se descargara el Catalogo mas actual. ¿Desea Continuar?", "Si", "No");
            if (answer == false)
                return;

            CreateDatabase();
            using (UserDialogs.Instance.Loading("Descargando...", null, true, MaskType.Black, null))
            {
                string Parametros = $"0,,,,,false,0";
                HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_InvCatalogo_Articulo");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound) return;
                    string resp = reader.ReadToEnd();
                    DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                    foreach (DataRow r in dt.Rows)
                    {
                        Global.folio_entrada_ = int.Parse(r[0].ToString());
                        InsertUpdateData(new RegArticulo
                        {
                            ID = int.Parse(r[0].ToString().Trim()),
                            CodigoActual = r[1].ToString().Trim(),
                            CodigoAnterior = r[2].ToString().Trim(),
                            Descripcion = r[3].ToString().Trim(),
                            ClaveFamilia = r[4].ToString().Trim(),
                            ClaveLinea = r[5].ToString().Trim(),
                            ClaveGrupo = r[6].ToString().Trim(),
                            desc_familia = r[7].ToString().Trim(),
                            desc_linea = r[8].ToString().Trim(),
                            desc_grupo = r[9].ToString().Trim(),
                            DescMarca = r[10].ToString().Trim(),
                            DescMedida = r[11].ToString().Trim(),
                            DescParte = r[12].ToString().Trim(),
                            existencia = double.Parse(r[13].ToString().Trim()),
                            Fisico = 0,
                            Inventario = "0",
                            Aplicado = "0",
                            Fecha_ = DateTime.Now.ToShortDateString().ToString()
                        });
                        await Task.Delay(1);
                    }
                }
            }

            var records = FindNumberRecords();
            await DisplayAlertAsync("Registros Descargados", records.ToString(), "OK");
        }

        private string InsertUpdateData(RegArticulo data)
        {
            try
            {
                var db = new SQLiteAsyncConnection(Global.PathCatalogo);
                db.InsertAsync(data);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }

        private string FechaCatalogo()
        {
            try
            {
                var db = new SQLiteConnection(Global.PathCatalogo);
                IEnumerable<RegArticulo> foo = db.Query<RegArticulo>("SELECT * FROM RegArticulo LIMIT 1");
                DateTime fecha_ = DateTime.Now;
                foreach (RegArticulo Item_ in foo)
                {
                    fecha_ = DateTime.Parse(Item_.Fecha_);
                    break;
                }
                string valor_ = "";
                valor_ = fecha_.Day.ToString() + "-" + fecha_.Month.ToString() + "-" + fecha_.Year.ToString();
                return valor_;
            }
            catch (SQLiteException)
            {
                return "Sin Datos";
            }
        }

        private int DeleteRecords()
        {
            try
            {
                var db = new SQLiteAsyncConnection(Global.PathCatalogo);
                // this counts all records in the database, it can be slow depending on the size of the database
                var count = db.ExecuteScalarAsync<int>("DROP TABLE IF EXISTS RegArticulo");
                // for a non-parameterless query
                // var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");
                return 0;
            }
            catch (SQLiteException)
            {
                return 1;
            }
        }

        private async void EliminarDatos_Clicked(object sender, EventArgs e)
        {
            var records = FindNumberRecords();
            if (records == 0)
            {
                await DisplayAlertAsync("Informacion", "No hay Datos para eliminar", "OK");
                return;
            }

            if (Funciones.ChkConnected() == false)
            {
                await DisplayAlertAsync("Informacion", "Esta opcion require conexion a la red", "OK");
                return;
            }
            var answer = await DisplayAlertAsync("Advertencia", "Se Eliminaran el Catalogo descargado. ¿Desea Continuar?", "Si", "No");
            if (answer == false)
                return;

            string result = await DisplayPromptAsync("Supervisor", "Ingrese la clave de Autorizacion, para continuar.", "OK", "Cancelar", "Clave de Autorizacion", keyboard: Keyboard.Password);
            if (!string.IsNullOrEmpty(result))
            {
                Verificar_autorizacion(result);
            }
        }

        private async void Verificar_autorizacion(string Clave)
        {
            string clave_aut_ = Clave;

            if (clave_aut_.Trim() == "")
                return;

            double cnivel_autorizacion_ = 0;

            string Parametros = $"{clave_aut_}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]")
                {
                    await DisplayAlertAsync("Advertencia", "Clave Ingresada Incorrecta", "OK");
                    return;
                }
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    cnivel_autorizacion_ = double.Parse(r[1].ToString());
                }
            }

            if (cnivel_autorizacion_ < 1)
            {
                await DisplayAlertAsync("Advertencia", "Nivel de Autorizacion Insuficiente", "OK");
                return;
            }

            DeleteRecords();
            await DisplayAlertAsync("Informacion", "Catalogo Inicializado", "OK");
        }

        private async void CrearPlantillaNueva(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CrearPlantilla());
        }

        private async void Capturar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventariosDisponibles());
        }

        private async void AplicarInvetarios_Clicked(Object sender, EventArgs e)
        {
            await Navigation.PushAsync(new xamlInventariosCerrados());
        }

        private int FindNumberRecords_actualizados()
        {
            try
            {
                var db = new SQLiteConnection(Global.PathCatalogo);
                // this counts all records in the database, it can be slow depending on the size of the database
                var count = db.ExecuteScalar<int>("SELECT Count(*) FROM RegArticulo where Inventario = '1' and Aplicado = 0");
                // for a non-parameterless query
                // var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");
                return count;
            }
            catch (SQLiteException)
            {
                return 0;
            }
        }

        private async void Syncronizar_Clicked(object sender, EventArgs e)
        {
            var records = FindNumberRecords();
            if (records == 0)
            {
                await DisplayAlertAsync("Informacion", "No hay registros descargados", "OK");
                return;
            }

            if (Funciones.ChkConnected() == false)
            {
                await DisplayAlertAsync("Advertencia", "Esta opcion require conexion a la red", "OK");
                return;
            }

            var records_update = FindNumberRecords_actualizados();
            if (records_update == 0)
            {
                await DisplayAlertAsync("Informacion", "No hay nuevos registros a sincronizar", "OK");
                return;
            }

            var answer = await DisplayAlertAsync("Sincronizar", "Los nuevos datos capturados se enviaran al servidor y no podran ser modificados al termino. ¿Desea Continuar?", "Si", "No");
            if (answer == false)
                return;

            string result = await DisplayPromptAsync("Supervisor", "Ingrese la clave de Autorizacion, para continuar.", "OK", "Cancelar", "Clave de Autorizacion", keyboard: Keyboard.Password);
            if (!string.IsNullOrEmpty(result))
            {
                AplicarInventario(result);
            }
        }

        private async void AplicarInventario(string Clave)
        {
            string clave_aut_ = Clave;

            if (clave_aut_.Trim() == "")
                return;

            double cnivel_autorizacion_ = 0;
            string Parametros = $"{clave_aut_}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                if (resp == "[]")
                {
                    await DisplayAlertAsync("Advertencia", "Clave Ingresada Incorrecta", "OK");
                    return;
                }
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    cnivel_autorizacion_ = double.Parse(r[1].ToString());
                }
            }

            if (cnivel_autorizacion_ < 1)
            {
                await DisplayAlertAsync("Advertencia", "Nivel de Autorizacion Insuficiente", "OK");
                return;
            }

            var db = new SQLiteConnection(Global.PathCatalogo);

            using (UserDialogs.Instance.Loading("sincronizando...", null, true, MaskType.Black, null))
            {
                string sResponce = "";
                IEnumerable<RegArticulo> foo = db.Query<RegArticulo>("SELECT * FROM RegArticulo where Inventario = '1' and Aplicado = 0");
                foreach (RegArticulo Item_ in foo)
                {
                    Parametros = $"{Item_.CodigoActual},{Item_.Fisico},{Item_.existencia},{Item_.FechaCapturado},{Item_.Fecha_},{Item_.Usuario_},I";
                    response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_InventarioRefaccion");
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            sResponce = "OK";
                        }
                    }
                }
                Parametros = $"'',0,0,'','',{Global.clave_usuario},A";
                response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_InventarioRefaccion");
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        sResponce = "OK";
                        var count = db.ExecuteScalar<int>("UPDATE RegArticulo set Aplicado = 1 where Inventario = '1' and Aplicado = 0");
                        await DisplayAlertAsync("Informacion", "Datos sincronizados corretamente.", "OK");
                    }
                }
            }
        }
    }
}