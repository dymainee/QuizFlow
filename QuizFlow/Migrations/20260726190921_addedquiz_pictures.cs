using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizFlow.Migrations
{
    /// <inheritdoc />
    public partial class addedquiz_pictures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Quiz",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Quiz");
        }
    }
}
