using Dapper;
using Microsoft.EntityFrameworkCore;
using AutoGenReplenishment.Data;
using AutoGenReplenishment.Models;

namespace AutoGenReplenishment.Services
{
    internal class ReplenishmentUnpostedResultService
    {
        private readonly AppDbContext _dbContext;

        public ReplenishmentUnpostedResultService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ReplenishmentUnpostedResultModel> GetReplenishmentUnpostedResultsAsync(string xferNo)
        {
            using var conn = _dbContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                // "exec dbo.USER_SP_RTDB_ReplenishmentGetUnpostedDocByDocNo @DocNo",
                "exec dbo.USER_SP_RTDB_ReplenishmentGetByXferNo @XFER_NO",
                new { XFER_NO = xferNo });


            var result = new ReplenishmentUnpostedResultModel
            {
                Meta = (await multi.ReadAsync<ReplenishmentUnpostedMeta>()).FirstOrDefault(),
                FromLocation = (await multi.ReadAsync<Location>()).FirstOrDefault(),
                ToLocation = (await multi.ReadAsync<Location>()).FirstOrDefault(),
                // LineCount = (await multi.ReadAsync<int>()).Single(),
                Lines = (await multi.ReadAsync<ReplenishmentUnpostedLineResultModel>()).AsList()
            };

            // Fix: l should be a List<List<ReplenishmentUnpostedLineResultModel>>, not List<List<List<ReplenishmentUnpostedLineResultModel>>>
            //List<ReplenishmentUnpostedLineResultModel> l = new();

            // for (int i = 0; i < result.LineCount; i++)
            //for (int i = 0; i < result.Lines.Count; i++)
            //{
            //    l.Add((await multi.ReadAsync<ReplenishmentUnpostedLineResultModel>()).SingleOrDefault());
            //}

            // result.Lines.AddRange(l);
            Console.WriteLine("Count: " + result.Lines.Count.ToString());

            return result;
        }
    }
}
