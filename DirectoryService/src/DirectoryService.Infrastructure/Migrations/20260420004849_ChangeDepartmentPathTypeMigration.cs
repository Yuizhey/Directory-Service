using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDepartmentPathTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,");

            migrationBuilder.Sql("""
                ALTER TABLE departments
                ALTER COLUMN path TYPE ltree
                USING path::ltree;
            """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS ix_departments_path_gist
                ON departments
                USING GIST (path);
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX IF EXISTS ix_departments_path_gist;
            """);

            migrationBuilder.Sql("""
                ALTER TABLE departments
                ALTER COLUMN path TYPE character varying(50)
                USING path::text;
            """);

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:ltree", ",,");
        }
    }
}