using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Http;

namespace ElsaMina.Commands.Ai.LanguageModel.Google;

public class Gemini25FlashProvider : GeminiLanguageModelProvider
{
    public Gemini25FlashProvider(IConfiguration configuration, IHttpService httpService) : base(configuration, httpService)
    {
    }

    protected override string Model => "gemini-2.5-flash";
}