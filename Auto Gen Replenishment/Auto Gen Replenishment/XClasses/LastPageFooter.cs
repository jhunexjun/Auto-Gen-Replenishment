using AutoGenReplenishment.Models;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace AutoGenReplenishment.Classes
{
    // Fix: Remove generic interface usage, implement non-generic IDynamicComponent
    internal class LastPageFooter : IDynamicComponent
    {
        private List<ReplenishmentUnpostedLineResultModel> _lines;

        public LastPageFooter(List<ReplenishmentUnpostedLineResultModel> replenishments)
        {
            this._lines = replenishments;
        }

        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            if (context.PageNumber != context.TotalPages)
            {
                // Don’t render anything for non-last pages
                return new DynamicComponentComposeResult
                {
                    Content = null,
                    HasMoreContent = false
                };
            }

            // Only render on the last page
            var content = context.CreateElement(element =>
            {
                element.Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("Item count: " + _lines.Count);
                        left.Item().Text("Item count2: " + _lines.Count);
                    });

                    row.ConstantItem(200).Column(right =>
                    {
                        right.Item().Text("Transfer total: " + _lines.Sum(i => i.ReplenishQty).ToString("#,#0.##"));
                        right.Item().Text("Transfer total 2: " + _lines.Sum(i => i.ReplenishQty).ToString("#,#0.##"));
                    });
                });
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
}
