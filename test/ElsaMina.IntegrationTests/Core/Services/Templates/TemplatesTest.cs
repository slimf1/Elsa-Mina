using ElsaMina.Core.Services.Templates;

namespace ElsaMina.IntegrationTests.Core.Services.Templates;

public class TemplatesTest
{
    [Test]
    public void Test_TemplatesManager_ShouldCompileTemplatesWithoutErrors()
    {
        var templateService = new TemplatesManager();
        Assert.DoesNotThrowAsync(async () => await templateService.CompileTemplatesAsync());
    }
}