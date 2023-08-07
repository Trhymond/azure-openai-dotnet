

[assembly: FunctionsStartup(typeof(Rhymond.OpenAI.Startup))]
namespace Rhymond.OpenAI {
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) {
            // builder.Services.AddAzureServices();
            // builder.Services.AddMemoryStore();
        }
    }
}