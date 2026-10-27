using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.Sql(@"
                INSERT INTO Genre (Title)
                VALUES ('Uncategorized')");

            migrationBuilder.Sql(@"
                INSERT INTO Genre (Title)
                SELECT DISTINCT Genre
                FROM Movie
                WHERE Genre IS NOT NULL
                  AND LTRIM(RTRIM(Genre)) <> ''
                  AND Genre <> 'Uncategorized'");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Movie",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE m
                SET m.GenreId = g.Id
                FROM Movie AS m
                INNER JOIN Genre AS g ON g.Title = m.Genre
                WHERE m.Genre IS NOT NULL AND LTRIM(RTRIM(m.Genre)) <> ''");

            migrationBuilder.Sql(@"
                UPDATE Movie
                SET GenreId = (SELECT Id FROM Genre WHERE Title = 'Uncategorized')
                WHERE GenreId IS NULL");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Movie");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movie",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Synopsis",
                table: "Movie",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "GenreId",
                table: "Movie",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movie_GenreId",
                table: "Movie",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movie_Genre_GenreId",
                table: "Movie",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movie_Genre_GenreId",
                table: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_Movie_GenreId",
                table: "Movie");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Movie");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Synopsis",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE m
                SET m.Genre = g.Title
                FROM Movie AS m
                INNER JOIN Genre AS g ON g.Id = m.GenreId");

            migrationBuilder.DropTable(
                name: "Genre");
        }
    }
}
