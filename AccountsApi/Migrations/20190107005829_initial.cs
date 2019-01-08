using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AccountsApi.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    sname = table.Column<string>(maxLength: 50, nullable: true),
                    fname = table.Column<string>(maxLength: 50, nullable: true),
                    country = table.Column<string>(maxLength: 50, nullable: true),
                    city = table.Column<string>(maxLength: 50, nullable: true),
                    phone = table.Column<string>(maxLength: 16, nullable: true),
                    email = table.Column<string>(maxLength: 100, nullable: true),
                    sex = table.Column<int>(nullable: false),
                    birth = table.Column<DateTime>(nullable: false),
                    joined = table.Column<DateTime>(nullable: false),
                    status = table.Column<string>(maxLength: 10, nullable: true),
                    interests = table.Column<string[]>(nullable: true),
                    like_ids = table.Column<long[]>(nullable: true),
                    like_tss = table.Column<long[]>(nullable: true),
                    premium_start = table.Column<long>(nullable: true),
                    premium_finish = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
