using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Ai.LanguageModel.OpenAi;

public class GptNano41Provider : GptLanguageModelProvider
{
    public GptNano41Provider(IHttpService httpService, IConfiguration configuration) : base(httpService, configuration)
    {
    }

    protected override string Model => "gpt-4.1-nano";
}