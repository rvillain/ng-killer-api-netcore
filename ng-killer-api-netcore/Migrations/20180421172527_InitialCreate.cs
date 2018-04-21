using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace NgKillerApiCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Difficulty = table.Column<string>(nullable: true),
                    GameId = table.Column<long>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GameId = table.Column<long>(nullable: false),
                    Life = table.Column<int>(nullable: false),
                    MissionId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    TargetId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agents_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agents_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agents_Agents_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<long>(nullable: false),
                    KillerId = table.Column<string>(nullable: true),
                    KillerName = table.Column<string>(nullable: true),
                    MissionId = table.Column<long>(nullable: true),
                    MissionTitle = table.Column<string>(nullable: true),
                    TargetId = table.Column<string>(nullable: true),
                    TargetName = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Actions_Agents_KillerId",
                        column: x => x.KillerId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actions_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Actions_Agents_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<string>(nullable: true),
                    DateCreation = table.Column<DateTime>(nullable: false),
                    EmitterId = table.Column<string>(nullable: true),
                    GameId = table.Column<long>(nullable: false),
                    IsTreated = table.Column<bool>(nullable: false),
                    ParentRequestId = table.Column<long>(nullable: true),
                    ReceiverId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Agents_EmitterId",
                        column: x => x.EmitterId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_Requests_ParentRequestId",
                        column: x => x.ParentRequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Agents_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_GameId",
                table: "Actions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_KillerId",
                table: "Actions",
                column: "KillerId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_MissionId",
                table: "Actions",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_TargetId",
                table: "Actions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_GameId",
                table: "Agents",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_MissionId",
                table: "Agents",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_TargetId",
                table: "Agents",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_GameId",
                table: "Missions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_EmitterId",
                table: "Requests",
                column: "EmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_GameId",
                table: "Requests",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ParentRequestId",
                table: "Requests",
                column: "ParentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests",
                column: "ReceiverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
