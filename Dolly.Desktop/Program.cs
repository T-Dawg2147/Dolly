using Dolly.Application.Abstraction;
using Dolly.Desktop.Services;
using Dolly.Desktop.ViewModels;
using Dolly.Desktop.ViewModels.Layout;
using Dolly.Desktop.ViewModels.Tabs;
using Dolly.Desktop.Views;
using Dolly.Infrastructure.Caching;
using Dolly.Infrastructure.Data;
using Dolly.Infrastructure.Export;
using Dolly.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Dolly.Desktop;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices(services =>
            {
                // ---------- Shared state ----------
                services.AddSingleton<SupplierContextState>();
                services.AddSingleton<ISupplierDetailsCache, SupplierDetailsCache>();

                // ---------- Data / domain services ----------
                services.AddSingleton<ISupplierQueriesService, SupplierQueriesService>();
                services.AddTransient<IProductEditService, ProductEditService>();

                // ---------- UI services ----------
                services.AddTransient<IProductEditDialogService, ProductEditDialogService>();
                services.AddTransient<IProductImageResolver, ProductImageResolver>();
                services.AddTransient<IFolderPickerService, FolderPickerService>();
                services.AddTransient<IReportExportService, ReportExportService>();
                services.AddTransient<IPublicationService, PublicationService>();
                services.AddTransient<IUnspscService, UnspscService>();
                services.AddTransient<ISearchService, SearchService>();
                services.AddTransient<ISearchResultsDialogService, SearchResultsDialogService>();
                services.AddTransient<IRelatedProductsService, RelatedProductsService>();
                services.AddTransient<IAddRelatedProductDialogService, AddRelatedProductDialogService>();
                services.AddTransient<IRelatedProductsDialogService, RelatedProductsDialogService>();
                services.AddTransient<IReportParameterPromptService, ReportParameterPromptService>();
                services.AddSingleton<ICurrentUserService, CurrentUserService>();
                services.AddSingleton<IUserDialogService, UserDialogService>();

                // TODO: move to appsettings.json later
                services.AddSingleton<IExcelTemplateExportService>(_ =>
                    new ExcelTemplateExportService(@"O:\Cat1\Sales Reports\Templates\"));

                services.AddTransient<SupplierPreloadService>();

                // ---------- ViewModels: Layout ----------
                services.AddTransient<ProductImageViewModel>();
                services.AddTransient<ProductOverviewPanelViewModel>();
                services.AddTransient<ProductsPaneViewModel>();
                services.AddTransient<SuppliersAndProductsShellViewModel>();

                // ---------- ViewModels: Tabs ----------
                services.AddTransient<MainTabsHostViewModel>();
                services.AddTransient<SupplierDetailsViewModel>();
                services.AddTransient<CarriageDetailsViewModel>();
                services.AddTransient<PurchaseOrderDetailsViewModel>();
                services.AddTransient<AdditionalInfoViewModel>();
                services.AddTransient<ReportsViewModel>();
                services.AddTransient<QualityAuditViewModel>();
                services.AddTransient<SalesOverviewViewModel>();
                services.AddTransient<PageNumbersViewModel>();
                services.AddTransient<UnspscUpdateViewModel>();
                services.AddTransient<ImageUpdatesViewModel>();

                // ---------- Other VMs ----------
                services.AddTransient<SuppliersViewModel>();
                services.AddTransient<ProductEditViewModel>();
                services.AddTransient<SearchResultsViewModel>();
                services.AddTransient<RelatedProductsViewModel>();
                services.AddTransient<AddRelatedProductViewModel>();
                services.AddTransient<MainWindowActionsViewModel>();

                // ---------- Views / windows ----------
                services.AddTransient<SuppliersAndProductsView>();
                services.AddTransient<SearchResultsWindow>();
                services.AddTransient<AddRelatedProductWindow>(); // TODO: Might need to add a "Related Product View" to actually view what is related
                services.AddSingleton<MainWindow>();
            })
            .Build();

        var app = new App();
        app.InitializeComponent();
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        app.Run(mainWindow);
    }
}