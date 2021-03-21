﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentGateway.Gateway.DataAccess;

namespace PaymentGateway.Gateway.Migrations
{
    [DbContext(typeof(PaymentGatewayContext))]
    [Migration("20210321160245_initialcreate")]
    partial class initialcreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.13");

            modelBuilder.Entity("PaymentGateway.Gateway.Models.Entities.PaymentEntity", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<long?>("BankPaymentId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CVV")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CardNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrencyISOCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ExpiryMonth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExpiryYear")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("MerchantId")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PaymentId");

                    b.ToTable("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}