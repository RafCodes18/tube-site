using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VRhfo.PL;

public partial class VRhfoEntities : DbContext
{
    public VRhfoEntities()
    {
    }

    public VRhfoEntities(DbContextOptions<VRhfoEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<tblComment> tblComments { get; set; }

    public virtual DbSet<tblCommentLike> tblCommentLikes { get; set; }

    public virtual DbSet<tblReply> tblReplies { get; set; }

    public virtual DbSet<tblUser> tblUsers { get; set; }

    public virtual DbSet<tblVideo> tblVideos { get; set; }

    public virtual DbSet<tblVideoView> tblVideoViews { get; set; }

    public virtual DbSet<tblVideosLiked> tblVideosLikes { get; set; }

    public virtual DbSet<tblWatchEntry> tblWatchEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
  => optionsBuilder.UseSqlServer("Server=tcp:vrhfosqlserver.database.windows.net,1433;Initial Catalog=vrhfo-db;Persist Security Info=False;User ID=sqladmin;Password=Par516170$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblComme__3214EC077DB2077C");

            entity.ToTable("tblComment");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DatePosted).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblCommentLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblComme__3214EC0701C37F76");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<tblReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblReply__3214EC07829234BD");

            entity.ToTable("tblReply");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.DatePosted).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblUser__3214EC07027C26E8");

            entity.ToTable("tblUser");

            entity.HasIndex(e => e.Email, "UQ__tblUser__A9D1053400031F6D").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessFailedCount).HasDefaultValueSql("((0))");
            entity.Property(e => e.Auth0UserId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ConcurrencyStamp).HasMaxLength(128);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstVisit).HasColumnType("datetime");
            entity.Property(e => e.IPaddress)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LockoutEnabled).HasDefaultValueSql("((0))");
            entity.Property(e => e.LockoutEnd).HasColumnType("datetime");
            entity.Property(e => e.NextRenewalDueDate).HasColumnType("date");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUsername).HasMaxLength(256);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(100);
            entity.Property(e => e.PasswordResetTokenExpiration).HasColumnType("datetime");
            entity.Property(e => e.SecurityStamp).HasMaxLength(128);
            entity.Property(e => e.SubscribedDate).HasColumnType("datetime");
            entity.Property(e => e.SubscriptionTier)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(120);
        });

        modelBuilder.Entity<tblVideo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblVideo__3214EC0768E205BA");

            entity.ToTable("tblVideo");

            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.ContentWarning).HasMaxLength(255);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.PreviewVideoURL).IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UploadDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblVideoView>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblVideo__3214EC07A2BCFDDC");

            entity.Property(e => e.IPAdress).HasMaxLength(50);
            entity.Property(e => e.ViewTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<tblVideosLiked>(entity =>
        {
            entity.HasKey(e => new { e.UserID, e.VideoID }).HasName("PK__tblVideo__AC269D88ABD386D6");

            entity.ToTable("tblVideosLiked");

            entity.Property(e => e.LikedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.tblVideosLikeds)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK__tblVideos__UserI__35BCFE0A");

            entity.HasOne(d => d.Video).WithMany(p => p.tblVideosLikeds)
                .HasForeignKey(d => d.VideoID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tblVideos__Video__36B12243");
        });

        modelBuilder.Entity<tblWatchEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblWatch__3214EC07D3C7F18C");

            entity.ToTable("tblWatchEntry");

            entity.Property(e => e.FirstViewed).HasColumnType("datetime");
            entity.Property(e => e.LastDateWatched).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
