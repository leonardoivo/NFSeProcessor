using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NFSeProcessor.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotaFiscal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CNPJPrestador = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    CNPJTomador = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DescricaoServico = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaFiscal", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotaFiscal_Numero",
                table: "NotaFiscal",
                column: "Numero",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotaFiscal");
        }
    }
}
