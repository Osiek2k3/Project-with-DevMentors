﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySpot.Infrastructure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Cleaning_Reservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Reservations",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Reservations");
        }
    }
}
