using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pikadit.Models
{
    public partial class SiteContext : DbContext
    {
        public SiteContext(DbContextOptions<SiteContext> options) : base(options) { }
        public SiteContext() : base() { }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasOne(e => e.User)
                .WithMany(e => e.Posts)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(e => e.Post)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.PostId);

            modelBuilder.Entity<Comment>()
                .HasOne(e => e.Post)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.PostId);

            modelBuilder.Entity<Vote>()
                .HasOne(e => e.User)
                .WithMany(e => e.Votes)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
               .HasOne(e => e.Post)
               .WithMany(e => e.Votes)
               .HasForeignKey(e => e.PostId)
               .IsRequired(false);

            modelBuilder.Entity<Vote>()
               .HasOne(e => e.Post)
               .WithMany(e => e.Votes)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
               .HasOne(e => e.Comment)
               .WithMany(e => e.Votes)
               .HasForeignKey(e => e.CommentId)
               .IsRequired(false);
        }
    }
}
