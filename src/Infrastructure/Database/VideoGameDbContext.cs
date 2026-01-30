using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VideoGame>().HasData(
              new VideoGame
              {
                  Id = 1,
                  Title = "The Legend of Zelda: Breath of the Wild",
                  Genre = "Action",
                  ReleaseYear = 2017
              },
            new VideoGame
            {
                Id = 2,
                Title = "The Witcher 3: Wild Hunt",
                Genre = "RPG",
                ReleaseYear = 2015
            },
            new VideoGame
            {
                Id = 3,
                Title = "DOOM Eternal",
                Genre = "Shooter",
                ReleaseYear = 2020
            },
            new VideoGame
            {
                Id = 4,
                Title = "Red Dead Redemption 2",
                Genre = "Adventure",
                ReleaseYear = 2018
            },
            new VideoGame
            {
                Id = 5,
                Title = "Civilization VI",
                Genre = "Strategy",
                ReleaseYear = 2016
            }
        );
    }
}
