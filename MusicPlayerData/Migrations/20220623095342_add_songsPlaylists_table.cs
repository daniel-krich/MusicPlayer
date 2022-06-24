using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayerData.Migrations
{
    public partial class add_songsPlaylists_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaPlaylistEntity_Playlists_PlayerlistId",
                table: "MediaPlaylistEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaPlaylistEntity_Songs_MediaId",
                table: "MediaPlaylistEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaPlaylistEntity",
                table: "MediaPlaylistEntity");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Songs");

            migrationBuilder.RenameTable(
                name: "MediaPlaylistEntity",
                newName: "SongsPlaylists");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPlaylistEntity_PlayerlistId",
                table: "SongsPlaylists",
                newName: "IX_SongsPlaylists_PlayerlistId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaPlaylistEntity_MediaId",
                table: "SongsPlaylists",
                newName: "IX_SongsPlaylists_MediaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SongsPlaylists",
                table: "SongsPlaylists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SongsPlaylists_Playlists_PlayerlistId",
                table: "SongsPlaylists",
                column: "PlayerlistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongsPlaylists_Songs_MediaId",
                table: "SongsPlaylists",
                column: "MediaId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongsPlaylists_Playlists_PlayerlistId",
                table: "SongsPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_SongsPlaylists_Songs_MediaId",
                table: "SongsPlaylists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SongsPlaylists",
                table: "SongsPlaylists");

            migrationBuilder.RenameTable(
                name: "SongsPlaylists",
                newName: "MediaPlaylistEntity");

            migrationBuilder.RenameIndex(
                name: "IX_SongsPlaylists_PlayerlistId",
                table: "MediaPlaylistEntity",
                newName: "IX_MediaPlaylistEntity_PlayerlistId");

            migrationBuilder.RenameIndex(
                name: "IX_SongsPlaylists_MediaId",
                table: "MediaPlaylistEntity",
                newName: "IX_MediaPlaylistEntity_MediaId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Songs",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaPlaylistEntity",
                table: "MediaPlaylistEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPlaylistEntity_Playlists_PlayerlistId",
                table: "MediaPlaylistEntity",
                column: "PlayerlistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaPlaylistEntity_Songs_MediaId",
                table: "MediaPlaylistEntity",
                column: "MediaId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
