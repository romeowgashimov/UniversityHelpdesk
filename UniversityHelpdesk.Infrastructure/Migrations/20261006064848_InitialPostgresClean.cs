using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UniversityHelpdesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<byte>(type: "smallint", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipmentType = table.Column<byte>(type: "smallint", nullable: false),
                    EquipmentDetails = table.Column<string>(type: "text", nullable: false),
                    RoomNumber = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    ApplicantId = table.Column<int>(type: "integer", nullable: false),
                    TechnicianId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Users_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "Login", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "student@univ.ru", "Иванов Иван", "ivan", "BCB15F821479B4D5772BD0CA866C00AD5F926E3580720659CC80D39C9D09802A", (byte)1 },
                    { 2, "dispatcher@univ.ru", "Петров Пётр", "petr", "4CC8F4D609B717356701C57A03E737E5AC8FE885DA8C7163D3DE47E01849C635", (byte)2 },
                    { 3, "technician@univ.ru", "Сидоров Сидор", "pidor", "68487DC295052AA79C530E283CE698B8C6BB1B42FF0944252E1910DBECDC5425", (byte)3 },
                    { 4, "roman@univ.ru", "Романов Роман", "roman", "69F7F7A7F8BCA9970FA6F9C0B8DAD06901D3EF23FD599D3213AA5EEE5621C3E3", (byte)3 },
                    { 5, "danilo@univ.ru", "Даниилов Данилл", "danilo", "AF41E68E1309FA29A5044CBDC36B90A3821D8807E68C7675A6C495112BC8A55F", (byte)3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ApplicantId",
                table: "Tickets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TechnicianId",
                table: "Tickets",
                column: "TechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
