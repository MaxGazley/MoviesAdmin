using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddAudienceRatingRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudienceRating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceRating", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO AudienceRating (Title)
                SELECT DISTINCT Rating
                FROM Movie
                WHERE Rating IS NOT NULL AND LTRIM(RTRIM(Rating)) <> ''");

            migrationBuilder.Sql(@"
                INSERT INTO AudienceRating (Title)
                SELECT 'Uncategorized'
                WHERE NOT EXISTS (SELECT 1 FROM AudienceRating WHERE Title = 'Uncategorized')");

            migrationBuilder.AddColumn<int>(
                name: "AudienceRatingId",
                table: "Movie",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE m
                SET m.AudienceRatingId = a.Id
                FROM Movie AS m
                INNER JOIN AudienceRating AS a ON a.Title = m.Rating");

            migrationBuilder.Sql(@"
                UPDATE Movie
                SET AudienceRatingId = (SELECT Id FROM AudienceRating WHERE Title = 'Uncategorized')
                WHERE AudienceRatingId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "AudienceRatingId",
                table: "Movie",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Movie");

            migrationBuilder.CreateIndex(
                name: "IX_Movie_AudienceRatingId",
                table: "Movie",
                column: "AudienceRatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_AudienceRating_AudienceRatingId",
                table: "Movie",
                column: "AudienceRatingId",
                principalTable: "AudienceRating",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_AudienceRating_AudienceRatingId",
                table: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_Movie_AudienceRatingId",
                table: "Movie");

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE m
                SET m.Rating = a.Title
                FROM Movie AS m
                INNER JOIN AudienceRating AS a ON a.Id = m.AudienceRatingId");

            migrationBuilder.DropColumn(
                name: "AudienceRatingId",
                table: "Movie");

            migrationBuilder.DropTable(
                name: "AudienceRating");
        }
    }
}
