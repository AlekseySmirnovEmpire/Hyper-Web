using RazorLight;

namespace Server.Services;

public class EmailTemplatingService
{
    private const string BaseEmailTemplatesFolderPath = @"\EmailTemplates";
    private readonly RazorLightEngine _engine;

    public EmailTemplatingService(IHostEnvironment environment)
    {
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(environment.ContentRootPath)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> GenerateHtml(string template, object? model = null)
    {
        var templateName = Path.Combine(BaseEmailTemplatesFolderPath, template) + ".cshtml";
        var result = await _engine.CompileRenderAsync(templateName, model);

        return result;
    }
}