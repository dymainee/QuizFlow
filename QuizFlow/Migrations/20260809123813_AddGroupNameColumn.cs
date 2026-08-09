using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizFlow.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "quizSessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "quizSessions");
        }
    }
}
