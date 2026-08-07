using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizFlow.Migrations
{
    /// <inheritdoc />
    public partial class added_UserAnswerToUserDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_quizSessions_UserId",
                table: "quizSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_quizSessions_Students_UserId",
                table: "quizSessions",
                column: "UserId",
                principalTable: "Students",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quizSessions_Students_UserId",
                table: "quizSessions");

            migrationBuilder.DropIndex(
                name: "IX_quizSessions_UserId",
                table: "quizSessions");
        }
    }
}
