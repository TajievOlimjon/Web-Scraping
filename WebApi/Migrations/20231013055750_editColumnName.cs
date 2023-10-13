using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApi;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class editColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductAttributeDetailName",
                table: "AttributeDetails");

            migrationBuilder.RenameColumn(
                name: "AttributeName",
                table: "Attributes",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AttributeDetails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageUrlAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PageUrl = table.Column<string>(type: "text", nullable: false),
                    VisitedAddress = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageUrlAddresses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageUrlAddresses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AttributeDetails");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Attributes",
                newName: "AttributeName");

            migrationBuilder.AddColumn<ProductAttributeDetailName>(
                name: "ProductAttributeDetailName",
                table: "AttributeDetails",
                type: "jsonb",
                nullable: true);
        }
    }
}
