using System.Data;

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

        public async Task<IEnumerable<ReplenishmentItemsByMinimum>> GetItemsByMinimum()
        {
            using var conn = _dbContext.Database.GetDbConnection();
            await conn.OpenAsync();

            /*
                @fromLocationId as varchar(50),
	            @toLocationId as varchar(50),
	            @calcMethod as smallint,
	            @vendorIds as VARCHAR(MAX),
	            @categoryIds as VARCHAR(MAX),
	            @subcategoryIds as VARCHAR(MAX)
             */

            using var multi = await conn.QueryMultipleAsync(
                // "exec dbo.USER_SP_RTDB_ReplenishmentGetUnpostedDocByDocNo @DocNo",
                "exec dbo.USER_SP_RTDB_ReplenishmentGetAllItemsByMinimum @fromLocationId, @toLocationId, @calcMethod, @vendorIds, @categoryIds, @subcategoryIds",
                new {
                    fromLocationId = "WAREHOUSE",
                    toLocationId = "MAIN",
                    calcMethod = 0,
                    vendorIds = "*",
                    categoryIds = "*",
                    subcategoryIds = "*"
                });

            var result = await multi.ReadAsync<ReplenishmentItemsByMinimum>();
            return result;
        }

        public async Task<ReplenishmentUnpostedResultModel> GetReplenishmentUnpostedAsync(string xferNo)
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

        public async Task<string> CreateReplenishmentAsync(ReplenishmentUnpostedResultModel replenishment)
        {
            using var conn = _dbContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FROM_LOC_ID", replenishment.Meta.FROM_LOC_ID);
                parameters.Add("@TO_LOC_ID", replenishment.Meta.TO_LOC_ID);
                parameters.Add("@LIN_CNT", replenishment.Lines.Count);
                parameters.Add("@SHIP_DAT", replenishment.Meta.SHIP_DAT);
                parameters.Add("@COMMNT_1", replenishment.Meta.COMMNT_1);
                parameters.Add("@COMMNT_2", replenishment.Meta.COMMNT_2);
                parameters.Add("@COMMNT_3", replenishment.Meta.COMMNT_3);
                parameters.Add("@BAT_ID", replenishment.Meta.BAT_ID);
                parameters.Add("@USR_calcMode", replenishment.Meta.CalcMode);
                parameters.Add("@SHIP_BY", replenishment.Meta.SHIP_BY);
                parameters.Add("@XFER_NO", dbType: DbType.String, size: 10, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(
                    "USER_SP_RTDB_replenishmentSaveOUT_11_Hdr",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var xferNo = parameters.Get<string>("@XFER_NO");

                for (int i = 0; i < replenishment.Lines.Count; i++)
                {
                    var l = replenishment.Lines[i];
                    parameters = new DynamicParameters();
                    parameters.Add("@XFER_NO", xferNo);
                    parameters.Add("@XFER_LIN_SEQ_NO", i++);
                    parameters.Add("@ITEM_NO", l.ITEM_NO);
                    parameters.Add("@XFER_QTY", l.XFER_QTY);
                    parameters.Add("@DIM_1_UPR", l.DIM_1_UPR);
                    parameters.Add("@DIM_2_UPR", l.DIM_2_UPR);
                    parameters.Add("@DIM_3_UPR", l.DIM_3_UPR);
                    
                    await conn.ExecuteAsync(
                        "USER_SP_RTDB_replenishmentSaveOUT_12_Lines",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }

                parameters = new DynamicParameters();
                parameters.Add("@XFER_NO", xferNo);

                await conn.ExecuteAsync(
                    "USER_SP_RTDB_replenishmentSaveOUT_13_Total",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                await transaction.CommitAsync();

                return xferNo;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }            
        }
    }
}
