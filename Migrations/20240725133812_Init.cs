using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Departments.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentConnections",
                schema: "dbo",
                columns: table => new
                {
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    ChildId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentConnections", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_DepartmentConnections_Departments_ChildId",
                        column: x => x.ChildId,
                        principalSchema: "dbo",
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DepartmentConnections_Departments_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentConnections_ChildId",
                schema: "dbo",
                table: "DepartmentConnections",
                column: "ChildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentConnections",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "dbo");
        }
    }
}
