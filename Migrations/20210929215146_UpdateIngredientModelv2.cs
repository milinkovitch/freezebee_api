using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace freezebee_api.Migrations
{
    public partial class UpdateIngredientModelv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientModels",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModelId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Weight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientModels", x => new { x.IngredientId, x.ModelId });
                    table.ForeignKey(
                        name: "FK_IngredientModels_Ingredients_IngredientId1",
                        column: x => x.IngredientId1,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientModels_Models_ModelId1",
                        column: x => x.ModelId1,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientModels_IngredientId1",
                table: "IngredientModels",
                column: "IngredientId1");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientModels_ModelId1",
                table: "IngredientModels",
                column: "ModelId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientModels_Ingredients_IngredientId",
                table: "IngredientModels");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientModels_Models_ModelId",
                table: "IngredientModels");

            migrationBuilder.DropIndex(
                name: "IX_IngredientModels_ModelId",
                table: "IngredientModels");

            migrationBuilder.AlterColumn<int>(
                name: "ModelId",
                table: "IngredientModels",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "IngredientId",
                table: "IngredientModels",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientId1",
                table: "IngredientModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId1",
                table: "IngredientModels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientModels_IngredientId1",
                table: "IngredientModels",
                column: "IngredientId1");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientModels_ModelId1",
                table: "IngredientModels",
                column: "ModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientModels_Ingredients_IngredientId1",
                table: "IngredientModels",
                column: "IngredientId1",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientModels_Models_ModelId1",
                table: "IngredientModels",
                column: "ModelId1",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
