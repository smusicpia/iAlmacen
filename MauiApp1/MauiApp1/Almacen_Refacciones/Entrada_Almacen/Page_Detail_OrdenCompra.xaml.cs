using iAlmacen.Clases;
using iAlmacen.Models;
using iAlmacen.WebApi;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Runtime.CompilerServices;

namespace iAlmacen
{
    public partial class Page_Detail_OrdenCompra : ContentPage
    {
        private int index = 1;
        private int cnivel_limite;

        private string _result;

        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }

        public Page_Detail_OrdenCompra()
        {
            InitializeComponent();
            lbl_index.Text = "Articulo 1 de " + Global.Items_orden_.Count.ToString();
            btn_anterior.IsEnabled = false;

            if (Global.Items_orden_.Count == 0 || Global.Items_orden_.Count == 1)
            {
                btn_siguiente.Text = "Validar";
            }

            lista_items.Steps = Global.Items_orden_.Count;
            lista_items.StepSelected = 1;
        }

        private async void Siguiente_Clicked(object sender, EventArgs e)
        {
            if (btn_siguiente.Text == "Validar")
            {
                Calcular_importes();
                Global.validar = true;
                await Navigation.PopAsync();
                return;
            }
            Validar_posicion("S");
        }

        private void Anterior_Clicked(object sender, EventArgs e)
        {
            Validar_posicion("A");
        }

        //private void Llenar_datos()
        //{
        //    Calcular_importes();
        //}

        private void Calcular_importes()
        {
            Item_orden_compra item_select_ = BindingContext as Item_orden_compra;

            if (item_select_.saldo_ > item_select_.saldo_original_)
            {
                item_select_.saldo_ = item_select_.saldo_original_;
                txt_cantidad.Text = item_select_.saldo_original_.ToString("G");
            }

            double importe_ = 0.00;
            double descuento_ = 0.00;
            double iva_ = 0.00;
            double total_ = 0.00;
            double porcentaje_iva_ = 0.00;

            if (!string.IsNullOrWhiteSpace(txt_cantidad.Text))
            {
                if (txt_cantidad.Text.Trim() == "")
                    item_select_.saldo_ = 0;
            }

            if (!string.IsNullOrWhiteSpace(txt_precio.Text))
            {
                if (txt_precio.Text.Trim() == "")
                    item_select_.precio_item_ = 0;
            }

            if (!string.IsNullOrWhiteSpace(txt_descuento.Text))
            {
                if (txt_descuento.Text.Trim() == "")
                    item_select_.descuento_item_ = 0;
            }

            try
            {
                importe_ = item_select_.saldo_ * item_select_.precio_item_;
            }
            catch (Exception)
            {
                importe_ = 0;
            }
            try
            {
                descuento_ = (item_select_.saldo_ * item_select_.precio_item_) * (item_select_.descuento_item_ / double.Parse("100"));
            }
            catch (Exception)
            {
                descuento_ = 0;
            }

            try
            {
                switch (item_select_.iva_item_)
                {
                    case "16":
                        porcentaje_iva_ = 0.16;
                        iva_ = (importe_ - descuento_) * porcentaje_iva_;
                        break;

                    case "16.0000":
                        porcentaje_iva_ = 0.16;
                        iva_ = (importe_ - descuento_) * porcentaje_iva_;
                        break;

                    case "0":
                        iva_ = 0;
                        break;

                    case "0.0000":
                        iva_ = 0;
                        break;

                    case "Exento":
                        iva_ = 0;
                        break;
                }
            }
            catch (Exception)
            {
                iva_ = 0;
            }

            if (item_select_.gasto_flete_ == true)
            {
                item_select_.retencion_item_ = importe_ * 0.04;
            }
            else
            {
                item_select_.retencion_item_ = 0;
            }
            importe_ = Math.Round(importe_, 4, MidpointRounding.AwayFromZero);
            iva_ = Math.Round(iva_, 4, MidpointRounding.AwayFromZero);
            descuento_ = Math.Round(descuento_, 4, MidpointRounding.AwayFromZero);

            item_select_.subtotal_calculo_ = Math.Round(importe_, 4, MidpointRounding.AwayFromZero);
            item_select_.descuento_calculo_ = Math.Round(descuento_, 4, MidpointRounding.AwayFromZero);
            item_select_.iva_calculo_ = Math.Round(iva_, 4, MidpointRounding.AwayFromZero);
            item_select_.total_calculo_ = Math.Round((importe_ - descuento_) + iva_, 4, MidpointRounding.AwayFromZero);

            item_select_.importe_item_ = importe_;

            lbl_subtotal.Text = importe_.ToString("C");
            lbl_descuento.Text = descuento_.ToString("C");
            lbl_iva.Text = iva_.ToString("C");
            total_ = (importe_ - descuento_) + iva_;
            lbl_total.Text = total_.ToString("C");

            lblSeccion.Text = item_select_.uSeccion;
            lblPasillo.Text = item_select_.uPasillo;
            lblEstanteria.Text = item_select_.uEstanteria;
            lblNivel.Text = item_select_.uNivel;
            lblTarima.Text = item_select_.uTarima;
            lblContenedor.Text = item_select_.uContenedor;

            Validar_cambios();
        }

