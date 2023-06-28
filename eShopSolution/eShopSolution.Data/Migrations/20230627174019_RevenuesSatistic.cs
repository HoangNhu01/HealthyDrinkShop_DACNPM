using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShopSolution.Data.Migrations
{
    public partial class RevenuesSatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"Create PROCEDURE [dbo].[CalculateRevenue]
	                        @start_date datetime, 
	                        @end_date datetime
                        as
                        BEGIN
                        SET NOCOUNT ON
                            SELECT SUM((od.quantity * (p.price - p.originalprice))) as Value,  FORMAT (o.OrderDate, 'MMM') as Month
                            FROM orderdetails od
                            INNER JOIN products p ON od.productid = p.id
                            INNER JOIN orders o ON od.orderid = o.id
                            WHERE o.status = 3
                            AND o.orderdate >= cast(@start_date as datetime2) AND o.orderdate <= cast(@end_date as datetime2)
	                        Group By FORMAT (o.OrderDate, 'MMM');
                        END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
