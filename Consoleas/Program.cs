using Consoleas;

class Project
{
    static async Task Main(string[] args)
    {
        string botToken = "6379268126:AAGdaibFUnzdcfxT6Odf-P7OdlJmbrzjemQ";
        Bothandler botH = new Bothandler(botToken);
        try
        {
            await botH.Bothandle();
        }
        catch
        {
            await botH.Bothandle();
        }
    }
}