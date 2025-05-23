﻿// <auto-generated />
using System;
using JarvisWeb.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JarvisWeb.Domain.Migrations
{
    [DbContext(typeof(JarvisWebDbContext))]
    [Migration("20241226211717_AddDataModels")]
    partial class AddDataModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("JarvisWeb.Domain.Models.ApiKey", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.DailySummary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EndOfDayNoteId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SummaryText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SummaryVideoPath")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EndOfDayNoteId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("DailySummaries");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.EndOfDayNote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AudioFilePath")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EndOfDayNotes");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsInOffice")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nicknames")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.ApiKey", b =>
                {
                    b.HasOne("JarvisWeb.Domain.Models.User", "User")
                        .WithMany("ApiKeys")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.DailySummary", b =>
                {
                    b.HasOne("JarvisWeb.Domain.Models.EndOfDayNote", "EndOfDayNote")
                        .WithOne("DailySummary")
                        .HasForeignKey("JarvisWeb.Domain.Models.DailySummary", "EndOfDayNoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JarvisWeb.Domain.Models.User", "User")
                        .WithMany("DailySummaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EndOfDayNote");

                    b.Navigation("User");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.EndOfDayNote", b =>
                {
                    b.HasOne("JarvisWeb.Domain.Models.User", "User")
                        .WithMany("EndOfDayNotes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.EndOfDayNote", b =>
                {
                    b.Navigation("DailySummary")
                        .IsRequired();
                });

            modelBuilder.Entity("JarvisWeb.Domain.Models.User", b =>
                {
                    b.Navigation("ApiKeys");

                    b.Navigation("DailySummaries");

                    b.Navigation("EndOfDayNotes");
                });
#pragma warning restore 612, 618
        }
    }
}
