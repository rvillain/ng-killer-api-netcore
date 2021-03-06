﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using NgKillerApiCore.DAL;
using System;

namespace NgKillerApiCore.Migrations
{
    [DbContext(typeof(KillerContext))]
    [Migration("20180424152528_device")]
    partial class device
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NgKillerApiCore.Models.Action", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreation");

                    b.Property<long>("GameId");

                    b.Property<string>("KillerId");

                    b.Property<string>("KillerName");

                    b.Property<long?>("MissionId");

                    b.Property<string>("MissionTitle");

                    b.Property<string>("TargetId");

                    b.Property<string>("TargetName");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("KillerId");

                    b.HasIndex("MissionId");

                    b.HasIndex("TargetId");

                    b.ToTable("Actions");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Agent", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("GameId");

                    b.Property<int>("Life");

                    b.Property<long?>("MissionId");

                    b.Property<string>("Name");

                    b.Property<string>("Photo");

                    b.Property<string>("Status");

                    b.Property<string>("TargetId");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("MissionId");

                    b.HasIndex("TargetId");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("PushAuth");

                    b.Property<string>("PushEndpoint");

                    b.Property<string>("PushP256DH");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Mission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Difficulty");

                    b.Property<long?>("GameId");

                    b.Property<bool>("IsUsed");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Request", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.Property<DateTime>("DateCreation");

                    b.Property<string>("EmitterId");

                    b.Property<long>("GameId");

                    b.Property<bool>("IsTreated");

                    b.Property<long?>("ParentRequestId");

                    b.Property<string>("ReceiverId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("EmitterId");

                    b.HasIndex("GameId");

                    b.HasIndex("ParentRequestId");

                    b.HasIndex("ReceiverId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Action", b =>
                {
                    b.HasOne("NgKillerApiCore.Models.Game", "Game")
                        .WithMany("Actions")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NgKillerApiCore.Models.Agent", "Killer")
                        .WithMany("ActionsAsKiller")
                        .HasForeignKey("KillerId");

                    b.HasOne("NgKillerApiCore.Models.Mission", "Mission")
                        .WithMany()
                        .HasForeignKey("MissionId");

                    b.HasOne("NgKillerApiCore.Models.Agent", "Target")
                        .WithMany("ActionsAsTarget")
                        .HasForeignKey("TargetId");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Agent", b =>
                {
                    b.HasOne("NgKillerApiCore.Models.Game", "Game")
                        .WithMany("Agents")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NgKillerApiCore.Models.Mission", "Mission")
                        .WithMany()
                        .HasForeignKey("MissionId");

                    b.HasOne("NgKillerApiCore.Models.Agent", "Target")
                        .WithMany()
                        .HasForeignKey("TargetId");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Mission", b =>
                {
                    b.HasOne("NgKillerApiCore.Models.Game", "Game")
                        .WithMany("Missions")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("NgKillerApiCore.Models.Request", b =>
                {
                    b.HasOne("NgKillerApiCore.Models.Agent", "Emitter")
                        .WithMany()
                        .HasForeignKey("EmitterId");

                    b.HasOne("NgKillerApiCore.Models.Game", "Game")
                        .WithMany("Requests")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NgKillerApiCore.Models.Request", "ParentRequest")
                        .WithMany()
                        .HasForeignKey("ParentRequestId");

                    b.HasOne("NgKillerApiCore.Models.Agent", "Receiver")
                        .WithMany("Requests")
                        .HasForeignKey("ReceiverId");
                });
#pragma warning restore 612, 618
        }
    }
}
