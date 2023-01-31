﻿// <auto-generated />
using System;
using Flamma.Auth.Data.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Flamma.Auth.Data.Access.Migrations
{
    [DbContext(typeof(AuthDbContext))]
    [Migration("20230131142813_AddBanField")]
    partial class AddBanField
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Flamma.Auth.Data.Access.Models.AdditionalUserInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<Guid>("PrimaryLocationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserDataId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserDataId")
                        .IsUnique();

                    b.ToTable("AdditionalUserInformation");
                });

            modelBuilder.Entity("Flamma.Auth.Data.Access.Models.UserData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BannedTill")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Salt")
                        .HasColumnType("bytea");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("Flamma.Auth.Data.Access.Models.AdditionalUserInformation", b =>
                {
                    b.HasOne("Flamma.Auth.Data.Access.Models.UserData", "UserData")
                        .WithOne("AdditionalUserInformation")
                        .HasForeignKey("Flamma.Auth.Data.Access.Models.AdditionalUserInformation", "UserDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserData");
                });

            modelBuilder.Entity("Flamma.Auth.Data.Access.Models.UserData", b =>
                {
                    b.Navigation("AdditionalUserInformation");
                });
#pragma warning restore 612, 618
        }
    }
}
