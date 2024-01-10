using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Me.Migrations
{
    public partial class AddExamSubmittedToQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExamSubmitted",
                table: "Quiz",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamSubmitted",
                table: "Quiz");
        }
    }
}
