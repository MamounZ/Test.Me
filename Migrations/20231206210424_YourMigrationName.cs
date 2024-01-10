using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Me.Migrations
{
    public partial class YourMigrationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Qid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tid = table.Column<int>(type: "int", nullable: false),
                    Qname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Qtime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Qid);
                    table.ForeignKey(
                        name: "FK_Quiz_Teachers_Tid",
                        column: x => x.Tid,
                        principalTable: "Teachers",
                        principalColumn: "Tid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Quid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qid = table.Column<int>(type: "int", nullable: false),
                    Qutext = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qurightanswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qumark = table.Column<int>(type: "int", nullable: false),
                    IsMultipleChoice = table.Column<bool>(type: "bit", nullable: false),
                    Firstop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Secondop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thirdop = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fourthop = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Quid);
                    table.ForeignKey(
                        name: "FK_Question_Quiz_Qid",
                        column: x => x.Qid,
                        principalTable: "Quiz",
                        principalColumn: "Qid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_Qid",
                table: "Question",
                column: "Qid");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_Tid",
                table: "Quiz",
                column: "Tid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "Quiz");
        }
    }
}
