﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Myndblock.MultiChain.Database.Migrations
{
    public partial class Create_Initial_Schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "multichain_transaction_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LastModifiedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    LastModifiedBy = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: false),
                    Blockchain = table.Column<string>(nullable: false),
                    Txid = table.Column<string>(nullable: false),
                    TargetMethod = table.Column<string>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_multichain_transaction_log", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_multichain_transaction_log_Txid",
                table: "multichain_transaction_log",
                column: "Txid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "multichain_transaction_log");
        }
    }
}
