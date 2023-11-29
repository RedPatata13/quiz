namespace quiz;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
		
		Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
		
		View view = new View();
		Model model = new Model();
		Controller controller = new Controller(view, model);
        Application.Run(view);
    }    
}