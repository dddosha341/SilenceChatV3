﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Silence.Web.Data;

#nullable disable

namespace Silence.Web.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241220141627_FixMessages")]
    partial class FixMessages
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Silence.Web.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("FromUserId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ToRoomId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToRoomId");

                    b.ToTable("Messages", (string)null);
                });

            modelBuilder.Entity("Silence.Web.Entities.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AdminId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Rooms", (string)null);
                });

            modelBuilder.Entity("Silence.Web.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Silence.Web.Entities.Message", b =>
                {
                    b.HasOne("Silence.Web.Entities.User", "FromUser")
                        .WithMany("Messages")
                        .HasForeignKey("FromUserId");

                    b.HasOne("Silence.Web.Entities.Room", "ToRoom")
                        .WithMany("Messages")
                        .HasForeignKey("ToRoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("ToRoom");
                });

            modelBuilder.Entity("Silence.Web.Entities.Room", b =>
                {
                    b.HasOne("Silence.Web.Entities.User", "Admin")
                        .WithMany("Rooms")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Silence.Web.Entities.Room", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Silence.Web.Entities.User", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
