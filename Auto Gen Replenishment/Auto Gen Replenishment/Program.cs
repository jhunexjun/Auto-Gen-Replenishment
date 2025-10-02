// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

//using AutoGenReplenishment.Classes;
using AutoGenReplenishment.Data;
using AutoGenReplenishment.Models;
using AutoGenReplenishment.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using MigraDoc.DocumentObjectModel;
//using MigraDoc.Rendering;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;


namespace AutoGenReplenishment;

using PdfSharp.Fonts;
//using PdfSharp.Quality;
using MigraDoc.DocumentObjectModel;
//using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Snippets.Font;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (Capabilities.Build.IsCoreBuild)
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

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

        if (company == null)
        {
            Console.WriteLine("Nothing found in the company.");
            return;
        }

        var replenishmentService = scope.ServiceProvider.GetRequiredService<ReplenishmentUnpostedResultService>();
        // var result = replenishmentService.GetReplenishmentUnpostedResultsAsync("28").Result; // or below.
        var result = await replenishmentService.GetReplenishmentUnpostedAsync("100207");

        using var scope3 = host.Services.CreateScope();
        replenishmentService = scope3.ServiceProvider.GetRequiredService<ReplenishmentUnpostedResultService>();
        var dfdsf = await replenishmentService.GetItemsByMinimum();

        if (result == null)
        {
            Console.WriteLine("Nothing found in the result set.");
            return;
        }

        if (result.Lines.Count == 0)
        {
            Console.WriteLine("There's no line results.");
            return;
        }

        using var scope2 = host.Services.CreateScope();
        replenishmentService = scope2.ServiceProvider.GetRequiredService<ReplenishmentUnpostedResultService>();
        string? xferNo = await replenishmentService.CreateReplenishmentAsync(result);
        if (xferNo == null)
        {
            Console.WriteLine("Creating replenishment, unsuccessful.");
        }
        else
        {
            Console.WriteLine($"Document No: {result.Meta.XFER_NO}");
            Console.WriteLine($"From Location: {result.FromLocation.LOC_ID} - {result.FromLocation.Descr}");

            // CreatePdf(company, result);
            GeneratePDF3(company, result);
        }


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

    //private static void CreatePdf(Company company, ReplenishmentUnpostedResultModel replenishments)
    //{
    //    //var filePath = "hello.pdf";

    //    //QuestPDF.Settings.License = LicenseType.Community;


    //    //Document.Create(container =>
    //    //{
    //    //    container.Page(page =>
    //    //    {
    //    //        page.Size(PageSizes.Letter.Landscape());
                
    //    //        page.Margin(1, Unit.Centimetre);
    //    //        page.DefaultTextStyle(x => x.FontSize(20));

    //    //        page.Header()
    //    //            .DefaultTextStyle(TextStyle.Default.FontSize(10))
    //    //           .Column(col =>
    //    //           {
    //    //               col.Item().AlignCenter().PaddingTop(10).Row(row =>
    //    //               {
    //    //                   row.ConstantColumn(160).Image(".\\assets\\images\\pos_hw_transparent.png", ImageScaling.FitArea);
    //    //               });

    //    //               col.Item().AlignCenter().Text("Unposted Replenishment Report")
    //    //                .SemiBold()
    //    //                .FontSize(10)
    //    //                .FontColor(Colors.Black);

    //    //               col.Item().PaddingTop(10).Row(row =>
    //    //               {
    //    //                   row.RelativeItem().Column(left =>
    //    //                   {
    //    //                       left.Item().Text($"{company.Nam}");
    //    //                       left.Item().Text($"{company.Adrs1}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.Adrs2))
    //    //                           left.Item().Text($"{company.Adrs2}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.Adrs3))
    //    //                           left.Item().Text($"{company.Adrs3}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.City) || !string.IsNullOrWhiteSpace(company.State) || !string.IsNullOrWhiteSpace(company.ZipCod))
    //    //                           left.Item().Text($"{company.City}, {company.State} {company.ZipCod}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.Cntry))
    //    //                           left.Item().Text($"Phone: {company.Phone1}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.Phone1))
    //    //                           left.Item().Text($"Contact: {company.Contact1}");
    //    //                       if (!string.IsNullOrWhiteSpace(company.EmailAdrs1))
    //    //                           left.Item().Text($"Email: {company.EmailAdrs1}");
    //    //                   });

    //    //                   row.ConstantItem(200).Column(col =>
    //    //                   {
    //    //                       col.Item().Text(text =>
    //    //                       {
    //    //                           text.Span("Page #: ");
    //    //                           text.CurrentPageNumber();
    //    //                           text.Span(" of ");
    //    //                           text.TotalPages();
    //    //                       });
    //    //                       col.Item().Text("Document #: 28");
    //    //                       col.Item().Text($"Date: {DateTime.Now:MM/dd/yyyy}");
    //    //                       col.Item().Text("Prepared by: Steve");
    //    //                   });
    //    //               });
    //    //           });

    //    //        //page.Content()
    //    //        //    .PaddingVertical(1, Unit.Centimetre)
    //    //        //    .Column(x =>
    //    //        //    {
    //    //        //        x.Item().Text("Hello, World! This PDF was generated in a console app.");
    //    //        //        x.Item().Text("Generated at: " + DateTime.Now);
    //    //        //    });
    //    //        page.Content()
    //    //            .DefaultTextStyle(TextStyle.Default.FontSize(8))
    //    //            .Column(col =>
    //    //            {
    //    //                // your table as-is
    //    //                col.Item().Border(1).PaddingBottom(10).Table(table =>
    //    //                {
    //    //                    table.ColumnsDefinition(columns =>
    //    //                    {
    //    //                        columns.ConstantColumn(60);
    //    //                        columns.ConstantColumn(140);
    //    //                        columns.ConstantColumn(35);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(45);
    //    //                        columns.ConstantColumn(43);
    //    //                        columns.ConstantColumn(43);
    //    //                        columns.ConstantColumn(43);
    //    //                        columns.RelativeColumn(1);
    //    //                    });

    //    //                    table.Header(header =>
    //    //                    {
    //    //                        IContainer CellStyle(IContainer c) => c.Border(1).Padding(3);
    //    //                        header.Cell().Element(CellStyle).Text("Item No");
    //    //                        header.Cell().Element(CellStyle).Text("Description");
    //    //                        header.Cell().Element(CellStyle).Text("Unit");
    //    //                        header.Cell().Element(CellStyle).Text("From Bin-1");
    //    //                        header.Cell().Element(CellStyle).Text("From Bin-2");
    //    //                        header.Cell().Element(CellStyle).Text("From Bin-3");
    //    //                        header.Cell().Element(CellStyle).Text("To Bin-1");
    //    //                        header.Cell().Element(CellStyle).Text("To Bin-2");
    //    //                        header.Cell().Element(CellStyle).Text("To Bin-3");
    //    //                        header.Cell().Element(CellStyle).Text("To Bin-4");
    //    //                        header.Cell().Element(CellStyle).Text("Dim 1");
    //    //                        header.Cell().Element(CellStyle).Text("Dim 2");
    //    //                        header.Cell().Element(CellStyle).Text("Dim 3");
    //    //                        header.Cell().Element(CellStyle).AlignRight().Text("Trx Qty");
    //    //                    });

    //    //                    IContainer Cell(IContainer c) => c.Padding(3);

    //    //                    foreach (var item in replenishments.Lines)
    //    //                    {
    //    //                        table.Cell().Element(Cell).Text(item.ItemNo);
    //    //                        table.Cell().Element(Cell).Text(item.Descr);
    //    //                        table.Cell().Element(Cell).Text(item.StkUnit);
    //    //                        table.Cell().Element(Cell).Text(item.FromBin1);
    //    //                        table.Cell().Element(Cell).Text(item.FromBin2);
    //    //                        table.Cell().Element(Cell).Text(item.FromBin3);
    //    //                        table.Cell().Element(Cell).Text(item.ToBin1);
    //    //                        table.Cell().Element(Cell).Text(item.ToBin2);
    //    //                        table.Cell().Element(Cell).Text(item.ToBin3);
    //    //                        table.Cell().Element(Cell).Text(item.ToBin4);
    //    //                        table.Cell().Element(Cell).Text(item.Dim1);
    //    //                        table.Cell().Element(Cell).Text(item.Dim2);
    //    //                        table.Cell().Element(Cell).Text(item.Dim3);
    //    //                        table.Cell().Element(Cell).AlignRight().Text(item.ReplenishQty.ToString("0.##"));
    //    //                    }
    //    //                });

    //    //                // occupy remaining vertical space on the last page only
    //    //                col.Item().Extend();

    //    //                // render summary ONCE at the very end (therefore on the last page)
    //    //                col.Item().ShowOnce().Element(summary =>
    //    //                {
    //    //                    summary.PaddingTop(6).DefaultTextStyle(TextStyle.Default.FontSize(8)).Row(row =>
    //    //                    {
    //    //                        row.RelativeItem().Column(left =>
    //    //                        {
    //    //                            left.Item().Text("Item count: " + replenishments.Lines.Count);
    //    //                            left.Item().Text("Item count2: " + replenishments.Lines.Count);
    //    //                        });

    //    //                        row.ConstantItem(200).Column(right =>
    //    //                        {
    //    //                            var total = replenishments.Lines.Sum(i => i.ReplenishQty);
    //    //                            right.Item().Text("Transfer total: " + total.ToString("#,#0.##"));
    //    //                            right.Item().Text("Transfer total 2: " + total.ToString("#,#0.##"));
    //    //                        });
    //    //                    });
    //    //                });
    //    //            });

    //    //        //page.Footer()
    //    //        //    .AlignCenter()
    //    //        //    .DefaultTextStyle(TextStyle.Default.FontSize(10))
    //    //        //    .Text(text =>
    //    //        //    {
    //    //        //        text.Span("Page ");
    //    //        //        text.CurrentPageNumber();
    //    //        //        text.Span(" of ");
    //    //        //        text.TotalPages();
    //    //        //    });
    //    //        //page.Footer()
    //    //        //    .DefaultTextStyle(TextStyle.Default.FontSize(8))
    //    //        //    //.ShowOnce()
    //    //        //    .Dynamic(new LastPageFooter(replenishments.Lines));

    //    //        //    row.ConstantItem(200).ShowOnce().Column(col =>
    //    //        //    {
    //    //        //        col.Item().Text(text =>
    //    //        //        {
    //    //        //            text.Span("Transfer total: ");
    //    //        //        });
    //    //        //    });
    //    //        //});
    //    //    });
    //    //})
    //    //.GeneratePdf(filePath);

    //    //Console.WriteLine($"PDF generated at: {filePath}");
    //}

    //private static void GeneratePDF2(Company company, ReplenishmentUnpostedResultModel replenishment)
    //{
    //    string getCityStateZipCode()
    //    {
    //        return company.City + ", " + company.State + ", " + company.ZipCod;
    //    }

    //    // 1. Create the document
    //    var document = new Document();
    //    document.Info.Title = "Unposted Replenishment Report";
    //    document.Info.Subject = "Replenishment Report";
    //    document.Info.Author = "Computant";

    //    // 2. Define styles
    //    var style = document.Styles["Normal"];
    //    // style.Font.Name = "Verdana";
    //    style.Font.Name = "Arial";

    //    document.Styles[StyleNames.Header].ParagraphFormat.Alignment = ParagraphAlignment.Center;
    //    document.Styles[StyleNames.Footer].ParagraphFormat.Alignment = ParagraphAlignment.Center;

    //    // 3. Add a section
    //    var section = document.AddSection();

    //    // 4. Header
    //    //section.Headers.Primary.AddParagraph("Unposted Replenishment Report")
    //        //.Format.Font.Size = 14;
    //    var header = section.Headers.Primary;
    //    header.AddParagraph("Unposted Replenishment Report");
    //    header.Format.Font.Size = 12;
        
    //    var headerTable = header.AddTable();
    //    headerTable.Borders.Width = 1;
    //    headerTable.AddColumn(Unit.FromCentimeter(8));
    //    headerTable.AddColumn(Unit.FromCentimeter(8));
    //    var headerRow = headerTable.AddRow();

    //    var leftnestedTavle = headerRow.Cells[0].Elements.AddTable();
    //    leftnestedTavle.Borders.Width = 1;
    //    leftnestedTavle.AddColumn(Unit.FromCentimeter(8));
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.Nam);
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.Adrs1);
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.Adrs2 ?? "");
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.Adrs3 ?? "");
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(getCityStateZipCode());
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.Phone1 ?? "");
    //    leftnestedTavle.AddRow().Cells[0].AddParagraph(company.EmailAdrs1 ?? "");

    //    // 5. Footer
    //    var footer = section.Footers.Primary.AddParagraph();
    //    footer.AddText("Page ");
    //    footer.AddPageField();
    //    footer.AddText(" of ");
    //    footer.AddNumPagesField();

    //    // 6. Title
    //    section.AddParagraph().AddLineBreak();
    //    var paragraph = section.AddParagraph("Employee Records");
    //    paragraph.Format.Font.Size = 16;
    //    paragraph.Format.Font.Bold = true;
    //    paragraph.Format.SpaceAfter = "1cm";

    //    // 7. Create table
    //    var table = section.AddTable();
    //    table.Borders.Width = 0.5;

    //    // Define columns
    //    table.AddColumn(Unit.FromCentimeter(4)); // Name
    //    table.AddColumn(Unit.FromCentimeter(3)); // Position
    //    table.AddColumn(Unit.FromCentimeter(3)); // Department

    //    // Create header row
    //    var headerDetailRow = table.AddRow();
    //    headerDetailRow.Shading.Color = Colors.LightGray;
    //    headerDetailRow.Cells[0].AddParagraph("Name");
    //    headerDetailRow.Cells[1].AddParagraph("Position");
    //    headerDetailRow.Cells[2].AddParagraph("Department");

    //    // Sample data
    //    string[,] employees = {
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Alice Smith", "Manager", "Sales" },
    //        { "Bob Johnson", "Developer", "IT" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //        { "Charlie Brown", "Designer", "Marketing" },
    //    };

    //    // Add rows
    //    //for (int i = 0; i < employees.GetLength(0); i++)
    //    //{
    //    //    var row = table.AddRow();
    //    //    row.Cells[0].AddParagraph(employees[i, 0]);
    //    //    row.Cells[1].AddParagraph(employees[i, 1]);
    //    //    row.Cells[2].AddParagraph(employees[i, 2]);
    //    //}

    //    // 8. Render and save PDF
    //    var renderer = new PdfDocumentRenderer(true);
    //    renderer.Document = document;
    //    renderer.RenderDocument();
    //    renderer.PdfDocument.Save("Report.pdf");

    //    Console.WriteLine("PDF created successfully!");
    //}

    private static void GeneratePDF3(Company company, ReplenishmentUnpostedResultModel replenishment)
    {
        string getCompanyMeta()
        {
            return string.Concat(company.Nam,
                                string.IsNullOrEmpty(company.Adrs1) ? null : $"\n{company.Adrs1}",
                                string.IsNullOrEmpty(company.Adrs2) ? null : $"\n{company.Adrs2}",
                                string.IsNullOrEmpty(company.Adrs3) ? null : $"\n{company.Adrs3}",
                                string.IsNullOrEmpty(company.Phone1) ? null : $"\n{company.Phone1}",
                                string.IsNullOrEmpty(company.EmailAdrs1) ? null : $"\n{company.EmailAdrs1}");
        }

        Func<string> getDocumentMeta = () => string.Concat($"Transfer out #: {replenishment.Meta.XFER_NO}",
                                                $"\nReplenish Date: {replenishment.Meta.SHIP_DAT.ToShortDateString()}",
                                                $"\nSaved Date: {replenishment.Meta.LST_MAINT_DT.ToString("g")}",
                                                $"\nPrepared by: {replenishment.Meta.SHIP_BY}",
                                                $"\nBatch id: {replenishment.Meta.BAT_ID}",
                                                $"\nPrint date: {DateTime.Today.ToString("g")}");

        Func<string> getFrom = () => string.Concat($"{replenishment.FromLocation.LOC_ID}",
                                                    $"\n{replenishment.FromLocation.Descr}",
                                                    string.IsNullOrEmpty(replenishment.FromLocation.City) ? null : $"\n{replenishment.FromLocation.City}",
                                                    string.IsNullOrEmpty(replenishment.FromLocation.State) ? null : $", {replenishment.FromLocation.State}",
                                                    string.IsNullOrEmpty(replenishment.FromLocation.Zip_Cod) ? null : $", {replenishment.FromLocation.Zip_Cod}");

        Func<string> getTo = () => string.Concat($"{replenishment.ToLocation.LOC_ID}",
                                                    $"\n{replenishment.ToLocation.Descr}",
                                                    string.IsNullOrEmpty(replenishment.ToLocation.City) ? null : $"\n{replenishment.ToLocation.City}",
                                                    string.IsNullOrEmpty(replenishment.ToLocation.State) ? null : $", {replenishment.ToLocation.State}",
                                                    string.IsNullOrEmpty(replenishment.ToLocation.Zip_Cod) ? null : $", {replenishment.ToLocation.Zip_Cod}");


        var document = new Document();
        document.Info.Title = "Unposted Replenishment Report";
        var section = document.AddSection();

        section.PageSetup.Orientation = Orientation.Landscape;
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.DifferentFirstPageHeaderFooter = true;

        // Header
        var header = section.Headers.FirstPage.AddParagraph();
        var headerImage = header.AddImage("assets/images/pos_hw_transparent.png");
        headerImage.Width = "6cm";
        header.AddFormattedText("\nUnposted Replenishment Report", TextFormat.Bold);
        header.Format.Font.Size = 11;
        header.Format.Alignment = ParagraphAlignment.Center;

        //header.Format.SpaceAfter = "1cm";

        // Meta info
        var metaTable = section.AddTable();
        //metaTable.Borders.Width = 0.2;
        metaTable.Format.Font.Size = 8;
        metaTable.AddColumn("22cm");    // To do: Maybe you can use calculations to get the good max size?
        metaTable.AddColumn("5cm");

        var row = metaTable.AddRow();
        //row.Cells[1].Format.Alignment = ParagraphAlignment.Right;
        row.Cells[0].AddParagraph(getCompanyMeta());
        row.Cells[1].AddParagraph(getDocumentMeta());

        section.AddParagraph().AddLineBreak();

        // From / To
        var addrTable = section.AddTable();
        addrTable.AddColumn("22cm");
        addrTable.AddColumn("4cm");
        addrTable.Format.Font.Size = 8;
        var addrRow = addrTable.AddRow();
        addrRow.Cells[0].AddParagraph($"From: {getFrom()}");
        addrRow.Cells[1].AddParagraph($"To: {getTo()}");

        section.AddParagraph().AddLineBreak();

        // Main table
        var table = section.AddTable();
        table.Borders.Width = 0.3;
        table.Format.Font.Size = 8;

        // Define columns
        table.AddColumn("2.5cm"); // Item #
        table.AddColumn("5cm");   // Description
        table.AddColumn("2cm");   // Unit
        table.AddColumn("1.5cm"); // From Bin-1
        table.AddColumn("1.5cm"); // From Bin-2
        table.AddColumn("1.5cm"); // From Bin-3
        table.AddColumn("1.5cm"); // To Bin-1
        table.AddColumn("1.5cm"); // To Bin-2
        table.AddColumn("1.5cm"); // To Bin-3
        table.AddColumn("1.5cm"); // To Bin-4
        table.AddColumn("1.5cm"); // Dim1
        table.AddColumn("1.5cm"); // Dim2
        table.AddColumn("1.5cm"); // Dim3
        table.AddColumn("2cm");   // Transfer Qty

        // Header row
        var headerRow = table.AddRow();
        headerRow.Shading.Color = Colors.LightGray;
        headerRow.HeadingFormat = true;
        headerRow.Format.Font.Bold = true;
        string[] headers = {
            "Item #", "Description", "Unit", "From Bin-1", "From Bin-2", "From Bin-3",
            "To Bin-1","To Bin-2","To Bin-3","To Bin-4","Dim 1","Dim 2","Dim 3","Transfer Qty"
        };
        for (int i = 0; i < headers.Length; i++)
            headerRow.Cells[i].AddParagraph(headers[i]);

        for (int i = 0; i < replenishment.Lines.Count; i++)
        {
            var dataRow = table.AddRow();
            dataRow.Cells[0].AddParagraph(replenishment.Lines[i].ITEM_NO);
            dataRow.Cells[1].AddParagraph(replenishment.Lines[i].Descr);
            dataRow.Cells[2].AddParagraph(replenishment.Lines[i].STK_UNIT);
            dataRow.Cells[3].AddParagraph(replenishment.Lines[i].FromBin1 ?? "");
            dataRow.Cells[4].AddParagraph(replenishment.Lines[i].FromBin2 ?? "");
            dataRow.Cells[5].AddParagraph(replenishment.Lines[i].FromBin3 ?? "");
            dataRow.Cells[6].AddParagraph(replenishment.Lines[i].ToBin1 ?? "");
            dataRow.Cells[7].AddParagraph(replenishment.Lines[i].ToBin2 ?? "");
            dataRow.Cells[8].AddParagraph(replenishment.Lines[i].ToBin3 ?? "");
            dataRow.Cells[9].AddParagraph(replenishment.Lines[i].ToBin4 ?? "");
            dataRow.Cells[10].AddParagraph(replenishment.Lines[i].DIM_1_UPR);
            dataRow.Cells[11].AddParagraph(replenishment.Lines[i].DIM_2_UPR);
            dataRow.Cells[12].AddParagraph(replenishment.Lines[i].DIM_3_UPR);
            dataRow.Cells[13].AddParagraph(replenishment.Lines[i].XFER_QTY.ToString("#,##0.##"));

            dataRow.Cells[13].Format.Alignment = ParagraphAlignment.Right;
        }
            

        // Footer
        section.AddParagraph().AddLineBreak();

        var tabelTotals = section.AddTable();
        tabelTotals.Borders.Width = 0;
        tabelTotals.AddColumn("21.5cm");
        tabelTotals.AddColumn("5cm");
        var tblTotalsRows = tabelTotals.AddRow();
        tblTotalsRows.Cells[0].AddParagraph("Item count: " + replenishment.Lines.Count.ToString("#,###.##"));
        tblTotalsRows.Cells[1].AddParagraph("Transfer total: " + replenishment.Lines.Sum(l => l.XFER_QTY).ToString("#,###.##"));

        tblTotalsRows.Cells[1].Format.Alignment = ParagraphAlignment.Right;

        section.AddParagraph().AddLineBreak();
        section.AddParagraph().AddLineBreak();

        var signature = section.AddTable();
        signature.Borders.Width = 0;
        signature.AddColumn("20.5cm");
        signature.AddColumn("6cm");
        var signatureRows = signature.AddRow();
        signatureRows.Cells[0].AddParagraph("\n__________________________\nPick from location by");
        signatureRows.Cells[1].AddParagraph("\n__________________________\nReplenish to location by");
        signatureRows.Cells[1].Format.Alignment= ParagraphAlignment.Right;

        // Render PDF
        var renderer = new PdfDocumentRenderer()
        {
            Document = document
        };
        renderer.RenderDocument();
        renderer.PdfDocument.Save("Report.pdf");
        renderer.PdfDocument.Close();
        //var renderer = new PdfDocumentRenderer(true);
        //renderer.Document = document;
        //renderer.RenderDocument();
        //renderer.PdfDocument.Save("Report.pdf");
    }
}
