using iAlmacen.Models;
using System.Globalization;

namespace iAlmacen.Converters
{
    public class AlternateColorConverter : IValueConverter
    {
        public Color EvenColor { get; set; } = Color.FromArgb("#F0F0F0");
        public Color OddColor { get; set; } = Colors.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Item_Inventario item)
            {
                return item.id % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_ResgEmpleado itemR)
            {
                return itemR.id % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_InventarioDetalle itemD)
            {
                return itemD.ID % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_RegArticulo itemA)
            {
                return itemA.index % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_entrada_vigilancia itemV)
            {
                return itemV.index % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_Virtual_Recoleccion itemVR)
            {
                return itemVR.index % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_ArticuloSalida artsal)
            {
                return artsal.index % 2 == 0 ? EvenColor : OddColor;
            }
            else if (value is Item_ArticuloEnResguardo artResg)
            {
                return artResg.index % 2 == 0 ? EvenColor : OddColor;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Implement your conversion logic here
            throw new NotImplementedException("AlternateColorConverter.ConvertBack");
        }
    }
}