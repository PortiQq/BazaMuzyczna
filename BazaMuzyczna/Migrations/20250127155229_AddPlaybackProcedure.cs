using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BazaMuzyczna.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaybackProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE OR REPLACE PROCEDURE upsert_playback(user_id INT, track_id INT)
                LANGUAGE plpgsql AS $$
                BEGIN
                  IF EXISTS (SELECT 1 FROM ""Playback"" WHERE ""UserId"" = user_id AND ""TrackId"" = track_id) THEN
                      UPDATE ""Playback""
                      SET ""Quantity"" = ""Quantity"" + 1
                      WHERE ""UserId"" = user_id AND ""TrackId"" = track_id;
                  ELSE
                      INSERT INTO ""Playback"" (""UserId"", ""TrackId"", ""Quantity"")
                      VALUES (user_id, track_id, 1);
                  END IF;
                END;
                $$;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP PROCEDURE IF EXISTS upsert_playback;"
            );
        }
    }
}
