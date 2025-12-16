using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobRecruitmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyPhotosAndStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyPhotos",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPhotos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_CompanyPhotos_Employers_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employers",
                        principalColumn: "EmployerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployerId",
                table: "Users",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPhotos_EmployerId",
                table: "CompanyPhotos",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Employers_EmployerId",
                table: "Users",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "EmployerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Employers_EmployerId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "CompanyPhotos");

            migrationBuilder.DropIndex(
                name: "IX_Users_EmployerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Users");
        }
    }
}
