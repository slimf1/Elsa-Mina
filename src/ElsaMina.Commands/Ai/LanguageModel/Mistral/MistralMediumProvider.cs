using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Ai.LanguageModel.Mistral;

public class MistralMediumProvider : MistralLanguageModelProvider
{
    public MistralMediumProvider(IHttpService httpService, IConfiguration configuration) : base(httpService, configuration)
    {
    }

    protected override string Model => "mistral-medium-latest";
}