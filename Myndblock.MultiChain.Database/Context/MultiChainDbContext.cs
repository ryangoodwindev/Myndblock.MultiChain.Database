﻿using Microsoft.EntityFrameworkCore;
using System;

namespace Myndblock.MultiChain.Database
{
    /// <summary>
    /// MultiChain database context offers transaction logging for the target blockchain node
    /// </summary>
    public class MultiChainDbContext : DbContext
    {
        /// <summary>
        /// MultiChain database context offers transaction logging for the target blockchain node
        /// </summary>
        /// <param name="options"></param>
        public MultiChainDbContext(DbContextOptions<MultiChainDbContext> options) 
            : base(options) { }

        /// <summary>
        /// Connection to the TransactionModel table
        /// </summary>
        public DbSet<TransactionModel> Transactions { get; set; }

        /// <summary>
        /// OnConfiguring override will detect when the <paramref name="optionsBuilder"/> is not configured
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // throw an ArgumentException when optionsBuilder is not configured
            if (!optionsBuilder.IsConfigured)
                throw new ArgumentException("DbContextOptionsBuilder is not configured");

            // pass optionsBuilder to underlying method
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// OnModelCreating override handles configuring the backing data models using Fluent API <paramref name="modelBuilder"/>
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // set primary key of TransactionModel
            modelBuilder.Entity<TransactionModel>().HasKey(key => key.Id);

            // infer that the primary key, Id, should be generated OnAdd
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.Id)
                .ValueGeneratedOnAdd();

            // set CreatedBy by to Optional
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.CreatedBy)
                .IsRequired(false);

            // set LastModifiedBy by to Optional
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.LastModifiedBy)
                .IsRequired(false);

            // infer that CreatedOn should be generated OnAdd
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.CreatedOn)
                .HasValueGenerator(typeof(UtcNowValueGenerator))
                .ValueGeneratedOnAdd();

            // infer that LastModifiedOn should be generated OnAddOrUpdate
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.LastModifiedOn)
                .HasValueGenerator(typeof(UtcNowValueGenerator))
                .ValueGeneratedOnAdd();

            // infer that the Txid property should be treated as a unique index
            modelBuilder.Entity<TransactionModel>()
               .HasIndex(index => index.Txid)
               .IsUnique();

            // set Txid by to Optional
            modelBuilder.Entity<TransactionModel>()
               .Property(prop => prop.Txid)
               .IsRequired(false);

            // set ErrorMessage by to Optional
            modelBuilder.Entity<TransactionModel>()
               .Property(prop => prop.ErrorMessage)
               .IsRequired(false);

            // infer that the RowVersion should be used to hand optimistic concurrency
            modelBuilder.Entity<TransactionModel>()
                .Property(prop => prop.RowVersion)
                .IsRowVersion();

            // pass modelBuilder to underlying method
            base.OnModelCreating(modelBuilder);
        }
    }
}