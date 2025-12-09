using Autofac;
using ElsaMina.Core.Services.Config;
using ElsaMina.Sheets;
using ElsaMina.Sheets.GoogleSheets;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;

namespace ElsaMina.Core.Modules;

public class SheetsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.Register(ctx =>
        {
            var configuration = ctx.Resolve<IConfiguration>();
            using var stream = new FileStream(configuration.SheetsAccessAccountCredentialsFile, FileMode.Open,
                FileAccess.Read);
            var serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(stream);
            var credential = GoogleCredential
                .FromServiceAccountCredential(serviceAccountCredential)
                .CreateScoped(SheetsService.ScopeConstants.Spreadsheets, DriveService.ScopeConstants.DriveReadonly);
            return new GoogleSheetProvider(credential);
        }).As<ISheetProvider>().SingleInstance();
    }
}