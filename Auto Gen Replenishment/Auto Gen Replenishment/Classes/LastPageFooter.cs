using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGenReplenishment.Classes
{
    internal class LastPageFooter : IDynamicComponent
    {
        // public int State { get; set; }

        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(element =>
            {
                if (context.PageNumber == context.TotalPages)
                {
                    element.Text("Last page here");
                }
                else
                {

                }
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false,
            };
        }
    }
}
