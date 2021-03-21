using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Gateway.Migrations
{
    public partial class initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardNumber = table.Column<int>(nullable: false),
                    CVV = table.Column<int>(nullable: false),
                    ExpiryYear = table.Column<int>(nullable: false),
                    ExpiryMonth = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    CurrencyISOCode = table.Column<string>(nullable: false),
                    MerchantId = table.Column<Guid>(nullable: false),
                    BankPaymentId = table.Column<long>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
