﻿// <auto-generated />
using System;
using ElsaMina.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElsaMina.DataAccess.Migrations
{
    [DbContext(typeof(BotDbContext))]
    partial class BotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ElsaMina.DataAccess.Models.AddedCommand", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id", "RoomId");

                    b.ToTable("AddedCommands");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.Badge", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<bool?>("IsTeamTournament")
                        .HasColumnType("boolean");

                    b.Property<bool?>("IsTrophy")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id", "RoomId");

                    b.ToTable("Badges");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.BadgeHolding", b =>
                {
                    b.Property<string>("BadgeId")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.HasKey("BadgeId", "UserId", "RoomId");

                    b.HasIndex("BadgeId", "RoomId");

                    b.HasIndex("UserId", "RoomId");

                    b.ToTable("BadgeHoldings");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.Repeat", b =>
                {
                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Delay")
                        .HasColumnType("bigint");

                    b.HasKey("RoomId", "Name");

                    b.ToTable("Repeats");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomBotParameterValue", b =>
                {
                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<string>("ParameterId")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("RoomId", "ParameterId");

                    b.ToTable("RoomBotParameterValues");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomParameters", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("RoomParameters");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomSpecificUserData", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<long?>("OnTime")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id", "RoomId");

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomTeam", b =>
                {
                    b.Property<string>("TeamId")
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .HasColumnType("text");

                    b.HasKey("TeamId", "RoomId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomTeams");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.Team", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Format")
                        .HasColumnType("text");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TeamJson")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RegDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.BadgeHolding", b =>
                {
                    b.HasOne("ElsaMina.DataAccess.Models.Badge", "Badge")
                        .WithMany("BadgeHolders")
                        .HasForeignKey("BadgeId", "RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElsaMina.DataAccess.Models.RoomSpecificUserData", "RoomSpecificUserData")
                        .WithMany("Badges")
                        .HasForeignKey("UserId", "RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Badge");

                    b.Navigation("RoomSpecificUserData");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomBotParameterValue", b =>
                {
                    b.HasOne("ElsaMina.DataAccess.Models.RoomParameters", "RoomParameters")
                        .WithMany("ParameterValues")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoomParameters");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomTeam", b =>
                {
                    b.HasOne("ElsaMina.DataAccess.Models.RoomParameters", "RoomParameters")
                        .WithMany("Teams")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElsaMina.DataAccess.Models.Team", "Team")
                        .WithMany("Rooms")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoomParameters");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.Badge", b =>
                {
                    b.Navigation("BadgeHolders");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomParameters", b =>
                {
                    b.Navigation("ParameterValues");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.RoomSpecificUserData", b =>
                {
                    b.Navigation("Badges");
                });

            modelBuilder.Entity("ElsaMina.DataAccess.Models.Team", b =>
                {
                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
