using System.ComponentModel.DataAnnotations;

namespace iAlmacen.Models
{
	public class UserLogin
	{
		[Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
		public string Username { get; set; }
		[Required(ErrorMessage = "La contraseña es obligatoria.")]
		public string Password { get; set; }
	}
}
