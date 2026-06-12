using UIKit;

namespace iAlmacen.iOS
{
    public class Program
    {
        // This is the main entry point of the application.
        private static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
				UIApplication.Main(args, null, typeof(AppDelegate));
			}
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
				throw new ApplicationException($"Error: {ex.Message}", ex);
			}
        }
    }
}