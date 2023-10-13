using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAttributeDetails",
                table: "ProductAttributeDetails");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductAttributeDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAttributeDetails",
                table: "ProductAttributeDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeDetails_ProductId",
                table: "ProductAttributeDetails",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAttributeDetails",
                table: "ProductAttributeDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeDetails_ProductId",
                table: "ProductAttributeDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductAttributeDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAttributeDetails",
                table: "ProductAttributeDetails",
                columns: new[] { "ProductId", "AttributeDetailId" });
        }
    }
}
