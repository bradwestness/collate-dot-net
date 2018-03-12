using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Tests.Data
{
    /// <summary>
    /// DbContext for the "chinook" SQLite sample database
    /// http://www.sqlitetutorial.net/sqlite-sample-database/
    /// </summary>
    public class TestDataContext : DbContext
    {
        public DbSet<Album> Albums { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        public DbSet<MediaType> MediaTypes { get; set; }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public TestDataContext() : base("TestData")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TestDataContext>(null);
            // base.OnModelCreating(modelBuilder);
        }
    }

    [Table("Album")]
    public class Album
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlbumId { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; }

        public int ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }
    }

    [Table("Artist")]
    public class Artist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArtistId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }

    [Table("Customer")]
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required, StringLength(40)]
        public string FirstName { get; set; }

        [Required, StringLength(20)]
        public string LastName { get; set; }

        [StringLength(80)]
        public string Company { get; set; }

        [StringLength(70)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(40)]
        public string State { get; set; }

        [StringLength(40)]
        public string Country { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [Required, StringLength(60)]
        public string Email { get; set; }

        public int SupportRepId { get; set; }

        [ForeignKey("SupportRepId")]
        public virtual Employee SupportRep { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }

    [Table("Employee")]
    public class Employee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required, StringLength(20)]
        public string LastName { get; set; }

        [Required, StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        [StringLength(70)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(40)]
        public string State { get; set; }

        [StringLength(40)]
        public string Country { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [StringLength(24)]
        public string Email { get; set; }

        public int ReportsTo { get; set; }

        [ForeignKey("ReportsTo")]
        public virtual Employee Supervisor { get; set; }
    }

    [Table("Genre")]
    public class Genre
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenreId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }

    [Table("Invoice")]
    public class Invoice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }

        public DateTime InvoiceDate { get; set; }

        [StringLength(70)]
        public string BillingAddress { get; set; }

        [StringLength(40)]
        public string BillingCity { get; set; }

        [StringLength(40)]
        public string BillingState { get; set; }

        [StringLength(40)]
        public string BillingCountry { get; set; }

        [StringLength(10)]
        public string BillingPostalCode { get; set; }

        public decimal Total { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<InvoiceLine> Items { get; set; }
    }

    [Table("InvoiceLine")]
    public class InvoiceLine
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceLineId { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int InvoiceId { get; set; }

        public int TrackId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("TrackId")]
        public virtual Track Track { get; set; }
    }

    [Table("MediaType")]
    public class MediaType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MediaTypeId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }

    [Table("Playlist")]
    public class Playlist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlaylistId { get; set; }

        [StringLength(120)]
        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }

    [Table("Track")]
    public class Track
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrackId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(220)]
        public string Composer { get; set; }

        public int Milliseconds { get; set; }

        public int Bytes { get; set; }

        public decimal UnitPrice { get; set; }

        public int AlbumId { get; set; }

        public int MediaTypeId { get; set; }

        public int GenreId { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album Album { get; set; }

        [ForeignKey("MediaTypeId")]
        public virtual MediaType MediaType { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
