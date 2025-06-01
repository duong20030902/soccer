using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Soccer.Data_Access.Models;

public partial class SoccerContext : DbContext
{
    public SoccerContext()
    {
    }

    public SoccerContext(DbContextOptions<SoccerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<FieldImage> FieldImages { get; set; }

    public virtual DbSet<FieldSchedule> FieldSchedules { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductColor> ProductColors { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Timeslot> Timeslots { get; set; }

    public virtual DbSet<TopUpTransaction> TopUpTransactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=(local); database=Soccer; uid=sa; pwd=123; Trusted_Connection=True; Encrypt=False");
*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("ServerConnection"));
        //optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Addresse__091C2A1BF0A61C2A");

            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.CityProvince).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.RecipientName).HasMaxLength(100);
            entity.Property(e => e.StreetAddress).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Addresses_Users");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACDED0449E3");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Commission)
                .HasComputedColumnSql("([Price]*(0.05))", true)
                .HasColumnType("numeric(13, 4)");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Booked");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Schedule");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F3BE83EF8E62");

            entity.HasIndex(e => e.BrandName, "UQ__Brands__2206CE9BBC858EAA").IsUnique();

            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.BrandName).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B0CAED7D3");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Categories_Parent");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__Colors__8DA7676D353706FF");

            entity.HasIndex(e => e.ColorName, "UQ__Colors__C71A5A7B99C2AEA1").IsUnique();

            entity.Property(e => e.ColorId).HasColumnName("ColorID");
            entity.Property(e => e.ColorCode).HasMaxLength(20);
            entity.Property(e => e.ColorName).HasMaxLength(50);
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PK__Fields__C8B6FF27C8D042CD");

            entity.Property(e => e.FieldId).HasColumnName("FieldID");
            entity.Property(e => e.FieldName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.PricePerHour).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<FieldImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__FieldIma__7516F4EC2205BF80");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.FieldId).HasColumnName("FieldID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");

            entity.HasOne(d => d.Field).WithMany(p => p.FieldImages)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FieldImages_Fields");
        });

        modelBuilder.Entity<FieldSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__FieldSch__9C8A5B69EF22DA2E");

            entity.ToTable("FieldSchedule");

            entity.HasIndex(e => new { e.FieldId, e.Date, e.TimeslotId }, "UQ_FieldSchedule_FieldDateTimeslot").IsUnique();

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.FieldId).HasColumnName("FieldID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");
            entity.Property(e => e.TimeslotId).HasColumnName("TimeslotID");

            entity.HasOne(d => d.Field).WithMany(p => p.FieldSchedules)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FieldSchedule_Fields");

            entity.HasOne(d => d.Timeslot).WithMany(p => p.FieldSchedules)
                .HasForeignKey(d => d.TimeslotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FieldSchedule_Timeslots");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFF7340A26");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.ShippingMethodId).HasColumnName("ShippingMethodID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Addresses");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PayMethods");

            entity.HasOne(d => d.ShippingMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShippingMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ShippingMethods");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A1ED9EEE3E");

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.OrderItems)
                .HasForeignKey(d => new { d.ProductId, d.SizeId })
                .HasConstraintName("FK_OrderItems_ProductSize");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE2043AC0ECE62");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.StatusName, "UQ__OrderSta__05E7698AA83F1C93").IsUnique();

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F3254B72E2");

            entity.HasIndex(e => e.MethodName, "UQ__PaymentM__218CFB17F7EB4E6D").IsUnique();

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED742B7052");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CostPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Brands");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<ProductColor>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.ColorId });

            entity.ToTable("ProductColor");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Color).WithMany(p => p.ProductColors)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductCo__Color__02FC7413");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductColors)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductCo__Produ__03F0984C");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ProductI__7516F4EC563FF370");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("ImageURL");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Products");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79AEBC242D9C");

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_Products");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_Users");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.SizeId });

            entity.ToTable("ProductSize");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSi__Produ__0A9D95DB");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSi__SizeI__0B91BA14");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A92E2FA14");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160AAECCF7C").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.ShippingMethodId).HasName("PK__Shipping__0C78338425E9C204");

            entity.HasIndex(e => e.MethodName, "UQ__Shipping__218CFB17D6530B34").IsUnique();

            entity.Property(e => e.ShippingMethodId).HasColumnName("ShippingMethodID");
            entity.Property(e => e.DeliveryTime).HasMaxLength(50);
            entity.Property(e => e.MethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__Sizes__83BD095AB49B1087");

            entity.HasIndex(e => e.SizeName, "UQ__Sizes__619EFC3EE0EFEE44").IsUnique();

            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.SizeName).HasMaxLength(20);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE6669418FEB008");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.ContactInfo).HasMaxLength(100);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<Timeslot>(entity =>
        {
            entity.HasKey(e => e.TimeslotId).HasName("PK__Timeslot__333FEA91475EA07D");

            entity.Property(e => e.TimeslotId).HasColumnName("TimeslotID");
        });

        modelBuilder.Entity<TopUpTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__TopUpTra__55433A4B1B1B3975");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.MoMoTransId)
                .HasMaxLength(50)
                .HasColumnName("MoMoTransID");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TransactionTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.TopUpTransactions)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TopUp_PayMethods");

            entity.HasOne(d => d.User).WithMany(p => p.TopUpTransactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TopUp_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC2216071");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534610A7149").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Balance).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.ConfirmationTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordResetTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