        private void Validar_posicion(string forma_)
        {
            switch (forma_)
            {
                case "S":
                    index += 1;
                    break;

                case "A":
                    index -= 1;
                    break;
            }

            lista_items.StepSelected = index;
            Validar_controles();
            Calcular_importes();
        }

        private void Validar_controles()
        {
            if (index < Global.Items_orden_.Count)
            {
                btn_siguiente.Text = "Siguiente";
                btn_anterior.IsEnabled = true;
            }

            if (index == Global.Items_orden_.Count)
            {
                btn_anterior.IsEnabled = true;
                btn_siguiente.Text = "Validar";
            }

            if (index == 1)
            {
                btn_anterior.IsEnabled = false;
            }

            lbl_index.Text = "Articulo " + index.ToString() + " de " + Global.Items_orden_.Count.ToString();
        }

        private void Verificar_autorizacion(string clave_aut_)
        {
            if (clave_aut_.Trim() == "")
                return;
            double cnivel_autorizacion_ = 0;
            string cautorizador_ = "";

            string Parametros = $"{clave_aut_}";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "spget_login_autorizacion");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    cnivel_autorizacion_ = double.Parse(dt.Rows[0][1].ToString());
                    cautorizador_ = dt.Rows[0][0].ToString(); ;
                }
            }

            if (cnivel_autorizacion_ < cnivel_limite)
            {
                DisplayAlertAsync("Advertencia", "Nivel de Autorizacion Insuficiente", "OK");
                return;
            }

            Item_orden_compra item_select_ = BindingContext as Item_orden_compra;
            switch (cnivel_limite)
            {
                case 1:
                    item_select_.nivel_autorizado_ = cnivel_limite;
                    item_select_.autorizado_super_ = true;
                    break;

                case 2:
                    item_select_.nivel_autorizado_ = cnivel_limite;
                    item_select_.autorizado_admin_ = true;
                    break;

                case 3:
                    item_select_.nivel_autorizado_ = cnivel_limite;
                    item_select_.autorizado_admin_ = true;
                    break;
            }
            item_select_.autorizador_ = cautorizador_;
            item_select_.Aplica_Autorizacion_ = true;
            fvalidacion.BackgroundColor = Color.FromHex("#41CB6F");
            btn_autorizar.Text = "Autorizado";
        }

        private void Iva_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker_ = (Picker)sender;
            int selectedIndex = cb_iva.SelectedIndex;

            if (selectedIndex != -1)
            {
                cb_iva.Title = (string)picker_.ItemsSource[selectedIndex];
            }
            Calcular_importes();
        }

        private void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            Calcular_importes();
        }

        private void Cambiar_index_TextChanged(object sender, TextChangedEventArgs e)
        {
            index = lista_items.StepSelected;
            BindingContext = Global.Items_orden_[index - 1] as Item_orden_compra;

            Validar_controles();
        }

        private async void Autorizar_Clicked(object sender, EventArgs e)
        {
            if (cnivel_limite == 0)
                return;

            string Titulo = string.Empty;
            switch (cnivel_limite)
            {
                case 1:
                    Titulo = "Supervisor";
                    break;

                case 2:
                    Titulo = "Administrador";
                    break;

                case 3:
                    Titulo = "Limites Superados";
                    break;
            }

            string result = await DisplayPromptAsync(Titulo, "Ingrese la clave de Autorizacion, para validar el limite.", "OK", "Cancelar", "Clave de Autorizacion", -1, keyboard: Keyboard.Password);
            if (!string.IsNullOrEmpty(result))
            {
                Verificar_autorizacion(result);
            }
        }

        private void Validar_cambios()
        {
            Item_orden_compra item_select_ = BindingContext as Item_orden_compra;
            double cdiferencia_ = 0;
            double climite_1 = 0;
            double climite_2 = 0;
            double climite_3 = 0;
            cdiferencia_ = item_select_.precio_item_ - item_select_.precio_original_;
            climite_1 = item_select_.precio_original_ * (Global.tasa_oper / 100);
            climite_2 = item_select_.precio_original_ * (Global.tasa_super / 100);
            climite_3 = item_select_.precio_original_ * (Global.tasa_admin / 100);

            if (cdiferencia_ > climite_3 && item_select_.nivel_autorizado_ < 3)
            {
                cnivel_limite = 3;

                //fvalidacion.BackgroundColor = Color.FromHex("#D61B1B");
                item_select_.autorizado_admin_ = false;
                //btn_autorizar.Text = "Administrador";
                return;
            }

            if (cdiferencia_ > climite_2 && item_select_.nivel_autorizado_ < 2)
            {
                cnivel_limite = 2;
                //fvalidacion.BackgroundColor = Color.FromHex("#D61B1B");
                item_select_.autorizado_super_ = false;
                //btn_autorizar.Text = "Administrador";
                return;
            }

            if (cdiferencia_ > climite_1 && item_select_.nivel_autorizado_ < 1)
            {
                cnivel_limite = 1;
                //fvalidacion.BackgroundColor = Color.FromHex("#D61B1B");
                item_select_.autorizado_super_ = false;
                //btn_autorizar.Text = "Supervisor";
                return;
            }

            //fvalidacion.BackgroundColor = Color.FromHex("#41CB6F");
            item_select_.autorizado_super_ = true;
            item_select_.autorizado_admin_ = true;
            cnivel_limite = 0;
            //btn_autorizar.Text = "Autorizado";
        }

        private void btnUbicacion_Clicked(Object sender, EventArgs e)
        {
            //Result = "0U13U1U0U0";
            BuscarUbicacion();
        }

        private void BuscarUbicacion()
        {
            string Codigo = Result;
            string[] Valores;

            Valores = Result.Split('U');

            // 1 = Estanteria
            // 2 = Nivel
            // 3 = Tarima
            // 4 = Contenedor
            Item_orden_compra item_select_ = BindingContext as Item_orden_compra;
            Boolean UbicacionCorrecta = false;

            string Parametros = "Seccion,Pasillo,Estanteria,Nivel,Tarima,Contenedor,Existencia";
            string Condicion = $"CodigoArticulo='{item_select_.codigo_articulo_.Trim()}' and Estanteria='{Valores[1]}' and Nivel='{Valores[2]}' and Tarima='{Valores[3]}' and Contenedor='{Valores[4]}'";
            HttpWebResponse response = ConfigAPI.GetAPI("GET", "api/Operacion", Parametros, "wsp_execute_qwerty", "CatalogoArticuloUbicacion", Condicion, "SELECT");
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                if (response.StatusCode == HttpStatusCode.NotFound) return;
                string resp = reader.ReadToEnd();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject<DataTable>(resp);
                foreach (DataRow r in dt.Rows)
                {
                    UbicacionCorrecta = true;
                    item_select_.uSeccion = r[0].ToString().Trim();
                    item_select_.uPasillo = r[1].ToString().Trim();
                    item_select_.uEstanteria = r[2].ToString().Trim();
                    item_select_.uNivel = r[3].ToString().Trim();
                    item_select_.uTarima = r[4].ToString().Trim();
                    item_select_.uContenedor = r[5].ToString().Trim();
                    item_select_.uCapturado = true;

                    lblSeccion.Text = item_select_.uSeccion;
                    lblPasillo.Text = item_select_.uPasillo;
                    lblEstanteria.Text = item_select_.uEstanteria;
                    lblNivel.Text = item_select_.uNivel;
                    lblTarima.Text = item_select_.uTarima;
                    lblContenedor.Text = item_select_.uContenedor;
                }
            }

            if (!UbicacionCorrecta)
            {
                item_select_.uSeccion = "";
                item_select_.uPasillo = "";
                item_select_.uEstanteria = "";
                item_select_.uNivel = "";
                item_select_.uTarima = "";
                item_select_.uContenedor = "";
                item_select_.uCapturado = false;

                lblSeccion.Text = item_select_.uSeccion;
                lblPasillo.Text = item_select_.uPasillo;
                lblEstanteria.Text = item_select_.uEstanteria;
                lblNivel.Text = item_select_.uNivel;
                lblTarima.Text = item_select_.uTarima;
                lblContenedor.Text = item_select_.uContenedor;
                DisplayAlertAsync("Advertencia", "Ubicacion Incorrecta", "OK");
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        private new void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}