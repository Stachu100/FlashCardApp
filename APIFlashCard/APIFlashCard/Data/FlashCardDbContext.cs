﻿using APIFlashCard.Models;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Data
{
    public class FlashCardDbContext : DbContext
    {
        public FlashCardDbContext(DbContextOptions<FlashCardDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<EncryptionKeys> EncryptionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .ToTable("category")
                .HasKey(c => c.ID_Category);

            modelBuilder.Entity<User>()
                .ToTable("user")
                .HasKey(u => u.ID_User);

            modelBuilder.Entity<Countries>()
                .ToTable("countries")
                .HasKey(co => co.ID_Country);

            modelBuilder.Entity<UserDetails>()
                .ToTable("userDetails")
                .HasKey(ud => ud.ID_Detailed);

            modelBuilder.Entity<EncryptionKeys>()
                .ToTable("encryptionKeys")
                .HasKey(ek => ek.ID_encryptionKeys);
        }
    }
}

