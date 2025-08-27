// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using AutoGenReplenishment.Data;
using AutoGenReplenishment.Models;
using AutoGenReplenishment.Services;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Threading.Tasks;
using AutoGenReplenishment.Classes;



namespace AutoGenReplenishment;

internal class Program
{
    static async Task Main(string[] args)
    {
        // string connectionString = "Server=(local);Database=MAUI_dblEntriesissueInReplenish;Integrated Security=True;TrustServerCertificate=True";

        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
                // Register other services if needed
                services.AddScoped<ReplenishmentUnpostedResultService>();
            })
            .Build();

        // Resolve DbContext from DI container
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var company = await db.Companies.ToListAsync().ContinueWith(t => t.Result.FirstOrDefault());

        var replenishmentService = scope.ServiceProvider.GetRequiredService<ReplenishmentUnpostedResultService>();
        // var result = replenishmentService.GetReplenishmentUnpostedResultsAsync("28").Result; // or below.
        var result = await replenishmentService.GetReplenishmentUnpostedResultsAsync("28");

        Console.WriteLine($"Document No: {result.Meta.DocNo}");
        Console.WriteLine($"From Location: {result.FromLocation.Code} - {result.FromLocation.Descr}");

        CreatePdf(company, result);

        // Ensure DB exists
        // db.Database.EnsureCreated();

        //var replenishMin = db.ReplenishmentItemsByMinimums
        //    //.FromSqlInterpolated($"EXEC dbo.[USER_SP_RTDB_ReplenishmentGetAllItemsByMinimum] 'MAIN', 'WAREHOUSE', 0, '*', '*', '*'")
        //    .FromSqlInterpolated($"exec dbo.[USER_SP_RTDB_ReplenishmentGetUnpostedDocByDocNo] '28'")
        //    .ToList();

        //// Create PDF with the retrieved data
        //db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //if (replenishMin.Count > 0)
        //{
        //    Console.WriteLine("Replenishment items retrieved successfully.");
        //    CreatePdf(replenishMin);
        //}
        //else
        //{
        //    Console.WriteLine("No replenishment items found.");
        //}




    }

    private static void CreatePdf(Company company, ReplenishmentUnpostedResultModel replenishments)
    {
        var filePath = "hello.pdf";

        QuestPDF.Settings.License = LicenseType.Community;


        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter.Landscape());
                
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header()
                    .DefaultTextStyle(TextStyle.Default.FontSize(10))
                   .Column(col =>
                   {
                       col.Item().AlignCenter().PaddingTop(10).Row(row =>
                       {
                           row.ConstantColumn(160).Image(".\\assets\\images\\pos_hw_transparent.png", ImageScaling.FitArea);
                       });

                       col.Item().AlignCenter().Text("Unposted Replenishment Report")
                        .SemiBold()
                        .FontSize(10)
                        .FontColor(Colors.Black);

                       col.Item().PaddingTop(10).Row(row =>
                       {
                           row.RelativeItem().Column(left =>
                           {
                               left.Item().Text($"{company.Nam}");
                               left.Item().Text($"{company.Adrs1}");
                               if (!string.IsNullOrWhiteSpace(company.Adrs2))
                                   left.Item().Text($"{company.Adrs2}");
                               if (!string.IsNullOrWhiteSpace(company.Adrs3))
                                   left.Item().Text($"{company.Adrs3}");
                               if (!string.IsNullOrWhiteSpace(company.City) || !string.IsNullOrWhiteSpace(company.State) || !string.IsNullOrWhiteSpace(company.ZipCod))
                                   left.Item().Text($"{company.City}, {company.State} {company.ZipCod}");
                               if (!string.IsNullOrWhiteSpace(company.Cntry))
                                   left.Item().Text($"Phone: {company.Phone1}");
                               if (!string.IsNullOrWhiteSpace(company.Phone1))
                                   left.Item().Text($"Contact: {company.Contact1}");
                               if (!string.IsNullOrWhiteSpace(company.EmailAdrs1))
                                   left.Item().Text($"Email: {company.EmailAdrs1}");
                           });

                           row.ConstantItem(200).Column(col =>
                           {
                               col.Item().Text(text =>
                               {
                                   text.Span("Page #: ");
                                   text.CurrentPageNumber();
                                   text.Span(" of ");
                                   text.TotalPages();
                               });
                               col.Item().Text("Document #: 28");
                               col.Item().Text($"Date: {DateTime.Now:MM/dd/yyyy}");
                               col.Item().Text("Prepared by: Steve");
                           });
                       });
                   });

                //page.Content()
                //    .PaddingVertical(1, Unit.Centimetre)
                //    .Column(x =>
                //    {
                //        x.Item().Text("Hello, World! This PDF was generated in a console app.");
                //        x.Item().Text("Generated at: " + DateTime.Now);
                //    });
                page.Content()
                    .Border(1)
                    .PaddingBottom(10)
                    .DefaultTextStyle(TextStyle.Default.FontSize(8))
                    .Table(table =>
                    {
                        // Define columns
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60); // For spacing
                            columns.ConstantColumn(140); // Description
                            columns.ConstantColumn(35); // Unit
                            columns.ConstantColumn(45); // FromBin1
                            columns.ConstantColumn(45); // FromBin2
                            columns.ConstantColumn(45); // FromBin3
                            columns.ConstantColumn(45); // ToBin1
                            columns.ConstantColumn(45); // ToBin2
                            columns.ConstantColumn(45); // ToBin3
                            columns.ConstantColumn(45); // ToBin4
                            columns.ConstantColumn(43); // Dim1
                            columns.ConstantColumn(43); // Dim2
                            columns.ConstantColumn(43); // Dim3
                            columns.RelativeColumn(1); // TransferQty
                        });
                        // Header row
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Item No");
                            header.Cell().Element(CellStyle).Text("Description");
                            header.Cell().Element(CellStyle).Text("Unit");
                            header.Cell().Element(CellStyle).Text("From Bin-1");
                            header.Cell().Element(CellStyle).Text("From Bin-2");
                            header.Cell().Element(CellStyle).Text("From Bin-3");
                            header.Cell().Element(CellStyle).Text("To Bin-1");
                            header.Cell().Element(CellStyle).Text("To Bin-2");
                            header.Cell().Element(CellStyle).Text("To Bin-3");
                            header.Cell().Element(CellStyle).Text("To Bin-4");
                            header.Cell().Element(CellStyle).Text("Dim 1");
                            header.Cell().Element(CellStyle).Text("Dim 2");
                            header.Cell().Element(CellStyle).Text("Dim 3");
                            header.Cell().Element(CellStyle).AlignRight().Text("Trx Qty");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                     //.PaddingVertical(5)
                                     //.PaddingHorizontal(10)
                                     .Border(1)
                                     .Padding(3);
                                    //.BorderColor(Colors.Black);
                                    //.AlignCenter()
                                    //.AlignMiddle();
                            }
                        });

                        static IContainer CellValueStyle(IContainer container) => container.Padding(3);

                        // Data rows
                        foreach (var item in replenishments.Lines)
                        {
                            table.Cell().Element(CellValueStyle).Text(item.ItemNo);
                            table.Cell().Element(CellValueStyle).Text(item.Descr);
                            table.Cell().Element(CellValueStyle).Text(item.StkUnit);
                            table.Cell().Element(CellValueStyle).Text(item.FromBin1);
                            table.Cell().Element(CellValueStyle).Text(item.FromBin2);
                            table.Cell().Element(CellValueStyle).Text(item.FromBin3);
                            table.Cell().Element(CellValueStyle).Text(item.ToBin1);
                            table.Cell().Element(CellValueStyle).Text(item.ToBin2);
                            table.Cell().Element(CellValueStyle).Text(item.ToBin3);
                            table.Cell().Element(CellValueStyle).Text(item.ToBin4);
                            table.Cell().Element(CellValueStyle).Text(item.Dim1);
                            table.Cell().Element(CellValueStyle).Text(item.Dim2);
                            table.Cell().Element(CellValueStyle).Text(item.Dim3);
                            table.Cell().Element(CellValueStyle).AlignRight().Text(item.ReplenishQty.ToString("0.##"));
                            //table.Cell().AlignRight().Text(item.FromQtyAvailable.ToString("N2"));
                        }
                    });

                //page.Footer()
                //    .AlignCenter()
                //    .DefaultTextStyle(TextStyle.Default.FontSize(10))
                //    .Text(text =>
                //    {
                //        text.Span("Page ");
                //        text.CurrentPageNumber();
                //        text.Span(" of ");
                //        text.TotalPages();
                //    });
                page.Footer()
                    // .Dynamic(new LastPageFooter())
                    .DefaultTextStyle(TextStyle.Default.FontSize(8))
                    .Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(text =>
                            {
                                text.Span("Item count: ");
                            });
                        });

                        row.ConstantItem(200).ShowOnce().Column(col =>
                        {
                            col.Item().Text(text =>
                            {
                                text.Span("Transfer total: ");
                            });
                        });
                    });
            });
        })
        .GeneratePdf(filePath);

        Console.WriteLine($"PDF generated at: {filePath}");
    }
}
