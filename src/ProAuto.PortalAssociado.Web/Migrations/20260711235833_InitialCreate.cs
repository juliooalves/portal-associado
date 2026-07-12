using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProAuto.PortalAssociado.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "associados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Placa = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Endereco_Numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Endereco_Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Endereco_Uf = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    Endereco_Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_associados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_associados_Cpf",
                table: "associados",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "associados");
        }
    }
}
