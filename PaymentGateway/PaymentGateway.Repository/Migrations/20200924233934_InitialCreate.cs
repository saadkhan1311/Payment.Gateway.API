using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Repository.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    Expiry_Month = table.Column<string>(maxLength: 2, nullable: false),
                    Expiry_Year = table.Column<string>(maxLength: 4, nullable: false),
                    Cvv = table.Column<string>(nullable: false),
                    Card_Holder_Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Secret_Key = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentResponses",
                columns: table => new
                {
                    Transaction_Reference_Id = table.Column<Guid>(nullable: false),
                    Acquirer_Reference_Id = table.Column<Guid>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Amount = table.Column<double>(nullable: false),
                    Card_id = table.Column<Guid>(nullable: false),
                    Processed_On = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentResponses", x => x.Transaction_Reference_Id);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name" },
                values: new object[] { "USD", "US Dollar" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name" },
                values: new object[] { "AUD", "Australian Dollar" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name" },
                values: new object[] { "CAD", "Canadian Dollar" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name" },
                values: new object[] { "INR", "Indian Rupee" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "Name" },
                values: new object[] { "EUR", "Euro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "PaymentResponses");
        }
    }
}
