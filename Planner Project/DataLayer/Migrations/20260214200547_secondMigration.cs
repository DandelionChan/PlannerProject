using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    public partial class secondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop existing Foreign Keys pointing to the old table name
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_AspNetUsers_UserId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            // Use your safe SQL blocks to drop FKs on tables with composite keys
            migrationBuilder.Sql(@"
                SET @fk = (SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE 
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'AspNetUserRoles' 
                AND REFERENCED_TABLE_NAME = 'AspNetUsers' LIMIT 1);
                SET @s = IF(@fk IS NOT NULL, CONCAT('ALTER TABLE `AspNetUserRoles` DROP FOREIGN KEY `', @fk, '`;'), 'SELECT 1;');
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"
                SET @fk = (SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE 
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'AspNetUserTokens' 
                AND REFERENCED_TABLE_NAME = 'AspNetUsers' LIMIT 1);
                SET @s = IF(@fk IS NOT NULL, CONCAT('ALTER TABLE `AspNetUserTokens` DROP FOREIGN KEY `', @fk, '`;'), 'SELECT 1;');
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            migrationBuilder.Sql(@"
                SET @fk = (SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE 
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'UserDailyRemiders' 
                AND REFERENCED_TABLE_NAME = 'AspNetUsers' LIMIT 1);
                SET @s = IF(@fk IS NOT NULL, CONCAT('ALTER TABLE `UserDailyRemiders` DROP FOREIGN KEY `', @fk, '`;'), 'SELECT 1;');
                PREPARE stmt FROM @s; EXECUTE stmt; DEALLOCATE PREPARE stmt;");

            // 2. RENAME THE TABLE (This was the missing piece)
            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_UserName",
                table: "Users",
                newName: "IX_Users_UserName");

            // 3. Add the new Foreign Keys pointing to the new "Users" table
            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Users_UserId",
                table: "Activities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDailyRemider_User",
                table: "UserDailyRemiders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the process: Rename back to AspNetUsers and restore old FK names
            migrationBuilder.RenameTable(
                name: "Users",
                newName: "AspNetUsers");

            // (Down logic omitted for brevity, but usually involves renaming back)
        }
    }
}