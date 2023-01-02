using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FcRoomBooking.Migrations
{
    public partial class AddEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExcept",
                table: "Participants",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByUserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_AspNetUsers_ByUserID",
                        column: x => x.ByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AddUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailId = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddUser_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AddUser_Email_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Email",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddUser_EmailId",
                table: "AddUser",
                column: "EmailId");

            migrationBuilder.CreateIndex(
                name: "IX_AddUser_UserID",
                table: "AddUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Email_ByUserID",
                table: "Email",
                column: "ByUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddUser");

            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropColumn(
                name: "IsExcept",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Participants");
        }
    }
}
